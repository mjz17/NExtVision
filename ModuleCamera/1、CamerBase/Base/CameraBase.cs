using HalconDotNet;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModuleCamera
{

    /// <summary>
    /// 相机的基础类
    /// </summary>
    [Serializable]
    public class CameraBase
    {

        public static int m_LastDeviceID = 0;//序列号ID

        /// <summary>
        /// 设备ID
        /// </summary>
        private string _DeviceID;

        public string m_DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }

        /// <summary>
        /// 相机品牌
        /// </summary>
        private DeviceType _DeviceType;

        public DeviceType m_DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }

        /// <summary>
        /// 标识符
        /// </summary>
        private string _UniqueLabe;

        public string m_UniqueLabe
        {
            get { return _UniqueLabe; }
            set { _UniqueLabe = value; }
        }

        /// <summary>
        /// 设备内部编号
        /// </summary>
        private string _SerialNO;

        public string m_SerialNO
        {
            set { _SerialNO = value; }
            get { return _SerialNO; }
        }

        /// <summary>
        /// SDK版本信息
        /// </summary>
        private string _ExtInfo;

        public string m_ExtInfo
        {
            get { return _ExtInfo; }
            set { _ExtInfo = value; }
        }

        /// <summary>
        /// 曝光
        /// </summary>
        private int _ExposeTime = 1360;

        public int m_ExposeTime
        {
            get { return _ExposeTime; }
            set { _ExposeTime = value; }
        }

        /// <summary>
        /// 增益
        /// </summary>
        private int _Gain = 1;

        public int m_Gain
        {
            get { return _Gain; }
            set { _Gain = value; }
        }

        /// <summary>
        /// 触发模式
        /// </summary>
        private TRIGGER_MODE _tRIGGER;

        public TRIGGER_MODE m_tRIGGER
        {
            get { return _tRIGGER; }
            set { _tRIGGER = value; }
        }


        /// <summary>
        /// 采集的图像
        /// </summary>
        [NonSerialized]
        private HImage _Image = new HImageExt();
        public HImage m_Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        /// <summary>
        /// 图像的宽度
        /// </summary>
        [NonSerialized]
        private int _ImgWidth;

        public int m_ImgWidth
        {
            get { return _ImgWidth; }
            set { _ImgWidth = value; }
        }

        /// <summary>
        /// 图像的高度
        /// </summary>
        [NonSerialized]
        private int _ImgHeight;

        public int m_ImgHeight
        {
            get { return _ImgHeight; }
            set { _ImgHeight = value; }
        }

        /// <summary>
        /// 设备连接状态
        /// </summary>
        private bool _bConnected = false;

        public bool m_bConnected
        {
            get { return _bConnected; }
            set { _bConnected = value; }
        }

        /// <summary>
        /// 是不是最新采集的图像
        /// </summary>
        [NonSerialized]
        private bool _IsNewImage;

        public bool m_IsNewImage
        {
            get { return _IsNewImage; }
            set { _IsNewImage = value; }
        }

        /// <summary>
        /// X方向像素当量
        /// </summary>
        private double dScaleX = 1.0;
        public double ScaleX
        {
            get { return dScaleX; }
            set { dScaleX = value; }
        }

        /// <summary>
        /// Y方向像素当量
        /// </summary>
        private double dScaleY = 1.0;
        public double ScaleY
        {
            get { return dScaleY; }
            set { dScaleY = value; }
        }

        /// <summary>
        /// 采集信号
        /// </summary>
        [NonSerialized]
        public AutoResetEvent eventWait = new AutoResetEvent(false);

        /// <summary>
        /// 构造函数1
        /// </summary>
        public CameraBase()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="deviceType">初始化时指定设备类型</param>
        public CameraBase(DeviceType deviceType)
        {
            this._DeviceType = deviceType;
            m_LastDeviceID++;
            _DeviceID = "Dev" + m_LastDeviceID;
        }

        /// <summary>
        /// 建立设备连接
        /// </summary>
        public virtual void ConnectDev() { }

        /// <summary>
        /// 断开设备连接
        /// </summary>
        public virtual void DisConnectDev() { }

        /// <summary>
        /// 设置触发模式
        /// </summary>
        public virtual bool SetTrigger(TRIGGER_MODE tRIGGER_) { return true; }

        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetExposureTime(int value) { return true; }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetGain(int value) { return true; }

        /// <summary>
        /// 获取参数
        /// </summary>
        public virtual void GetSetting() { }

        /// <summary>
        /// 设置参数
        /// </summary>
        public virtual void SetSetting() { }

        /// <summary>
        /// 取图
        /// </summary>
        /// <returns></returns>
        public virtual bool CaptureImage(bool byHand) { return true; }

        [OnDeserializing()]
        internal void OnDeSerializingMethod(StreamingContext context)
        {
            _Image = new HImage();
            eventWait = new AutoResetEvent(false);
        }
    }
}
