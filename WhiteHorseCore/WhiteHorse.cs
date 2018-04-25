using System;
using System.IO;
using System.Collections.Generic;
using WF = System.Windows.Forms;

using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using AD = Autodesk.Revit.DB;
using Revit.Elements;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.GeometryConversion;
using Autodesk.Revit.DB.Electrical;

using QRCoder;
using System.Windows.Media;
using System.Net;
using System.Text;

namespace Geometry
{
    #region Curves类
    /// <summary>
    /// Curves类
    /// </summary>
    public class WH_Curve
    {
        internal WH_Curve() { }

        /// <summary>
        /// 合并多条Curve为PolyCurve
        /// 支持包含独立Curve，不会报错
        /// </summary>
        /// <param name="curves">需合并的曲线列表</param>
        /// <returns name="PolyCurve">合并后的结果</returns>
        /// <returns name="AloneCurve">独立的Curve</returns>
        /// <search>curve,join,merge</search>
        [MultiReturn(new string[] { "PolyCurve", "AloneCurve" })]
        public static Dictionary<string, object> MergeCurve(List<Curve> curves)
        {
            List<Curve> curveSource = RemoveDuplicates(curves);
            List<Curve> allCurves = new List<Curve>();
            List<Curve> mergedCurves = new List<Curve>();
            List<Curve> alongCurves = new List<Curve>();

            while (curveSource.Count != 0)
            {
                Curve first = curveSource[0];
                curveSource.RemoveAt(0);
                int n = 0;
                while (n < curveSource.Count)
                {
                    if (GeomTools.WHFunctions.CanJoin(first, curveSource[n]))
                    {
#pragma warning disable CS0618 // 类型或成员已过时
                        first = first.Join(curveSource[n]);
#pragma warning restore CS0618 // 类型或成员已过时
                        curveSource.RemoveAt(n);
                        n = 0;
                        continue;
                    }
                    n++;
                }
                allCurves.Add(first);
            }
            foreach (var item in allCurves)
            {
                if (item is PolyCurve)
                {
                    mergedCurves.Add(item);
                }
                else
                {
                    alongCurves.Add(item);
                }
            }

            return new Dictionary<string, object>
            {
                {"PolyCurve",mergedCurves },
                {"AloneCurve", alongCurves }
            };
        }

        /// <summary>
        /// 分析曲线
        /// </summary>
        /// <param name="curve">曲线</param>
        /// <param name="parameter">指定parameter</param>
        /// <returns name="Point">指定parameter处的点</returns>
        /// <returns name="Tangent">指定parameter处的曲线的垂直向量</returns>
        /// <returns name="Plane">指定parameter处的Plane</returns>
        /// <search>curve,evaluate,normal,point,pointatparameter,planeatparameter,normalatparameter</search>
        [MultiReturn(new string[] { "Point", "Tangent", "Plane" })]
        public static Dictionary<string, object> EvaluateCurve(Curve curve, double parameter = 0.5)
        {
            return new Dictionary<string, object>
            {
                {"Point", curve.PointAtParameter(parameter) },
                {"Tangent", curve.NormalAtParameter(parameter) },
                {"Plane", curve.PlaneAtParameter(parameter) }
            };
        }

        /// <summary>
        /// 获取Curve的起始点
        /// </summary>
        /// <param name="curve"></param>
        /// <returns name="StartPoint">起点</returns>
        /// <returns name="EndPoint">终点</returns>
        /// <search>point,curve,startpoint,endpoint</search>
        [MultiReturn(new string[] { "StartPoint", "EndPoint" })]
        public static Dictionary<string, object> GetPoints(Curve curve)
        {
            return new Dictionary<string, object>
            {
                {"StartPoint", curve.StartPoint },
                {"EndPoint", curve.EndPoint }
            };
        }

