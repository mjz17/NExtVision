using Common;
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

namespace Plugin.RobotCotrol
{
    /// <summary>
    /// RobotPostion.xaml 的交互逻辑
    /// </summary>
    public partial class RobotPostion : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public RobotPostion(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            //加载参数
            frm_ModuleObj = (ModuleObj)obj;
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

        public void theFirsttime() { }

        public void theSecondTime()
        {
            InputImage_x = frm_ModuleObj.m_InputMachX;   //输入图像坐标X
            InputImage_y = frm_ModuleObj.m_InputMachY;   //输入图像坐标Y
            InPutPhi = frm_ModuleObj.m_InputMachPhi;     //输入角度
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
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(RobotPostion), new PropertyMetadata(default(string)));

        #endregion

        #region 机械手X(mm)

        public string InputImage_x
        {
            get { return (string)this.GetValue(InputImage_xProperty); }
            set { this.SetValue(InputImage_xProperty, value); }
        }

        public static readonly DependencyProperty InputImage_xProperty =
            DependencyProperty.Register("InputImage_x", typeof(string), typeof(RobotPostion), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputImageX_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputMachX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputImgX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 机械手Y(mm)

        public string InputImage_y
        {
            get { return (string)this.GetValue(InputImage_yProperty); }
            set { this.SetValue(InputImage_yProperty, value); }
        }

        public static readonly DependencyProperty InputImage_yProperty =
            DependencyProperty.Register("InputImage_y", typeof(string), typeof(RobotPostion), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputImageY_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputMachY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputImgY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 机械手角度(°)

        public string InPutPhi
        {
            get { return (string)this.GetValue(InPutPhiProperty); }
            set { this.SetValue(InPutPhiProperty, value); }
        }

        public static readonly DependencyProperty InPutPhiProperty =
            DependencyProperty.Register("InPutPhi", typeof(string), typeof(RobotPostion), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InpuPhi_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputMachPhi = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputPhi = data;
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
