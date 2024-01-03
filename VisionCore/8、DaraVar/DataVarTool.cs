using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    [Serializable]
    public class DataVarTool
    {

        public DataQueueType m_DataType { get; set; }           //数据类型

        public string m_DataName { get; set; }                  //数据名称

        public string Link_DataVar_Read_Name { get; set; }      //链接读取数据

        public DataVar Link_DataVar_Read { get; set; }          //链接读取模块ID

        public string Link_DataVar_Write_Name { get; set; }     //链接写入数据

        public DataVar Link_DataVar_Write { get; set; }         //链接写入数据

        public object m_Data_Result { get; set; } = null;       //结果

        public string m_DataTip { get; set; }                   //注释

    }
}