        /// <summary>
        /// 沿Surface生成指定数量的Curve
        /// </summary>
        /// <param name="surface">基面</param>
        /// <param name="number">线数量</param>
        /// <param name="accuracy">精度</param>
        /// <param name="direction">方向，0为U方向，1为V方向</param>
        /// <returns></returns>
        public static List<Curve> ByAlongSurface(Surface surface, int number, int accuracy = 3, int direction = 0)
        {
            double[] parameters = new double[accuracy];
            for (int i = 0; i < accuracy; i++)
            {
                if (i == accuracy)
                {
                    parameters[i] = 1;
                }
                else
                {
                    parameters[i] = i * (1.0 / (accuracy - 1));
                }
            }
            List<Curve> baseCurves = new List<Curve>();
            foreach (var i in parameters)
            {
                var tmp = surface.GetIsoline(1 - direction, i);
                baseCurves.Add(tmp);
                tmp.Dispose();
            }
            // 线数量
            double[] ts = new double[number];
            for (int i = 0; i < number; i++)
            {
                if (i == number - 1)
                {
                    ts[i] = 1;
                }
                else
                {
                    ts[i] = i * (1.0 / (number - 1));
                }
            }
            List<Curve> outCurves = new List<Curve>();
            foreach (var t in ts)
            {
                List<Point> pts = new List<Point>();
                foreach (var crv in baseCurves)
                {
                    pts.Add(crv.PointAtParameter(t));
                }
                var tmp = NurbsCurve.ByPoints(pts);
                outCurves.Add(tmp);
                tmp.Dispose();
            }
            return outCurves;
        }

        /// <summary>
        /// 去除重复曲线
        /// </summary>
        /// <param name="curves">线列表</param>
        /// <returns>返回的曲线</returns>
        /// <search>curve, duplicate, remove</search>
        public static List<Curve> RemoveDuplicates(List<Curve> curves)
        {
            Curve firstCurve = curves[0];
            List<Curve> outCurves = new List<Curve>() { firstCurve };
            // 除第一个外的Curve
            List<Curve> tmpCurves = new List<Curve>();
            for (int i = 1; i < curves.Count; i++)
            {
                tmpCurves.Add(curves[i]);
            }

            foreach (var crv in tmpCurves)
            {
                bool b = true;
                foreach (var i in outCurves)
                {
                    if (GeomTools.WHFunctions.JudgeDuplicateCurves(crv, i))
                    {
                        b = false;
                    }
                }
                if (b)
                {
                    outCurves.Add(crv);
                }
            }
            return outCurves;
        }

        /// <summary>
        /// 在两条线之间生成指定数量的曲线
        /// </summary>
        /// <param name="curve1">第一条</param>
        /// <param name="curve2">第二条</param>
        /// <param name="number">数量</param>
        /// <param name="accuracy">精度，如是直线和忽略</param>
        /// <param name="contain">是否包含起始</param>
        /// <returns>生成的曲线</returns>
        public static List<Curve> ByBetweenTwoCurves(Curve curve1, Curve curve2, int number, int accuracy = 2, bool contain = false)
        {
            List<Curve> outCurves = new List<Curve>();
            int n = number;
            if (!contain)
            {
                n = number + 2;
            }
            double[] parameters = new double[n];
            for (int i = 0; i < n; i++)
            {
                if (i == n)
                {
                    parameters[i] = 1;
                }
                else
                {
                    parameters[i] = i * (1.0 / (n - 1));
                }
            }
            // 如果为Line
            if ((curve1 as Line) != null && (curve2 as Line) != null)
            {
                if (!DetermineDirection(curve1, curve2))
                {
                    curve2 = curve2.Reverse();
                }
                var line1 = Line.ByStartPointEndPoint(curve1.StartPoint, curve2.StartPoint);
                var line2 = Line.ByStartPointEndPoint(curve1.EndPoint, curve2.EndPoint);
                List<Point> ptList1 = new List<Point>();
                List<Point> ptList2 = new List<Point>();

                foreach (var i in parameters)
                {
                    ptList1.Add(line1.PointAtParameter(i));
                    ptList2.Add(line2.PointAtParameter(i));
                }

                for (int i = 0; i < n; i++)
                {
                    var crv = Line.ByStartPointEndPoint(ptList1[i], ptList2[i]);
                    outCurves.Add(crv);
                }
            }
            // 如果为PolyCurve
            else if ((curve1 as PolyCurve) != null || (curve2 as PolyCurve) != null)
            {
                throw new Exception("暂不支持PolyCurve!");
            }
            else
            {
                List<Curve> crossCurves = new List<Curve>() { curve1, curve2 };
                Surface surface = Surface.ByLoft(crossCurves);
                outCurves = ByAlongSurface(surface, number, accuracy, 0);
                surface.Dispose();
            }
            if (!contain)
            {
                outCurves.RemoveAt(0);
                outCurves.RemoveAt(outCurves.Count - 1);
            }
            return outCurves;
        }

