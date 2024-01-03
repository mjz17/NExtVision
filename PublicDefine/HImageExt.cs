using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using Common;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static System.Net.Mime.MediaTypeNames;

namespace PublicDefine
{

    [Serializable]
    public class HImageExt : HImage, ISerializable
    {
        /// <summary>
        /// 采集当前图像的时候位置X
        /// </summary>
        public double X { get; set; } = 0;

        /// <summary>
        /// 采集当前图像的时候位置Y
        /// </summary>
        public double Y { get; set; } = 0;

        /// <summary>
        /// 采集当前图像的时候位置Z
        /// </summary>
        public double Z { get; set; } = 0;

        /// <summary>
        /// X轴和直角坐标系X轴夹角
        /// </summary>
        public double PhiX { get; set; }

        /// <summary>
        /// Y轴和直角坐标系Y轴夹角
        /// </summary>
        public double PhiY { get; set; }

        /// <summary>
        /// X方向像素比例
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// Y方向像素比例
        /// </summary>
        public double ScaleY { get; set; }


        #region 区域标定映射

        /// <summary>
        /// 标定板行坐标
        /// </summary>
        public HTuple BoardRow { get; set; }

        /// <summary>
        /// 标定板列坐标
        /// </summary>
        public HTuple BoardCol { get; set; }

        /// <summary>
        /// 标定板X坐标
        /// </summary>
        public HTuple BoardX { get; set; }

        /// <summary>
        /// 标定板Y坐标
        /// </summary>
        public HTuple BoardY { get; set; }

        /// <summary>
        /// 是否标定过
        /// </summary>
        public bool blnCalibrated { get; set; }

        #endregion

        #region 当前图像原点，用户指定

        public double CoorX { get; set; }
        public double CoorY { get; set; }
        public double CoorPhi { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数1
        /// </summary>
        public HImageExt() : base()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="fileName"></param>
        public HImageExt(string fileName) : base(fileName)
        {

        }

        /// <summary>
        /// 构造函数3
        /// </summary>
        /// <param name="key"></param>
        /// <param name="copy"></param>
        public HImageExt(IntPtr key, bool copy) : base(key, copy)
        {

        }

        /// <summary>
        /// 构造函数4
        /// </summary>
        /// <param name="obj"></param>
        public HImageExt(HObject obj) : base(obj)
        {

        }

        /// <summary>
        /// 构造函数5
        /// </summary>
        /// <param name="type"></param>
        /// <param name="width"></param>
        /// <param name="heigth"></param>
        /// <param name="pixe"></param>
        public HImageExt(string type, int width, int heigth, IntPtr pixe) : base(type, width, heigth, pixe)
        {

        }

        /// <summary>
        /// 构造函数6
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public HImageExt(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new System.ArgumentException("info");
            }

            this.X = info.GetDouble("X");
            this.Y = info.GetDouble("X");
            this.Z = info.GetDouble("Z");
            this.PhiX = info.GetDouble("PhiX");
            this.PhiY = info.GetDouble("PhiY");
            this.ScaleX = info.GetDouble("ScaleX");
            this.ScaleY = info.GetDouble("ScaleY");

            try
            {
                this.moduleROIlist = (List<ModuleROI>)info.GetValue("moduleROIlist", typeof(List<ModuleROI>));
                this.moduleTxtlist = (List<ModuleROIText>)info.GetValue("moduleTxtlist", typeof(List<ModuleROIText>));

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentException("info");
            }
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
            info.AddValue("PhiX", PhiX);
            info.AddValue("PhiY", PhiY);
            info.AddValue("ScaleX", ScaleX);
            info.AddValue("ScaleY", ScaleY);
            info.AddValue("moduleROIlist", moduleROIlist);
            info.AddValue("moduleTxtlist", moduleTxtlist);

            HSerializedItem item = this.SerializeImage();

            byte[] bufer = (byte[])item;
            item.Dispose();
            info.AddValue("data", bufer, typeof(byte[]));
        }

        #endregion

        #region 静态函数

        public static HImageExt Instance(HObject img)
        {
            return new HImageExt(img);
        }

        /// <summary>
        /// 将引用图像的扩展信息复制到新的图像
        /// </summary>
        /// <param name="refImg"></param>
        public void SetExtData(HImageExt refImg)
        {
            this.X = refImg.X;
            this.Y = refImg.Y;
            this.Z = refImg.Z;
            this.PhiX = refImg.PhiX;
            this.PhiY = refImg.PhiY;
            this.ScaleX = refImg.ScaleX;
            this.ScaleY = refImg.ScaleY;
            this.BoardRow = refImg.BoardRow;
            this.BoardCol = refImg.BoardCol;
            this.BoardX = refImg.BoardX;
            this.BoardY = refImg.BoardY;
            this.blnCalibrated = refImg.blnCalibrated;
            this.CoorX = refImg.CoorX;
            this.CoorY = refImg.CoorY;
            this.CoorPhi = refImg.CoorPhi;

            try
            {
                this.moduleROIlist = refImg.moduleROIlist.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 显示的ROI
        /// </summary>
        public List<ModuleROI> moduleROIlist = new List<ModuleROI>();

        /// <summary>
        /// 显示的Txt
        /// </summary>
        public List<ModuleROIText> moduleTxtlist = new List<ModuleROIText>();

        /// <summary>
        /// 从he文件中获取ImageExt对象
        /// </summary>
        /// <param name="fileName">文件所在的路径</param>
        /// <returns></returns>
        public static HImageExt ReadImageExt(string fileName)
        {
            HImageExt ImgExt = null;
            try
            {
                string ext = System.IO.Path.GetExtension(fileName.ToLower());
                if (ext == ".he")
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    fs.Seek(0, SeekOrigin.Begin);
                    BinaryFormatter binaryFmt = new BinaryFormatter();
                    ImgExt = (HImageExt)binaryFmt.Deserialize(fs);
                }
                else
                {
                    HImageExt temp = new HImageExt(fileName);
                    ImgExt = HImageExt.Instance(temp);
                }
                return ImgExt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return ImgExt;
            }
        }

        object obj = new object();

        /// <summary>
        /// 保存保存HimageExt到本地
        /// </summary>
        /// <param name="fileName"></param>
        public void WriteImageExt(string fileName)
        {
            lock (obj)
            {
                string ext = Path.GetExtension(fileName);
                if (ext == ".he")
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    BinaryFormatter binaryFmt = new BinaryFormatter();
                    fs.Seek(0, SeekOrigin.Begin);
                    binaryFmt.Serialize(fs, this);
                }
                else if (ext == "")
                {
                    string type = this.GetImageType().ToString();

                    if (type.Contains("real"))
                    {
                        this.WriteImage("tiff", 0, fileName);
                    }
                    else
                    {
                        this.WriteImage("png best", 0, fileName);
                    }
                }
                else
                {
                    this.WriteImage(ext.Substring(1), 0, fileName);
                }
            }
        }

