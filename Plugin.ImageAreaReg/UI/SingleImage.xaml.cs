using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Plugin.ImageAreaReg
{
    /// <summary>
    /// SingleImage.xaml 的交互逻辑
    /// </summary>
    public partial class SingleImage : UserControl
    {
        //后台数据
        private ModuleObj frm_ModuleObj;

        public SingleImage(ModuleObj obj)
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
            this.txt_Path.Text = frm_ModuleObj.SingleImagePath;
        }

        private void btn_SearchFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string NvName = System.IO.Path.GetFullPath(openFileDialog.FileName);
                txt_Path.Text = frm_ModuleObj.SingleImagePath = NvName;
            }
        }
    }
}
