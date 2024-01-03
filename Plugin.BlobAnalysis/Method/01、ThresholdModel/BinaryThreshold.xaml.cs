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

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// BinaryThreshold.xaml 的交互逻辑
    /// </summary>
    public partial class BinaryThreshold : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public BinaryThreshold(ModuleObj obj)
        {
            InitializeComponent();
            //加载参数
            frm_ModuleObj = (ModuleObj)obj;
            this.Loaded += BinaryThreshold_Loaded;
        }

        private void BinaryThreshold_Loaded(object sender, RoutedEventArgs e)
        {
            cmb_BinaryThread.ItemsSource = Enum.GetNames(typeof(BinaryThread));
            cmb_BinaryThread.SelectedIndex = (int)frm_ModuleObj.m_BinThread;
        }

        private void cmb_BinaryThread_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_BinThread = (BinaryThread)cmb_BinaryThread.SelectedIndex;
        }
    }
}
