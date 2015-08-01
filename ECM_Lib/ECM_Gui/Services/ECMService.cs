using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECM_Gui.Services
{

    class ECMService
    {
        
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);


        [DllImport("ECM_Lib.dll")]
        public static extern int ConvertToECM(string Source, string Dest, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        [DllImport("ECM_Lib.dll")]
        public static extern void test();

        public ECMService()
        {

        }
    }
}
