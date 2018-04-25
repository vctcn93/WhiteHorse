using System;
using System.IO;
using System.Collections.Generic;

using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace Excel
{
    /// <summary>
    /// 创建EXCEL文件
    /// </summary>
    public class CreatExcel
    {
        internal CreatExcel() { }

        /// <summary>
        /// 将数据导出为.xlsx文件
        /// </summary>
        /// <param name="filePath">文件路径，比如：\n"d:\壹匹BIM.xlsx"</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="data">待写入的数据\n请处理为一维或者二维列表</param>
        /// <param name="startRow">设置插入行，第一行为1</param>
        /// <param name="startCol">设置插入列，第一列为1</param>
        /// <param name="wrap">文字自动换行</param>
        /// <param name="transpose">行列转换</param>
        /// <returns>生成的文件路径</returns>
        /// <search>excel,export,make</search>
        public static string ExportExcel(string filePath, string sheetName, object[][] data, int startRow = 1, int startCol = 1, bool wrap = false, bool transpose = false)
        {
            if (startCol <= 0 || startRow <= 0)
            {
                throw new Exception("startRow和startCol必须大于1！");
            }
            using (ExcelPackage ExcelPkg = new ExcelPackage())
            {
                ExcelWorksheet workSheet = ExcelPkg.Workbook.Worksheets.Add(sheetName);
                workSheet.Protection.IsProtected = false;
                workSheet.Protection.AllowSelectLockedCells = false;
                workSheet.Cells.Style.WrapText = wrap;

                for (int i = 0; i < data.Length; i++)
                {
                    ExcelRange rng;

                    for (int j = 0; j < data[i].Length; j++)
                    {
                        if (transpose)
                        { rng = workSheet.Cells[startRow + i, j + startCol]; }
                        else
                        { rng = workSheet.Cells[j + startRow, startCol + i]; }

                        rng.Value = data[i][j];
                    }
                }
                ExcelPkg.SaveAs(new FileInfo(filePath));
            }
            return filePath;
        }

        /// <summary>
        /// 创建图表
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">表格名称</param>
        /// <param name="chartName">图表名称</param>
        /// <param name="chartType">图表样式</param>
        /// <param name="xData">X方向数据</param>
        /// <param name="yData">X方向数据</param>
        /// <param name="xPos">图表左上角X位置，0最小</param>
        /// <param name="yPos">图表左上角Y位置，0最小</param>
        /// <param name="xSize">图标X方向尺寸</param>
        /// <param name="ySize">图标Y方向尺寸</param>
        /// <returns>创建的文件路径</returns>
        /// <search>excel,chart,make</search>
        public static string MakeChart(string filePath, string sheetName, string chartName, string chartType, List<object> xData, List<object> yData, int xPos = 0, int yPos = 0, int xSize = 600, int ySize = 600)
        {
            using (ExcelPackage epk = new ExcelPackage())
            {
                var worksheet = epk.Workbook.Worksheets.Add(sheetName);
                var type = (eChartType)(Enum.Parse(typeof(eChartType), chartType));
                var chart = worksheet.Drawings.AddChart(chartName, type);
                chart.SetPosition(xPos, yPos);
                chart.SetSize(xSize, ySize);

                string xStr = string.Format("A1:A{0}", xData.Count);
                string yStr = string.Format("B1:B{0}", yData.Count);

                for (int i = 0; i < xData.Count; i++)
                {
                    worksheet.Cells[i + 1, 1].Value = xData[i];
                }
                for (int i = 0; i < yData.Count; i++)
                {
                    worksheet.Cells[i + 1, 2].Value = yData[i];
                }
                var serie = chart.Series.Add(worksheet.Cells[xStr], worksheet.Cells[yStr]);
                epk.SaveAs(new FileInfo(filePath));
            }
            return filePath;
        }
    }

   /// <summary>
   /// 读取EXCEL文件
   /// </summary>
    public class ReadExcel
    {
        internal ReadExcel() { }

        /// <summary>
        /// 读取Excel文件，可切换读取方式
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="sheetName">表格名称</param>
        /// <param name="transpose">行列颠倒</param>
        /// <returns>返回的数据</returns>
        /// <search>excel,import,read</search>
        public static List<List<object>> ImportExcel(string filePath, string sheetName, bool transpose = false)
        {
            List<List<object>> list = new List<List<object>>();
            FileStream file = new FileStream(filePath, FileMode.Open);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets[sheetName];

                int sa = sheet.Dimension.End.Row >= sheet.Dimension.End.Column 
                        ? sheet.Dimension.End.Row 
                        : sheet.Dimension.End.Column;

                for (int i = sheet.Dimension.Start.Row; i <= sa; i++)
                {
                    List<object> tmp = new List<object>();
                    for (int m = sheet.Dimension.Start.Column; m <= sa; m++)
                    {
                        object t = sheet.Cells[m, i].Value;
                        if (t != null)
                        {
                            tmp.Add(t);
                        }
                    }
                    if (tmp.Count!=0)
                    {
                        list.Add(tmp);
                    }
                }
            }
            file.Dispose();
            list = transpose ? FuncTools.Tools.Rotate(list) : list;
            return list;
        }
    }
}
