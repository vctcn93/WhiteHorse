using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace WhiteHorseLib.Extension
{
    public static class ElementIdExtension
    {
        public static ElementId ToElementId(this int id)
        {
            return new ElementId(id);
        }

    }
}
