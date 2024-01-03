using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 数据出队
    /// </summary>
    [Serializable]
    public class DataOutQueue
    {
        //容器
        public static Dictionary<string, DataOutQueue> s_QueueDic = new Dictionary<string, DataOutQueue>();
        public static Dictionary<string, AutoResetEvent> s_QueueSignDic = new Dictionary<string, AutoResetEvent>();

        public string QueueKey { get; set; }

        /// <summary>
        /// 是否限制长度
        /// </summary>
        public bool IsLimitLength = true;

        /// <summary>
        /// 队列的长度 最小值为1
        /// </summary>
        public int LimitLength = 1;

        /// <summary>
        /// 是否阻塞
        /// </summary>
        public bool IsWait = false;

        /// <summary>
        /// 数据出队的时候 是否删除出队的数据
        /// </summary>
        public bool IsDeleteData = true;

        /// <summary>
        /// 忽略错误
        /// </summary>
        public bool IsIgnoreError = false;

        /// <summary>
        /// 队列
        /// </summary>
        public List<Object> m_DataQueueList = new List<object>();

        /// <summary>
        /// 队列类型
        /// </summary>
        public List<DataQueueType> m_DataTypeList = new List<DataQueueType>();

        public DataOutQueue()
        {

        }

        public DataOutQueue(string QueneName)
        {
            QueueKey = QueneName;
            s_QueueDic[QueueKey] = this;
            s_QueueSignDic[QueueKey] = new AutoResetEvent(false);
        }

        #region 定义数据队列

        public void DefineBoolQueue()
        {
            m_DataQueueList.Add(new List<bool>());
            m_DataTypeList.Add(DataQueueType.Bool);
        }

        public void DefineIntQueue()
        {
            m_DataQueueList.Add(new List<int>());
            m_DataTypeList.Add(DataQueueType.Int);
        }

        public void DefineStringQueue()
        {
            m_DataQueueList.Add(new List<string>());
            m_DataTypeList.Add(DataQueueType.String);
        }

        public void DefineIntListQueue()
        {
            m_DataQueueList.Add(new List<List<int>>());
            m_DataTypeList.Add(DataQueueType.IntArr);
        }

        public void DefineStringListQueue()
        {
            m_DataQueueList.Add(new List<List<string>>());
            m_DataTypeList.Add(DataQueueType.StringArr);
        }

        #endregion

        /// <summary>
        /// 获取队列的长度
        /// </summary>
        /// <returns></returns>
        public int GetQueueCount()
        {
            return m_DataQueueList.Count;
        }

        /// <summary>
        /// 根据索引获取DataType
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataQueueType GetDataType(int index)
        {
            return m_DataTypeList[index];
        }

        /// <summary>
        /// 根据索引获取 对应的 Queue
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Object GetDataQueue(int index)
        {
            return m_DataQueueList[index];
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                m_DataQueueList.Clear();
                m_DataTypeList.Clear();
                s_QueueSignDic[QueueKey].Set();
            }
        }

    }
}
