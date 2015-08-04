using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECM_Gui.ClassExtension;

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
                textBox_source.Text = _objToConvert.Source.FullName;
                textBox_dest.Text = _objToConvert.Dest.FullName;
            }
        }

        private void ChangeStatus(StatusObj s)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ChangeStatus(s); });

            }
            else
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
                else if (_Status == StatusObj.Waiting)
                {
                    this.BackColor = SystemColors.Control;
                    button1.Text = TextConvert;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button1.Visible = true;
                }
                else if (_Status == StatusObj.Working)
                {
                    this.BackColor = SystemColors.Control;
                    button1.Text = TextStop;
                    button1.Visible = true;

                    button1.Enabled = GlobVar.Status != StatusApp.OnConvertMultiple;

                    button2.Enabled = false;
                }
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
                ChangeStatus(value);
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

            AsyncStartStop();
        }


        public void AsyncStartStop()
        {
            if (GlobVar.Status == StatusApp.Waiting)
            {
                if (_Status == StatusObj.Waiting)
                {
                    Status = StatusObj.Working;
                    _objToConvert.Progress += _objToConvert_Progress;
                    _objToConvert.Done += _objToConvert_Done;

                    _objToConvert.AsyncConvert();
                }
                else if (_Status == StatusObj.Working)
                {
                    Status = StatusObj.Stopping;
                    _objToConvert.AsyncStop();
                }
                else
                    MessageBox.Show("La conversione è già stata eseguita");
            }
            else
            {
                MessageBox.Show("completare l'operazione in corso e riprovare");
            }
        }

        public void Convert(bool silent=false)
        {
            if (_Status == StatusObj.Waiting)
            {
                Status = StatusObj.Working;
                _objToConvert.Progress += _objToConvert_Progress;
                _objToConvert.Done += _objToConvert_Done;

                _objToConvert.Convert();
            }
            else if( !silent)
                MessageBox.Show("La conversione è già stata eseguita");
        }



        private void _objToConvert_Done(StatusConvert sc,String msg="")
        {
            

            if (sc == StatusConvert.Complete)
                Status = StatusObj.Complete;
            else if (sc == StatusConvert.Error)
            {
                Status = StatusObj.Error;
                SetErrMsg(msg);
            }
            else if (sc == StatusConvert.Aborted)
                Status = StatusObj.Waiting;
        }

        private void _objToConvert_Progress(int progress)
        {
            if(progressBar1.InvokeRequired)
                progressBar1.Invoke((MethodInvoker)delegate { _objToConvert_Progress(progress); });
            else
                progressBar1.Value = progress;

        }


        private void SetErrMsg(String msg)
        {
            if (textBox_err.InvokeRequired)
                textBox_err.Invoke((MethodInvoker)delegate { SetErrMsg(msg); });
            else
                textBox_err.Text = msg;
        }
        public enum StatusObj
        {
            Waiting,
            Working,
            Stopping,
            Complete,
            Error
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if( _Status!=StatusObj.Stopping && _Status != StatusObj.Working && GlobVar.Status!=StatusApp.OnConvertMultiple)
                if (Parent is ConvertObjPanel)
                    ((ConvertObjPanel)Parent).RemoveConvertObjControl(this);
            
        }
    }
    
}
