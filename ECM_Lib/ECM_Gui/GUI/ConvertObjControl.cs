using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECM_Gui
{
    public partial class ConvertObjControl : UserControl
    {
        private ToConvertObj _objToConvert = null;
        private StatusObj _Status = StatusObj.Waiting;

        public ToConvertObj objToConvert
        {
            get
            {
                return _objToConvert;
            }
            set
            {
                _objToConvert = value;
                textBox1.Text = _objToConvert.Source.FullName;
                textBox2.Text = _objToConvert.Dest.FullName;
            }
        }
        public StatusObj Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                if (_Status == StatusObj.Complete)
                    this.BackColor = Color.FromArgb(128, 255, 128);
                else if (_Status == StatusObj.Error)
                    this.BackColor = Color.FromArgb(255, 108, 108);
                else
                    this.BackColor = SystemColors.Control;
            }
        }



        public ConvertObjControl()
        {
            InitializeComponent();

        }

        public ConvertObjControl(ToConvertObj o)
        {
            InitializeComponent();
            objToConvert = o;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _objToConvert.Progress += _objToConvert_Progress;
            _objToConvert.Done += _objToConvert_Done;

            _objToConvert.Convert();
        }

        private void _objToConvert_Done(StatusConvert sc)
        {
            if (sc == StatusConvert.Complete)
                Status = StatusObj.Complete;
            else if (sc == StatusConvert.Error)
                Status = StatusObj.Error;
        }

        private void _objToConvert_Progress(int progress)
        {
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progress; });
            
        }
        public enum StatusObj
        {
            Waiting,
            Working,
            Complete,
            Error
        }
    }
    
}
