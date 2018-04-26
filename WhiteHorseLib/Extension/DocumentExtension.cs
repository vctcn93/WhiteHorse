using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using RevitServices.Persistence;

namespace WhiteHorseLib.Extension
{
    public static class DocumentExtension
    {
        /// <summary>
        /// 当前文档的过滤器
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static FilteredElementCollector GetCollector(this Document doc)
        {
            return new FilteredElementCollector(doc);
        }
    }
}