        /// <summary>
        /// 判断Curve是否差不多方向
        /// 夹角小于90°判定为同向，否则判定为反向
        /// </summary>
        /// <param name="curve">原始curve</param>
        /// <param name="other">要判断的curve</param>
        /// <returns>同向为True，反向为False</returns>
        /// <search>curve, line, vector</search>
        public static bool DetermineDirection(Curve curve, Curve other)
        {
            Point pt1 = curve.StartPoint;
            Point pt2 = curve.EndPoint;
            Vector vector1 = Vector.ByTwoPoints(pt1, pt2);

            Point pt3 = other.StartPoint;
            Point pt4 = other.EndPoint;
            Vector vector2 = Vector.ByTwoPoints(pt3, pt4);

            if (vector1.AngleWithVector(vector2) <= 90)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    #region PolyCurve类
    /// <summary>
    /// PolyCurve
    /// </summary>
    public class WH_PolyCurve
    {
        internal WH_PolyCurve() { }

        /// <summary>
        /// 分析PolyCurve，返回组成的线和点等
        /// </summary>
        /// <param name="polyCurve">需分析的polycurve</param>
        /// <returns name="Curves">polycurve的组成curve</returns>
        /// <returns name="Points">polycurve的顶点</returns>
        /// <returns name="NumOfCurves">polycurve的curve数量</returns>
        /// <search>polycurve,point,explode,curve</search>
        [MultiReturn(new string[] { "Curves", "Points", "NumOfCurves" })]
        public static Dictionary<string, object> DeconstructePolyCurve(PolyCurve polyCurve)
        {
            Curve[] curves = polyCurve.Curves();
            List<Point> points = new List<Point>
            {
                curves[0].StartPoint
            };
            foreach (var item in curves)
            {
                points.Add(item.EndPoint);
            }

            return new Dictionary<string, object>
            {
                {"Curves",curves },
                {"Points",points },
                {"NumOfCurves",curves.Length }
            };
        }
    }
    #endregion

    #region Points类
    /// <summary>
    /// Points类
    /// </summary>
    public class WH_Point
    {
        internal WH_Point() { }

        /// <summary>
        /// 去除重复点
        /// </summary>
        /// <param name="points">原始点</param>
        /// <returns>处理后的点</returns>
        /// <search>point,duplicate,remove,delete</search>
        public static List<Point> RemoveDuplicates(List<Point> points)
        {
            List<Point> outPoints = new List<Point>() { points[0] };
            points.RemoveAt(0);
            foreach (var p in points)
            {
                bool b = true;
                foreach (var i in outPoints)
                {
                    if (GeomTools.WHFunctions.JudgeDuplicatePoints(p, i))
                    {
                        b = false;
                        break;
                    }
                }
                if (b)
                {
                    outPoints.Add(p);
                }
            }
            return outPoints;
        }

        /// <summary>
        /// 获取点的三个坐标
        /// </summary>
        /// <param name="point">点</param>
        /// <returns name="X">X坐标值</returns>
        /// <returns name="Y">Y坐标值</returns>
        /// <returns name="Z">Z坐标值</returns>
        /// <search>point,deconstruct,x,y,z</search>
        [MultiReturn(new string[] { "X", "Y", "Z" })]
        public static Dictionary<string, object> DeconstructPoint(Point point)
        {
            return new Dictionary<string, object>
            {
                { "X", point.X },
                { "Y", point.Y },
                { "Z", point.Z }
            };
        }

        /// <summary>
        /// 沿曲线排序点
        /// </summary>
        /// <param name="points">要排序的点</param>
        /// <param name="curve">用来参照的曲线</param>
        /// <param name="reverse">指定是否反转点序</param>
        /// <returns name="Points">排序后的点</returns>
        /// <returns name="Indices">原始点的索引</returns>
        /// <search>point,along,sort,curve</search>
        [MultiReturn(new string[] { "Points", "Indices" })]
        public static Dictionary<string, object> SortAlongCurve(List<Point> points, Curve curve, bool reverse = false)
        {
            List<Point> closePoints = new List<Point>();
            List<double> closeParam = new List<double>();
            List<double> otherParam = new List<double>();
            List<int> indices = new List<int>();
            int n = 0;

            foreach (var pt in points)
            {
                Point point = curve.ClosestPointTo(pt);
                double param = curve.ParameterAtPoint(point);
                closePoints.Add(point);
                closeParam.Add(param);
                otherParam.Add(param);
                indices.Add(n);
                n++;
                point.Dispose();
            }
            Point[] outPoints = points.ToArray();
            double[] keys = closeParam.ToArray();
            double[] keys2 = otherParam.ToArray();
            int[] outIndices = indices.ToArray();

            Array.Sort<double, Point>(keys, outPoints);
            Array.Sort<double, int>(keys2, outIndices);
            if (reverse)
            {
                Array.Reverse(outPoints);
                Array.Reverse(outIndices);
            }
            return new Dictionary<string, object>
            {
                {"Points",outPoints },
                {"Indices",outIndices }
            };
        }

    }
    #endregion

    #region Surface类
    /// <summary>
    /// Surface
    /// </summary>
    public class WH_Surface
    {
        internal WH_Surface() { }

        /// <summary>
        /// 返回surface的边线和顶点
        /// </summary>
        /// <param name="surface">需要分析的面</param>
        /// <returns name="PerimeterCurves">边线</returns>
        /// <returns name="Points">顶点</returns>
        /// <search>perimeter,surface,edge</search>
        [MultiReturn(new string[] { "PerimeterCurves", "Points" })]
        public static Dictionary<string, object> DeconstructSurface(Surface surface)
        {
            Curve[] perimeterCurves;
            List<Point> points = new List<Point>();

            perimeterCurves = surface.PerimeterCurves();
            foreach (var item in perimeterCurves)
            {
                points.Add(item.StartPoint);
            }

            return new Dictionary<string, object>
            {
                {"PerimeterCurves", perimeterCurves },
                {"Points", points }
            };
        }

    }
    #endregion

    #region Mesh类
    /// <summary>
    /// Mesh
    /// </summary>
    public class WH_Mesh
    {
        internal WH_Mesh() { }

        /// <summary>
        /// 将Mesh转化为PolySurface
        /// </summary>
        /// <param name="mesh">网格</param>
        /// <returns>转化后的PolySurface</returns>
        public static PolySurface ConvertToPolySurface(Mesh mesh)
        {
            List<Surface> surfaces = new List<Surface>();
            Point[] vertexs = mesh.VertexPositions;
            IndexGroup[] indexGroup = mesh.FaceIndices;
            foreach (var item in indexGroup)
            {
                if (item.Count == 3)
                {
                    Point pt1 = vertexs[item.A];
                    Point pt2 = vertexs[item.B];
                    Point pt3 = vertexs[item.C];
                    Surface surface = Surface.ByPerimeterPoints(new Point[] { pt1, pt2, pt3 });
                    surfaces.Add(surface);
                    pt1.Dispose();
                    pt2.Dispose();
                    pt3.Dispose();
                    surface.Dispose();
                }
                else
                {
                    Point pt1 = vertexs[item.A];
                    Point pt2 = vertexs[item.B];
                    Point pt3 = vertexs[item.C];
                    Point pt4 = vertexs[item.D];

                    Surface surface = Surface.ByPerimeterPoints(new Point[] { pt1, pt2, pt3, pt4 });
                    surfaces.Add(surface);
                    pt1.Dispose();
                    pt2.Dispose();
                    pt3.Dispose();
                    surface.Dispose();
                }
            }
            return PolySurface.ByJoinedSurfaces(surfaces);
        }
    }
    #endregion
}

namespace Revit
{
    #region 过滤器
    /// <summary>
    /// Filter
    /// </summary>
    public class WH_Filter
    {
        internal WH_Filter()
        {
        }

