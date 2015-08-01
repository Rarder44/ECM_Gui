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



        private string TextConvert = "Converti";
        private string TextStop = "Stop";


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

        delegate void InvokeChangeStatusDel(StatusObj s);
        private void InvocheChangeStatus(StatusObj s)
        {
            _Status = s;
            if (_Status == StatusObj.Complete)
            {
                progressBar1.Value = 100;
                this.BackColor = Color.FromArgb(128, 255, 128);
                button1.Visible = false;
                button1.Enabled = false;
            }
                
            else if (_Status == StatusObj.Error)
            {
                progressBar1.Value = 0;
                this.BackColor = Color.FromArgb(255, 108, 108);
                button1.Visible = false;
                button1.Enabled = false;
            }
            else if (_Status == StatusObj.Stopping)
            {
                this.BackColor = SystemColors.Control;
                button1.Enabled = false;
                button1.Visible = true;
            }
            else if (_Status == StatusObj.Waiting)
            {
                this.BackColor = SystemColors.Control;
                button1.Text = TextConvert;
                button1.Enabled = true;
                button1.Visible = true;
            }
            else if (_Status == StatusObj.Working)
            {
                this.BackColor = SystemColors.Control;
                button1.Text = TextStop;
                button1.Enabled = true;
                button1.Visible = true;
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

                if(this.InvokeRequired)
                    this.Invoke(new InvokeChangeStatusDel(InvocheChangeStatus), new object[] { value });
                
                else
                    InvocheChangeStatus(value);
                
               
            }
        }



        public ConvertObjControl()
        {
            InitializeComponent();
            button1.Text = TextConvert;

        }

        public ConvertObjControl(ToConvertObj o)
        {
            InitializeComponent();
            objToConvert = o;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (_Status == StatusObj.Waiting)
            {
                Status = StatusObj.Working;
                _objToConvert.Progress += _objToConvert_Progress;
                _objToConvert.Done += _objToConvert_Done;

                _objToConvert.Convert();
            }
            else if (_Status == StatusObj.Working)
            {
                Status = StatusObj.Stopping;
                _objToConvert.Stop();
            }
            else
                MessageBox.Show("La conversione è già stata eseguita");
        }

        private void _objToConvert_Done(StatusConvert sc)
        {
            

            if (sc == StatusConvert.Complete)
                Status = StatusObj.Complete;
            else if (sc == StatusConvert.Error)
                Status = StatusObj.Error;
            else if (sc == StatusConvert.Aborted)
                Status = StatusObj.Waiting;
        }

        private void _objToConvert_Progress(int progress)
        {
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = progress; });
            
        }
        public enum StatusObj
        {
            Waiting,
            Working,
            Stopping,
            Complete,
            Error
        }
    }
    
}
