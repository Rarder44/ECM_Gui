using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECM_Gui.ClassExtension;

namespace ECM_Gui
{
    public class ToConvertObj
    {
        ConvertAction _Action = ConvertAction.NoAction;
        FileInfo _Source;
        FileInfo _Dest;

         public ConvertAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
        public FileInfo Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
        public FileInfo Dest
        {
            get { return _Dest; }
            set { _Dest = value; }
        }


        public ToConvertObj(String Source, String Dest, ConvertAction Action)
        {
            this._Source = new FileInfo(Source);
            this._Dest = new FileInfo(Dest);
            this._Action = Action;
        }
        public ToConvertObj(String Source)
        {
            this._Source = new FileInfo(Source);
            SetOutputFromSource();
            SetActionFromSourceExtension();
        }

        
        public override String ToString()
        {
            return _Source.Name + " -> " + _Dest.Name;
            
        }
        public override bool Equals(object obj)
        {
            if(obj is ToConvertObj)
            {
                return ((ToConvertObj)obj)._Source.FullName == this._Source.FullName;
            }
            return false;
        }

        /// <summary>
        /// Setta l'azione in base all'esenzione del Source
        /// </summary>
        public void SetActionFromSourceExtension()
        {
            _Action = GetActionFromSourceExtension(_Source.Extension);
        }
        /// <summary>
        /// Setta l'estensione dell'output in base al Source
        /// </summary>
        public void SetOutputExtensionFromSourceExtension()
        {
            _Dest = new FileInfo(Path.Combine(_Dest.DirectoryName + "/", "/" + _Dest.Name, CalculateOutpuExtensionFromSourceExtension(_Source.Extension)));
        }

        /// <summary>
        /// Setta FileName e estensione dell'output in base al Source
        /// </summary>
        public void SetOutputFromSource()
        {
            _Dest = new FileInfo(Path.Combine(_Source.DirectoryName, _Source.GetFileNameWithoutExtension() + "." + CalculateOutpuExtensionFromSourceExtension(_Source.Extension)));
        }







        /// <summary>
        /// Calcola l'estensione di Output in Base All'estenzione data
        /// </summary>
        /// <param name="extension"> estenzione senza . introduttivo ( esempio:  "EXE" non ".EXE" )</param>
        /// <returns></returns>
        public static String CalculateOutpuExtensionFromSourceExtension(String extension)
        {
            if (extension.StartsWith("."))
                extension = extension.Remove(0, 1);

            if (GlobVar.ImgExtension.Contains((extension.ToUpper())))
            {
                return GlobVar.DefaultECMExtension;
            }
            else if (GlobVar.ECMExtension.Contains((extension.ToUpper())))
            {
                return GlobVar.DefaultImgExtension;
            }
            return "";
        }



        /// <summary>
        /// Restituisce l'azione di conversione in base all'estenzione data ( Source )
        /// </summary>
        /// <param name="extension"> estenzione senza . introduttivo ( esempio:  "EXE" non ".EXE" )</param>
        /// <returns></returns>
        public static ConvertAction GetActionFromSourceExtension(String extension)
        {
            if (extension.StartsWith("."))
                extension = extension.Remove(0, 1);
            if (GlobVar.ImgExtension.Contains((extension.ToUpper())))
            {
                return ConvertAction.ToECM;
            }
            else if (GlobVar.ECMExtension.Contains((extension.ToUpper())))
            {
                return ConvertAction.ToIMG;
            }
            return ConvertAction.NoAction;
        }
    }
    public enum ConvertAction { ToECM, ToIMG, NoAction };

}
