using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.LogicCycleStart
{
    [Category("逻辑工具")]
    [DisplayName("循环开始")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 循环类型
        /// </summary>
        public LogicCycle m_logicCycle;

        #region Start

        /// <summary>
        /// Start
        /// </summary>
        public string m_InputStart = string.Empty;

        /// <summary>
        /// Start
        /// </summary>
        public DataVar Link_InputStart;

        /// <summary>
        /// Start
        /// </summary>
        [NonSerialized]
        public int InputStart_Data = 0;

        #endregion

        #region End

        /// <summary>
        /// End
        /// </summary>
        public string m_InputEnd = string.Empty;

        /// <summary>
        /// End
        /// </summary>
        public DataVar Link_InputEnd;

        /// <summary>
        /// End
        /// </summary>
        [NonSerialized]
        public int InputEnd_Data = 0;

        #endregion

        #region Array

        /// <summary>
        /// Array
        /// </summary>
        public string m_InputArray = string.Empty;

        /// <summary>
        /// Array
        /// </summary>
        public DataVar Link_InputArray;

        /// <summary>
        /// Array
        /// </summary>
        [NonSerialized]
        public int InputArray_Data = 0;

        #endregion

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
                    switch (m_logicCycle)
                    {
                        case LogicCycle.从Start到End:
                            //查询Start参数
                            if (m_InputStart != null)
                            {
                                int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputStart.m_DataName &&
                                c.m_DataModuleID == Link_InputStart.m_DataModuleID);
                                if (Info1 > -1)
                                {
                                    //加载Start参数
                                    Link_InputStart = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                    InputStart_Data = (int)Link_InputStart.m_DataValue;
                                }
                            }

                            //查询End参数
                            if (m_InputEnd != null)
                            {
                                int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputEnd.m_DataName &&
                                c.m_DataModuleID == Link_InputEnd.m_DataModuleID);
                                if (Info1 > -1)
                                {
                                    //加载End参数
                                    Link_InputEnd = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                    InputEnd_Data = (int)Link_InputEnd.m_DataValue;
                                }
                            }

                            //循环次数
                            ModuleParam.CyclicCount = InputEnd_Data - InputStart_Data;

                            break;
                        case LogicCycle.从End到Start:
                            break;
                        case LogicCycle.无限循环:
                            break;
                        case LogicCycle.遍历数组:
                            //查询Start参数
                            if (m_InputStart != null)
                            {
                                int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputArray.m_DataName &&
                                c.m_DataModuleID == Link_InputArray.m_DataModuleID);
                                if (Info1 > -1)
                                {
                                    //加载Start参数
                                    Link_InputArray = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                    if (Link_InputArray.m_DataValue is List<double>)
                                    {
                                        InputArray_Data = ((List<double>)Link_InputArray.m_DataValue).Count;
                                        //循环次数
                                        ModuleParam.CyclicCount = InputArray_Data;
                                    }
                                }
                            } 
                            break;
                        default:
                            break;
                    }

                    //当前循环次数
                    {
                        DataVar Index = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.索引.ToString(),
                    DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ModuleParam.pIndex);
                        ModuleProject.UpdateLocalVarValue(Index);
                    }

                    ModuleParam.BlnSuccessed = true;
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

    public enum LogicCycle
    {
        从Start到End,
        从End到Start,
        无限循环,
        遍历数组,
    }

}
