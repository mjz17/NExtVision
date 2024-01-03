using Common;
using DefineImgRoI;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DistanceLL
{
    [Category("几何测量")]
    [DisplayName("线线距离")]
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

        /// <summary>
        /// 测量模式
        /// </summary>
        public MeasureLL m_MeasureLL { get; set; }

        /// <summary>
        /// 是否使用转换
        /// </summary>
        public bool ConverValue = false;

        [NonSerialized]
        public double m_minRow = 0.0;

        [NonSerialized]
        public double m_minCol = 0.0;

        [NonSerialized]
        public double m_maxRow = 0.0;

        [NonSerialized]
        public double m_maxCol = 0.0;

        [NonSerialized]
        public int isParallel = 0;//平行1，不平行0

        //执行模块
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



                            //VBA_Function.IntersectionLl(Word_line1, Word_line2, out m_Row, out m_Col, out isParallel);
                            //double phi = Math.Round(180 / Math.PI * VBA_Function.CalAngleL2L(Word_line1, Word_line2), 3);

                            ////显示交点位置
                            //m_PointCross.GenCrossContourXld(m_Col, m_Row, 30, new HTuple(Math.Abs(phi)).TupleRad());
                            //ModuleROI roi检测点 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                            // ModuleParam.ModuleDesCribe, ModuleRoiType.检测点.ToString(), "cyan", new HObject(m_PointCross), m_IsDispOutPoint);
                            //m_Image.UpdateRoiList(roi检测点);


                            ////组合一个集合
                            //List<double> CircleInfo = new List<double>();
                            //CircleInfo.Add(m_Col);
                            //CircleInfo.Add(m_Row);
                            //DataVar data_CircleInfo = new DataVar(ModuleParam.ModuleID, DataVarType.DataGroup.单量, DataVarType.DataType.坐标系, 1,
                            //    DataVarType.DataAtrribution.全局变量, ConVar.outCircle, ModuleParam.ModuleName, "0", CircleInfo);
                            //ModuleProject.UpdateVariableValue(data_CircleInfo);

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

    public enum MeasureLL
    {
        最小距离,
        最大距离,
        平均距离,
    }
}
