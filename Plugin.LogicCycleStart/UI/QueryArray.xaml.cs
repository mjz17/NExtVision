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
using VisionCore;

namespace Plugin.LogicCycleStart
{
    /// <summary>
    /// QueryArray.xaml 的交互逻辑
    /// </summary>
    public partial class QueryArray : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public QueryArray(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            this.frm_ModuleObj = obj;
            this.Loaded += QueryArray_Loaded;
        }

        private void QueryArray_Loaded(object sender, RoutedEventArgs e)
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
            QueryArrayValue = frm_ModuleObj.m_InputArray;
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
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(QueryArray), new PropertyMetadata(default(string)));

        #endregion

        #region 初始值Array

        public string QueryArrayValue
        {
            get { return (string)this.GetValue(QueryArrayValueProperty); }
            set { this.SetValue(QueryArrayValueProperty, value); }
        }

        public static readonly DependencyProperty QueryArrayValueProperty =
            DependencyProperty.Register("QueryArrayValue", typeof(string), typeof(QueryArray), new PropertyMetadata(default(string)));

        private void UcDataLink_Global_StartValue(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double_Array)
            {
                try
                {
                    frm_ModuleObj.m_InputArray = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputArray = data;
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