        /// <summary>
        /// 从选择的元素中过滤出指定类别的元素
        /// </summary>
        /// <param name="elements">待过滤的元素(Elements)</param>
        /// <param name="category">用来过滤的类别</param>
        /// <returns name="Elements">筛选出的元素(Elements)</returns>
        /// <returns name="Amount">筛选出的元素总数</returns>
        /// <search>
        /// elementfilter, filter, category, wh
        /// </search>
        [MultiReturn(new string[]
        {
            "Elements",
            "Amount"
        })]
        public static Dictionary<string, object> ByCategory(List<Element> elements, Category category)
        {
            List<Element> list = new List<Element>();
            foreach (Element current in elements)
            {
                Category getCategory = current.GetCategory;
                bool flag = getCategory.Name == category.Name;
                if (flag)
                {
                    list.Add(current);
                }
            }
            return new Dictionary<string, object>
            {
                {
                    "Elements",
                    list
                },
                {
                    "Amount",
                    list.Count
                }
            };
        }

        /// <summary>
        /// 从选择的元素中过滤出指定名称的元素
        /// </summary>
        /// <param name="elements">待过滤的元素(Elements)</param>
        /// <param name="name">用来过滤的名称</param>
        /// <returns name="Elements">筛选出的元素(Elements)</returns>
        /// <returns name="Amount">筛选出的元素总数</returns>
        /// <search>
        /// elementfilter, filter, category, wh
        /// </search>
        [MultiReturn(new string[]
        {
            "Elements",
            "Amount"
        })]
        public static Dictionary<string, object> ByName(List<Element> elements, string name)
        {
            List<Element> list = new List<Element>();
            foreach (Element current in elements)
            {
                bool flag = current.Name == name;
                if (flag)
                {
                    list.Add(current);
                }
            }
            return new Dictionary<string, object>
            {
                {
                    "Elements",
                    list
                },
                {
                    "Amount",
                    list.Count
                }
            };
        }

