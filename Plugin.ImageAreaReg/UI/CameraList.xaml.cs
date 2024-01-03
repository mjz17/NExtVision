using ModuleCamera;
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
using Common;
using StyleBase;

namespace Plugin.ImageAreaReg
{
    /// <summary>
    /// CameraList.xaml 的交互逻辑
    /// </summary>
    public partial class CameraList : UserControl
    {

        private ModuleObj frm_ModuleObj;

        private CameraBase m_AcqDevice;

        public CameraList(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;

            //加载参数
            frm_ModuleObj = (ModuleObj)obj;

            InitCameraList();
            InitCameraModel();

            if (frm_ModuleObj.blnNewModule)
                theSecondTime();

        }

        public void theSecondTime()
        {
            if (frm_ModuleObj.m_DeviceID != null)
                CameraInfo_List.Text = frm_ModuleObj.m_DeviceID.ToString();

            m_AcqDevice = SysProcessPro.g_CameraList.FirstOrDefault(c => c.m_DeviceID == CameraInfo_List.Text);
            if (m_AcqDevice != null && CameraInfo_List.SelectedValue != null)
            {
                if (m_AcqDevice.m_bConnected)
                {
                    m_AcqDevice.GetSetting();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("相机未链接！");
                    Log.Error("相机未链接！");
                }

                frm_ModuleObj.m_DeviceID = CameraInfo_List.SelectedValue.ToString();
                CameraTriger_List.SelectedIndex = (int)m_AcqDevice.m_tRIGGER;//触发模式

                Exposure = m_AcqDevice.m_ExposeTime;
                Gain = m_AcqDevice.m_Gain;

                Txt_ExposeTime.TextValueEvent += Txt_ExposeTime_TextValueEvent;
                Txt_Gain.TextValueEvent += Txt_Gain_TextValueEvent;
                CameraTriger_List.SelectionChanged += CameraTriger_List_SelectionChanged;
            }
        }

        #region 相机列表

        private void InitCameraList()
        {
            List<string> CameraListInfo = new List<string>();
            for (int i = 0; i < SysProcessPro.g_CameraList.Count; i++)
            {
                CameraListInfo.Add(SysProcessPro.g_CameraList[i].m_DeviceID);
            }
            CameraInfo_List.ItemsSource = CameraListInfo;
        }

        #endregion

        #region 触发模式

        private void InitCameraModel()
        {
            CameraTriger_List.ItemsSource = Enum.GetNames(typeof(TRIGGER_MODE));
        }

        #endregion

        #region 曝光

        public int Exposure
        {
            get { return (int)this.GetValue(ExposureProperty); }
            set { this.SetValue(ExposureProperty, value); }
        }

        public static readonly DependencyProperty ExposureProperty =
            DependencyProperty.Register("Exposure", typeof(int), typeof(CameraList), new PropertyMetadata(default(int)));

        #endregion

        #region 增益

        public int Gain
        {
            get { return (int)this.GetValue(GainProperty); }
            set { this.SetValue(GainProperty, value); }
        }

        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register("Gain", typeof(int), typeof(CameraList), new PropertyMetadata(default(int)));

        #endregion

        /// <summary>
        /// 相机采集列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraInfo_List_DropDownClosed(object sender, EventArgs e)
        {
            CameraTriger_List.SelectionChanged -= CameraTriger_List_SelectionChanged;
            Txt_ExposeTime.TextValueEvent -= Txt_ExposeTime_TextValueEvent;
            Txt_Gain.TextValueEvent -= Txt_Gain_TextValueEvent;

            m_AcqDevice = SysProcessPro.g_CameraList.FirstOrDefault(c => c.m_DeviceID == CameraInfo_List.Text);
            if (m_AcqDevice != null)
            {
                if (m_AcqDevice.m_bConnected)
                {
                    m_AcqDevice.GetSetting();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("相机未链接！");
                    Log.Error("相机未链接！");
                }

                CameraTriger_List.SelectedIndex = (int)m_AcqDevice.m_tRIGGER;
                frm_ModuleObj.m_ExposureTime = Exposure = m_AcqDevice.m_ExposeTime;
                frm_ModuleObj.m_Gain = Gain = m_AcqDevice.m_Gain;
            }

            if (CameraInfo_List.SelectedValue != null)
            {
                frm_ModuleObj.m_DeviceID = CameraInfo_List.SelectedValue.ToString();
            }

            CameraTriger_List.SelectionChanged += CameraTriger_List_SelectionChanged;
            Txt_ExposeTime.TextValueEvent += Txt_ExposeTime_TextValueEvent;
            Txt_Gain.TextValueEvent += Txt_Gain_TextValueEvent;
        }

        /// <summary>
        /// 相机触发方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraTriger_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CameraInfo_List.SelectedIndex != -1)
            {
                m_AcqDevice.m_tRIGGER = (TRIGGER_MODE)CameraTriger_List.SelectedIndex;
            }
        }

        /// <summary>
        /// 采集超时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Txt_TimeOut_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Txt_ExposeTime_TextValueEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CameraInfo_List.SelectedIndex != -1)
                {
                    frm_ModuleObj.m_ExposureTime = Exposure;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        private void Txt_Gain_TextValueEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CameraInfo_List.SelectedIndex != -1)
                {
                    frm_ModuleObj.m_Gain = Gain;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }
    }
}
