using Common;
using CommunaCationPLC;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.PlcReceiveMessage
{
    [Category("通讯测试")]
    [DisplayName("PLC读取")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessName { get; set; }

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

                //识别通讯文件
                string[] Info = ProcessName.Split('.');
                int ProcessNum = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Info[0]);
                if (ProcessNum > -1)
                {
                    m_Communa = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList.Find(c => c.ModuleParam.PluginName.Contains(Info[1])).m_Communa;
                }

                m_Communa.VarLength.Clear();

                //组码,读取数据长度
                if (ReceData.Count != 0)
                {
                    foreach (EcomData item in ReceData)
                    {
                        m_Communa.VarLength.Add(item.VarType);
                    }
                }

                //寄存器读取起始地址
                m_Communa.Address = startAddress;

                //读取数据
                m_Communa.Read();

                //解码
                for (int i = 0; i < m_Communa.varResult.Count; i++)
                {
                    ReceData[i].DataObj = m_Communa.varResult[i];
                }

                DataVar dataVar = new DataVar();

                //添加数据
                for (int i = 0; i < ReceData.Count; i++)
                {
                    if (ReceData[i].VarType == VarType.Bit)
                    {
                        bool status = false;

                        if (ReceData[i].DataObj != null)
                        {
                            if ((int)ReceData[i].DataObj > 0)
                            {
                                status = true;
                            }
                            else
                            {
                                status = false;
                            }
                        }

                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ReceData[i].DataName,
                   DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, status);

                    }
                    else if (ReceData[i].VarType == VarType.Int16)
                    {
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ReceData[i].DataName,
                  DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ReceData[i].DataObj);
                    }
                    else if (ReceData[i].VarType == VarType.Int32)
                    {
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ReceData[i].DataName,
                 DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ReceData[i].DataObj);
                    }
                    else if (ReceData[i].VarType == VarType.Int64)
                    {
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ReceData[i].DataName,
                 DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ReceData[i].DataObj);
                    }
                    else if (ReceData[i].VarType == VarType.Float)
                    {
                        dataVar = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ReceData[i].DataName,
                 DataVarType.DataType.Float, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ReceData[i].DataObj);
                    }
                    ModuleProject.UpdateLocalVarValue(dataVar);
                }

                //运行成功
                ModuleParam.BlnSuccessed = true;
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

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            //ReceData = new List<EcomData>();
        }

    }
}