        /// <summary>
        /// 从选择的元素中过滤出名称包含指定字符串的元素
        /// </summary>
        /// <param name="elements">待过滤的元素(Elements)</param>
        /// <param name="str">用来过滤的字符串</param>
        /// <returns name="Elements">筛选出的元素(Elements)</returns>
        /// <returns name="Amount">筛选出的元素总数</returns>
        /// <search>
        /// elementfilter, filter, category, wh
        /// </search>
        [MultiReturn(new string[]
        {
            "Elements",
            "Amount"
        })]
        public static Dictionary<string, object> ByNameContain(List<Element> elements, string str)
        {
            List<Element> list = new List<Element>();
            foreach (Element current in elements)
            {
                bool flag = current.Name.Contains(str);
                if (flag)
                {
                    list.Add(current);
                }
            }
            return new Dictionary<string, object>
            {
                {
                    "Elements",
                    list
                },
                {
                    "Amount",
                    list.Count
                }
            };
        }

        /// <summary>
        /// 从选择的元素中过滤出指定参数值(字符串)的元素
        /// </summary>
        /// <param name="elements">待过滤的元素(Elements)</param>
        /// <param name="parameter">元素参数</param>
        /// <param name="value">参数值</param>
        /// <returns name="Elements">筛选出的元素(Elements)</returns>
        /// <returns name="Amount">筛选出的元素总数</returns>
        /// <search>
        /// elementfilter, filter, category, wh
        /// </search>
        [MultiReturn(new string[]
        {
            "Elements",
            "Amount"
        })]
        public static Dictionary<string, object> ByParamValue(List<Element> elements, string parameter, string value)
        {
            List<Element> list = new List<Element>();
            foreach (Element current in elements)
            {
                string a = current.GetParameterValueByName(parameter).ToString();
                bool flag = a == value;
                if (flag)
                {
                    list.Add(current);
                }
            }
            return new Dictionary<string, object>
            {
                {
                    "Elements",
                    list
                },
                {
                    "Amount",
                    list.Count
                }
            };
        }

