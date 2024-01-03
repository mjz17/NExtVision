using Common;
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

namespace Plugin.LogicCycleStart
{
    /// <summary>
    /// StartToEnd.xaml 的交互逻辑
    /// </summary>
    public partial class StartToEnd : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public StartToEnd(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            this.frm_ModuleObj = obj;
            this.Loaded += StartToEnd_Loaded;
        }

        private void StartToEnd_Loaded(object sender, RoutedEventArgs e)
        {
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            if (!frm_ModuleObj.blnNewModule)
            {
                theFirsttime();
            }
            else
            {
                theSecondTime();
            }
        }

        private void theFirsttime() { }

        private void theSecondTime()
        {
            StartValue = frm_ModuleObj.m_InputStart;
            EndValue = frm_ModuleObj.m_InputEnd;
        }

        #region 当前模块ID

        /// <summary>
        /// 当前模块ID
        /// </summary>
        public string CurrentModelID
        {
            get { return (string)this.GetValue(CurrentModelIDProperty); }
            set { this.SetValue(CurrentModelIDProperty, value); }
        }

        public static readonly DependencyProperty CurrentModelIDProperty =
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(StartToEnd), new PropertyMetadata(default(string)));

        #endregion

        #region 初始值Start

        public string StartValue
        {
            get { return (string)this.GetValue(StartValueProperty); }
            set { this.SetValue(StartValueProperty, value); }
        }

        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(string), typeof(StartToEnd), new PropertyMetadata(default(string)));

        private void UcDataLink_Global_StartValue(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Int)
            {
                try
                {
                    frm_ModuleObj.m_InputStart = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputStart = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 初始值End

        public string EndValue
        {
            get { return (string)this.GetValue(EndValueProperty); }
            set { this.SetValue(EndValueProperty, value); }
        }

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(string), typeof(StartToEnd), new PropertyMetadata(default(string)));

        private void UcDataLink_Global_EndValue(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Int)
            {
                try
                {
                    frm_ModuleObj.m_InputEnd = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputEnd = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

    }
}
