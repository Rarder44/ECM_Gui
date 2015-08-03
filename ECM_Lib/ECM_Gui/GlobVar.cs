using ECM_Gui.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECM_Gui
{
    class GlobVar
    {
        public static List<String> ImgExtensions = new List<string>();
        public static List<String> ECMExtensions = new List<string>();
        public static String DefaultImgExtension = "IMG";
        public static String DefaultECMExtension = "ECM";

        public static StatusApp Status =StatusApp.Starting;

        public static List<Tuple<byte[], string>> ResourceToExtract = new List<Tuple<byte[], string>>();


        public static void InitGlobVar()
        {
            ImgExtensions.Add("ISO");
            ImgExtensions.Add("IMG");
            ImgExtensions.Add("BIN");

            ECMExtensions.Add("ECM");


            for (int i = 0; i < ImgExtensions.Count; i++)
                ImgExtensions[i] = ImgExtensions[i].ToUpper();

            for (int i = 0; i < ECMExtensions.Count; i++)
                ECMExtensions[i] = ECMExtensions[i].ToUpper();

            ResourceToExtract.Add(new Tuple<byte[], string>(ECM_Gui.Properties.Resources.ECM_Lib, "ECM_Lib.dll"));

        } 
    }


    public enum StatusApp
    {
        Starting,
        Waiting,
        OnConvertSingle,
        OnConvertMultiple

    }
}
