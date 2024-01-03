using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleDataVar;

namespace StyleBase
{
    public class DataVarModel : NoitifyBase
    {

        /// <summary>
        /// 所属模块ID
        /// </summary>
        private int _DataModuleID;

        public int m_DataModuleID
        {
            get { return _DataModuleID; }
            set { _DataModuleID = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量属性
        /// </summary>
        private DataVarType.DataAtrribution _DataAtrr;

        public DataVarType.DataAtrribution m_DataAtrr
        {
            get { return _DataAtrr; }
            set { _DataAtrr = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量名称
        /// </summary>
        private String _DataName;

        public String m_DataName
        {
            get { return _DataName; }
            set { _DataName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        private DataVarType.DataType _DataType;

        public DataVarType.DataType m_DataType
        {
            get { return _DataType; }
            set { _DataType = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量所属数据类型
        /// </summary>
        private DataVarType.DataGroup _DataGroup;

        public DataVarType.DataGroup m_DataGroup
        {
            get { return _DataGroup; }
            set { _DataGroup = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量个数
        /// </summary>
        private int _Data_Num;

        public int m_Data_Num
        {
            get { return _Data_Num; }
            set { _Data_Num = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量初始值
        /// </summary>
        private String _Data_InitValue;

        public String m_Data_InitValue
        {
            get { return _Data_InitValue; }
            set { _Data_InitValue = value; this.DoNitify(); }
        }

        /// <summary>
        /// 变量初始值
        /// </summary>
        private object _DataValue;

        public object m_DataValue
        {
            get { return _DataValue; }
            set { _DataValue = value; this.DoNitify(); }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private String _DataTip;

        public String m_DataTip
        {
            get { return _DataTip; }
            set { _DataTip = value; this.DoNitify(); }
        }

        /// <summary>
        /// 选择
        /// </summary>
        private bool _Check;

        public bool m_Check
        {
            get { return _Check; }
            set { _Check = value; this.DoNitify(); }
        }

    }
}
