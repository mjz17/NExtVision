using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DataVarSet
{
    [Category("变量工具")]
    [DisplayName("变量设置")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 变量定义的数据
        /// </summary>
        public List<DataVarTool> m_DataVar = new List<DataVarTool>();

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
                DataVar link_write;

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //遍历保存数据
                    foreach (DataVarTool item in m_DataVar)
                    {
                        //链接读取数据
                        DataVar link_read = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataModuleID == item.Link_DataVar_Read.m_DataModuleID
                        && c.m_DataTip == item.Link_DataVar_Read.m_DataTip && c.m_DataName == item.Link_DataVar_Read.m_DataName);

                        //链接写入数据
                        {
                            if (item.Link_DataVar_Write.m_DataAtrr.ToString().Contains("全局变量"))
                            {
                                //读取数据
                                DataVar obj = OperDataVar.GetGlobalValueBackData(item.Link_DataVar_Write.m_DataName);
                                if (obj.m_DataValue != null)
                                {
                                    //是否保存数据
                                    if (SysProcessPro.SaveUpValue)
                                    {
                                        //将数据写入初始值
                                        obj.m_Data_InitValue = link_read.m_DataValue.ToString();
                                        //将数据写入链接模块//系统变量
                                        obj.m_DataValue = link_read.m_DataValue;
                                    }
                                    else
                                    {
                                        //将数据写入链接模块//系统变量
                                        obj.m_DataValue = link_read.m_DataValue;
                                    }
                                    OperDataVar.UpdateGlobalValue(ref SysProcessPro.g_VarList, obj);
                                    ModuleParam.BlnSuccessed = true;
                                }
                                else
                                {
                                    ModuleParam.BlnSuccessed = false;
                                    Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{"数据查询失败"}"));
                                    break;
                                }
                            }
                            else//局部变量
                            {
                                int Index1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataModuleID == item.Link_DataVar_Write.m_DataModuleID
                                && c.m_DataName == item.Link_DataVar_Write.m_DataName);

                                if (Index1 > -1)
                                {
                                    //判断数据类型
                                    if (link_read.m_DataType == SysProcessPro.g_ProjectList[proIndex].m_Var_List[Index1].m_DataType)
                                    {
                                        link_write = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Index1];
                                        //将数据写入链接模块//全局变量
                                        link_write.m_DataValue = link_read.m_DataValue;
                                        ModuleProject.UpdateLocalVarValue(link_write);
                                        ModuleParam.BlnSuccessed = true;
                                    }
                                    else
                                    {
                                        ModuleParam.BlnSuccessed = false;
                                        Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{"数据查询失败"}"));
                                        break;
                                    }
                                }
                                else
                                {
                                    ModuleParam.BlnSuccessed = false;
                                    Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{"数据查询失败"}"));
                                    break;
                                }
                            }
                        }
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
