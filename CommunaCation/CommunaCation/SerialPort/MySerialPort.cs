using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace CommunaCation
{
    /// <summary>
    /// 串口数据的规则
    /// </summary>
    /// <param name="serialPort"></param>
    /// <returns></returns>
    public delegate string SerialPortDataReceivedFunction(SerialPort serialPort);

    public class MySerialPort
    {
        /// <summary>
        /// 接受数据事件
        /// </summary>
        public event ReceiveString OnReceiveString;

        /// <summary>
        /// 数据接收委托
        /// </summary>
        public SerialPortDataReceivedFunction DataReceivedFunction { get; set; } = null;

        /// <summary>
        /// 枚举保存的消息类型
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };

        /// <summary>
        /// 消息变量
        /// </summary>
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        private SerialPort comPort = new SerialPort();


        //消息属性

        /// <summary>
        /// 串口状态
        /// </summary>
        public bool isPortOpen
        {
            get { return comPort.IsOpen; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// 奇偶校验
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// 端口名称
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        public MySerialPort()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _portName = "COM1";
            comPort.Encoding = Encoding.Default;
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isSendByHex"></param>
        /// <returns></returns>
        public bool WriteData(string msg, bool isSendByHex)
        {
            try
            {
                if (!(comPort.IsOpen == true))
                {
                    DisplayData(MessageType.Error, $"[{PortName}] 串口未打开!");
                    return false;
                }
                if (isSendByHex == true)
                {
                    byte[] bytes = HexTool.HexToByte(HexTool.StrToHexStr(msg));
                    comPort.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    comPort.Write(msg);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool OpenPort()
        {
            try
            {
                ClosePort();

                //set the properties of our SerialPort Object
                comPort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comPort.DataBits = int.Parse(_dataBits);    //DataBits
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comPort.PortName = _portName;   //PortName
                //now open the port
                comPort.Open();
                //display message
                DisplayData(MessageType.Normal, "Port opened at " + DateTime.Now + "\n");
                //return true
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            if (comPort.IsOpen == true) comPort.Close();
        }

        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string msg = "";

            if (DataReceivedFunction == null)
            {
                Thread.Sleep(50);//一定要延迟, 才能直接去读.因为流还没有写完,就会触发事件进入 magical 2019-3-1 20:21:43
                msg = comPort.ReadExisting().Trim();
            }
            else
            {
                msg = DataReceivedFunction.Invoke(comPort);
            }

            if (msg.Length < 6)
            {
                ;
                // string s = comPort.ReadExisting().Trim();
            }

            //display the data to the user
            if (msg.Length > 0)
                DisplayData(MessageType.Incoming, msg + "\n");

            OnReceiveString?.Invoke(msg);
        }

        [STAThread]
        private void DisplayData(MessageType type, string msg)
        {
            switch (type)
            {
                case MessageType.Incoming:
                    Log.Info(msg);
                    break;
                case MessageType.Outgoing:
                    Log.Info(msg);
                    break;
                case MessageType.Normal:
                    Log.Info(msg);
                    break;
                case MessageType.Warning:
                    Log.Warn(msg);
                    break;
                case MessageType.Error:
                    Log.Error(msg);
                    break;
                default:
                    break;
            }
        }

    }
}