        /// <summary>
        /// 从选择的元素中过滤出参数值(字符串)包含指定字符串的元素
        /// </summary>
        /// <param name="elements">待过滤的元素(Elements)</param>
        /// <param name="parameter">元素参数</param>
        /// <param name="str">搜索的字符串</param>
        /// <returns name="Elements">筛选出的元素(Elements)</returns>
        /// <returns name="Amount">筛选出的元素总数</returns>
        /// <search>
        /// elementfilter, filter, category, wh
        /// </search>
        [MultiReturn(new string[]
        {
            "Elements",
            "Amount"
        })]
        public static Dictionary<string, object> ByParamValueContain(List<Element> elements, string parameter, string str)
        {
            List<Element> list = new List<Element>();
            foreach (Element current in elements)
            {
                string text = current.GetParameterValueByName(parameter).ToString();
                bool flag = text.Contains(str);
                if (flag)
                {
                    list.Add(current);
                }
            }
            return new Dictionary<string, object>
            {
                {
                    "Elements",
                    list
                },
                {
                    "Amount",
                    list.Count
                }
            };
        }
    }
    #endregion

    #region 洞口
    /// <summary>
    /// Opening
    /// </summary>
    public class WH_Opening
    {
        internal WH_Opening()
        {
        }

        /// <summary>
        /// 在指定板(屋顶，楼板，天花板)上开洞
        /// 支持圆和各种曲线，除了圆，其它曲线
        /// 请转为多段线且需闭合!
        /// </summary>
        /// <param name="host">需开洞的楼板</param>
        /// <param name="curve">洞口边线</param>
        /// <return name="Opening">生成的洞口</return>
        /// <search>opening, open, floor, wh</search>
        [MultiReturn(new string[]
        {
            "Opening"
        })]
        public static Dictionary<string, Element> ByHostAndCurve(Element host, Curve curve)
        {
            AD.Document currentDBDocument = DocumentManager.Instance.CurrentDBDocument;
            AD.Element internalElement = host.InternalElement;
            AD.CurveArray curveArray = new AD.CurveArray();
            try
            {

                curveArray.Append(ProtoToRevitCurve.ToRevitType(curve, true));
            }
            catch (Exception)
            {
                PolyCurve polyCurve = (PolyCurve)curve;
                Curve[] array = polyCurve.Curves();
                for (int i = 0; i < array.Length; i++)
                {
                    Curve crv = array[i];
                    curveArray.Append(ProtoToRevitCurve.ToRevitType(crv, true));
                }
            }
            TransactionManager.Instance.EnsureInTransaction(currentDBDocument);
            AD.Opening opening = currentDBDocument.Create.NewOpening(internalElement, curveArray, true);
            Element value = ElementWrapper.ToDSType(opening, false);
            TransactionManager.Instance.TransactionTaskDone();
            return new Dictionary<string, Element>
            {
                {
                    "Opening",
                    value
                }
            };
        }
    }
    #endregion

    #region 族
    /// <summary>
    /// Family
    /// </summary>
    public class WH_Family
    {
        internal WH_Family()
        {
        }

        /// <summary>
        /// 在主体元素上创建族实例，比如在墙上创建门窗等
        /// </summary>
        /// <param name="point">族实例的插入点</param>
        /// <param name="familyType">要插入的族类型</param>
        /// <param name="host">主体元素</param>
        /// <returns name="FamilyInstance">生成的族实例</returns>
        /// <search>family, door, window, wh</search>
        [MultiReturn(new string[]
        {
            "FamilyInstance"
        })]
        public static Dictionary<string, object> InstanceByPointAndHost(Point point, FamilyType familyType, Element host)
        {
            AD.Document currentDBDocument = DocumentManager.Instance.CurrentDBDocument;
            AD.XYZ xYZ = GeometryPrimitiveConverter.ToXyz(point, true);
            AD.FamilySymbol familySymbol = (AD.FamilySymbol)(familyType.InternalElement);
            if (!familySymbol.IsActive)
            {
                familySymbol.Activate();
            }
            AD.Element element = host.InternalElement;
            AD.Level level = element.Document.GetElement(element.LevelId) as AD.Level;
            TransactionManager.Instance.EnsureInTransaction(currentDBDocument);
            AD.FamilyInstance element2 = currentDBDocument.Create.NewFamilyInstance(xYZ, familySymbol, element, level, 0);
            var ele = element2.ToDSType(false);
            TransactionManager.Instance.TransactionTaskDone();
            return new Dictionary<string, object>
            {
                {
                    "FamilyInstance",
                    ele
                }
            };
        }

