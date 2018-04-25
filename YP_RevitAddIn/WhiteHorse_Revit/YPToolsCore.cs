using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace WhiteHorseCore
{
    [Transaction(TransactionMode.Manual)]
    class DelBackupFunc : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var currentPath = doc.PathName;
            if (currentPath == "")
            {
                TaskDialog.Show("警告", "当前文件并未保存！");
                return Result.Failed;
            }
            string docPath = Path.GetDirectoryName(currentPath);

            List<string> names = ExternalFuncs.GetFileNames(docPath);
            DelBackupUI mainWindow = new DelBackupUI(names, docPath);
            mainWindow.ShowDialog();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    class BathCreatSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            BathCreatSheetUI window = new BathCreatSheetUI();
            window.ShowDialog();
            return Result.Succeeded;
        }
    }

    //[Transaction(TransactionMode.Manual)]
    //class PCFloorFunc : IExternalCommand
    //{
    //    private static string _dllPath = typeof(YPToolsRibbon).Assembly.Location;
    //    private static string _dirPath = Path.GetDirectoryName(_dllPath);

    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        var uiDoc = commandData.Application.ActiveUIDocument;
    //        var doc = uiDoc.Document;
    //        //创建过滤器
    //        FilteredElementCollector floorCollector = new FilteredElementCollector(doc);
    //        //过滤出楼板类型
    //        var floorTypeCol = floorCollector.OfClass(typeof(FloorType)).WhereElementIsElementType();
    //        List<Element> floorTypes = floorTypeCol.ToElements().ToList();
    //        //过滤出钢筋类型
    //        FilteredElementCollector barTypeCollector = new FilteredElementCollector(doc);
    //        var barTypeCol = barTypeCollector.OfClass(typeof(RebarBarType));
    //        List<Element> rebarTypes = barTypeCol.ToElements().ToList();
    //        //过滤出弯钩类型
    //        FilteredElementCollector hookTypeCollector = new FilteredElementCollector(doc);
    //        var hookTypeCol = hookTypeCollector.OfClass(typeof(RebarHookType));
    //        List<Element> hookTypes = hookTypeCol.ToElements().ToList();
    //        ////过滤出保护层
    //        //FilteredElementCollector coverTypeCollector = new FilteredElementCollector(doc);
    //        //var coverTypeCol = coverTypeCollector.OfClass(typeof(RebarCoverType));
    //        //List<Element> coverTypes = coverTypeCol.ToElements().ToList();

    //        //创建FloorType数据源
    //        Dictionary<string, FloorType> dataSource = new Dictionary<string, FloorType>();
    //        foreach (var floor in floorTypes)
    //        {
    //            dataSource.Add(floor.Name, (FloorType)floor);
    //        }
    //        //创建RebarType数据源
    //        Dictionary<string, RebarBarType> rebarSource = new Dictionary<string, RebarBarType>();
    //        foreach (var rebar in rebarTypes)
    //        {
    //            rebarSource.Add(rebar.Name, (RebarBarType)rebar);
    //        }

    //        //创建HookType数据源
    //        Dictionary<string, RebarHookType> hookSource = new Dictionary<string, RebarHookType>();
    //        foreach (var item in hookTypes)
    //        {
    //            hookSource.Add(item.Name, (RebarHookType)item);
    //        }


    //        //创建窗体
    //        PCFloorUI mainWindow = new PCFloorUI(uiDoc, doc);
    //        mainWindow.headerImage.Source = new BitmapImage(new Uri(
    //                String.Format(@"{0}\Images\floorOverView.png", _dirPath)));
    //        //绑定楼板类型combobox
    //        mainWindow.floorCombo.ItemsSource = dataSource;
    //        mainWindow.floorCombo.SelectedValuePath = "Value";
    //        mainWindow.floorCombo.DisplayMemberPath = "Key";
    //        mainWindow.floorCombo.SelectedIndex = 0;

    //        #region 不忍直视的代码，简单粗暴！
    //        //绑定钢筋类型combobox
    //        mainWindow.rebar1Type.ItemsSource = mainWindow.rebar2Type.ItemsSource = mainWindow.rebar3Type.ItemsSource = rebarSource;
    //        mainWindow.rebar1Type.SelectedValuePath = mainWindow.rebar2Type.SelectedValuePath = mainWindow.rebar3Type.SelectedValuePath = "Value";
    //        mainWindow.rebar1Type.DisplayMemberPath = mainWindow.rebar2Type.DisplayMemberPath = mainWindow.rebar3Type.DisplayMemberPath = "Key";
    //        mainWindow.rebar1Type.SelectedIndex = mainWindow.rebar2Type.SelectedIndex = mainWindow.rebar3Type.SelectedIndex = 0;
    //        //绑定弯钩类型combobox
    //        mainWindow.rebar1SHook.ItemsSource = mainWindow.rebar2SHook.ItemsSource = mainWindow.rebar3SHook.ItemsSource =
    //            mainWindow.rebar1EHook.ItemsSource = mainWindow.rebar2EHook.ItemsSource = mainWindow.rebar3EHook.ItemsSource = hookSource;
    //        mainWindow.rebar1SHook.SelectedValuePath = mainWindow.rebar2SHook.SelectedValuePath = mainWindow.rebar3SHook.SelectedValuePath =
    //            mainWindow.rebar1EHook.SelectedValuePath = mainWindow.rebar2EHook.SelectedValuePath = mainWindow.rebar3EHook.SelectedValuePath =
    //            "Value";
    //        mainWindow.rebar1SHook.DisplayMemberPath = mainWindow.rebar2SHook.DisplayMemberPath = mainWindow.rebar3SHook.DisplayMemberPath =
    //            mainWindow.rebar1EHook.DisplayMemberPath = mainWindow.rebar2EHook.DisplayMemberPath = mainWindow.rebar3EHook.DisplayMemberPath =
    //            "Key";
    //        #endregion

    //        mainWindow.ShowDialog();
    //        if (mainWindow.DialogResult == true)
    //        {
    //            //x
    //            ExternalFuncs.ConvertLength(doc, mainWindow.xText.Text, out double xValue);
    //            ExternalFuncs.ConvertLength(doc, mainWindow.x1Text.Text, out double x1Value);
    //            ExternalFuncs.ConvertLength(doc, mainWindow.a1Text.Text, out double a1Value);
    //            ExternalFuncs.ConvertLength(doc, mainWindow.a2Text.Text, out double a2Value);
    //            ExternalFuncs.ConvertLength(doc, mainWindow.b1Text.Text, out double b1Value);
    //            ExternalFuncs.ConvertLength(doc, mainWindow.b2Text.Text, out double b2Value);
    //            if (xValue == 0 || x1Value == 0 || a1Value == 0 || a2Value == 0 || b1Value == 0 || b2Value == 0)
    //            {
    //                TaskDialog.Show("警告", "钢筋分布参数设置有误！");
    //                return Result.Cancelled;
    //            }
    //            //创建楼板
    //            string floorX = mainWindow.floorX.Text;
    //            string floorY = mainWindow.floorY.Text;
    //            bool xbool = double.TryParse(floorX, out double xLength);
    //            bool ybool = double.TryParse(floorY, out double yLength);
    //            if (!(xbool && ybool))
    //            {
    //                TaskDialog.Show("警告", "请检查板长宽是否填写正确！");
    //                return Result.Failed;
    //            }
    //            XYZ pt0 = uiDoc.Selection.PickPoint("请点选楼板放置位置");

    //            xLength /= 304.8;
    //            yLength /= 304.8;

    //            XYZ pt1 = new XYZ(pt0.X + xLength, pt0.Y, pt0.Z);
    //            XYZ pt2 = new XYZ(pt0.X + xLength, pt0.Y - yLength, pt0.Z);
    //            XYZ pt3 = new XYZ(pt0.X, pt0.Y - yLength, pt0.Z);
    //            CurveArray curves = new CurveArray();

    //            curves.Append(Line.CreateBound(pt0, pt1));
    //            curves.Append(Line.CreateBound(pt1, pt2));
    //            curves.Append(Line.CreateBound(pt2, pt3));
    //            curves.Append(Line.CreateBound(pt3, pt0));
    //            using (Transaction t = new Transaction(doc, "CreatFloor"))
    //            {
    //                t.Start();
    //                //创建0mm保护层并设置给楼板
    //                RebarCoverType coverType = RebarCoverType.Create(doc, "无保护层", 0);
    //                Floor floor = doc.Create.NewFloor(curves, true);
    //                floor.FloorType = (FloorType)mainWindow.floorCombo.SelectedValue;
    //                RebarHostData.GetRebarHostData(floor).SetCommonCoverType(coverType);
    //                //楼板厚
    //                double thick = floor.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();
    //                //创建1#Rebar
    //                RebarBarType rebar1Bar = (RebarBarType)mainWindow.rebar1Type.SelectedValue;
    //                RebarHookType rebar1SHook = (RebarHookType)mainWindow.rebar1SHook.SelectedValue;
    //                RebarHookType rebar1EHook = (RebarHookType)mainWindow.rebar1EHook.SelectedValue;

    //                double rebar1d = rebar1Bar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
    //                //1#钢筋路径
    //                List<Curve> rebar1curves = new List<Curve>();
    //                double rebar1Z = pt0.Z - thick + rebar1d / 2 + 20 / 304.8;
    //                double xLeft = 25 / 304.8 + x1Value;
    //                double xRest = xLength - xLeft - 25 / 304.8;
    //                int amount1 = (int)(xRest / xValue+0.5);

    //                XYZ rebar1Fs = new XYZ(pt0.X + xLeft, pt0.Y + b2Value, rebar1Z);
    //                XYZ rebar1Fe = new XYZ(pt0.X + xLeft, pt3.Y - b1Value, rebar1Z);

    //                for (int i = 0; i < amount1; i++)
    //                {
    //                    XYZ p1 = new XYZ(rebar1Fs.X + i * xValue, rebar1Fs.Y, rebar1Z);
    //                    XYZ p2 = new XYZ(rebar1Fe.X + i * xValue, rebar1Fe.Y, rebar1Z);
    //                    rebar1curves.Add(Line.CreateBound(p1, p2));
    //                }

    //                foreach (var curve in rebar1curves)
    //                {
    //                    Rebar.CreateFromCurves(
    //                    doc,
    //                    RebarStyle.Standard,
    //                    rebar1Bar,
    //                    rebar1SHook,
    //                    rebar1EHook,
    //                    floor, new XYZ(1, 0, 0), new List<Curve> { curve }, RebarHookOrientation.Right, RebarHookOrientation.Right, true, true
    //                    );
    //                }

    //                //创建3#Rebar
    //                RebarBarType rebar3Bar = (RebarBarType)mainWindow.rebar3Type.SelectedValue;
    //                double rebar3d = rebar3Bar.get_Parameter(BuiltInParameter.REBAR_BAR_DIAMETER).AsDouble();
    //                //3#钢筋路径
    //                List<Curve> rebar3curves = new List<Curve>();
    //                rebar3curves.Add(
    //                    Line.CreateBound(new XYZ(pt0.X + 25 / 304.8, pt0.Y - 25 / 304.8, rebar1Z), new XYZ(pt3.X + 25 / 304.8, pt3.Y + 25 / 304.8, rebar1Z))
    //                    );
    //                rebar3curves.Add(
    //                    Line.CreateBound(new XYZ(pt1.X - 25 / 304.8, pt1.Y - 25 / 304.8, rebar1Z), new XYZ(pt2.X - 25 / 304.8, pt2.Y + 25 / 304.8, rebar1Z))
    //                    );

    //                foreach (var curve in rebar3curves)
    //                {
    //                    Rebar.CreateFromCurves(
    //                    doc,
    //                    RebarStyle.Standard,
    //                    rebar3Bar,
    //                    null,
    //                    null,
    //                    floor, new XYZ(0, 0, 1), new List<Curve> { curve }, RebarHookOrientation.Left, RebarHookOrientation.Right, true, false
    //                    );
    //                }


    //                t.Commit();
    //            }
    //        }


    //        return Result.Succeeded;
    //    }
    //}


}
