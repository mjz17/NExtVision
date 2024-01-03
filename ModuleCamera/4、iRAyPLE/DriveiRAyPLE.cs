using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThridLibray;

namespace ModuleCamera
{
    /// <summary>
    /// 华睿SDK
    /// </summary>

    [Serializable]
    public class DriveiRAyPLE : CameraBase
    {
        /// <summary>
        /// 图像缓存列表
        /// </summary>
        [NonSerialized]
        List<IGrabbedRawData> m_frameList = new List<IGrabbedRawData>();

        /// <summary>
        /// 线程锁，保证多线程安全
        /// </summary>
        [NonSerialized]
        Mutex m_mutex = new Mutex();

        /// <summary>
        /// 设备对象
        /// </summary>
        [NonSerialized]
        IDevice m_dev;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_DeviceType"></param>
        public DriveiRAyPLE(DeviceType _DeviceType) : base(_DeviceType)
        {
        }

        /// <summary>
        /// 查询相机，添加到相机列表中
        /// </summary>
        /// <param name="m_CamInfoList"></param>
        public static void SearchCameras(out List<CamInfo> m_CamInfoList)
        {
            m_CamInfoList = new List<CamInfo>();
            List<IDeviceInfo> li = Enumerator.EnumerateDevices();
            if (li.Count > 0)
            {
                for (int i = 0; i < li.Count; i++)
                {
                    m_CamInfoList.Add(new CamInfo
                    {
                        m_DeviceType = DeviceType.大华相机,//相机类型
                        m_SerialNO = li[i].Key,//相机ID
                        m_ExtInfo = li[i].Version,//相机版本
                    });
                }
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

                //创建对象（设备厂商名:设备序列号）
                //利用相机ID获取相机对象
                List<IDeviceInfo> li = Enumerator.EnumerateDevices();
                m_dev = Enumerator.GetDeviceByKey(m_SerialNO);

                if (m_dev == null)
                {
                    System.Windows.Forms.MessageBox.Show("Open camera failed！");
                    Log.Error("打开相机失败！");
                    m_bConnected = false;
                    return;
                }

                /* 注册连接事件 */
                m_dev.CameraOpened += OnCameraOpen;
                m_dev.ConnectionLost += OnConnectLoss;
                m_dev.CameraClosed += OnCameraClose;

                //打开设备
                if (!m_dev.Open())
                {
                    System.Windows.Forms.MessageBox.Show("Open camera failed！");
                    Log.Error("打开相机失败！");
                    m_bConnected = false;
                    return;
                }

                //获取相机参数
                GetSetting();

                //设置相机参数
                SetSetting();

                //注册回调
                //注册码流回调事件
                m_dev.StreamGrabber.ImageGrabbed += OnImageGrabbed;

                //开启抓图
                StartGrabImage();

                m_bConnected = true;
            }
            catch (Exception)
            {
                m_bConnected = false;
            }
        }

        // 相机打开回调。进入该回调表示相机打开
        private void OnCameraOpen(object sender, EventArgs e)
        {
            // 用户自定义操作
        }

        // 相机关闭回调，进入该回调表示相机关闭相机打开
        private void OnCameraClose(object sender, EventArgs e)
        {
            // 用户自定义操作
        }

        // 相机断线回调
        private void OnConnectLoss(object sender, EventArgs e)
        {
            // 用户自定义操作

            // 关闭相机操作
            m_dev.ShutdownGrab();
            m_dev.Dispose();
            m_dev = null;
        }

