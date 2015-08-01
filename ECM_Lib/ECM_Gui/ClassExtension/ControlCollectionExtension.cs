using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECM_Gui.ClassExtension
{
    public static class ControlCollectionExtension
    {
        public static void AddVertical(this Control.ControlCollection self,Control c)
        {
            int y = 0;
            foreach(Control cc in self )
            {
                if (cc.Location.Y + cc.Height > y)
                    y = cc.Location.Y + cc.Height;
            }
            c.Location = new System.Drawing.Point(0, y);
            self.Add(c);
        }
    }
}
