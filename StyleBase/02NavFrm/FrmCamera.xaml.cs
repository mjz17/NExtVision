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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Common;
using ModuleCamera;
using VisionCore;
using HalconDotNet;
using System.Threading;
using System.Reflection;

namespace StyleBase
{
    /// <summary>
    /// FrmCamera.xaml 的交互逻辑
    /// </summary>
    public partial class FrmCamera : Window
    {

        private List<CameraBase> CameraDgv = new List<CameraBase>();//DatagridView的后台列表

        private List<CamInfo> _CamInfoList = new List<CamInfo>();//相机信息

        private List<string> _CameraAddressLst = new List<string>();//设备列表

        public FrmCamera()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += FrmCamera_Loaded;
        }

        private void FrmCamera_Loaded(object sender, RoutedEventArgs e)
        {
            InitCameraType();
            InitCameraDgv();
            InitCameraTrigger();
        }

        /// <summary>
        /// 初始化设备型号
        /// </summary>
        private void InitCameraType()
        {
            List<string> list = new List<string>();
            foreach (string cameraInfo in Enum.GetNames(typeof(ModuleCamera.DeviceType)))
            {
                list.Add(cameraInfo);
            }
            Cmb_CameraType.ItemsSource = list;
            Cmb_CameraType.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化Dgv列表
        /// </summary>
        private void InitCameraDgv()
        {
            CameraDgv.Clear();
            foreach (CameraBase item in SysProcessPro.g_CameraList)
            {
                CameraDgv.Add(new CameraBase
                {
                    m_DeviceID = item.m_DeviceID,
                    m_bConnected = item.m_bConnected ? true : false,
                    m_SerialNO = item.m_SerialNO,
                    m_UniqueLabe = item.m_UniqueLabe,
                    m_DeviceType = item.m_DeviceType,
                });
            }
            dgv_Camera.ItemsSource = CameraDgv;
        }

        /// <summary>
        /// 触发模式
        /// </summary>
        private void InitCameraTrigger()
        {
            this.Cmb_Trigger.ItemsSource = Enum.GetNames(typeof(TRIGGER_MODE)).ToList();
        }

        /// <summary>
        /// 相机类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmb_CameraType_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            _CamInfoList.Clear();
            switch ((DeviceType)combo.SelectedIndex)
            {
                case DeviceType.None:
                    break;
                case DeviceType.海康相机:
                    #region 海康相机

                    DriveHKVision.SearchCameras(out _CamInfoList);

                    #endregion
                    break;
                case DeviceType.大华相机:
                    #region 大华相机

                    DriveiRAyPLE.SearchCameras(out _CamInfoList);

                    #endregion
                    break;
                default:
                    break;
            }
            _CameraAddressLst.Clear();
            foreach (CamInfo item in _CamInfoList)
            {
                _CameraAddressLst.Add(item.m_SerialNO);
            }

            Cmb_CameraLst.ItemsSource = null;
            Cmb_CameraLst.ItemsSource = _CameraAddressLst;
            Cmb_CameraLst.SelectedIndex = _CameraAddressLst.Count > 0 ? 0 : -1;
        }

        /// <summary>
        /// 添加至列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            //判断是否添加至列表
            int index = SysProcessPro.g_CameraList.FindIndex(c => c.m_SerialNO == Cmb_CameraLst.Text);
            if (index > -1)
            {
                System.Windows.Forms.MessageBox.Show("设备已经添加至列表,无需重复添加！");
                return;
            }

            //判断设备列表是否为空
            if (Cmb_CameraLst.Text.Length <= 0)
            {
                System.Windows.Forms.MessageBox.Show("没有需要添加至列表的设备！");
                return;
            }

            CameraBase camera;

