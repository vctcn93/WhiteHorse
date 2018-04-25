using System;
using System.IO;
using System.Collections.Generic;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using AD = Autodesk.Revit.DB;
using Revit.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.GeometryConversion;

namespace Revit
{
    /// <summary>
    /// rebar
    /// </summary>
    public class WH_Rebar
    {
        /// <summary>
        /// 
        /// </summary>
        internal WH_Rebar()
        {
        }

        /// <summary>
        /// 通过curve创建钢筋
        /// </summary>
        /// <param name="curves">钢筋基线</param>
        /// <param name="hostElement">钢筋所在的主体元素</param>
        /// <param name="rebarStyle">钢筋样式，从RebarArguments中选择</param>
        /// <param name="rebarBarType">钢筋类型，从RebarArguments中选择</param>
        /// <param name="startHookOrientation">起始弯钩方向，从RebarArguments中选择</param>
        /// <param name="endHookOrientation">结束弯钩方向，从RebarArguments中选择</param>
        /// <param name="startHookType">起始弯钩类型，从RebarArguments中选择</param>
        /// <param name="endHookType">结束弯钩类型，从RebarArguments中选择</param>
        /// <param name="normal">钢筋所在平面方向</param>
        /// <returns name="Rebars">生成的钢筋</returns>
        /// <search>rebar</search>
        [MultiReturn(new string[] { "Rebars"})]
        public static Dictionary<string, object> ByCurves(List<Curve> curves, Element hostElement, string rebarStyle, Element rebarBarType, string startHookOrientation, string endHookOrientation, Element startHookType, Element endHookType, Vector normal)
        {
            if (rebarStyle == null)
            {
                throw new ArgumentNullException("请输入Rebar Style！");
            }
            if (rebarBarType == null)
            {
                throw new ArgumentNullException("请输入Rebar Bar Type！");
            }
            if (startHookOrientation == null)
            {
                throw new ArgumentNullException("请输入Start Hook Orientation！");
            }
            if (endHookOrientation == null)
            {
                throw new ArgumentNullException("请输入End Hook Orientation！");
            }
            if (normal == null)
            {
                throw new ArgumentNullException("请输入Vector！");
            }
            if (startHookType == null)
            {
                throw new ArgumentNullException("请输入Start Hook Type！");
            }
            if (endHookType == null)
            {
                throw new ArgumentNullException("请输入End Hook Type！");
            }

            List<AD.Curve> baseCurves = new List<AD.Curve>();
            foreach (var c in curves)
            {
                //if (typeof(c.GetType()) == typeof(Autodesk.DesignScript.Geometry.NurbsCurve))
                //{

                //}
                baseCurves.Add(c.ToRevitType());
                c.Dispose();
            }

            AD.Document doc = DocumentManager.Instance.CurrentDBDocument;

            TransactionManager.Instance.EnsureInTransaction(doc);

            var rebars = AD.Structure.Rebar.CreateFromCurves(
                                                    doc,
                                                    (AD.Structure.RebarStyle)Enum.Parse(typeof(AD.Structure.RebarStyle), rebarStyle),
                                                    (AD.Structure.RebarBarType)rebarBarType.InternalElement,
                                                    (AD.Structure.RebarHookType)startHookType.InternalElement,
                                                    (AD.Structure.RebarHookType)endHookType.InternalElement,
                                                    hostElement.InternalElement,
                                                    normal.ToXyz(),
                                                    baseCurves,
                                                    (AD.Structure.RebarHookOrientation)Enum.Parse(typeof(AD.Structure.RebarHookOrientation), startHookOrientation),
                                                    (AD.Structure.RebarHookOrientation)Enum.Parse(typeof(AD.Structure.RebarHookOrientation), endHookOrientation),
                                                    true,
                                                    true
                                                );

            TransactionManager.Instance.TransactionTaskDone();

            return new Dictionary<string, object> {
                {"Rebars", rebars.ToDSType(false) }
            };
        }


    }
}