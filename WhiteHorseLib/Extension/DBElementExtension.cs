using System;
using System.Collections.Generic;
using System.Text;
using Revit.Elements;

namespace WhiteHorseLib.Extension
{
    public static class DBElementExtension
    {
        /// <summary>
        /// 转换为Dynamo的element
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public static Element ToPyElement(this Autodesk.Revit.DB.Element ele)
        {
           return ele.ToDSType(true);
        }
    }
}
