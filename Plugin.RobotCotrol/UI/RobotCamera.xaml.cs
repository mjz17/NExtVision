using Common;
using ModuleDataVar;
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
    /// RobotCamera.xaml 的交互逻辑
    /// </summary>
    public partial class RobotCamera : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public RobotCamera(ModuleObj frm_ModuleObj)
        {
            InitializeComponent();

            this.DataContext = this;
            this.frm_ModuleObj = frm_ModuleObj;
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
            InputImage_x = frm_ModuleObj.m_InputImgX;           //输入图像坐标X
            InputImage_y = frm_ModuleObj.m_InputImgY;           //输入图像坐标Y
            InPutPhi = frm_ModuleObj.m_InputPhi;                //输入角度

            ReferenceX = frm_ModuleObj.m_InputReferenceX;       //参考位置X
            ReferenceY = frm_ModuleObj.m_InputReferenceY;       //参考位置Y
            ReferencePhi = frm_ModuleObj.m_InputReferencePhi;   //参考角度
            AcceptPhi = frm_ModuleObj.m_InputAcceptPhi;         //接受角度差
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
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

        #endregion

        #region 输入图像坐标X

        public string InputImage_x
        {
            get { return (string)this.GetValue(InputImage_xProperty); }
            set { this.SetValue(InputImage_xProperty, value); }
        }

        public static readonly DependencyProperty InputImage_xProperty =
            DependencyProperty.Register("InputImage_x", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

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
                    frm_ModuleObj.m_InputImgX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputImgX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入图像坐标Y

        public string InputImage_y
        {
            get { return (string)this.GetValue(InputImage_yProperty); }
            set { this.SetValue(InputImage_yProperty, value); }
        }

        public static readonly DependencyProperty InputImage_yProperty =
            DependencyProperty.Register("InputImage_y", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

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
                    frm_ModuleObj.m_InputImgY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputImgY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入角度

        public string InPutPhi
        {
            get { return (string)this.GetValue(InPutPhiProperty); }
            set { this.SetValue(InPutPhiProperty, value); }
        }

        public static readonly DependencyProperty InPutPhiProperty =
            DependencyProperty.Register("InPutPhi", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

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
                    frm_ModuleObj.m_InputPhi = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputPhi = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 参考位置X

        public string ReferenceX
        {
            get { return (string)this.GetValue(ReferenceXProperty); }
            set { this.SetValue(ReferenceXProperty, value); }
        }

        public static readonly DependencyProperty ReferenceXProperty =
            DependencyProperty.Register("ReferenceX", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Reference_X_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputReferenceX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputReferenceX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 参考位置Y

        public string ReferenceY
        {
            get { return (string)this.GetValue(ReferenceYProperty); }
            set { this.SetValue(ReferenceYProperty, value); }
        }

        public static readonly DependencyProperty ReferenceYProperty =
            DependencyProperty.Register("ReferenceY", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Reference_Y_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputReferenceY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputTranY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 参考角度

        public string ReferencePhi
        {
            get { return (string)this.GetValue(ReferencePhiProperty); }
            set { this.SetValue(ReferencePhiProperty, value); }
        }

        public static readonly DependencyProperty ReferencePhiProperty =
            DependencyProperty.Register("ReferencePhi", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_ReferencePhi_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputReferencePhi = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputSupple_Angle = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 接受角度差

        public string AcceptPhi
        {
            get { return (string)this.GetValue(AcceptPhiProperty); }
            set { this.SetValue(AcceptPhiProperty, value); }
        }

        public static readonly DependencyProperty AcceptPhiProperty =
            DependencyProperty.Register("AcceptPhi", typeof(string), typeof(RobotCamera), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_AcceptPhi_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputAcceptPhi = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputAcceptPhi = data;
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