        /// <summary>
        /// 保存保存HimageExt到本地
        /// </summary>
        /// <param name="SavePath">路径</param>
        /// <param name="TextName">文件夹名称</param>
        /// <param name="Imageformat">图像格式</param>
        public void WriteImageExt(string SavePath, string TextName, string Imageformat)
        {
            lock (obj)
            {
                //保存路径(路径+TextName+当前日期创建的文件夹)
                string DataDay = DateTime.Now.ToString("yyyy-MM-dd");
                string path = SavePath + "\\" + TextName + "\\" + DataDay + "\\";

                //年月日小时分钟秒
                string Datedate = DateTime.Now.ToString("HH_mm_ss_fff");
                //yyyy-MM-dd HH:mm:ss:fff:ffffff
                string strPaht = path + Datedate + "." + Imageformat;

                try
                {
                    //判断是否有文件夹
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string ext = Path.GetExtension(strPaht);

                    this.WriteImage(ext.Substring(1), 0, strPaht);

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 保存窗体截图到指定的文件夹
        /// </summary>
        /// <param name="SavePath">路径</param>
        /// <param name="TextName">文件夹名称</param>
        /// <param name="Imageformat">图像格式</param>
        /// <param name="hWindow">窗体句柄</param>
        public void WriteImageExt(string SavePath, string TextName, string Imageformat, HWindow hWindow)
        {
            lock (obj)
            {
                //保存路径(路径+TextName+当前日期创建的文件夹)
                string DataDay = DateTime.Now.ToString("yyyy-MM-dd");
                string path = SavePath + "\\" + TextName + "\\" + DataDay + "\\";

                //年月日小时分钟秒
                string Datedate = DateTime.Now.ToString("HH_mm_ss_fff");

                string strPaht = path + Datedate;

                try
                {
                    //判断是否有文件夹
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    hWindow.DumpWindow(Imageformat, strPaht);

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <returns></returns>
        public HHomMat2D getHomImg()
        {
            HHomMat2D hom = new HHomMat2D();
            hom = hom.HomMat2dScaleLocal(ScaleX, ScaleY);
            return hom;
        }

        /// <summary>
        /// 获取校正相机夹角和校正轴矩阵
        /// </summary>
        /// <returns></returns>
        public HHomMat2D getHomAxis()
        {
            HHomMat2D homA = new HHomMat2D();
            homA = homA.HomMat2dRotateLocal(PhiY);
            return homA;
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <returns></returns>
        public new HImageExt Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                moduleROIlist = moduleROIlist.Where(c => c != null && (c.hObject == null || c.hObject.IsInitialized())).ToList();
                moduleTxtlist = moduleTxtlist.Where(c => c != null).ToList();

                formatter.Serialize(stream, this);
                stream.Position = 0;
                return formatter.Deserialize(stream) as HImageExt;
            }
        }

        public void UpdateRoiList(ModuleROI ROI)
        {
            try
            {
                int index = moduleROIlist.FindIndex(e => e.ModuleID == ROI.ModuleID && e.RoiType == ROI.RoiType);
                if (index > -1)
                    moduleROIlist[index] = ROI;
                else
                    moduleROIlist.Add(ROI);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void UpdateTxtList(ModuleROIText Txt)
        {
            try
            {
                //模块ID,模块轮廓类型，模块描述
                int index = moduleTxtlist.FindIndex(e => e.ModuleID == Txt.ModuleID && e.RoiType == Txt.RoiType && e.ModuleDescribe == Txt.ModuleDescribe);
                if (index > -1)
                    moduleTxtlist[index] = Txt;
                else
                    moduleTxtlist.Add(Txt);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            foreach (ModuleROI ROI in moduleROIlist)
            {
                if (ROI.hObject == null || !ROI.hObject.IsInitialized())//修复为null 错误 magical 20171103
                {
                    ROI.hObject = null;
                }
            }
            //foreach (ModuleROIText ROI in moduleTxtlist)
            //{
            //    if (ROI.hObject == null || !ROI.hObject.IsInitialized())//修复为null 错误 magical 20171103
            //    {
            //        ROI.hObject = null;
            //    }
            //}
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            //如果he为老版本没有x y比例 手动设置为1,否则离线读取数据计算时候会出现异常   yoga 20180824
            if (ScaleX == 0)
            {
                ScaleX = 1;
            }
            if (ScaleY == 0)
            {
                ScaleY = 1;
            }
        }
    }
}
