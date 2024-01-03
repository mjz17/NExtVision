using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    public enum DataQueueType
    {
        None = -1,
        Bool,
        Int,
        Double,
        String,
        BoolArr,
        IntArr,
        DoubleArr,
        StringArr
    }


    [Serializable]
    public class DataInInfo
    {
        public int DataID { get; set; }          //流程ID

        public string DataName { get; set; }        //流程名称

        public Object m_DataQueueIn { get; set; }  //要插入到队列的数据

        public string DataTip { get; set; }         //注释

        public DataQueueType m_DataTypeIn { get; set; } //插入队列的数据类型

    }
}
