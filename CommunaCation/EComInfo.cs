using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunaCation
{
    public class EComInfo
    {

        /// <summary>
        /// 通讯设备key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 是否正在连接  没有连接上 正在连接也返回true
        /// </summary>
        public bool IsConnected { get; set; }


        public CommunicationModel CommunicationModel { get; set; }


        public EComInfo(string key, bool isConnected, CommunicationModel communicationModel)
        {
            Key = key;
            IsConnected = isConnected;
            CommunicationModel = communicationModel;
        }

    }
}
