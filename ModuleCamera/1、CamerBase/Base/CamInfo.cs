using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCamera
{
    /// <summary>
    /// 相机信息
    /// </summary>
    public struct CamInfo
    {

        /// <summary>
        /// 相机品牌
        /// </summary>
        private DeviceType _DeviceType;

        public DeviceType m_DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }


        /// <summary>
        /// 设备内部编号
        /// </summary>
        private string _SerialNO;

        public string m_SerialNO
        {
            set { _SerialNO = value; }
            get { return _SerialNO; }
        }


        /// <summary>
        /// 版本信息
        /// </summary>
        private string _ExtInfo;

        public string m_ExtInfo
        {
            get { return _ExtInfo; }
            set { _ExtInfo = value; }
        }

        /// <summary>
        /// 连接状态
        /// </summary>
        private bool _BConnected;

        public bool m_BConnected
        {
            get { return _BConnected; }
            set { _BConnected = value; }
        }

    }
}
