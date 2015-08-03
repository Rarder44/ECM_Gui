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
        private static extern IntPtr ConvertToECM([MarshalAs(UnmanagedType.LPStr)]string Source, [MarshalAs(UnmanagedType.LPStr)]string Dest, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ConvertToIMG([MarshalAs(UnmanagedType.LPStr)]string Source, [MarshalAs(UnmanagedType.LPStr)]string Dest, [MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetIntErr(IntPtr ptr);

        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetStringErr(IntPtr ptr);

        [DllImport("ECM_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TryCloseFileStream();



        

        public static ReturnObj ECM(string Source,string Dest, ProgressCallback callbackPointer)
        {
            IntPtr t=ConvertToECM(Source, Dest, callbackPointer);
            return GetReturnObj(t);
        }
        public static ReturnObj UnECM(string Source, string Dest, ProgressCallback callbackPointer)
        {
            IntPtr t = ConvertToIMG(Source, Dest, callbackPointer);
            return GetReturnObj(t);
        }

        public static ReturnObj GetReturnObj(IntPtr ptr)
        {
            return new ReturnObj(GetIntErr(ptr), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(GetStringErr(ptr)));
        }

        public ECMService()
        {

        }
    }
}
