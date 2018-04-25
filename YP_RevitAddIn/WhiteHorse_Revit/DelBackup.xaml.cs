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
using System.Windows.Shapes;

using Autodesk.Revit.UI;

namespace WhiteHorseCore
{
    /// <summary>
    /// DelBackup.xaml 的交互逻辑
    /// </summary>
    public partial class DelBackupUI : Window
    {
        private List<string> _currentNames;
        private string _currentDirectory;
        private string _delDirectory;
        public DelBackupUI(List<string> names, string currentPath)
        {
            this._currentNames = names;
            this._currentDirectory = currentPath;
            InitializeComponent();
            this.currentRadio.IsChecked = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!this.fileList.HasItems)
            {
                TaskDialog.Show("警告", "恭喜你！当前路径不存在备份文件~");
                return;
            }
            foreach (var name in ExternalFuncs.GetFileNames(this._delDirectory))
            {
                string filePath = System.IO.Path.Combine(this._delDirectory, name);
                System.IO.File.Delete(filePath);
            }
            this.fileList.Items.Clear();
            TaskDialog.Show("提示", "备份文件已删除！");
        }

        private void CurrentRadio_Checked(object sender, RoutedEventArgs e)
        {
            this._delDirectory = this._currentDirectory;
            if (this.fileList.HasItems)
            {
                
                this.fileList.Items.Clear();
            }

            foreach (var item in ExternalFuncs.GetFileNames(this._currentDirectory))
            {
                ListViewItem i = new ListViewItem { Content = item };
                this.fileList.Items.Add(i);
            }
        }

        private void OtherRadio_Checked(object sender, RoutedEventArgs e)
        {
            WF.FolderBrowserDialog fb = new WF.FolderBrowserDialog();
            WF.DialogResult result = fb.ShowDialog();

            if (result == WF.DialogResult.Cancel)
            {
                return;
            }

            if (this.fileList.HasItems)
            {
                this.fileList.Items.Clear();
            }
            
            string m_Dir = fb.SelectedPath.Trim();
            this._delDirectory = m_Dir;
            List<string> names = ExternalFuncs.GetFileNames(m_Dir);
            foreach (var item in names)
            {
                ListViewItem i = new ListViewItem { Content = item };
                this.fileList.Items.Add(i);
            }
        }
    }
}
