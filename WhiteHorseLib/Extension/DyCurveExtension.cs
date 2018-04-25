using System;
using System.Collections.Generic;
using System.Text;
using DyCurve= Autodesk.DesignScript.Geometry.Curve;
namespace WhiteHorseLib.Extension
{
    public static class DyCurveExtension
    {
        /// <summary>
        /// 判断两条曲线相等(通过两条曲线上的点来判断精确度越高判断越准确
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="curve2"></param>
        /// <param name="Accuracy"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualsTo(this DyCurve curve,DyCurve curve2,int Accuracy=100)
        {
            //判断从0到1的 分段点上  点相等 而且 所在点的切线平行
            if (Accuracy==0)
            {
                throw new Exception("wrong parameter");
            }
            for (int i = 0; i <= Accuracy; i++)
            {
                double accuracyPercent = i==0?0:i / Accuracy;
                if (!curve.PointAtParameter(accuracyPercent).IsAlmostEqualTo(curve2.PointAtParameter(accuracyPercent))||
                    curve.TangentAtParameter(accuracyPercent).IsParallel(curve2.TangentAtParameter(accuracyPercent))
                    )
                {
                    return false;
                }
            }
            return true;
        }


    }
}
