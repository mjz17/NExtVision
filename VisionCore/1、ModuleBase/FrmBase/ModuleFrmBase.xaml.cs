using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Common;

namespace VisionCore
{
    /// <summary>
    /// ModuleFrmBase.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrmBase : UserControl
    {

        /// <summary>
        /// 父窗体
        /// </summary>
        private PluginFrmBase m_PluginFrmBase { get; set; }

        /// <summary>
        /// 窗体使用数据
        /// </summary>
        private ModuleObjBase m_ModuleObjBase { get; set; }

        /// <summary>
        /// 模块状态
        /// </summary>
        public Visibility RunStatusVisibility { get; set; }

        /// <summary>
        /// 执行按钮的显示
        /// </summary>
        public Visibility RunBtnVisibility { get; set; }

        public ModuleFrmBase()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += ModuleFrmBase_Loaded;
        }

        private void ModuleFrmBase_Loaded(object sender, RoutedEventArgs e)
        {
            //如果是窗体设计时期，就不执行相关代码即可
            if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
            {

            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                m_PluginFrmBase = (PluginFrmBase)Window.GetWindow(this);
                m_PluginFrmBase.ModuleFrmBase = this;
                m_PluginFrmBase.m_ModuleObjBase = m_PluginFrmBase.m_ModuleObjBase;
            }
        }

        private void run_Click(object sender, RoutedEventArgs e)
        {
            m_PluginFrmBase.ExModule();
            if (RunStatusVisibility != Visibility.Hidden)
            {
                txtUserTime.Text = "耗时：" + m_PluginFrmBase.m_ModuleObjBase.ModuleParam.ModuleCostTime.ToString() + "ms";
                txtStatus.Text = "状态：" + (m_PluginFrmBase.m_ModuleObjBase.ModuleParam.BlnSuccessed ? "OK" : "NG");
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            m_PluginFrmBase.SaveModuleParam();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            m_PluginFrmBase.CancelModuleParam();
        }

    }
}
