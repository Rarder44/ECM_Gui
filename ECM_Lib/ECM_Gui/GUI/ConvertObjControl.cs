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

        }
       

    }
}
