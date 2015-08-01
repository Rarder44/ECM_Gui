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


        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ConvertToECM([MarshalAs(UnmanagedType.LPStr)]string Source, [MarshalAs(UnmanagedType.LPStr)]string Dest, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TryCloseFileStream();



        public ECMService()
        {

        }
    }
}