            switch ((DeviceType)Cmb_CameraType.SelectedIndex)
            {
                case DeviceType.None:
                    break;
                case DeviceType.海康相机:
                    #region 海康相机

                    camera = new DriveHKVision(DeviceType.海康相机);
                    CamInfo Hkinfo = _CamInfoList.Find(c => c.m_SerialNO == Cmb_CameraLst.Text);
                    camera.m_DeviceType = DeviceType.海康相机;
                    camera.m_SerialNO = Hkinfo.m_SerialNO;
                    ((DriveHKVision)camera).m_ExtInfo = Hkinfo.m_ExtInfo;
                    SysProcessPro.g_CameraList.Add(camera);//添加之项目
                    CameraDgv.Add(camera);//添加之Dgv

                    #endregion
                    break;
                case DeviceType.大华相机:
                    #region 大华相机

                    camera = new DriveiRAyPLE(DeviceType.大华相机);
                    CamInfo Rayinfo = _CamInfoList.Find(c => c.m_SerialNO == Cmb_CameraLst.Text);
                    camera.m_DeviceType = DeviceType.大华相机;
                    camera.m_SerialNO = Rayinfo.m_SerialNO;
                    ((DriveiRAyPLE)camera).m_ExtInfo = Rayinfo.m_ExtInfo;
                    SysProcessPro.g_CameraList.Add(camera);//添加之项目
                    CameraDgv.Add(camera);//添加之Dgv

                    #endregion
                    break;
                default:
                    break;
            }
            RefreshDgv();
        }

        /// <summary>
        /// 点击DataView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_Camera_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                int index = dgv_Camera.SelectedIndex;
                if (index > -1)
                {
                    CameraInfoShow(SysProcessPro.g_CameraList[index]);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        #region 曝光

        public int Exposure
        {
            get { return (int)this.GetValue(ExposureProperty); }
            set { this.SetValue(ExposureProperty, value); }
        }

        public static readonly DependencyProperty ExposureProperty =
            DependencyProperty.Register("Exposure", typeof(int), typeof(FrmCamera), new PropertyMetadata(default(int)));

        #endregion

        #region 增益

        public int Gain
        {
            get { return (int)this.GetValue(GainProperty); }
            set { this.SetValue(GainProperty, value); }
        }

        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register("Gain", typeof(int), typeof(FrmCamera), new PropertyMetadata(default(int)));

        #endregion

        private void RefreshDgv()
        {
            CameraDgv.Clear();
            foreach (CameraBase item in SysProcessPro.g_CameraList)
            {
                CameraDgv.Add(new CameraBase
                {
                    m_DeviceID = item.m_DeviceID,
                    m_bConnected = item.m_bConnected ? true : false,
                    m_SerialNO = item.m_SerialNO,
                    m_UniqueLabe = item.m_UniqueLabe,
                    m_DeviceType = item.m_DeviceType,
                });
            }
            dgv_Camera.ItemsSource = CameraDgv;

            int index = dgv_Camera.SelectedIndex;
            dgv_Camera.ItemsSource = null;
            dgv_Camera.ItemsSource = CameraDgv;
            if (index > -1)
                dgv_Camera.SelectedIndex = index;
        }

        public void CameraInfoShow(CameraBase cam)
        {
            try
            {
                //相机名称
                this.txt_CameraName.Text = cam.m_DeviceType.ToString();
                //触发模式
                //this.Cmb_Trigger.ItemsSource = Enum.GetNames(typeof(TRIGGER_MODE)).ToList();
                //当前相机触发模式
                this.Cmb_Trigger.SelectedIndex = (int)cam.m_tRIGGER;

                if (cam.m_bConnected)
                {
                    DevConnect.IsEnabled = false;
                    DisConnect.IsEnabled = true;
                    CaptureImage.IsEnabled = true;
                    Exposure = cam.m_ExposeTime;
                    Gain = cam.m_Gain;
                }
                else
                {
                    DevConnect.IsEnabled = true;
                    DisConnect.IsEnabled = false;
                    CaptureImage.IsEnabled = false;
                    Exposure = 1380;
                    Gain = 1;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 连接图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_Camera.SelectedIndex;
                if (index > -1)
                {
                    CameraBase camera = SysProcessPro.g_CameraList[index];
                    if (camera != null)
                    {
                        camera.ConnectDev();

                        if (camera.m_bConnected)
                        {
                            DevConnect.IsEnabled = false;
                            DisConnect.IsEnabled = true;
                            CaptureImage.IsEnabled = true;
                            Exposure = camera.m_ExposeTime;
                            Gain = camera.m_Gain;
                        }
                        else
                        {
                            DevConnect.IsEnabled = true;
                            DisConnect.IsEnabled = false;
                            CaptureImage.IsEnabled = false;
                            Exposure = 1380;
                            Gain = 1;
                        }
                    }
                    CameraBase cameraBase = CameraDgv.Find(c => c.m_DeviceType == camera.m_DeviceType && c.m_SerialNO == camera.m_SerialNO);
                    if (cameraBase != null)
                    {
                        cameraBase.m_bConnected = camera.m_bConnected;
                    }
                    RefreshDgv();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("未选中相机");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_Camera.SelectedIndex;
                if (index > -1)
                {
                    CameraBase camera = SysProcessPro.g_CameraList[index];
                    if (camera != null)
                    {
                        camera.DisConnectDev();
                        if (camera.m_bConnected)
                        {
                            DevConnect.IsEnabled = false;
                            DisConnect.IsEnabled = true;
                            CaptureImage.IsEnabled = true;
                            Exposure = camera.m_ExposeTime;
                            Gain = camera.m_Gain;
                        }
                        else
                        {
                            DevConnect.IsEnabled = true;
                            DisConnect.IsEnabled = false;
                            CaptureImage.IsEnabled = false;
                            Exposure = 1380;
                            Gain = 1;
                        }
                    }
                    CameraBase cameraBase = CameraDgv.Find(c => c.m_DeviceType == camera.m_DeviceType && c.m_SerialNO == camera.m_SerialNO);
                    if (cameraBase != null)
                    {
                        cameraBase.m_bConnected = camera.m_bConnected;
                    }
                    RefreshDgv();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("未选中相机");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 采集图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Cmb_Trigger.SelectedIndex == -1)
                {
                    System.Windows.Forms.MessageBox.Show("当前相机未选择触发模式！");
                    return;
                }

                int index = dgv_Camera.SelectedIndex;
                if (index > -1)
                {
                    CameraBase camera = SysProcessPro.g_CameraList[index];
                    if (camera != null)
                    {
                        switch ((TRIGGER_MODE)Cmb_Trigger.SelectedIndex)
                        {
                            case TRIGGER_MODE.软件触发:
                                camera.m_ExposeTime = Exposure;//曝光
                                camera.m_Gain = Gain;//增益
                                camera.m_tRIGGER = TRIGGER_MODE.软件触发;
                                camera.SetSetting();
                                camera.eventWait.Reset();
                                camera.CaptureImage(true);
                                camera.eventWait.WaitOne();
                                if (camera.m_Image != null && camera.m_Image.IsInitialized())
                                {
                                    Main_HalconView.HobjectToHimage(camera.m_Image);
                                }
                                break;
                            case TRIGGER_MODE.连续采集:

                                //判断是否有连续采集的相机
                                foreach (CameraBase item in SysProcessPro.g_CameraList)
                                {
                                    if (item.m_tRIGGER == TRIGGER_MODE.连续采集)
                                    {
                                        //先将相机模式关闭
                                        item.m_tRIGGER = TRIGGER_MODE.软件触发;
                                        item.SetSetting();
                                    }
                                }

                                Thread.Sleep(250);//2023.3.31 赵一添加

                                camera.m_ExposeTime = Exposure;//曝光
                                camera.m_Gain = Gain;//增益
                                camera.m_tRIGGER = TRIGGER_MODE.连续采集;
                                camera.SetSetting();

                                Task.Run(new Action(() =>
                                {
                                    while (camera.m_tRIGGER == TRIGGER_MODE.连续采集)
                                    {
                                        if (camera.m_Image != null && camera.m_Image.IsInitialized())
                                        {
                                            Main_HalconView.HobjectToHimage(camera.m_Image);
                                        }
                                        Thread.Sleep(100);
                                    }
                                }));
                                break;
                            case TRIGGER_MODE.硬件触发:

                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("未选中相机");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            bool CameraStatus = false;
            foreach (CameraBase item in SysProcessPro.g_CameraList)
            {
                //判断是否连接
                if (item.m_bConnected)
                {
                    if (item.m_tRIGGER == TRIGGER_MODE.连续采集)
                    {
                        CameraStatus = true;
                        break;
                    }
                }
            }

            //如果有相机连续采集的
            if (CameraStatus)
            {
                System.Windows.Forms.MessageBox.Show("当前相机正处于连续采集，请停止连续采集，再关闭窗体");
                return;
            }
            //关闭窗体
            this.Close();
        }

        /// <summary>
        /// 右键鼠标弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_Camera_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_Camera.SelectedIndex;
                if (index > -1)
                {
                    //判断是否连接中
                    CameraBase camera = SysProcessPro.g_CameraList[index];
                    if (camera != null)
                    {
                        if (camera.m_bConnected)
                        {
                            System.Windows.Forms.MessageBox.Show("该设备处于连接状态！");
                            return;
                        }
                        SysProcessPro.g_CameraList.RemoveAt(index);
                        RefreshDgv();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 删除全部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (CameraBase item in SysProcessPro.g_CameraList)
                {
                    if (item.m_bConnected)
                    {
                        System.Windows.Forms.MessageBox.Show(item.m_DeviceID + "该设备处于连接状态！");
                        return;
                    }
                }
                SysProcessPro.g_CameraList.Clear();
                CameraBase.m_LastDeviceID = 0;
                RefreshDgv();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleFrm_CloseWindow(object sender, RoutedEventArgs e)
        {
            btn_Confirm_Click(null, null);
        }

    }
}
