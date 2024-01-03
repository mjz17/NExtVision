using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunaCationPLC
{
    /// <summary>
    /// 通讯类型
    /// </summary>
    [Serializable]
    public enum CommunaType
    {
        None,
        ModbusRtu,
        ModbusTcp,
    }

    /// <summary>
    /// 数据格式
    /// </summary>
    [Serializable]
    public enum DataFormat
    {
        ABCD,
        BADC,
        CDAB,
        DCBA
    }

    /// <summary>
    /// 变量数据类型
    /// </summary>
    [Serializable]
    public enum VarType
    {
        Bit,
        Int16,
        Int32,
        Int64,
        Float,
        Double,
    }
}
