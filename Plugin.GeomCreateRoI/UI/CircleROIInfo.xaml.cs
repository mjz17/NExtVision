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

namespace Plugin.GeomCreateRoI
{

    //圆形委托
    public delegate void CircleDelegate(double m_Row, double m_Col, double m_Radius);

    /// <summary>
    /// CircleROIInfo.xaml 的交互逻辑
    /// </summary>
    public partial class CircleROIInfo : UserControl
    {
        //后台数据
        private ModuleObj frm_ModuleObj;
        //创建委托变量
        public CircleDelegate circleDelegate;
        //构造函数
        public CircleROIInfo(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            circleDelegate = Circle;

            frm_ModuleObj = (ModuleObj)obj;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            if (frm_ModuleObj.blnNewModule)
            {
                theSecondTime();
            }
        }

        private void theSecondTime()
        {
            if (frm_ModuleObj.m_ExternalRow)
            {
                Row = frm_ModuleObj.m_Row;
            }
            else
            {
                Row = frm_ModuleObj.m_Row == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Row).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalCol)
            {
                Col = frm_ModuleObj.m_Col;
            }
            else
            {
                Col = frm_ModuleObj.m_Col == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Col).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalRadius)
            {
                Radius = frm_ModuleObj.m_Radius;
            }
            else
            {
                Radius = frm_ModuleObj.m_Radius == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Radius).ToString("f3");
            }
        }

        #region 圆形的参数

        //Row
        public string Row
        {
            get { return (string)this.GetValue(RowProperty); }
            set { this.SetValue(RowProperty, value); }
        }

        public static readonly DependencyProperty RowProperty =
            DependencyProperty.Register("Row", typeof(string), typeof(CircleROIInfo), new PropertyMetadata(default(string)));

        private void Gen_LinkRow_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                //如果开启跟随模式
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Row = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Row = data;
                        frm_ModuleObj.m_ExternalRow = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalRow = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //Col
        public string Col
        {
            get { return (string)this.GetValue(ColProperty); }
            set { this.SetValue(ColProperty, value); }
        }

        public static readonly DependencyProperty ColProperty =
            DependencyProperty.Register("Col", typeof(string), typeof(CircleROIInfo), new PropertyMetadata(default(string)));

        private void Gen_LinkCol_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                //如果开启跟随模式
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Col = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Col = data;
                        frm_ModuleObj.m_ExternalCol = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalCol = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //Radius
        public string Radius
        {
            get { return (string)this.GetValue(RadiusProperty); }
            set { this.SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(string), typeof(CircleROIInfo), new PropertyMetadata(default(string)));

        private void Gen_LinkRadius_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                //如果开启跟随模式
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Radius = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Radius = data;
                        frm_ModuleObj.m_ExternalRadius = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalRadius = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 当前模块ID
        /// </summary>
        public string CurrentModelID
        {
            get { return (string)this.GetValue(CurrentModelIDProperty); }
            set { this.SetValue(CurrentModelIDProperty, value); }
        }

        public static readonly DependencyProperty CurrentModelIDProperty =
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(CircleROIInfo), new PropertyMetadata(default(string)));

        //[委托方法]
        public void Circle(double m_Row, double m_Col, double m_Radius)
        {
            //如果开启跟随模式
            if (frm_ModuleObj.IsFollow)
            {
                frm_ModuleObj.m_Row = Row = m_Row.ToString("f3");
                frm_ModuleObj.m_Col = Col = m_Col.ToString("f3");
                frm_ModuleObj.m_Radius = Radius = m_Radius.ToString("f3");
            }
            else
            {
                if (!frm_ModuleObj.m_ExternalRow)
                {
                    frm_ModuleObj.m_Row = Row = m_Row.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalCol)
                {
                    frm_ModuleObj.m_Col = Col = m_Col.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalRadius)
                {
                    frm_ModuleObj.m_Radius = Radius = m_Radius.ToString("f3");
                }
            }
        }

    }
}