        /// <summary>
        /// 改族名或族类型名
        /// </summary>
        /// <param name="element">族类型(FamilyType)或 族(Family)</param>
        /// <param name="name">想要修改成的字符串</param>
        /// <returns name="Element">修改完返回输入的元素</returns>
        /// <search>family, familytype, rename, name, wh, yipi</search>
        [MultiReturn(new string[]
        {
            "Element"
        })]
        public static Dictionary<string, Element> ChangeName(Element element, string name)
        {
            AD.Element internalElement = element.InternalElement;
            AD.Document currentDBDocument = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(currentDBDocument);
            internalElement.Name = name;
            TransactionManager.Instance.TransactionTaskDone();
            return new Dictionary<string, Element>
            {
                {
                    "Element",
                    element
                }
            };
        }
    }
    #endregion


}

namespace Display
{
    /// <summary>
    /// 
    /// </summary>
    public static class Tag
    {
        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="text">要显示的文字</param>
        /// <param name="point">文字基点</param>
        /// <param name="size">缩放大小</param>
        /// <returns></returns>
        public static IEnumerable<Curve> ByTextPointAndSize(string text, Point point, double size)
        {
            //http://msdn.microsoft.com/en-us/library/ms745816(v=vs.110).aspx

            var crvs = new List<Curve>();

            var font = new System.Windows.Media.FontFamily("Arial");
            var fontStyle = System.Windows.FontStyles.Normal;
            var fontWeight = System.Windows.FontWeights.Medium;

            //if (Bold == true) fontWeight = FontWeights.Bold;
            //if (Italic == true) fontStyle = FontStyles.Italic;


            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(
                    font,
                    fontStyle,
                    fontWeight,
                    System.Windows.FontStretches.Normal),
                1,
                System.Windows.Media.Brushes.Black
                );


            var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
            foreach (var figure in textGeometry.GetFlattenedPathGeometry().Figures)
            {
                var init = figure.StartPoint;
                var a = figure.StartPoint;
                System.Windows.Point b;
                foreach (var segment in figure.GetFlattenedPathFigure().Segments)
                {
                    var lineSeg = segment as LineSegment;
                    if (lineSeg != null)
                    {
                        b = lineSeg.Point;
                        var crv = LineBetweenPoints(point, size, a, b);
                        a = b;
                        crvs.Add(crv);
                    }

                    var plineSeg = segment as PolyLineSegment;
                    if (plineSeg != null)
                    {
                        foreach (var segPt in plineSeg.Points)
                        {
                            var crv = LineBetweenPoints(point, size, a, segPt);
                            a = segPt;
                            crvs.Add(crv);
                        }
                    }

                }
            }

            return crvs;
        }

        private static Line LineBetweenPoints(Point origin, double scale, System.Windows.Point a, System.Windows.Point b)
        {
            var pt1 = Point.ByCoordinates((a.X * scale) + origin.X, ((-a.Y + 1) * scale) + origin.Y, origin.Z);
            var pt2 = Point.ByCoordinates((b.X * scale) + origin.X, ((-b.Y + 1) * scale) + origin.Y, origin.Z);
            var crv = Line.ByStartPointEndPoint(pt1, pt2);
            return crv;
        }
    }
}

namespace QRcode
{
    /// <summary>
    /// 二维码
    /// </summary>
    public class QRcode
    {
        internal QRcode() { }

