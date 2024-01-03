using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Common;
using DefineImgRoI;
using HalconDotNet;
using PublicDefine;

namespace ModuleDataVar
{
    /// <summary>
    /// 变量
    /// </summary>
    [Serializable]
    public struct DataVar : ICloneable
    {

        /// <summary>
        /// 当前流程中，模块对应ID
        /// </summary>
        private int _DataModuleID;

        public int m_DataModuleID
        {
            get { return _DataModuleID; }
            set { _DataModuleID = value; }
        }

        /// <summary>
        /// 变量类型
        /// </summary>
        private DataVarType.DataGroup _DataGroup;

        public DataVarType.DataGroup m_DataGroup
        {
            get { return _DataGroup; }
            set { _DataGroup = value; }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        private DataVarType.DataType _DataType;

        public DataVarType.DataType m_DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        /// <summary>
        /// 变量的个数
        /// </summary>
        private int _Data_Num;

        public int m_Data_Num
        {
            get { return _Data_Num; }
            set { _Data_Num = value; }
        }

        /// <summary>
        /// 变量范围
        /// </summary>
        private DataVarType.DataAtrribution _DataAtrr;

        public DataVarType.DataAtrribution m_DataAtrr
        {
            get { return _DataAtrr; }
            set { _DataAtrr = value; }
        }

        /// <summary>
        /// 变量名称
        /// </summary>
        private String _DataName;

        public String m_DataName
        {
            get { return _DataName; }
            set { _DataName = value; }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private String _DataTip;

        public String m_DataTip
        {
            get { return _DataTip; }
            set { _DataTip = value; }
        }

        /// <summary>
        /// 变量初始值
        /// </summary>
        private String _Data_InitValue;

        public String m_Data_InitValue
        {
            get { return _Data_InitValue; }
            set { _Data_InitValue = value; }
        }

        /// <summary>
        /// 变量
        /// </summary>
        /// <returns></returns>
        [NonSerialized]
        private object _DataValue;

        public object m_DataValue
        {
            get { return _DataValue; }
            set { _DataValue = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_Atrr">变量属性，全局or局部</param>
        /// <param name="_ModuleID">变量所属模块ID</param>
        /// <param name="_Name">变量名称</param>
        /// <param name="_Type">数据类型</param>
        /// <param name="_Group">变量类型</param>
        /// <param name="_Num">数据个数</param>
        /// <param name="_InitValue">初始值</param>
        /// <param name="_Tip">备注</param>
        /// <param name="_Value">输入值</param>
        public DataVar(DataVarType.DataAtrribution _Atrr, int _ModuleID, string _Name, DataVarType.DataType _Type, DataVarType.DataGroup _Group,
            int _Num, string _InitValue, string _Tip, object _Value)
        {
            _DataAtrr = _Atrr;//全局or局部
            _DataModuleID = _ModuleID;//所属流程ID
            _DataName = _Name;//变量名称
            _DataType = _Type;//数据类型
            _DataGroup = _Group;//所属类型
            _Data_Num = _Num;
            _DataTip = _Tip;
            _Data_InitValue = _InitValue;
            _DataValue = _Value;
        }

        /// <summary>
        /// 初始化变量1
        /// </summary>
        /// <param name="_ModuleID"></param>
        /// <param name="_Name"></param>
        /// <param name="_DataType"></param>
        /// <param name="_InitValue"></param>
        public void InitValue(int _ModuleID, string _Name, DataVarType.DataType _DataType, string _InitValue)
        {
            _DataModuleID = _ModuleID;
            _DataName = _Name;
            InitValue(_DataType, _InitValue);
        }

        /// <summary>
        /// 初始化变量值
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_InitValue"></param>
        public void InitValue(DataVarType.DataType _data, string _InitValue)
        {
            _DataType = _data;
            _Data_InitValue = _InitValue;
            //初始化控件
            switch (_data)
            {
                case DataVarType.DataType.Int:
                    _DataValue = int.Parse(_InitValue);
                    break;
                case DataVarType.DataType.Double:
                    _DataValue = double.Parse(_InitValue);
                    break;
                case DataVarType.DataType.Bool:
                    _DataValue = Convert.ToBoolean(_InitValue);
                    break;
                case DataVarType.DataType.String:
                    _DataValue = _InitValue;
                    break;
                case DataVarType.DataType.Int_Array:
                    _DataValue = new List<int>() { int.Parse(_InitValue) };
                    break;
                case DataVarType.DataType.Double_Array:
                    _DataValue = new List<double>() { double.Parse(_InitValue) };
                    break;
                case DataVarType.DataType.Bool_Array:
                    _DataValue = new List<bool>() { bool.Parse(_InitValue) };
                    break;
                case DataVarType.DataType.String_Array:
                    _DataValue = new List<string>() { _InitValue };
                    break;
                case DataVarType.DataType.Image:
                    _DataValue = new List<HImageExt>() { new HImageExt() };
                    break;
                case DataVarType.DataType.Line:
                    _DataValue = new List<Line_INFO>() { new Line_INFO() };
                    break;
                case DataVarType.DataType.Circle:
                    _DataValue = new List<Circle_INFO>() { new Circle_INFO() };
                    break;
                case DataVarType.DataType.Rectangle1:
                    _DataValue = new List<Rectangle_INFO>() { new Rectangle_INFO() };
                    break;
                case DataVarType.DataType.Rectangle2:
                    _DataValue = new List<Rectangle2_INFO>() { new Rectangle2_INFO() };
                    break;
                case DataVarType.DataType.坐标系:
                    _DataValue = new List<Coordinate_INFO>() { new Coordinate_INFO() };
                    break;
                case DataVarType.DataType.位置转换2D:
                    _DataValue = new List<HHomMat2D>() { new HHomMat2D() };
                    break;
                case DataVarType.DataType.平面信息:
                    _DataValue = new List<Plane_INFO>() { new Plane_INFO() };
                    break;
                case DataVarType.DataType.区域:
                    _DataValue = new List<HRegion>() { new HRegion() };
                    break;
                default:
                    break;
            }
        }

        [OnDeserializing()]
        internal void OnDeSerializingMethod(StreamingContext context)
        {
            _Data_InitValue = "1";
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            InitValue(_DataType, _Data_InitValue);
        }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }
    }
}
