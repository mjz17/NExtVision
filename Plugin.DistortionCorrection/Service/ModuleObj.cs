using Common;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using StyleBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionCore;

namespace Plugin.DistortionCorrection
{
    [Category("图像处理")]
    [DisplayName("畸变校正")]
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

        [NonSerialized]
        public HImage hv_Image;

        /// <summary>
        /// 精度模式
        /// </summary>
        private PrecisionModel _precision;

        public PrecisionModel m_Precision
        {
            get { return _precision; }
            set { _precision = value; }
        }

        /// <summary>
        /// 相机内参
        /// </summary>
        public HCamPar camPar;

        /// <summary>
        /// 相机外参
        /// </summary>
        public HCamPar out_camPar;

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
                            int DeepLength = 0;
                            switch (m_Precision)
                            {
                                case PrecisionModel.低精度:
                                    DeepLength = 1;
                                    break;
                                case PrecisionModel.高精度:
                                    DeepLength = 5;
                                    break;
                                case PrecisionModel.超高精度:
                                    DeepLength = 10;
                                    break;
                                default:
                                    break;
                            }

                            if (camPar != null && out_camPar != null)
                            {
                                SysVisionCore.DistortioCorrection(m_Image, camPar, out_camPar, out hv_Image);
                            }
                            else
                            {
                                SysVisionCore.CreateDistortioCorrection(m_Image, DeepLength, out hv_Image, out camPar, out out_camPar);
                            }

                            //图像
                            {
                                List<HImageExt> M_ImageList = new List<HImageExt>()
                                {
                                    new HImageExt(hv_Image),
                                };

                                DataVar data_Img = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.图像效正.ToString(),
                                 DataVarType.DataType.Image, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, M_ImageList);
                                ModuleProject.UpdateLocalVarValue(data_Img);
                            }
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
                else
                {
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

    }

    public enum PrecisionModel
    {
        低精度,
        高精度,
        超高精度,
    }
}
