using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECM_Gui.ClassExtension
{
	public static class ListBoxObjectCollection
	{
		public static void AddUnique(this ListBox.ObjectCollection self,object obj)
		{
			foreach(object o in self)
            {
                if (o.Equals(obj))
                    return;
            }
            self.Add(obj);
		}
	}
}


