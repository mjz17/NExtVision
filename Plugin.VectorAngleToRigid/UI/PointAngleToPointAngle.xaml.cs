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
    /// PointAngleToPointAngle.xaml 的交互逻辑
    /// </summary>
    public partial class PointAngleToPointAngle : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public PointAngleToPointAngle(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            this.frm_ModuleObj = obj;
            this.Loaded += PointAngleToPointAngle_Loaded;
        }

        private void PointAngleToPointAngle_Loaded(object sender, RoutedEventArgs e)
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
            InputFormX = frm_ModuleObj.m_InputFormX;
            InputFormX = frm_ModuleObj.m_InputFormY;
            InputFormAngle = frm_ModuleObj.m_InputFormAngle;
            InputFormX = frm_ModuleObj.m_InputToX;
            InputFormX = frm_ModuleObj.m_InputToY;
            InputToAngle = frm_ModuleObj.m_InputToAngle;
        }

        #region FormX

        public string InputFormX
        {
            get { return (string)this.GetValue(InputFormXProperty); }
            set { this.SetValue(InputFormXProperty, value); }
        }

        public static readonly DependencyProperty InputFormXProperty =
            DependencyProperty.Register("InputFormX", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputFormX_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputFormX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputFormX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region FormY

        public string InputFormY
        {
            get { return (string)this.GetValue(InputFormYProperty); }
            set { this.SetValue(InputFormYProperty, value); }
        }

        public static readonly DependencyProperty InputFormYProperty =
            DependencyProperty.Register("InputFormY", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputFormY_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputFormY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputFormY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region FormAngle

        public string InputFormAngle
        {
            get { return (string)this.GetValue(InputFormAngleProperty); }
            set { this.SetValue(InputFormAngleProperty, value); }
        }

        public static readonly DependencyProperty InputFormAngleProperty =
            DependencyProperty.Register("InputFormAngle", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputFormAngle_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputFormAngle = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputFormAngle = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region ToX

        public string InputToX
        {
            get { return (string)this.GetValue(InputToXProperty); }
            set { this.SetValue(InputToXProperty, value); }
        }

        public static readonly DependencyProperty InputToXProperty =
            DependencyProperty.Register("InputToX", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputToX_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputToX = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputToX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region ToY

        public string InputToY
        {
            get { return (string)this.GetValue(InputToYProperty); }
            set { this.SetValue(InputToYProperty, value); }
        }

        public static readonly DependencyProperty InputToYProperty =
            DependencyProperty.Register("InputToY", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputToY_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputToY = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputToY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion
        #region ToAngle

        public string InputToAngle
        {
            get { return (string)this.GetValue(InputToAngleProperty); }
            set { this.SetValue(InputToAngleProperty, value); }
        }

        public static readonly DependencyProperty InputToAngleProperty =
            DependencyProperty.Register("InputToAngle", typeof(string), typeof(PointToPoint), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_InputToAngle_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_InputFormAngle = data.m_DataTip + "." + data.m_DataName;
                    frm_ModuleObj.Link_InputFormAngle = data;
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
