using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECM_Gui
{
    class GlobVar
    {
        public static List<String> ImgExtension = new List<string>();
        public static List<String> ECMExtension = new List<string>();
        public static String DefaultImgExtension = "IMG";
        public static String DefaultECMExtension = "ECM";

        public static StatusApp Status =StatusApp.Starting;

        public static void InitGlobVar()
        {
            ImgExtension.Add("ISO");
            ImgExtension.Add("IMG");
            ImgExtension.Add("BIN");

            ECMExtension.Add("ECM");


            for (int i = 0; i < ImgExtension.Count; i++)
                ImgExtension[i] = ImgExtension[i].ToUpper();

            for (int i = 0; i < ECMExtension.Count; i++)
                ECMExtension[i] = ECMExtension[i].ToUpper();
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
