using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DataVarDefinition
{
    [Category("变量工具")]
    [DisplayName("变量定义")]
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
                DataVar dataVar = new DataVar();

                //添加数据
                for (int i = 0; i < m_DataVar.Count; i++)
                {
                    if (m_DataVar[i].m_DataType == DataQueueType.Bool)
                    {
                        //存储数据Bool
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, m_DataVar[i].m_DataName,
                   DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, false);
                    }
                    else if (m_DataVar[i].m_DataType == DataQueueType.Int)
                    {
                        //存储数据Int
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, m_DataVar[i].m_DataName,
                   DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, 0);
                    }
                    else if (m_DataVar[i].m_DataType == DataQueueType.Double)
                    {
                        //存储数据Double
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, m_DataVar[i].m_DataName,
                   DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, 0);
                    }
                    else if (m_DataVar[i].m_DataType == DataQueueType.String)
                    {
                        //存储数据String
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, m_DataVar[i].m_DataName,
                   DataVarType.DataType.String, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, "0");
                    }
                    ModuleProject.UpdateLocalVarValue(dataVar);
                }

                ModuleParam.BlnSuccessed = true;
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                sw.Reset();
            }
            catch (Exception ex)
            {
                //运行失败
                ModuleParam.BlnSuccessed = false;
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                sw.Reset();
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

        public void QueryData()
        {
            try
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    foreach (DataVarTool item in m_DataVar)
                    {

                        int index = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataTip == ModuleParam.ModuleName
                        && c.m_DataName == item.m_DataName);

                        if (index > -1)
                        {
                            item.m_Data_Result = SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
