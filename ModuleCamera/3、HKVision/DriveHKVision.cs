using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvCamCtrl.NET;
using Common;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization;
using HalconDotNet;
using ThridLibray;
using MvCamCtrl.NET.CameraParams;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;

namespace ModuleCamera
{

    /// <summary>
    /// 委托
    /// </summary>
    /// <param name="image"></param>
    /// <param name="_user"></param>
    public delegate void DispImageCallback(HImage image, int _user);

    /// <summary>
    /// 海康威视
    /// </summary>
    [Serializable]
    public class DriveHKVision : CameraBase
    {
        /// <summary>
        /// 线程锁，保证多线程安全
        /// </summary>
        [NonSerialized]
        Mutex m_mutex = new Mutex();

        [NonSerialized]
        private CCamera m_pMyCamera = new CCamera();

        [NonSerialized]
        public cbOutputExdelegate ImageCallback;

        static List<CCameraInfo> m_ltDeviceList = new List<CCameraInfo>();

        [NonSerialized]
        Bitmap m_pcBitmap = null;

        /// <summary>
        /// 委托变量
        /// </summary>
        [NonSerialized]
        public DispImageCallback dispImgCallback = null;

        [NonSerialized]
        PixelFormat m_enBitmapPixelFormat = PixelFormat.DontCare;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_DeviceType"></param>
        public DriveHKVision(DeviceType _DeviceType) : base(_DeviceType)
        {

        }

        /// <summary>
        /// 查询相机，添加到相机列表中
        /// </summary>
        /// <param name="m_CamInfoList"></param>
        public static void SearchCameras(out List<CamInfo> m_CamInfoList)
        {
            System.GC.Collect();
            m_CamInfoList = new List<CamInfo>();
            //List<CCameraInfo> ltDeviceList = new List<CCameraInfo>();

            // ch:创建设备列表 | en:Create Device List
            int nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE | CSystem.MV_USB_DEVICE, ref m_ltDeviceList);
            if (0 != nRet)
            {
                System.Windows.Forms.MessageBox.Show("查找设备失败！");
                Log.Error("查找设备失败！");
                return;
            }

            // ch:打印设备信息 en:Print device info
            for (int i = 0; i < m_ltDeviceList.Count; i++)
            {
                CamInfo _camInfo = new CamInfo();
                if (CSystem.MV_GIGE_DEVICE == m_ltDeviceList[i].nTLayerType)//网口类型
                {
                    CGigECameraInfo gigeInfo = (CGigECameraInfo)m_ltDeviceList[i];
                    if (gigeInfo.UserDefinedName != "")
                    {
                        _camInfo.m_SerialNO = gigeInfo.chSerialNumber;
                        _camInfo.m_DeviceType = DeviceType.海康相机;
                    }
                    else
                    {
                        _camInfo.m_SerialNO = gigeInfo.chSerialNumber;
                        _camInfo.m_DeviceType = DeviceType.海康相机;
                    }
                }
                else if (CSystem.MV_USB_DEVICE == m_ltDeviceList[i].nTLayerType)//USB类型
                {
                    CUSBCameraInfo usbInfo = (CUSBCameraInfo)m_ltDeviceList[i];
                    if (usbInfo.UserDefinedName != "")
                    {
                        _camInfo.m_SerialNO = usbInfo.chSerialNumber;
                        _camInfo.m_DeviceType = DeviceType.海康相机;
                    }
                    else
                    {
                        _camInfo.m_SerialNO = usbInfo.chSerialNumber;
                        _camInfo.m_DeviceType = DeviceType.海康相机;
                    }
                }
                m_CamInfoList.Add(_camInfo);
            }

        }

        /// <summary>
        /// 连接相机
        /// </summary>
        public override void ConnectDev()
        {
            try
            {
                // 如果设备已经连接先断开
                DisConnectDev();

                // ch:打开设备 | en:Open device
                if (m_pMyCamera == null)
                {
                    m_pMyCamera = new CCamera();
                    if (m_pMyCamera == null)
                    {
                        return;
                    }
                }

                // ch:获取选择的设备信息 | en:Get selected device information
                CCameraInfo device = m_ltDeviceList.Find(item => ((CGigECameraInfo)item).chSerialNumber == m_SerialNO);
                if (device == null)
                {
                    System.Windows.Forms.MessageBox.Show("camera failed！");
                    Log.Error("设备查询异常！");
                    m_bConnected = false;
                    return;
                }

                int nRet = m_pMyCamera.CreateHandle(ref device);
                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Open camera failed！");
                    Log.Error("打开相机失败！");
                    m_bConnected = false;
                    return;
                }

                nRet = m_pMyCamera.OpenDevice();
                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Device open fail！");
                    Log.Error("打开相机失败！");
                    m_bConnected = false;
                    return;
                }

                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                if (device.nTLayerType == CSystem.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_pMyCamera.GIGE_GetOptimalPacketSize();
                    if (0 < nPacketSize)
                    {
                        nRet = m_pMyCamera.SetIntValue("GevSCPSPacketSize", (uint)nPacketSize);
                        if (nRet != CErrorDefine.MV_OK)
                        {
                            System.Windows.Forms.MessageBox.Show("Set Packet Size failed！");
                            Log.Error("设置数据包大小失败！");
                        }
                    }
                }

