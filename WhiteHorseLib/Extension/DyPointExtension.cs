using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Revit.GeometryConversion;
using Line = Autodesk.DesignScript.Geometry.Line;
using Point = Autodesk.DesignScript.Geometry.Point;
using DBline = Autodesk.Revit.DB.Line;
using DBcurve = Autodesk.Revit.DB.Curve;
//using DBpoint = Autodesk.Revit.DB.XYZ;

namespace WhiteHorseLib.Extension
{
    public static class DyPointExtension
    {
        /// <summary>
        /// 判断点相等
        /// </summary>
        /// <param name="po1"></param>
        /// <param name="po2"></param>
        /// <returns></returns>
        public static bool IsEqualsTo(this Point po1, Point po2)
        {
            return po1.IsAlmostEqualTo(po2);
        }
        /// <summary>
        /// 点投影到线
        /// </summary>
        /// <param name="po"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Point ProjectTo(this Point po, Line l)
        {
            XYZ dbpo = po.ToXyz();
            DBcurve dbline = l.ToRevitType();
            XYZ newpo = dbline.Project(dbpo).XYZPoint;
            return Point.ByCoordinates(newpo.X, newpo.Y, newpo.Z);
        }

    }
}
