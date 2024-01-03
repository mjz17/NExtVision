using Common;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.ImageDisplay
{
    [Category("图像处理")]
    [DisplayName("显示图像")]
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
        /// 窗体名称
        /// </summary>
        public string HwindowName = string.Empty;

        /// <summary>
        /// 链接的数据
        /// </summary>
        public List<DispInfo> LinkInfo = new List<DispInfo>();

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

                        //读取链接的区域
                        foreach (DispInfo item in LinkInfo)
                        {
                            //查询
                            DataVar data1 = ModuleProject.GetLocalVarValue(item.m_DataVar);
                            if (data1.m_DataType == DataVarType.DataType.区域)
                            {
                                if (data1.m_DataValue is HRegion)
                                {
                                    //放入ROI队列
                                    ModuleROI roi = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                                         ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red",
                                         new HObject((HRegion)data1.m_DataValue), true);
                                    m_Image.UpdateRoiList(roi);
                                }
                            }
                            else if (data1.m_DataType == DataVarType.DataType.轮廓)
                            {
                                if (data1.m_DataValue is HXLDCont)
                                {
                                    //放入ROI队列
                                    ModuleROI roi = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                                         ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red",
                                         new HObject((HXLDCont)data1.m_DataValue), true);
                                    m_Image.UpdateRoiList(roi);
                                }
                            }
                        }

                        //保存显示图像窗体
                        m_ModuleProject.ProjectInfo.m_DispHwindowName = HwindowName;
                        //获得需要更新的窗体
                        int index1 = SysLayout.Frm_Info.FindIndex(c => c.Name == HwindowName);
                        if (index1 > -1)
                        {
                            m_ModuleProject.m_Viewhwindow.hWindow = SysLayout.Frm_Info[index1].HWindow;//设置显示窗体
                        }
                        //更新窗体显示
                        if (m_Image != null || m_Image.IsInitialized())
                        {
                            m_ModuleProject.m_Viewhwindow.m_HImageExt = m_Image;
                            m_ModuleProject.m_Viewhwindow.m_RepaintHwindow();
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
            catch (Exception ex)
            {
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
    ///链接的数据
    /// </summary>
    [Serializable]
    public class DispInfo
    {
        public string m_linkInfo { get; set; } = "#####";

        public DataVar m_DataVar;
    }

}