        //码流数据回调 
        private void OnImageGrabbed(object sender, GrabbedEventArgs e)
        {
            try
            {
                m_mutex.WaitOne();
                m_frameList.Add(e.GrabResult.Clone());
                m_mutex.ReleaseMutex();
                if (m_frameList.Count == 0)
                {
                    Thread.Sleep(50);
                    return;
                }

                // 获取图像队列最新帧 
                m_mutex.WaitOne();
                IGrabbedRawData frame = m_frameList.ElementAt(m_frameList.Count - 1);
                m_frameList.Clear();
                m_mutex.ReleaseMutex();

                // 图像转码成bitmap图像 
                var bitmap = frame.ToBitmap(false);
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData srcBmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                //bitmap转halcon的Image
                HalconDotNet.HObject himg;
                HalconDotNet.HOperatorSet.GenImage1(out himg, "byte", bitmap.Width, bitmap.Height, srcBmpData.Scan0);
                m_Image = new HalconDotNet.HImage(himg);
                eventWait.Set();

                // 主动调用回收垃圾 
                GC.Collect();
            }
            catch (Exception)
            {
                eventWait.Set();
            }
        }

        /// <summary>
        /// 开始取流
        /// </summary>
        public void StartGrabImage()
        {
            if (m_dev == null)
            {
                throw new InvalidOperationException("Start Grab faild");
            }
            else
                m_dev.StreamGrabber.Start();
        }

        /// <summary>
        /// 断开相机
        /// </summary>
        public override void DisConnectDev()
        {

            if (m_dev == null)
            {
                //throw new InvalidOperationException("Device is invalid");
            }

            if (m_dev != null)
            {
                m_dev.StreamGrabber.ImageGrabbed -= OnImageGrabbed;  // 反注册回调 | unregister grab event callback 
                m_dev.ShutdownGrab(); // 停止码流 | stop grabbing 
                m_dev.Close();  // 关闭相机 | close camera 
            }

            m_bConnected = false;

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
                if (m_dev != null && m_bConnected)
                {
                    using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
                    {
                        bool status = p.SetValue(Convert.ToDouble(m_ExposeTime));
                        if (status)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
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
            if (m_dev != null && m_bConnected)
            {
                using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.GainRaw])
                {
                    bool status = p.SetValue(Convert.ToDouble(m_Gain));
                    if (status)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
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
                if (m_dev != null && m_bConnected)
                {
                    switch (tRIGGER_)
                    {
                        case TRIGGER_MODE.软件触发:
                            m_dev.TriggerSet.Open(TriggerSourceEnum.Software);
                            using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                            {
                                p.SetValue(TriggerModeEnum.On);//内触发
                            }
                            using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.AcquisitionMode])
                            {
                                p.SetValue(AcquisitionModeEnum.Continuous);
                            }
                            break;
                        case TRIGGER_MODE.连续采集:

                            using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                            {
                                p.SetValue(TriggerModeEnum.Off);
                            }

                            break;
                        case TRIGGER_MODE.硬件触发:

                            m_dev.TriggerSet.Open(TriggerSourceEnum.Software);
                            using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.TriggerMode])
                            {
                                p.SetValue(TriggerModeEnum.On);//外触发
                            }
                            using (IEnumParameter p = m_dev.ParameterCollection[ParametrizeNameSet.AcquisitionMode])
                            {
                                p.SetValue(AcquisitionModeEnum.SingleFrame);
                            }

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
        /// <param name="byHand"></param>
        /// <returns></returns>
        public override bool CaptureImage(bool byHand)
        {
            try
            {
                if (m_dev == null && m_bConnected)
                {
                    return false;
                }

                m_dev.ExecuteSoftwareTrigger();

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
            if (m_dev == null && m_bConnected)
                return;
            //设置曝光
            SetExposureTime(m_ExposeTime);
            //设置增益 
            SetGain(m_Gain);
            // 设置缓存个数为8（默认值为16） 
            m_dev.StreamGrabber.SetBufferCount(8);
            //设置触发模式
            SetTrigger(m_tRIGGER);

        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public override void GetSetting()
        {
            if (m_dev == null && m_bConnected)
                return;
            //获取曝光参数     
            using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.ExposureTime])
            {
                m_ExposeTime = Convert.ToInt32(p.GetValue());
            }

            //获取增益参数        
            using (IFloatParameter p = m_dev.ParameterCollection[ParametrizeNameSet.GainRaw])
            {
                m_Gain = Convert.ToInt32(p.GetValue());
            }
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            m_mutex = new Mutex();
            m_frameList = new List<IGrabbedRawData>();
        }

    }
}
