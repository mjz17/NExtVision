using Common;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionCore;

namespace Plugin.BlobAnalysis
{
    [Category("检测识别")]
    [DisplayName("斑点分析")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        #region 读取图像

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Image_Data;

        #endregion

        //模式
        public WorkStation m_WorkStation { get; set; }

        //Roi模式
        public ROIInfo m_ROIInfo { get; set; }

        #region 链接ROI

        /// <summary>
        /// ROI名称
        /// </summary>
        public string m_CurentRoiName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Roi_Data;

        #endregion

        #region 链接Index

        /// <summary>
        /// Index名称
        /// </summary>
        public string m_CurentIndexName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Index_Data;

        #endregion

        #region 二值化

        //二值化类型
        public ThresholdModel m_ThresholdModel;

        //普通二值化阈值min
        public double MinValue = 50;
        //普通二值化阈值max
        public double MaxValue = 150;

        //自动二值化
        public BinaryThread m_BinThread;

        #endregion

        /// <summary>
        /// Blob方法集合
        /// </summary>
        public List<BaseMethod> m_Blob = new List<BaseMethod>();

        /// <summary>
        /// 是否特征排序
        /// </summary>
        public bool IsSort = true;

        /// <summary>
        /// 升序列/降序
        /// </summary>
        public bool Order = false;

        [NonSerialized]
        Region_features _Features = new Region_features();

        //结果判断
        public List<SelectedShapeInfo> m_shapeInfo = new List<SelectedShapeInfo>();

        //判断方式
        public shapeModel m_shapeModel;

        /// <summary>
        /// 输出的参数
        /// </summary>
        [NonSerialized]
        public List<Region_Info> m_OutRegionInfo;

        /// <summary>
        /// 输出Himage
        /// </summary>
        [NonSerialized]
        public HImage m_InHimage;

        /// <summary>
        /// 输入的Region区域
        /// </summary>
        [NonSerialized]
        public HRegion m_InputRegion;

        /// <summary>
        /// 输出Region
        /// </summary>
        [NonSerialized]
        public HRegion m_OutRegion;

        /// <summary>
        /// 输出Region
        /// </summary>
        [NonSerialized]
        public HRegion m_DispRegion;

        /// <summary>
        /// Blob分析筛选
        /// </summary>
        [NonSerialized]
        public bool Out_Result = false;

        /// <summary>
        /// 执行流程
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //加载链接图像
                    DataVar data = ModuleProject.GetLocalVarValue(Link_Image_Data);

                    if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                    {
                        m_Image = ((List<HImageExt>)(data).m_DataValue)[0];

                        if (m_Image != null || m_Image.IsInitialized())
                        {
                            //图像模式&区域模式
                            switch (m_WorkStation)
                            {
                                case WorkStation.图像模式:
                                    m_InHimage = m_Image;

                                    m_InHimage.GetImageSize(out HTuple width, out HTuple height);
                                    m_InputRegion = new HRegion(0, 0, height, width);

                                    //输入的Region
                                    DataVar InImage_Region = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.阈值区域.ToString(),
                                   DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_InputRegion);
                                    ModuleProject.UpdateLocalVarValue(InImage_Region);

                                    break;
                                case WorkStation.区域模式:
                                    // 加载链接ROI区域
                                    DataVar roi_data = ModuleProject.GetLocalVarValue(Link_Roi_Data);
                                    //链接索引
                                    DataVar Index_data = ModuleProject.GetLocalVarValue(Link_Index_Data);
                                    int index = (int)Index_data.m_DataValue + 2;
                                    m_InputRegion = ((HRegion)roi_data.m_DataValue).SelectObj(index);

                                    Log.Info(index.ToString());

                                    VBA_Function.RegionRedumain(m_Image, m_InputRegion, out m_InHimage);

                                    //输入的Region
                                    DataVar InPut_Region = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.阈值区域.ToString(),
                                   DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_InputRegion);
                                    ModuleProject.UpdateLocalVarValue(InPut_Region);

                                    break;
                                default:
                                    break;
                            }

                            //对图像进行二值化
                            switch (m_ThresholdModel)
                            {
                                case ThresholdModel.二值化:
                                    VBA_Function.ThresholdImage(m_InHimage, MinValue, MaxValue, out m_OutRegion);
                                    break;
                                case ThresholdModel.自动二值化:
                                    VBA_Function.BinaryThresholdImage(m_InHimage, m_BinThread.ToString(), out m_OutRegion, out int number);
                                    break;
                                case ThresholdModel.均值二值化:
                                    //VBA_Function.ThresholdImage(m_InHimage, MinValue, MaxValue, out m_OutRegion);
                                    break;
                                default:
                                    break;
                            }

                            //执行区域分析
                            if (m_OutRegion != null)
                            {
                                foreach (BaseMethod item in m_Blob)
                                {
                                    //如果启用
                                    if (item.m_EnableOrnot)
                                    {
                                        //是否是上一个区域
                                        if (item.m_LinkIndex == "上一个区域")
                                        {
                                            item.m_InHRegion = new HRegion(m_OutRegion);
                                            item.SetRram();
                                            m_OutRegion = new HRegion(item.m_OutObj);
                                        }
                                        else
                                        {
                                            //查询链接的区域
                                            BaseMethod method = m_Blob.Find(c => c.m_Index == Convert.ToInt16(item.m_LinkIndex));
                                            if (method != null)
                                            {
                                                item.m_InHRegion = new HRegion(method.m_OutObj);
                                                item.SetRram();
                                                m_OutRegion = new HRegion(item.m_OutObj);
                                            }
                                        }
                                    }
                                }
                            }

                            //是否需要排序
                            if (IsSort)
                            {
                                m_OutRegion = _Features.SortRegion(m_OutRegion, SortModel.面积, Order);
                            }

                            //获得区域分析的参数
                            m_OutRegionInfo = _Features.Gne_Feature(m_OutRegion);

                            //特征筛选
                            foreach (SelectedShapeInfo item in m_shapeInfo)
                            {
                                if (item.m_EnableOrnot)
                                {
                                    switch (item.m_shapeType)
                                    {
                                        case SelectedshapeType.个数:
                                            item.m_result = false;
                                            HTuple num = m_OutRegion.RegionFeatures(new HTuple("connect_num")).TupleSum();
                                            if (num > item.m_min && num < item.m_max)
                                            {
                                                item.m_result = true;
                                            }
                                            else
                                            {
                                                item.m_result = false;
                                            }

                                            break;
                                        case SelectedshapeType.总面积:

                                            HTuple allNum = m_OutRegion.RegionFeatures(new HTuple("area")).TupleSum();
                                            item.m_result = false;
                                            if (allNum > item.m_min && allNum < item.m_max)
                                            {
                                                item.m_result = true;
                                            }
                                            else
                                            {
                                                item.m_result = false;
                                            }

                                            break;
                                        case SelectedshapeType.各个面积:
                                            item.m_result = false;
                                            HTuple singleNum = m_OutRegion.RegionFeatures(new HTuple("area"));
                                            for (int i = 0; i < singleNum.Length; i++)
                                            {
                                                if (singleNum[i] > item.m_min && singleNum[i] < item.m_max)
                                                {
                                                    item.m_result = true;
                                                }
                                                else
                                                {
                                                    item.m_result = false;
                                                    break;
                                                }
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            //判断输出
                            for (int i = 0; i < m_shapeInfo.Count; i++)
                            {
                                if (m_shapeModel == shapeModel.and)
                                {
                                    if (m_shapeInfo.Count == 1)
                                    {
                                        Out_Result = m_shapeInfo[i].m_result;
                                    }

                                    if (i < m_shapeInfo.Count - 1)
                                    {
                                        Out_Result = m_shapeInfo[i].m_result && m_shapeInfo[i].m_result;
                                    }

                                    if (Out_Result == false)
                                        break;
                                }
                                else
                                {
                                    if (i < m_shapeInfo.Count - 1)
                                    {
                                        Out_Result = m_shapeInfo[i].m_result || m_shapeInfo[i].m_result;
                                    }
                                    if (Out_Result == false)
                                        break;
                                }
                            }

                            //输出面积
                            List<double> Area = new List<double>();
                            List<double> Row = new List<double>();
                            List<double> Col = new List<double>();

                            foreach (var item in m_OutRegionInfo)
                            {
                                Area.Add(item.m_Area);
                                Row.Add(item.m_Row);
                                Col.Add(item.m_Column);
                            }
                            DataVar Out_Area = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.面积.ToString(),
                         DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, Area);
                            ModuleProject.UpdateLocalVarValue(Out_Area);

                            DataVar Out_Row = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Area_Row.ToString(),
                         DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, Row);
                            ModuleProject.UpdateLocalVarValue(Out_Row);

                            DataVar Out_Col = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Area_Col.ToString(),
                         DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, Col);
                            ModuleProject.UpdateLocalVarValue(Out_Col);

                            DataVar selectStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.筛选结果.ToString(),
                          DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, Out_Result);
                            ModuleProject.UpdateLocalVarValue(selectStatus);

                            ModuleParam.BlnSuccessed = true;

                        }
                        else
                        {
                            ModuleParam.BlnSuccessed = false;
                        }
                    }
                    else
                    {
                        ModuleParam.BlnSuccessed = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //运行失败
                ModuleParam.BlnSuccessed = false;
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
            }
            finally
            {
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                //模块运行状态
                {
                    DataVar objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.状态.ToString(),
                       DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, ModuleParam.BlnSuccessed);
                    ModuleProject.UpdateLocalVarValue(objStatus);
                }
                //模块运行状态
                {
                    DataVar objTime = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.时间.ToString(),
                       DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ModuleParam.ModuleCostTime);
                    ModuleProject.UpdateLocalVarValue(objTime);
                }
                //输出Region
                DataVar Out_Region = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.区域.ToString(),
               DataVarType.DataType.区域, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, m_OutRegion);
                ModuleProject.UpdateLocalVarValue(Out_Region);
                sw.Reset();
            }
        }

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            _Features = new Region_features();
        }

    }

    [Serializable]
    public enum BinaryThread
    {
        dark,
        light,
    }

    [Serializable]
    public enum WorkStation
    {
        图像模式,
        区域模式,
    }

    [Serializable]
    public enum ROIInfo
    {
        ROI绘制,
        ROI链接,
    }

    [Serializable]
    public enum ThresholdModel
    {
        二值化,
        均值二值化,
        自动二值化,
    }

    [Serializable]
    public enum BlobMethod
    {

        连通,
        合并,
        补集,
        相减,
        相交,
        空洞填充,

        闭运算,
        开运算,
        腐蚀,
        膨胀,

        特征筛选,
        转换,
        矩形分割,
        动态分割,
    }

}