                // ch:设置触发模式为off || en:set trigger mode as off
                nRet = m_pMyCamera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Set TriggerMode failed！");
                    Log.Error("触发模式设置错误！");
                    m_bConnected = false;
                    return;
                }

                //获取相机参数
                GetSetting();

                //设置相机参数
                SetSetting();

                // ch:注册回调函数 | en:Register image callback
                ImageCallback = new cbOutputExdelegate(ImageCallbackFunc);
                nRet = m_pMyCamera.RegisterImageCallBackEx(ImageCallback, IntPtr.Zero);

                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Register image callback failed！");
                    Log.Error("注册回调函数异常！");
                    m_bConnected = false;
                    return;
                }

                // ch:前置配置 | en:pre-operation
                nRet = NecessaryOperBeforeGrab();
                if (CErrorDefine.MV_OK != nRet)
                {
                    m_bConnected = false;
                    return;
                }

                // ch:开启抓图 || en: start grab image
                nRet = m_pMyCamera.StartGrabbing();
                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Start grabbing failed！");
                    Log.Error("开启抓图异常！");
                    m_bConnected = false;
                    return;
                }

                m_bConnected = true;
            }
            catch (Exception ex)
            {
                m_bConnected = false;
            }
        }

        /// <summary>
        /// 断开相机连接
        /// </summary>
        public override void DisConnectDev()
        {
            try
            {
                if (m_pMyCamera == null)
                {
                    return;
                }

                if (m_pMyCamera != null)
                {
                    m_pMyCamera.CloseDevice();
                    m_pMyCamera.DestroyHandle();
                }

                m_bConnected = false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 回调的方法
        /// </summary>
        /// <param name="pData"></param>
        /// <param name="pFrameInfo"></param>
        /// <param name="pUser"></param>
        public void ImageCallbackFunc(IntPtr pData, ref MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            try
            {
                // 获取图像队列最新帧 
                m_mutex.WaitOne();
                m_mutex.ReleaseMutex();

                if (pFrameInfo.enPixelType == MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    m_Image.GenImage1("byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);
                }
                else
                {
                    //bitmap转halcon的Image
                    HalconDotNet.HObject himg;
                    HalconDotNet.HOperatorSet.GenImage1(out himg, "byte", pFrameInfo.nWidth, pFrameInfo.nHeight, pData);
                    m_Image = new HalconDotNet.HImage(himg);
                }

                eventWait.Set();
                if (dispImgCallback != null)
                {
                    dispImgCallback(m_Image, 0);
                }

                // 主动调用回收垃圾 
                GC.Collect();
            }
            catch (Exception)
            {
                eventWait.Set();
            }
        }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetExposureTime(int value)
        {
            m_ExposeTime = value;
            try
            {
                if (m_pMyCamera != null && m_bConnected)
                {
                    m_pMyCamera.SetEnumValue("ExposureAuto", 0);
                    int nRet = m_pMyCamera.SetFloatValue("ExposureTime", (float)value);
                    if (nRet != CErrorDefine.MV_OK)
                    {
                        System.Windows.Forms.MessageBox.Show("Set Exposure Time Fail!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetGain(int value)
        {
            m_Gain = value;
            try
            {
                if (m_pMyCamera != null && m_bConnected)
                {
                    m_pMyCamera.SetEnumValue("GainAuto", 0);
                    int nRet = m_pMyCamera.SetFloatValue("Gain", (float)value);
                    if (nRet != CErrorDefine.MV_OK)
                    {
                        System.Windows.Forms.MessageBox.Show("Set Gain Fail!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        /// <param name="tRIGGER_"></param>
        /// <returns></returns>
        public override bool SetTrigger(TRIGGER_MODE tRIGGER_)
        {
            try
            {
                if (m_pMyCamera != null && m_bConnected)
                {
                    switch (tRIGGER_)
                    {
                        case TRIGGER_MODE.软件触发:
                            m_pMyCamera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);
                            m_pMyCamera.SetEnumValue("TriggerSource", (uint)MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                            break;
                        case TRIGGER_MODE.连续采集:
                            m_pMyCamera.SetEnumValue("TriggerMode", (uint)MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF);
                            break;
                        case TRIGGER_MODE.硬件触发:
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 采集图像
        /// </summary>
        /// <param name="byHand">是否手动采集图像</param>
        /// <returns></returns>
        public override bool CaptureImage(bool byHand)
        {
            try
            {
                if (m_pMyCamera == null)
                {
                    m_bConnected = false;
                    return false;
                }

                // ch:触发命令 | en:Trigger command
                int nRet = m_pMyCamera.SetCommandValue("TriggerSoftware");
                if (CErrorDefine.MV_OK != nRet)
                {
                    System.Windows.Forms.MessageBox.Show("Open camera failed！");
                    Log.Error("打开相机失败！");
                    eventWait.Set();//2023.9.28 Nick添加
                    m_bConnected = false;
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        public override void SetSetting()
        {
            if (m_pMyCamera == null && !m_bConnected)
                return;

            //设置曝光
            SetExposureTime(m_ExposeTime);
            //设置增益 
            SetGain(m_Gain);
            // 设置缓存个数为8（默认值为16） 
            //m_dev.StreamGrabber.SetBufferCount(8);
            //设置触发模式
            SetTrigger(m_tRIGGER);

        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public override void GetSetting()
        {
            if (m_pMyCamera == null && !m_bConnected)
                return;
            CFloatValue pcFloatValue = new CFloatValue();
            int nRet = CErrorDefine.MV_OK;
            // 获取曝光时间
            nRet = m_pMyCamera.GetFloatValue("ExposureTime", ref pcFloatValue);
            if (CErrorDefine.MV_OK == nRet)
            {
                m_ExposeTime = Convert.ToInt32(pcFloatValue.CurValue);
            }
            //获取增益参数        
            nRet = m_pMyCamera.GetFloatValue("Gain", ref pcFloatValue);
            if (CErrorDefine.MV_OK == nRet)
            {
                m_Gain = Convert.ToInt32(pcFloatValue.CurValue);
            }
        }

        //取图前的必要操作步骤
        private Int32 NecessaryOperBeforeGrab()
        {
            // ch:取图像宽 | en:Get Iamge Width
            CIntValue pcWidth = new CIntValue();
            int nRet = m_pMyCamera.GetIntValue("Width", ref pcWidth);
            if (CErrorDefine.MV_OK != nRet)
            {
                System.Windows.Forms.MessageBox.Show("Get Width Info Fail!");
                return nRet;
            }
            // ch:取图像高 | en:Get Iamge Height
            CIntValue pcHeight = new CIntValue();
            nRet = m_pMyCamera.GetIntValue("Height", ref pcHeight);
            if (CErrorDefine.MV_OK != nRet)
            {
                System.Windows.Forms.MessageBox.Show("Get Height Info Fail!");
                return nRet;
            }
            // ch:取像素格式 | en:Get Pixel Format
            CEnumValue pcPixelFormat = new CEnumValue();
            nRet = m_pMyCamera.GetEnumValue("PixelFormat", ref pcPixelFormat);
            if (CErrorDefine.MV_OK != nRet)
            {
                System.Windows.Forms.MessageBox.Show("Get Pixel Format Fail!");
                return nRet;
            }

            // ch:设置bitmap像素格式
            if ((Int32)MvGvspPixelType.PixelType_Gvsp_Undefined == (Int32)pcPixelFormat.CurValue)
            {
                System.Windows.Forms.MessageBox.Show("Unknown Pixel Format!");
                return CErrorDefine.MV_E_UNKNOW;
            }
            else if (IsMono((MvGvspPixelType)pcPixelFormat.CurValue))
            {
                m_enBitmapPixelFormat = PixelFormat.Format8bppIndexed;
            }
            else
            {
                m_enBitmapPixelFormat = PixelFormat.Format24bppRgb;
            }

            if (null != m_pcBitmap)
            {
                m_pcBitmap.Dispose();
                m_pcBitmap = null;
            }
            m_pcBitmap = new Bitmap((Int32)pcWidth.CurValue, (Int32)pcHeight.CurValue, m_enBitmapPixelFormat);

            // ch:Mono8格式，设置为标准调色板 | en:Set Standard Palette in Mono8 Format
            if (PixelFormat.Format8bppIndexed == m_enBitmapPixelFormat)
            {
                ColorPalette palette = m_pcBitmap.Palette;
                for (int i = 0; i < palette.Entries.Length; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                m_pcBitmap.Palette = palette;
            }

            return CErrorDefine.MV_OK;


        }

        private Boolean IsMono(MvGvspPixelType enPixelType)
        {
            switch (enPixelType)
            {
                case MvGvspPixelType.PixelType_Gvsp_Mono1p:
                case MvGvspPixelType.PixelType_Gvsp_Mono2p:
                case MvGvspPixelType.PixelType_Gvsp_Mono4p:
                case MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MvGvspPixelType.PixelType_Gvsp_Mono8_Signed:
                case MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                case MvGvspPixelType.PixelType_Gvsp_Mono14:
                case MvGvspPixelType.PixelType_Gvsp_Mono16:
                    return true;
                default:
                    return false;
            }
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            // ch:创建设备列表 | en:Create Device List
            int nRet = CSystem.EnumDevices(CSystem.MV_GIGE_DEVICE | CSystem.MV_USB_DEVICE, ref m_ltDeviceList);
            if (0 != nRet)
            {
                System.Windows.Forms.MessageBox.Show("查找设备失败！");
                Log.Error("查找设备失败！");
                return;
            }

            m_mutex = new Mutex();
        }

    }
}
