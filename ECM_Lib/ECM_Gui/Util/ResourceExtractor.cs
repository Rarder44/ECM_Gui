using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECM_Gui.Util
{
    public static class ResourceExtractor
    {
        public static void ExtractResourceToFile(byte[] resource, string filename)
        {

            if (!System.IO.File.Exists(filename))
                using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                {
                    fs.Write(resource, 0, resource.Length);
                }
        }
    }
}
