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

    //[声明委托]
    public delegate void RectDelegate(double m_Length1, double m_Length2, double m_CenterRow, double m_CenterCol, double m_Phi);

    /// <summary>
    /// RectROIInfo.xaml 的交互逻辑
    /// </summary>
    public partial class RectROIInfo : UserControl
    {
        //后台数据
        private ModuleObj frm_ModuleObj;

        //创建委托变量
        public RectDelegate rectDelegate;

        //构造函数
        public RectROIInfo(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;

            frm_ModuleObj = (ModuleObj)obj;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            rectDelegate = Rect;
            if (frm_ModuleObj.blnNewModule)
            {
                theSecondTime();
            }
        }

        private void theSecondTime()
        {
            if (frm_ModuleObj.m_ExternalLength1)
            {
                Length1 = frm_ModuleObj.m_Length1;
            }
            else
            {
                Length1 = frm_ModuleObj.m_Length1 == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Length1).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalLength2)
            {
                Length2 = frm_ModuleObj.m_Length2;
            }
            else
            {
                Length2 = frm_ModuleObj.m_Length2 == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Length2).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalCenterRow)
            {
                CenterRow = frm_ModuleObj.m_CenterRow;
            }
            else
            {
                CenterRow = frm_ModuleObj.m_CenterRow == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_CenterRow).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalCenterCol)
            {
                CenterCol = frm_ModuleObj.m_CenterCol;
            }
            else
            {
                CenterCol = frm_ModuleObj.m_CenterCol == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_CenterCol).ToString("f3");
            }

            if (frm_ModuleObj.m_ExternalPhi)
            {
                Phi = frm_ModuleObj.m_Phi;
            }
            else
            {
                Phi = frm_ModuleObj.m_Phi == string.Empty ? "" : Convert.ToDouble(frm_ModuleObj.m_Phi).ToString("f3");
            }

        }

        #region 矩形的参数

        //Length1
        public string Length1
        {
            get { return (string)this.GetValue(Length1Property); }
            set { this.SetValue(Length1Property, value); }
        }

        public static readonly DependencyProperty Length1Property =
            DependencyProperty.Register("Length1", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        private void Gen_LinkLength1_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Length1 = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Length1 = data;
                        frm_ModuleObj.m_ExternalLength1 = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalLength1 = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //Length2
        public string Length2
        {
            get { return (string)this.GetValue(Length2Property); }
            set { this.SetValue(Length2Property, value); }
        }

        public static readonly DependencyProperty Length2Property =
            DependencyProperty.Register("Length2", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        private void Gen_LinkLength2_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Length2 = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Length2 = data;
                        frm_ModuleObj.m_ExternalLength2 = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalLength2 = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //CenterRow
        public string CenterRow
        {
            get { return (string)this.GetValue(CenterRowProperty); }
            set { this.SetValue(CenterRowProperty, value); }
        }

        public static readonly DependencyProperty CenterRowProperty =
            DependencyProperty.Register("CenterRow", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        private void Gen_CenterRow_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_CenterRow = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_CenterRow = data;
                        frm_ModuleObj.m_ExternalCenterRow = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalCenterRow = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //CenterCol
        public string CenterCol
        {
            get { return (string)this.GetValue(CenterColProperty); }
            set { this.SetValue(CenterColProperty, value); }
        }

        public static readonly DependencyProperty CenterColProperty =
            DependencyProperty.Register("CenterCol", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        private void Gen_CenterCol_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_CenterCol = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_CenterCol = data;
                        frm_ModuleObj.m_ExternalCenterCol = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalCenterCol = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //CenterCol
        public string Phi
        {
            get { return (string)this.GetValue(PhiProperty); }
            set { this.SetValue(PhiProperty, value); }
        }

        public static readonly DependencyProperty PhiProperty =
            DependencyProperty.Register("Phi", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        private void Gen_Phi_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!frm_ModuleObj.IsFollow)
                {
                    ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                    if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
                    {
                        frm_ModuleObj.m_Phi = data.m_DataTip + "." + data.m_DataName;
                        frm_ModuleObj.Link_Phi = data;
                        frm_ModuleObj.m_ExternalPhi = true;
                    }
                    else
                    {
                        frm_ModuleObj.m_ExternalPhi = false;
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
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(RectROIInfo), new PropertyMetadata(default(string)));

        //[委托方法]
        public void Rect(double m_CenterRow, double m_CenterCol, double m_Phi, double m_Length1, double m_Length2)
        {
            //开启跟随模式
            if (frm_ModuleObj.IsFollow)
            {
                frm_ModuleObj.m_CenterRow = CenterRow = m_CenterRow.ToString("f3");
                frm_ModuleObj.m_CenterCol = CenterCol = m_CenterCol.ToString("f3");
                frm_ModuleObj.m_Phi = Phi = m_Phi.ToString("f3");
                frm_ModuleObj.m_Length1 = Length1 = m_Length1.ToString("f3");
                frm_ModuleObj.m_Length2 = Length2 = m_Length2.ToString("f3"); 
            }
            else
            {
                if (!frm_ModuleObj.m_ExternalCenterRow)
                {
                    frm_ModuleObj.m_CenterRow = CenterRow = m_CenterRow.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalCenterCol)
                {
                    frm_ModuleObj.m_CenterCol = CenterCol = m_CenterCol.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalPhi)
                {
                    frm_ModuleObj.m_Phi = Phi = m_Phi.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalLength1)
                {
                    frm_ModuleObj.m_Length1 = Length1 = m_Length1.ToString("f3");
                }
                if (!frm_ModuleObj.m_ExternalLength2)
                {
                    frm_ModuleObj.m_Length2 = Length2 = m_Length2.ToString("f3");
                }            
            }
        }

    }
}
