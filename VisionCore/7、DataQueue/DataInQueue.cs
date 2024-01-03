using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 数据入队
    /// </summary>
    public class DataInQueue
    {
        /// <summary>
        /// 队列的key
        /// </summary>
        public string QueueKey = "";

        /// <summary>
        /// 起始索引
        /// </summary>
        public int QueueIndex = 0;

        /// <summary>
        /// 要插入到队列的数据,使用后就清空
        /// </summary>
        private List<Object> m_DataQueueInList = new List<object>();

        /// <summary>
        /// 对应的数据入队的数据类型
        /// </summary>
        private List<DataQueueType> m_DataTypeInList = new List<DataQueueType>();

      

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            bool flag = false;

            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
