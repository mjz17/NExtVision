using Common;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.ImagePretreat
{

    [Category("图像处理")]
    [DisplayName("预先处理")]
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

        /// <summary>
        /// 图像增强列表
        /// </summary>
        public List<BaseMethod> m_Emhan = new List<BaseMethod>();

        /// <summary>
        /// 输出图像
        /// </summary>
        [NonSerialized]
        public HImageExt m_OutImage;

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
                            m_OutImage = new HImageExt(m_Image);

                            foreach (BaseMethod item in m_Emhan)
                            {
                                //如果启用
                                if (item.m_EnableOrnot)
                                {
                                    item.m_ImageExt = m_OutImage;
                                    item.WritepRram();
                                    m_OutImage = new HImageExt(item.m_OutObj);
                                }
                            }

                            //图像
                            {
                                List<HImageExt> M_ImageList = new List<HImageExt>()
                                {
                                    new HImageExt(m_OutImage),
                                };

                                DataVar data_Img = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.图像预处理.ToString(),
                                 DataVarType.DataType.Image, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, M_ImageList);
                                ModuleProject.UpdateLocalVarValue(data_Img);
                            }

                            ModuleParam.BlnSuccessed = true;
                        }
                        else
                        {
                            //运行失败
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
                sw.Reset();
            }
        }

    }

    /// <summary>
    /// 图像增强
    /// </summary>
    public enum EnhanType
    {
        None = 0,
        二值化,
        均值滤波,
        中值滤波,
        高斯滤波,
        锐化,
        对比度,
        灰度开运算,
        灰度闭运算,
        反色,
        灰度膨胀,
        灰度腐蚀,
    }


}
