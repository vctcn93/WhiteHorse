using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Revit.Elements;
using Element = Revit.Elements.Element;

namespace WhiteHorseLib.Extension
{
    public static class PyElementExtension
    {
        //根据id获取element
        public static Element ToDyElement(this int Id)
        {
            return ElementSelector.ByElementId(Id);
        }
        //根据elementid获取element
        public static Element ToDyElement(this ElementId id)
        {
            return ElementSelector.ByElementId(id.IntegerValue);
        }
        


    }
}
