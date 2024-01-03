using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using VisionCore;

namespace Plugin.ArraySet
{
    [Category("变量工具")]
    [DisplayName("数组设置")]
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
                            int Index1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataModuleID == item.Link_DataVar_Write.m_DataModuleID
                            && c.m_DataName == item.Link_DataVar_Write.m_DataName);

                            if (Index1 > -1)
                            {
                                //判断数据类型

                                link_write = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Index1];
                                //将数据写入链接模块//全局变量
                                //link_read.m_DataValue

                                switch (item.m_DataType)
                                {
                                    case DataQueueType.BoolArr:
                                        List<bool> bool_list = (List<bool>)link_read.m_DataValue;
                                        bool_list.Add((bool)link_write.m_DataValue);
                                        link_read.m_DataValue = bool_list;
                                        //将数据写入链接模块//全局变量
                                        ModuleProject.UpdateLocalVarValue(link_read);
                                        break;
                                    case DataQueueType.IntArr:
                                        List<int> int_list = (List<int>)link_read.m_DataValue;
                                        int_list.Add((int)link_write.m_DataValue);
                                        link_read.m_DataValue = int_list;
                                        //将数据写入链接模块//全局变量
                                        ModuleProject.UpdateLocalVarValue(link_read);
                                        break;
                                    case DataQueueType.StringArr:
                                        List<string> string_list = (List<string>)link_read.m_DataValue;
                                        string_list.Add((string)link_write.m_DataValue);
                                        link_read.m_DataValue = string_list;
                                        //将数据写入链接模块//全局变量
                                        ModuleProject.UpdateLocalVarValue(link_read);
                                        break;
                                    case DataQueueType.DoubleArr:
                                        List<double> double_list = (List<double>)link_read.m_DataValue;
                                        double_list.Add((double)link_write.m_DataValue);
                                        link_read.m_DataValue = double_list;
                                        //将数据写入链接模块//全局变量
                                        ModuleProject.UpdateLocalVarValue(link_read);
                                        break;
                                }
                                ModuleParam.BlnSuccessed = true;
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
