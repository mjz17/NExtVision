using Common;
using CommunaCationPLC;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.PlcSendMessage
{
    [Category("通讯测试")]
    [DisplayName("PLC写入")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// 寄存器读取起始地址
        /// </summary>
        public int startAddress { get; set; } = 0;

        /// <summary>
        /// 通讯读取数据列表
        /// </summary>
        public List<EcomData> ReceData = new List<EcomData>();

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
                //PLC通讯
                string[] Info = ProcessName.Split('.');
                int ProcessNum = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Info[0]);
                if (ProcessNum > -1)
                {
                    m_Communa = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList.Find(c => c.ModuleParam.PluginName.Contains(Info[1])).m_Communa;
                }

                if (ReceData.Count == 0)
                {
                    ModuleParam.BlnSuccessed = false;
                    return;
                }

                //寄存器读取起始地址
                m_Communa.Address = startAddress;

                //组码,写入数据格式
                m_Communa.VarLength.Clear();
                foreach (EcomData item in ReceData)
                {
                    m_Communa.VarLength.Add(item.VarType);
                }

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);

                if (proIndex > -1)
                {
                    //对PLC地址写入数据
                    m_Communa.writeVar.Clear();
                    DataVar data;
                    foreach (EcomData item in ReceData)
                    {
                        //链接地址获得值
                        //识别通讯文件

                        if (item.DataAtrr.Contains("全局变量"))
                        {
                            //data = SysProcessPro.g_VarList.Find(c => c.m_DataName == item.DataName);
                            object obj = OperDataVar.GetGlobalValue(item.DataName);
                            m_Communa.writeVar.Add(obj);
                        }
                        else
                        {
                            data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataModuleID == item.LinkDataModuleID && c.m_DataName == item.DataName);
                            m_Communa.writeVar.Add(data.m_DataValue);
                        }
                    }

                    //写入数据
                    m_Communa.Write();
                    //运行成功
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
}
