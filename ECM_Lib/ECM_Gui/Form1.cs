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
using ECM_Gui.Util;

namespace ECM_Gui
{
    public partial class Form1 : Form
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Form1()
        {
            InitializeComponent();
            GlobVar.InitGlobVar();

            foreach(Tuple<string,string> t in GlobVar.ResourceToExtract)
                ResourceExtractor.ExtractResourceToFile(t.Item1, t.Item2);
            
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();

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
            if (GlobVar.Status == StatusApp.Waiting)
            {

            }
            else
                MessageBox.Show("completare l'operazione in corso e riprovare");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (GlobVar.Status == StatusApp.Waiting)
                convertObjPanel1.Controls.Clear();
            else
                MessageBox.Show("completare l'operazione in corso e riprovare");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (GlobVar.Status == StatusApp.Waiting)
            {
                openFileDialog1.FileName = "";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog1.FileNames)
                    {
                        if (File.Exists(file))
                        {
                            convertObjPanel1.AddUnique(new ToConvertObj(file));
                        }
                    }
                }
            }
            else
                MessageBox.Show("completare l'operazione in corso e riprovare");
        }
    }
}
