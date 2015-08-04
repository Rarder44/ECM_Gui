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
using System.Threading;
using ECM_Gui.Services;

namespace ECM_Gui
{
    public partial class ConvertObjPanel : UserControl
    {
        public delegate void DoneCallback();
        /// <summary>
        /// Quando il ConvertAll termina
        /// </summary>
        public event DoneCallback Done;

        public delegate void ProgressCallback(int progress);
        /// <summary>
        /// Quando viene convertito un nuovo Obj
        /// </summary>
        public event ProgressCallback Progress;



        public ConvertObjPanel()
        {
            InitializeComponent();
        }

        private void ConvertObjPanel_Load(object sender, EventArgs e)
        {

        }

        public void AddUnique(ToConvertObj o)
        {
            foreach(Control c in  Controls)
            {
                if( c is ConvertObjControl)
                {
                    if (((ConvertObjControl)c).objToConvert.Source.FullName == o.Source.FullName)
                    {
                        return;
                    }
                }  
            }
            Controls.AddVertical(new ConvertObjControl(o));
        }

        public void RemoveConvertObjControl(ConvertObjControl c)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { RemoveConvertObjControl(c); });
            }
            else
            {
                Controls.Remove(c);
                RelocateControls();
            }
        }

        public void RelocateControls()
        {
            int y = 0;
            foreach (Control c in Controls)
            {
                c.Location = new Point(c.Location.X, y);
                y = c.Location.Y + c.Size.Height;                   
            }
        }

        Thread Conversion;
        public void AsyncStartConvertAll()
        {
            Conversion = new Thread(()=>
            {
                int i = 0;
                foreach(Control c in Controls)
                {
                    if( c is ConvertObjControl)
                    {
                        ((ConvertObjControl)c).Convert(true);
                        if (Progress != null)
                            Progress(++i);
                    }
                }
                if (Done != null)
                    Done();
            });
            Conversion.Start();

        }

        public Thread AsyncStopAll()
        {
            Thread t = new Thread(() =>
              {
                  if (Conversion != null && Conversion.IsAlive)
                  {
                      Conversion.Abort();
                      Conversion.Join();
                      ECMService.TryCloseFileStream();
                      if (Done != null)
                          Done();
                  }
              });
            t.Start();
            return t;

        }

        public int GetCountConvertObjControl()
        {
            int i= 0;
            foreach (Control c in Controls)
                if (c is ConvertObjControl)
                    i++;
            return i;
        }
    }
}
