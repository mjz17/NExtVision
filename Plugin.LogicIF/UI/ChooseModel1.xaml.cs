using Common;
using ModuleDataVar;
using StyleBase;
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

namespace Plugin.LogicIF
{
    /// <summary>
    /// ChooseModel1.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseModel1 : UserControl
    {

        private ModuleObj frm_ModuleObj;

        private DataVar m_DataVar;

        public ChooseModel1(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;

            //加载参数
            frm_ModuleObj = (ModuleObj)obj;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            if (frm_ModuleObj.blnNewModule)
            {
                theSecondTime();
            }
        }

        public void theSecondTime()
        {
            LinkDataName1 = frm_ModuleObj.m_Link_Data1;//链接数据
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
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(ChooseModel1), new PropertyMetadata(default(string)));

        #endregion

        #region 链接的模块名称

        public string LinkDataName1
        {
            get { return (string)this.GetValue(LinkDataName1Property); }
            set { this.SetValue(LinkDataName1Property, value); }
        }

        public static readonly DependencyProperty LinkDataName1Property =
            DependencyProperty.Register("LinkDataName1", typeof(string), typeof(ChooseModel1), new PropertyMetadata(default(string)));

        /// <summary>
        /// 链接1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Gen_InputDataName1_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Bool)
            {
                try
                {
                    frm_ModuleObj.m_Link_Data1 = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_ChooseModel1 = data;
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
