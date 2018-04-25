using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Utility;

namespace WhiteHorseCore
{
    static class ExternalFuncs
    {
        public static List<string> GetFileNames(string path)
        {
            string[] filepaths = Directory.GetFiles(path);
            List<string> outFiles = new List<string>();

            foreach (var item in filepaths)
            {
                bool b = Regex.IsMatch(item, @".\d{4}.rvt$");
                if (b)
                {
                    outFiles.Add(Path.GetFileName(item));
                }
            }
           
            return outFiles;
        }

       public static void ConvertLength(Document doc, string text, out double value)
        {
            value = (UnitFormatUtils.TryParse(doc.GetUnits(), UnitType.UT_Length, text, out double xValue)) ? xValue : 0;
        }
    }
}
