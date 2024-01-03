using DefineImgRoI;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using HalconDotNet;
using PublicDefine;
using Common;
using System.Runtime.Serialization;

namespace Plugin.IntersectionPoint
{
    [Category("几何测量")]
    [DisplayName("线线交点")]
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

        #region 输入直线1

        /// <summary>
        /// 输入点1名称
        /// </summary>
        public string m_Line1 = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_m_Line1_Data;

        /// <summary>
        /// 直线1数据
        /// </summary>
        public Line_INFO _Line1;

        #endregion

        #region 输入直线2

        /// <summary>
        /// 输入直线2名称
        /// </summary>
        public string m_Line2 = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_m_Line2_Data;

        /// <summary>
        /// 直线2数据
        /// </summary>
        public Line_INFO _Line2;

        #endregion

        [NonSerialized]
        public double m_Row = 0.0;

        [NonSerialized]
        public double m_Col = 0.0;

        [NonSerialized]
        public double m_phi = 0.0;

        [NonSerialized]
        public int isParallel = 0;//平行1，不平行0

        [NonSerialized]
        public HXLDCont m_PointCross = new HXLDCont();

        //执行模块
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //初始化
            m_Row = 0.0;
            m_Col = 0.0;
            m_phi = 0.0;
            isParallel = 0;

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
                            //加载直线1信息
                            {
                                if (m_Line1 != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_m_Line1_Data.m_DataName &&
                                    c.m_DataModuleID == Link_m_Line1_Data.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        _Line1 = (Line_INFO)SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1].m_DataValue;
                                    }
                                }
                            }

                            //加载直线2信息
                            {
                                if (m_Line2 != null)
                                {
                                    int Info2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_m_Line2_Data.m_DataName &&
                                    c.m_DataModuleID == Link_m_Line2_Data.m_DataModuleID);
                                    if (Info2 > -1)
                                    {
                                        _Line2 = (Line_INFO)SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info2].m_DataValue;
                                    }
                                }
                            }

                            //线线交点
                            VBA_Function.IntersectionLl(_Line1, _Line2, out m_Row, out m_Col, out isParallel);

                            //线线角度
                            m_phi = HMisc.AngleLl(_Line1.StartX, _Line1.StartY, _Line1.EndX, _Line1.EndY,
                                _Line2.StartX, _Line2.StartY, _Line2.EndX, _Line2.EndY);

                            //显示交点位置
                            m_PointCross.GenCrossContourXld(m_Row, m_Col, 30, new HTuple(Math.Abs(m_phi)).TupleRad());
                            ModuleROI roi检测点 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                             ModuleParam.ModuleDesCribe, ModuleRoiType.检测点.ToString(), "cyan", new HObject(m_PointCross), m_IsDispOutPoint);
                            m_Image.UpdateRoiList(roi检测点);

                            //模块对应数据(实际的图像坐标)
                            {
                                //坐标X
                                DataVar data_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_Row);
                                ModuleProject.UpdateLocalVarValue(data_x);

                                //坐标Y
                                DataVar data_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_Col);
                                ModuleProject.UpdateLocalVarValue(data_y);
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
                //模块对应数据(实际的图像坐标)
                {
                    //坐标X
                    DataVar data_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_Row);
                    ModuleProject.UpdateLocalVarValue(data_x);

                    //坐标Y
                    DataVar data_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_Col);
                    ModuleProject.UpdateLocalVarValue(data_y);
                }

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


        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            m_PointCross = new HXLDCont();
        }

    }
}
