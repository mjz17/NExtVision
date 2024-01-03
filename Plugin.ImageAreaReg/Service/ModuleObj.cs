using Common;
using HalconDotNet;
using ModuleCamera;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using VisionCore;

namespace Plugin.ImageAreaReg
{
    [Category("图像处理")]
    [DisplayName("采集图像")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        [NonSerialized]
        public CameraBase m_AcqDevice;

        //采集方式
        public CameraModel m_CameraModel = CameraModel.指定图像;

        #region 指定图片

        public string SingleImagePath = string.Empty;

        #endregion

        #region 文件目录

        public FileImageMethod Filemethod;

        public List<FileImageModel> files = new List<FileImageModel>();

        #endregion

        #region 相机

        //相机ID
        private string _DeviceID;
        public string m_DeviceID
        {
            set
            {
                _DeviceID = value;
                m_AcqDevice = (CameraBase)SysProcessPro.g_CameraList.FirstOrDefault(c => c.m_DeviceID == _DeviceID);
            }
            get
            {
                return _DeviceID;
            }
        }

        //曝光
        public int m_ExposureTime { get; set; } = 3600;

        //增益
        public int m_Gain { get; set; } = 0;

        #endregion

        #region 图像处理

        /// <summary>
        /// 图片读取方式
        /// </summary>
        public IMG_ADJUST m_ImgAdjust = IMG_ADJUST.None;

        #endregion

        //显示窗体Visibility
        public Visibility m_DispImage { get; set; } = Visibility.Visible;

        //显示测量标定Visibility
        public Visibility m_MeasureVis { get; set; } = Visibility.Visible;

        /// <summary>
        /// 读取图像集合
        /// </summary>
        [NonSerialized]
        public List<HImageExt> M_ImageList;

        /// <summary>
        /// 中转数据
        /// </summary>
        [NonSerialized]
        public DataVar data_Var;

        /// <summary>
        /// 测量标定
        /// </summary>
        private string MeasureCalibration;
        public string m_MeasureCalibration
        {
            get { return MeasureCalibration; }
            set { MeasureCalibration = value; }
        }

        /// <summary>
        /// 停止取消SDK的线程锁
        /// </summary>
        public override void Stop()
        {
            if (m_AcqDevice != null)
            {
                m_AcqDevice.eventWait.Set();
            }
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //是否采图成功
            bool capture = false;

            try
            {
                HImage tempImg = new HImage();
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //图像队列
                    M_ImageList = new List<HImageExt>();
                    //图像数据
                    data_Var = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.图像.ToString(),
                        DataVarType.DataType.Image, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, M_ImageList);
                    ModuleProject.UpdateLocalVarValue(data_Var);

                    switch (m_CameraModel)
                    {
                        case CameraModel.指定图像:
                            tempImg = ReadSingleImage(SingleImagePath, ref capture);
                            break;
                        case CameraModel.文件目录:
                            tempImg = ReadFileImage(ref capture);
                            break;
                        case CameraModel.相机:
                            tempImg = ReadSDKImage(ref capture);
                            break;
                        default:
                            break;
                    }

                    if (!tempImg.IsInitialized() && capture == false)
                    {
                        ModuleParam.BlnSuccessed = false;
                        Log.Info(string.Format($"{"["}{ModuleProject.ProjectInfo.m_ProjectName}{"]."}{ModuleParam.ModuleName}{",运行失败"}"));
                        return;
                    }

                    //图像变换
                    tempImg = SysVisionCore.AffineImage(tempImg, m_ImgAdjust);
                    //创建图像对象
                    HImageExt imageExt = new HImageExt(tempImg);
                    //由于是反复使用一张图片,所以需要手动清除
                    imageExt.moduleROIlist.Clear();
                    //测量标定
                    MeasureData(imageExt);

                    M_ImageList.Add(imageExt);
                    ((List<HImageExt>)data_Var.m_DataValue)[0] = imageExt;//最好在此返回采集图像完毕信号
                    ModuleProject.UpdateLocalVarValue(data_Var);//写入局部变量模块

                    ModuleParam.BlnSuccessed = true;//运行成功
                    Log.Info(string.Format($"{"["}{ModuleProject.ProjectInfo.m_ProjectName}{"]."}{ModuleParam.ModuleName}{",执行成功"}"));
                }
                else
                {
                    ModuleParam.BlnSuccessed = false;//运行失败
                }
            }
            catch (Exception ex)
            {
                ModuleParam.BlnSuccessed = false;//运行失败
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{"."}{ModuleParam.ModuleName}{",执行失败,"}{ex}"));
            }
            finally
            {
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                //模块运行状态
                {
                    DataVar objVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.状态.ToString(),
                       DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, ModuleParam.BlnSuccessed);
                    ModuleProject.UpdateLocalVarValue(objVar);
                }
                //模块运行时间
                {
                    DataVar objTime = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.时间.ToString(),
                       DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ModuleParam.ModuleCostTime);
                    ModuleProject.UpdateLocalVarValue(objTime);
                }
                sw.Reset();
            }
        }

        #region 读取单独的图片

        /// <summary>
        /// 读取单独的图片
        /// </summary>
        private HImage ReadSingleImage(string fileaddress, ref bool CaptureStatus)
        {
            try
            {
                CaptureStatus = true;
                return new HImage(fileaddress);
            }
            catch (Exception)
            {
                CaptureStatus = false;
                throw;
            }
        }

        #endregion

        #region 读取文件夹图片

        /// <summary>
        /// 读取文件夹图片
        /// </summary>
        /// <returns></returns>
        private HImage ReadFileImage(ref bool CaptureStatus)
        {
            try
            {
                CaptureStatus = true;
                return Filemethod.GetFileToImage(files);
            }
            catch (Exception)
            {
                CaptureStatus = false;
                throw;
            }
        }

        #endregion

        #region 利用相机SDK采集图片

        /// <summary>
        /// 利用相机SDK采集图片
        /// </summary>
        /// <param name="CaptureStatus"></param>
        /// <returns></returns>
        private HImage ReadSDKImage(ref bool CaptureStatus)
        {
            try
            {
                //如果相机曝光不等于设定曝光
                if (m_AcqDevice.m_ExposeTime != m_ExposureTime)
                {
                    m_AcqDevice.SetExposureTime(m_ExposureTime);
                }

                //如果相机曝光不等于设定增意
                if (m_AcqDevice.m_Gain != m_Gain)
                {
                    m_AcqDevice.SetGain(m_Gain);
                }

                //清空线程锁的状态
                m_AcqDevice.eventWait.Reset();

                //发送采集命令
                if (!m_AcqDevice.CaptureImage(true))
                {
                    CaptureStatus = false;
                    Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{"."}{ModuleParam.ModuleName}{",执行失败,"}"));
                }
                else
                {
                    CaptureStatus = true;
                }

                //等待线程锁的状态
                m_AcqDevice.eventWait.WaitOne();
                return m_AcqDevice.m_Image;
            }
            catch (Exception)
            {
                CaptureStatus = false;
                throw;
            }
        }

        #endregion

        #region 测量标定数据引用

        /// <summary>
        /// 测量标定数据引用
        /// </summary>
        private void MeasureData(HImageExt imageExt, double x = 0, double y = 0, double z = 0)
        {
            double scaleX = 1.0, scaleY = 1.0;

            imageExt.ScaleX = scaleX;
            imageExt.ScaleY = scaleY;
            imageExt.X = x;
            imageExt.Y = y;
            imageExt.Z = z;

            //判断是否为空
            if (MeasureCalibration == null)
            {
                return;
            }

            //测量数据是否有值
            if (MeasureCalibration.Length == 0)
            {
                return;
            }

            //测量链接
            string[] Info = MeasureCalibration.Split('.');

            //查询流程名称
            Project pro = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName == Info[0]);

            //查询模块名称
            if (pro != null)
            {
                ModuleObjBase obj = pro.m_ModuleObjList.Find(c => c.ModuleParam.ModuleName.Contains(Info[1]));

                if (obj != null)
                {
                    imageExt.blnCalibrated = true;
                    imageExt.BoardRow = obj.ModuleImageParam.BoardRow;
                    imageExt.BoardCol = obj.ModuleImageParam.BoardCol;
                    imageExt.BoardX = obj.ModuleImageParam.BoardX;
                    imageExt.BoardY = obj.ModuleImageParam.BoardY;

                    imageExt.ScaleX = obj.ModuleImageParam.ScaleX;
                    imageExt.ScaleY = obj.ModuleImageParam.ScaleY;
                }
            }
        }

        #endregion

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            switch (m_CameraModel)
            {
                case CameraModel.指定图像:
                    break;
                case CameraModel.文件目录:
                    break;
                case CameraModel.相机:
                    m_AcqDevice = SysProcessPro.g_CameraList.FirstOrDefault(c => c.m_DeviceID == _DeviceID);
                    if (m_AcqDevice == null)
                    {
                        System.Windows.Forms.MessageBox.Show(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}" +
                            $"{ModuleParam.ModuleName}{",采集设备不存在，请重新绑定"}"));
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

    }

    public enum CameraModel
    {
        指定图像,
        文件目录,
        相机,
    }

}