        /// <summary>
        /// 从指定字符串生成二维码
        /// </summary>
        /// <param name="content">二维码内容</param>
        /// <param name="imagePath">生成二维码存放的路径，如"D:\壹匹BIM\"</param>
        /// <param name="imageName">二维码图像名称，\n应以图片格式后缀，如：\n"白马.png"</param>
        /// <param name="show">生成完是否预览二维码</param>
        /// <returns name="ImagePath">二维码文件路径</returns>
        /// <search>string,qr,qrcode,image</search>
        [MultiReturn(new string[] { "ImagePath" })]
        public static Dictionary<string, object> MakeQR(string content, string imagePath, string imageName, bool show)
        {
            string filePath;
            if (!Directory.Exists(imagePath))
            {
                try
                {
                    Directory.CreateDirectory(imagePath);
                    filePath = Path.Combine(imagePath, imageName);
                }
                catch { throw new Exception("请检查imagePath是否为合法的路径格式！"); }
            }
            else
            {
                filePath = Path.Combine(imagePath, imageName);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                System.Drawing.Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save(filePath);
                qrCodeImage.Dispose();

                if (show)
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = filePath;
                    process.StartInfo.Arguments = "rundll32.exe C:\\WINDOWS\\system32\\shimgvw.dll,ImageView_Fullscreen";
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    process.Start();
                    process.Close();
                }
            }
            return new Dictionary<string, object>
                {
                    {"ImagePath",filePath }
                };
        }
    }
}

namespace Setting
{
    /// <summary>
    /// WH设置文件等
    /// </summary>
    public class Setting
    {
        internal Setting() { }

        private static string[] _whpath =
        {
            String.Format("{0}\\Dynamo\\Dynamo Revit\\1.3\\packages\\WhiteHorse\\extra", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
            String.Format("{0}\\Dynamo\\Dynamo Revit\\1.2\\packages\\WhiteHorse\\extra", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
        };

        /// <summary>
        /// 返回WhiteHorse包所在的路径,供导入PyScript中使用
        /// </summary>
        /// <returns>路径</returns>
        /// <search>path</search>
        public static string[] GetPath()
        {
            return _whpath;
        }

        /// <summary>
        /// 关于WhiteHorse
        /// </summary>
        public static void About()
        {
            foreach (var path in _whpath)
            {
                if (Directory.Exists(path))
                {
                    System.Diagnostics.Process.Start("notepad.exe", path + "\\WhiteHorse\\resource\\version.txt");
                    break;
                }
            }
        }

        /// <summary>
        /// 显示QQ群和公众号二维码
        /// </summary>
        public static void Join()
        {
            WF.Form form = new WF.Form();
            form.Text = "关于";
            form.Height = 400;
            form.Width = 540;
            form.FormBorderStyle = WF.FormBorderStyle.FixedToolWindow;
            form.StartPosition = WF.FormStartPosition.CenterScreen;

            foreach (var path in _whpath)
            {
                if (Directory.Exists(path))
                {
                    WF.PictureBox pic = new WF.PictureBox
                    {
                        ImageLocation = path + "\\WhiteHorse\\resource\\saker.png",
                        SizeMode = WF.PictureBoxSizeMode.AutoSize,
                        Parent = form
                    };
                    WF.Application.Run(form);
                    break;
                }
            }
        }

        /// <summary>
        /// 检查更新，需联网
        /// </summary>
        public static void CheckUpdate()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://coding.net/u/sakernan/p/WhiteHorseHelp/git/raw/master/version");
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("gb2312"));
                string version = reader.ReadLine();
                reader.Close();
                reader.Dispose();

                string tmp_version = null;
                foreach (var path in _whpath)
                {
                    if (Directory.Exists(path))
                    {
                        StreamReader reader1 = new StreamReader(path + "\\WhiteHorse\\resource\\version.txt");

                        for (int i = 0; i < 40; i++)
                        {
                            string tmp = reader1.ReadLine();
                            if (i == 39)
                            {
                                tmp_version = tmp;
                            }
                        }
                        reader1.Close();
                        reader1.Dispose();
                        break;
                    }
                }
                if (version != tmp_version)
                {
                    string message = string.Format("●当前版本为：{0}\n●安装版本为：{1}\n●需要更新！\n★可点击【Setting-Join】加群获取安装包~", version, tmp_version);
                    WF.MessageBox.Show(message, "WhiteHorse检查更新");
                }
                else
                {
                    WF.MessageBox.Show("✔恭喜您，当前WhiteHorse是最新版！", "WhiteHorse检查更新");
                }
            }
            catch (WebException)
            {

                WF.MessageBox.Show("✘该功能需要联网，请检查网络是否通畅", "WhiteHorse检查更新");
            }
        }


    }
}