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
    public partial class ConvertObjPanel : UserControl
    {
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
    }
}
