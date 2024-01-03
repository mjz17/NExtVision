using CommunaCation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommunaCationPLC
{
    /// <summary>
    /// 通讯基础类
    /// </summary>
    [Serializable]
    public class CommunaBase
    {
        /// <summary>
        /// 通讯类型
        /// </summary>
        public CommunaType communaType { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port = 0;

        /// <summary>
        /// 从站地址
        /// </summary>
        public int SlaveAddress { get; set; } = 1;

        public int MaxCycleTimer { get; set; } = 5;

        /// <summary>
        /// 发送超时
        /// </summary>
        public int SendTimeOut { get; set; } = 2000;

        /// <summary>
        /// 接收超时
        /// </summary>
        public int ReceiveTimeOut { get; set; } = 2000;

        /// <summary>
        /// 数据格式
        /// </summary>
        public DataFormat format { get; set; } = DataFormat.ABCD;

        /// <summary>
        /// 整数变量类型
        /// </summary>
        public VarType Int_Type { get; set; } = VarType.Int32;

        /// <summary>
        /// 浮点变量类型
        /// </summary>
        public VarType Float_Type { get; set; } = VarType.Float;

        /// <summary>
        /// 起始地址
        /// </summary>
        public int Address { get; set; } = 0;

        /// <summary>
        /// 读取长度
        /// </summary>
        public int iLength { get; set; } = 1;

        /// <summary>
        /// 变量结果
        /// </summary>
        public List<object> varResult = new List<object>();

        /// <summary>
        /// 变量数据类型
        /// </summary>
        public List<VarType> VarLength = new List<VarType>();

        /// <summary>
        /// 写入数据
        /// </summary>
        public List<object> writeVar = new List<object>();

        /// <summary>
        /// 端口数据
        /// </summary>
        public ECommunacation eCom;

        [NonSerialized]
        public bool IsConnected = false;

        //互斥锁
        [NonSerialized]
        public SimpleHybirdLock InteractiveLock = new SimpleHybirdLock();

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommunaBase(CommunaType type)
        {
            communaType = type;
        }

        /// <summary>
        /// 连接
        /// </summary>
        public virtual void Connect()
        {

        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public virtual void DisConnect()
        {
            IsConnected = false;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public virtual void Read() { }

        /// <summary>
        /// 写入数据
        /// </summary>
        public virtual void Write() { }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            InteractiveLock = new SimpleHybirdLock();
            VarLength = new List<VarType>();
            varResult = new List<object>();
        }

    }
}
