using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 系统状态
    /// </summary>
    [Serializable]
    public struct SysStatus
    {
        private RunMode _RunMode;
        private EnviMode _EnviMode;//0联机模式//1脱机模式
        private bool _bAutoRun;//是否自动运行中

        public RunMode m_RunMode
        {
            set { _RunMode = value; }
            get { return _RunMode; }
        }
        public EnviMode m_EnviMode
        {
            set { _EnviMode = value; }
            get { return _EnviMode; }
        }
        public bool m_bAutoRun
        {
            set { _bAutoRun = value; }
            get { return _bAutoRun; }
        }
    }

    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunMode
    {
        None,
        单步运行,
        执行一次,
        循环运行
    }

    /// <summary>
    /// 连接模式
    /// </summary>
    public enum EnviMode
    {
        联机模式,
        脱机模式
    }

}
