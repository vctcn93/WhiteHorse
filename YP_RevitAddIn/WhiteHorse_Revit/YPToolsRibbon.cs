using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace WhiteHorseCore
{
    [Transaction(TransactionMode.Manual)]
    public class YPToolsRibbon : IExternalApplication
    {
        private static string _dllPath = typeof(YPToolsRibbon).Assembly.Location;
        private static string _dirPath = Path.GetDirectoryName(_dllPath);
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            application.CreateRibbonTab("壹匹BIM");

            //文件panel
            RibbonPanel fileTools = application.CreateRibbonPanel("壹匹BIM", "文件");
            //删除备份按钮
            PushButtonData delBack = new PushButtonData("delBackButton", "删除备份", _dllPath, "WhiteHorseCore.DelBackupFunc")
            {
                LargeImage = new BitmapImage(new Uri(
                    String.Format(@"{0}\Icons\delbackup.png", _dirPath))),
                ToolTip = "删除Revit备份文件"
            };

            fileTools.AddItem(delBack);

            //预制构件panel
            RibbonPanel pcTools = application.CreateRibbonPanel("壹匹BIM", "预制构件");
            //创建叠合板按钮
            PushButtonData creatFloor = new PushButtonData("creatFloorButton", "创建叠合板", _dllPath, "WhiteHorseCore.PCFloorFunc")
            {
                LargeImage = new BitmapImage(new Uri(
                    String.Format(@"{0}\Icons\delbackup.png", _dirPath))),
                ToolTip = "创建叠合板"
            };

            pcTools.AddItem(creatFloor);

            return Result.Succeeded;
        }
    }
}
