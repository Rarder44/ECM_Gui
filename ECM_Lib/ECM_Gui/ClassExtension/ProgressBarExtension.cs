using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECM_Gui.ClassExtension
{
    public static class ProgressBarExtension
    {

        public static void SetValueInvoke(this ProgressBar p, int Value)
        {
            if (p.InvokeRequired)
                p.Invoke((MethodInvoker)delegate { p.SetValueInvoke(Value); });
            else
                p.Value = Value;
        }
    }
}
