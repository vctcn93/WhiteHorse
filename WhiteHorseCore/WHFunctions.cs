using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;

namespace GeomTools
{
    /// <summary>
    /// 几何体相关方法
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public static class WHFunctions
    {
        /// <summary>
        /// 判断点是否重合
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static bool JudgeDuplicatePoints(Autodesk.DesignScript.Geometry.Point point1, Autodesk.DesignScript.Geometry.Point point2)
        {
            if (point1.IsAlmostEqualTo(point2))
            {
                return true;
            }
            //if (point1.X == point2.X && point1.Y == point2.Y && point1.Z == point2.Z)
            //{
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// 判断线是否重合
        /// </summary>
        /// <param name="crv1"></param>
        /// <param name="crv2"></param>
        /// <returns></returns>
        public static bool JudgeDuplicateCurves(Autodesk.DesignScript.Geometry.Curve crv1, Autodesk.DesignScript.Geometry.Curve crv2)
        {
            Autodesk.DesignScript.Geometry.Point pt1 = crv1.StartPoint;
            Autodesk.DesignScript.Geometry.Point pt2 = crv1.EndPoint;
            bool b1 = JudgeDuplicatePoints(pt1, crv2.StartPoint) &&
                      JudgeDuplicatePoints(pt2, crv2.EndPoint);
            bool b2 = JudgeDuplicatePoints(pt1, crv2.EndPoint) &&
                      JudgeDuplicatePoints(pt2, crv2.StartPoint);
            bool b3 = crv1.TangentAtParameter(0.3).IsParallel(crv2.TangentAtParameter(0.3)) || crv1.TangentAtParameter(0.3).IsParallel(crv2.TangentAtParameter(0.7));
            if ((b1 || b2) && b3)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断能否合并
        /// </summary>
        /// <param name="curve1"></param>
        /// <param name="curve2"></param>
        /// <returns></returns>
        public static bool CanJoin(Autodesk.DesignScript.Geometry.Curve curve1, 
            Autodesk.DesignScript.Geometry.Curve curve2)
        {
            Autodesk.DesignScript.Geometry.Point pt1 = curve1.StartPoint;
            Autodesk.DesignScript.Geometry.Point pt2 = curve1.EndPoint;
            List<Autodesk.DesignScript.Geometry.Point> crvPts = 
                new List<Autodesk.DesignScript.Geometry.Point>() { curve2.StartPoint, curve2.EndPoint };
            bool b = false;
            foreach (var p in crvPts)
            {
                if (JudgeDuplicatePoints(p, pt1) || JudgeDuplicatePoints(p, pt2))
                {
                    b = true;
                    break;
                }
            }
            return b;

        }

        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Autodesk.DesignScript.Geometry.Point GetPoint(Autodesk.DesignScript.Geometry.Curve curve, double t)
        {
            return curve.PointAtParameter(t);
        }
        /// <summary>
        /// 创建点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Autodesk.DesignScript.Geometry.Point CreatPoint(double x, double y, double z)
        {
            return Autodesk.DesignScript.Geometry.Point.ByCoordinates(x, y, z);
        }
    }
}
namespace FuncTools
{
    /// <summary>
    /// 方法库
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public static class Tools
    {
        /// <summary>
        /// 行列转换数组
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object[,] Rotate(object[,] array)
        {
            int x = array.GetUpperBound(0); //一维 
            int y = array.GetUpperBound(1); //二维 
            object[,] newArray = new object[y + 1, x + 1]; //构造转置二维数组
            for (int i = 0; i <= x; i++)
            {
                for (int j = 0; j <= y; j++)
                {
                    newArray[j, i] = array[i, j];
                }
            }
            return newArray;
        }

        /// <summary>
        /// 将二维列表(List)转换成二维数组，二维数组转置，然后将二维数组转换成列表
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static List<List<object>> Rotate(List<List<object>> original)
        {
            List<object>[] array = original.ToArray();
            List<List<object>> lists = new List<List<object>>();
            if (array.Length == 0)
            {
                throw new IndexOutOfRangeException("Index OutOf Range");
            }
            int x = array[0].Count;
            int y = original.Count;

            //将列表抓换成数组
            object[,] twoArray = new object[y, x];
            for (int i = 0; i < y; i++)
            {
                int j = 0;
                foreach (object s in array[i])
                {
                    twoArray[i, j] = s;
                    j++;
                }
            }

            object[,] newTwoArray = new object[x, y];
            newTwoArray = Rotate(twoArray);//转置

            //二维数组转换成二维List集合
            for (int i = 0; i < x; i++)
            {
                List<object> list = new List<object>();
                for (int j = 0; j < y; j++)
                {
                    list.Add(newTwoArray[i, j]);
                }
                lists.Add(list);
            }
            return lists;
        }
        


    }
}

