using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DataEntry
{

    [Category("变量工具")]
    [DisplayName("数据入队")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 数据出队流程
        /// </summary>
        public string LinkDataOut { get; set; }

        /// <summary>
        /// 起始索引
        /// </summary>
        public int m_QueueIndex { get; set; }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="info"></param>
        public void AddQueue(DataInInfo info)
        {
            DataInInfo.Add(info);
        }

        /// <summary>
        /// 数据队列(key:流程名称，对应的值)
        /// </summary>
        public List<DataInInfo> DataInInfo = new List<DataInInfo>();

        /// <summary>
        /// 数据出列
        /// </summary>
        public DataOutQueue m_dataOut;

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
                //数据出列模块
                string[] Info = LinkDataOut.Split('.');
                int ProcessNum = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Info[0]);
                if (ProcessNum > -1)
                {
                    //根据链接查询出链接模块数据//数据出列
                    m_dataOut = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList.Find(c => c.ModuleParam.ModuleName.Contains(Info[1])).m_DataOut;

                    //查询索引
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        //查询数据
                        for (int i = 0; i < DataInInfo.Count; i++)
                        {
                            //加载数据//输入数据的时候呗搞错了，2023.1.16
                            DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataModuleID == DataInInfo[i].DataID &&
                     c.m_DataName == DataInInfo[i].DataName && c.m_DataTip == DataInInfo[i].DataTip);

                            DataInInfo[i].m_DataQueueIn = data.m_DataValue;
                        }
                    }

                    //锁
                    lock (m_dataOut)
                    {
                        int dataOutLength = m_dataOut.GetQueueCount();

                        //判断插入的位置是否存在
                        if (dataOutLength < (m_QueueIndex + DataInInfo.Count()))
                        {
                            Log.Error("入队变量的长度  超过 数据出队的变量的长度");
                            ModuleParam.BlnSuccessed = false;
                            return;
                        }
                        else if (m_QueueIndex < 0)
                        {
                            Log.Error("入队变量的索引 为负值");
                            ModuleParam.BlnSuccessed = false;
                            return;
                        }

                        for (int i = 0; i < DataInInfo.Count; i++)
                        {
                            if (DataInInfo[i].m_DataTypeIn == m_dataOut.GetDataType(i + m_QueueIndex))
                            {
                                switch (DataInInfo[i].m_DataTypeIn)
                                {
                                    case DataQueueType.None:
                                        break;
                                    case DataQueueType.Bool:
                                        bool value0 = (bool)DataInInfo[i].m_DataQueueIn;//获取值
                                        List<bool> tempList0 = (List<bool>)m_dataOut.GetDataQueue(i + m_QueueIndex);//获取队列
                                        tempList0.Add(value0);//更新队列
                                        break;
                                    case DataQueueType.Int:
                                        int value1 = (int)DataInInfo[i].m_DataQueueIn;//获取值
                                        List<int> tempList1 = (List<int>)m_dataOut.GetDataQueue(i + m_QueueIndex);//获取队列
                                        tempList1.Add(value1);//更新队列
                                        break;
                                    case DataQueueType.IntArr:
                                        List<int> value2 = (List<int>)DataInInfo[i].m_DataQueueIn;//获取值
                                        List<List<int>> tempList2 = (List<List<int>>)m_dataOut.GetDataQueue(i + m_QueueIndex);//获取队列
                                        tempList2.Add(value2);//更新队列
                                        break;
                                    case DataQueueType.String:
                                        string value3 = (string)DataInInfo[i].m_DataQueueIn;//获取值
                                        List<string> tempList3 = (List<string>)m_dataOut.GetDataQueue(i + m_QueueIndex);//获取队列
                                        tempList3.Add(value3);//更新队列
                                        break;
                                    case DataQueueType.StringArr:
                                        List<string> value4 = (List<string>)DataInInfo[i].m_DataQueueIn;//获取值
                                        List<List<string>> tempList4 = (List<List<string>>)m_dataOut.GetDataQueue(i + m_QueueIndex);//获取队列
                                        tempList4.Add(value4);//更新队列
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                Log.Error("数据入队类型 ] 与 对应的数据出队类型 不匹配");
                                WakeAll();
                                ModuleParam.BlnSuccessed = false;
                                return;
                            }
                        }
                    }

                    //运行成功
                    ModuleParam.BlnSuccessed = true;
                }
                else
                {
                    //运行失败
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
                WakeAll();
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

        private void WakeAll()
        {
            //DataInInfo.Clear();//入队后,将数据全部清空
            DataOutQueue.s_QueueSignDic[LinkDataOut].Set();
        }


        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            //ReceData = new List<EcomData>();
        }

    }
}
