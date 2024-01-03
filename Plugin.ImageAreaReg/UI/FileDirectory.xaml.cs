using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Plugin.ImageAreaReg
{
    /// <summary>
    /// FileDirectory.xaml 的交互逻辑
    /// </summary>
    public partial class FileDirectory : UserControl
    {

        //后台数据
        private ModuleObj frm_ModuleObj;

        public FileDirectory(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            frm_ModuleObj = (ModuleObj)obj;
            if (frm_ModuleObj.blnNewModule)
            {
                theSecondTime();
            }
        }

        private void theSecondTime()
        {
            try
            {
                //是否自动切换
                if (frm_ModuleObj.Filemethod != null)
                {
                    txt_FilePath.Text = frm_ModuleObj.Filemethod.m_filePath;
                    Chk_Acuauto.IsChecked = frm_ModuleObj.Filemethod.m_cycleRun;
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_SearchFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                frm_ModuleObj.Filemethod = new FileImageMethod();
                frm_ModuleObj.Filemethod.m_filePath = txt_FilePath.Text = dialog.SelectedPath;
                frm_ModuleObj.files = frm_ModuleObj.Filemethod.ReadFileImageName();
                frm_ModuleObj.Filemethod.m_cycleRun = Chk_Acuauto.IsChecked == true ? true : false;
                Refresh();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox chk && frm_ModuleObj.Filemethod != null)
            {
                frm_ModuleObj.Filemethod.m_cycleRun = chk.IsChecked == true ? true : false;
            }
        }

        private void Refresh()
        {
            this.dgv_List.ItemsSource = null;
            this.dgv_List.ItemsSource = frm_ModuleObj.files;
        }

    }
}
