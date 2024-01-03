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

namespace Plugin.DataQueue
{
    [Category("变量工具")]
    [DisplayName("数据出队")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 是否阻塞
        /// </summary>
        public bool IsWait = false;

        /// <summary>
        /// 数据出队的时候 是否删除出队的数据
        /// </summary>
        public bool IsDeleteData = true;

        /// <summary>
        /// 是否限制长度
        /// </summary>
        public bool IsLimitLength = true;

        /// <summary>
        /// 队列的长度 最小值为1
        /// </summary>
        public int LimitLength = 1;

        /// <summary>
        /// 数据队列(key:流程名称，对应的值)
        /// </summary>
        public List<DataInInfo> DataInInfo = new List<DataInInfo>();

        public override void Stop()
        {
            //DataOutQueue.s_QueueDic[m_DataOut.QueueKey].Clear();
            DataOutQueue.s_QueueSignDic[m_DataOut.QueueKey].Set();//释放阻塞
        }

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

                //将数据读取出来
                m_DataOut = DataOutQueue.s_QueueDic[m_DataOut.QueueKey];

                //是否阻塞
                m_DataOut.IsWait = IsWait;

                //是否删除出队数据
                m_DataOut.IsDeleteData = IsDeleteData;

                //是否限制长度
                m_DataOut.IsLimitLength = IsLimitLength;

                //队列的长度
                m_DataOut.LimitLength = LimitLength;

                bool flag = false;

                //判断是否阻塞
                if (m_DataOut.IsWait)
                {
                    DataOutQueue.s_QueueSignDic[m_DataOut.QueueKey].Reset();
                    lock (m_DataOut)
                    {
                        //判断能否出队成功,出队成功就不阻塞
                        bool tempFlag = m_DataOut.IsDeleteData;//先试着出一次队出队成功就不用阻塞,出队失败就阻塞
                        m_DataOut.IsDeleteData = false;
                        m_DataOut.IsIgnoreError = true;//屏蔽错误提示

                        flag = FlushDataOut();

                        m_DataOut.IsIgnoreError = false;//屏蔽错误提示
                        m_DataOut.IsDeleteData = tempFlag;

                        //运行成功
                        ModuleParam.BlnSuccessed = flag;
                    }
                    if (flag == false)
                    {
                        DataOutQueue.s_QueueSignDic[m_DataOut.QueueKey].WaitOne();//阻塞
                        flag = FlushDataOut();
                        ModuleParam.BlnSuccessed = flag;
                    }
                }
                else
                {
                    flag = FlushDataOut();
                    //运行成功
                    ModuleParam.BlnSuccessed = flag;
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

        public bool FlushDataOut()
        {
            bool flag = true;
            for (int i = 0; i < m_DataOut.m_DataTypeList.Count; i++)
            {
                int length = 0;

                switch (m_DataOut.m_DataTypeList[i])
                {
                    case DataQueueType.None:
                        break;
                    case DataQueueType.Bool:
                        List<bool> tempList0 = (List<bool>)m_DataOut.m_DataQueueList[i];
                        length = tempList0.Count();
                        if (length > 0)
                        {
                            if (IsLimitLength == false || (IsLimitLength == true && length >= LimitLength))
                            {
                                if (IsLimitLength == true)
                                    tempList0.RemoveRange(0, length - LimitLength);//长度限制,删除多余的数据
                                bool value;
                                if (IsDeleteData == true)
                                {
                                    value = tempList0.First();
                                    tempList0.RemoveAt(0);//删除第一个
                                }
                                else
                                {
                                    value = tempList0.First();
                                }

                                if (m_DataOut.IsIgnoreError == false)
                                {
                                    Log.Info($"{m_ModuleProject.ProjectInfo.m_ProjectName},输出出队数据{value}"); //模块数据
                                }
                                else
                                {
                                    //Log.Info($"{m_ModuleProject.ProjectInfo.m_ProjectName},输出出队数据{value}"); //模块数据
                                }
                            }
                            else
                            {

                                flag = false;
                            }
                        }
                        else
                        {
                            if (m_DataOut.IsIgnoreError == false)
                                Log.Error($"{m_DataOut.m_DataTypeList[i].ToString() }数据出队失败");
                            flag = false;
                        }
                        break;
                    case DataQueueType.Int:
                        List<int> tempList1 = (List<int>)m_DataOut.m_DataQueueList[i];
                        length = tempList1.Count();
                        if (length > 0)
                        {
                            if (IsLimitLength == false || (IsLimitLength == true && length >= LimitLength))
                            {
                                if (IsLimitLength == true)
                                    tempList1.RemoveRange(0, length - LimitLength);//长度限制,删除多余的数据
                                int value;
                                if (IsDeleteData == true)
                                {
                                    value = tempList1.First();
                                    tempList1.RemoveAt(0);//删除第一个
                                }
                                else
                                {
                                    value = tempList1.First();
                                }

                                if (m_DataOut.IsIgnoreError == false)
                                {
                                    Log.Info($"{m_ModuleProject.ProjectInfo.m_ProjectName},输出出队数据{value}"); //模块数据
                                }
                                else
                                {
                                    Log.Info($"{m_ModuleProject.ProjectInfo.m_ProjectName},输出出队数据{value}"); //模块数据
                                }
                            }
                            else
                            {

                                flag = false;
                            }
                        }
                        else
                        {
                            if (m_DataOut.IsIgnoreError == false)
                                Log.Error($"{m_DataOut.m_DataTypeList[i].ToString() }数据出队失败");
                            flag = false;
                        }

                        break;
                    case DataQueueType.IntArr:
                        break;
                    case DataQueueType.String:
                        break;
                    case DataQueueType.StringArr:
                        break;
                    default:
                        break;
                }
            }
            if (flag == false)
            {
                if (m_DataOut.IsIgnoreError == false)
                    Log.Info("输出出队数据失败");
            }
            return flag;
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            m_DataOut = new DataOutQueue(m_ModuleProject.ProjectInfo.m_ProjectName + "." + ModuleParam.ModuleName);
            for (int i = 0; i < DataInInfo.Count; i++)
            {
                switch (DataInInfo[i].m_DataTypeIn)
                {
                    case DataQueueType.Bool:
                        m_DataOut.DefineBoolQueue();
                        break;
                    case DataQueueType.Int:
                        m_DataOut.DefineIntQueue();
                        break;
                    case DataQueueType.IntArr:
                        break;
                    case DataQueueType.String:
                        break;
                    case DataQueueType.StringArr:
                        break;
                    default:
                        break;
                }
            }

            //s_QueueDic = new Dictionary<string, DataOutQueue>();
            //s_QueueSignDic = new Dictionary<string, AutoResetEvent>();
        }

    }
}
