using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.MeasureCalibration
{
    [Category("坐标变换")]
    [DisplayName("测量标定")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Image_Data;

        /// <summary>
        /// 是否标定
        /// </summary>
        public bool Calibration { get; set; }

        /// <summary>
        /// 标定类型
        /// </summary>
        public CalibrationModel m_Calibration;

        /// <summary>
        /// 搜索区域
        /// </summary>
        public ROI m_SearchRegion;

        /// <summary>
        /// 兴趣区域 
        /// </summary>
        public HRegion DetectRegion = new HRegion();

        /// <summary>
        /// 屏蔽区域 
        /// </summary>
        public HRegion DisableRegion = new HRegion();

        /// <summary>
        /// 物理间距
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// 阈值
        /// </summary>
        public int ThresholdValue { get; set; }

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 左上原点在标定板上的距离
        /// </summary>
        public double BigWorldOriginX { get; set; }
        public double BigWorldOriginY { get; set; }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                if (Calibration)
                {
                    blnNewModule = true;
                    //运行成功
                    ModuleParam.BlnSuccessed = true;
                }
                else
                {
                    blnNewModule = false;
                    ModuleParam.BlnSuccessed = false;
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
                sw.Reset();
            }
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            if (DisableRegion == null || !DisableRegion.IsInitialized())//修复为null 错误 magical 20171103
            {
                DisableRegion = null;
            }
            if (DetectRegion == null || !DetectRegion.IsInitialized())
            {
                DetectRegion = null;
            }
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (DetectRegion == null)
            {
                DetectRegion = new HRegion();
            }
            if (DisableRegion == null)
            {
                DisableRegion = new HRegion();
            }
        }
    }

    public enum CalibrationModel
    {
        孔板模式,
        孔板效正图像模式,
        多相机孔板模式,
    }

}
