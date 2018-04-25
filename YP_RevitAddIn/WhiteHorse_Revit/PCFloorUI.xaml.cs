using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WF = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace WhiteHorseCore
{
    /// <summary>
    /// PCFloorUI.xaml 的交互逻辑
    /// </summary>
    public partial class PCFloorUI : Window
    {
        private UIDocument _uiDoc;
        private Document _doc;
        public PCFloorUI(UIDocument uiDoc, Document doc)
        {
            this._uiDoc = uiDoc;
            this._doc = doc;

            InitializeComponent();

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.floorX.Text=="" || this.floorX.Text=="")
            {
                TaskDialog.Show("警告", "请检查板长宽是否填写正确！");
                return;
            }
            this.DialogResult = true;
            Close();
        }
    }
}
