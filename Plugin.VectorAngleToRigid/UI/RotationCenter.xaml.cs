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

namespace Plugin.VectorAngleToRigid
{
    /// <summary>
    /// RotationCenter.xaml 的交互逻辑
    /// </summary>
    public partial class RotationCenter : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public RotationCenter(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            this.frm_ModuleObj = obj;
            this.Loaded += RotationCenter_Loaded;
        }

        private void RotationCenter_Loaded(object sender, RoutedEventArgs e)
        {
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
            CenterX = frm_ModuleObj.m_InputCenterX;
            CenterY = frm_ModuleObj.m_InputCenterY;
            CenterAngle = frm_ModuleObj.m_InputCenterAngle;
        }

        #region CenterX

        public string CenterX
        {
            get { return (string)this.GetValue(CenterXroperty); }
            set { this.SetValue(CenterXroperty, value); }
        }

        public static readonly DependencyProperty CenterXroperty =
            DependencyProperty.Register("CenterX", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_CenterX_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputCenterX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputCenterX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region CenterY

        public string CenterY
        {
            get { return (string)this.GetValue(CenterYroperty); }
            set { this.SetValue(CenterYroperty, value); }
        }

        public static readonly DependencyProperty CenterYroperty =
            DependencyProperty.Register("CenterY", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_CenterY_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputCenterY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputCenterY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region CenterAngle

        public string CenterAngle
        {
            get { return (string)this.GetValue(CenterAngleProperty); }
            set { this.SetValue(CenterAngleProperty, value); }
        }

        public static readonly DependencyProperty CenterAngleProperty =
            DependencyProperty.Register("CenterAngle", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_CenterAngle_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputCenterAngle = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_CenterAngle = data;
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
