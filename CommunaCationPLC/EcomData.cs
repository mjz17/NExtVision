using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunaCationPLC
{
    /// <summary>
    /// 变量数据结果
    /// </summary>
    [Serializable]
    public class EcomData
    {

        public int DataID { get; set; }                         //寄存器地址

        public string DataAtrr { get; set; }                    //变量所属

        public int LinkDataModuleID { get; set; }               //模块ID

        public string DataType { get; set; }                    //寄存器类型

        public CommunaCationPLC.VarType VarType { get; set; }   //寄存器类型

        public string DataName { get; set; } = string.Empty;    //寄存器名称

        public object DataObj { get; set; }                     //寄存器数据结果

        public string DataTip { get; set; }                     //注释

    }
}
