using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.LogicIF
{
    [Category("逻辑工具")]
    [DisplayName("如果")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        #region 链接数据1

        /// <summary>
        /// 链接数据1
        /// </summary>
        public string m_Link_Data1 = string.Empty;

        /// <summary>
        /// 链接数据信息1
        /// </summary>
        public DataVar Link_ChooseModel1;

        #endregion

        #region 链接数据2

        /// <summary>
        /// 链接数据2
        /// </summary>
        public string m_Link_Data2 = string.Empty;

        /// <summary>
        /// 链接数据信息2
        /// </summary>
        public DataVar Link_ChooseModel2;

        #endregion

        /// <summary>
        /// 判断方式
        /// </summary>
        public LogicInfo m_LogicInfo;

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();//模块运行时间
            sw.Start();

            try
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    switch (m_LogicInfo)
                    {
                        case LogicInfo.True:
                            #region 链接为True

                            if (Link_ChooseModel1.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                //加载数据
                                Link_ChooseModel1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }
                            else
                            {
                                //获得数据
                                Link_ChooseModel1.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel1.m_DataName);

                                //加载数据
                                //Link_ChooseModel1 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }

                            //判断数据为True//模块运行成功
                            if (Link_ChooseModel1.m_DataValue == null)
                            {
                                ModuleParam.BlnSuccessed = false;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = (bool)Link_ChooseModel1.m_DataValue;
                            }

                            #endregion
                            break;
                        case LogicInfo.False:
                            #region 链接为False

                            if (Link_ChooseModel1.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                //加载数据
                                Link_ChooseModel1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }
                            else
                            {
                                //获得数据
                                Link_ChooseModel1.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel1.m_DataName);
                                //Link_ChooseModel1 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }

                            //判断数据为False//模块运行成功
                            if (Link_ChooseModel1.m_DataValue == null)
                            {
                                ModuleParam.BlnSuccessed = false;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = !(bool)Link_ChooseModel1.m_DataValue;
                            }

                            #endregion
                            break;
                        case LogicInfo.上升沿:
                            #region 链接为上升沿

                            //读取为局部变量
                            if (Link_ChooseModel1.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                Link_ChooseModel1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }
                            else//读取为全局变量
                            {
                                //获得数据
                                Link_ChooseModel1.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel1.m_DataName);

                                //Link_ChooseModel1 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }

                            //读取为局部变量
                            if (Link_ChooseModel2.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                Link_ChooseModel2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel2.m_DataName &&
                                c.m_DataTip == Link_ChooseModel2.m_DataTip && c.m_DataModuleID == Link_ChooseModel2.m_DataModuleID);
                            }
                            else//读取为全局变量
                            {
                                //获得数据
                                Link_ChooseModel2.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel2.m_DataName);

                                //Link_ChooseModel2 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel2.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel2.m_DataTip && c.m_DataModuleID == Link_ChooseModel2.m_DataModuleID);
                            }

                            //判断数据为True//模块运行成功
                            bool choose1;
                            bool choose2;

                            //判断数据为True//模块运行成功
                            if (Link_ChooseModel1.m_DataValue == null)
                            {
                                choose1 = false;
                            }
                            else
                            {
                                choose1 = (bool)Link_ChooseModel1.m_DataValue;
                            }

                            //判断数据为True//模块运行成功
                            if (Link_ChooseModel2.m_DataValue == null)
                            {
                                choose2 = false;
                            }
                            else
                            {
                                choose2 = (bool)Link_ChooseModel2.m_DataValue;
                            }

                            //第一个参数为true，第二个参数为False
                            if (choose1 && !choose2)
                            {
                                ModuleParam.BlnSuccessed = true;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = false;
                            }

                            #endregion
                            break;
                        case LogicInfo.下降沿:
                            #region 链接为下降沿

                            if (Link_ChooseModel1.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                //加载数据
                                Link_ChooseModel1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }
                            else
                            {
                                //获得数据
                                Link_ChooseModel1.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel1.m_DataName);

                                //加载数据
                                //Link_ChooseModel1 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel1.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel1.m_DataTip && c.m_DataModuleID == Link_ChooseModel1.m_DataModuleID);
                            }

                            if (Link_ChooseModel2.m_DataAtrr == DataVarType.DataAtrribution.局部变量)
                            {
                                //加载数据
                                Link_ChooseModel2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_ChooseModel2.m_DataName &&
                                c.m_DataTip == Link_ChooseModel2.m_DataTip && c.m_DataModuleID == Link_ChooseModel2.m_DataModuleID);
                            }
                            else
                            {
                                //获得数据
                                Link_ChooseModel2.m_DataValue = OperDataVar.GetGlobalValue(Link_ChooseModel2.m_DataName);

                                //加载数据
                                //Link_ChooseModel2 = SysProcessPro.g_VarList.Find(c => c.m_DataName == Link_ChooseModel2.m_DataName &&
                                //c.m_DataTip == Link_ChooseModel2.m_DataTip && c.m_DataModuleID == Link_ChooseModel2.m_DataModuleID);
                            }

                            //判断数据为True//模块运行成功
                            bool choose3;
                            bool choose4;

                            //判断数据为True//模块运行成功
                            if (Link_ChooseModel1.m_DataValue == null)
                            {
                                choose3 = false;
                            }
                            else
                            {
                                choose3 = (bool)Link_ChooseModel1.m_DataValue;
                            }

                            //判断数据为True//模块运行成功
                            if (Link_ChooseModel2.m_DataValue == null)
                            {
                                choose4 = false;
                            }
                            else
                            {
                                choose4 = (bool)Link_ChooseModel2.m_DataValue;
                            }

                            //第一个参数为true，第二个参数为False
                            if (!choose3 && choose4)
                            {
                                ModuleParam.BlnSuccessed = true;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = false;
                            }

                            #endregion
                            break;
                        default:
                            break;
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
}
