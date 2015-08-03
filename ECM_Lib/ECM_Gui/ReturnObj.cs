using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECM_Gui
{
    
    class ReturnObj
    {

        public int errnum = 0;
        public String errstr;
        public ReturnObj(int errnum, String errstr)
        {
            this.errnum = errnum;
            this.errstr = errstr;
        }
        public ReturnObj()
        {

        }


    }


}
