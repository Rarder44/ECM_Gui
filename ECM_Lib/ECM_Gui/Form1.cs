using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ECM_Gui.ClassExtension;

namespace ECM_Gui
{
    public partial class Form1 : Form
    {
        [DllImport("TestLib.dll")]
        public static extern string ConvertToECM(string Source, string Dest);

        public Form1()
        {
            InitializeComponent();
            GlobVar.InitGlobVar();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void DragDrop(object sender, DragEventArgs e)
        {
            Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (a != null)
            {
                foreach(String ss in a )
                {
                    if (File.Exists(ss))
                    {
                        convertObjPanel1.AddUnique(new ToConvertObj(ss));
                    }

                }         
            }

        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }






        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            convertObjPanel1.Controls.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                foreach(string file in  openFileDialog1.FileNames)
                {
                    if (File.Exists(file))
                    {
                        convertObjPanel1.AddUnique(new ToConvertObj(file));
                    }
                }
            }
        }
    }
}
