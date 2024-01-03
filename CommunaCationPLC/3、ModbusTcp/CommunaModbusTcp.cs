using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Common;
using CommunaCation;
using HslCommunication;
using HslCommunication.ModBus;
using static System.Collections.Specialized.BitVector32;

namespace CommunaCationPLC
{
    /// <summary>
    /// ModbusTcp
    /// </summary>
    [Serializable]
    public class CommunaModbusTcp : CommunaBase
    {

        [NonSerialized]
        private ModbusTcpNet busTcpClient = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        public CommunaModbusTcp(CommunaType type, ECommunacation communa) : base(type)
        {
            eCom = communa;//端口数据
        }

        /// <summary>
        /// 连接
        /// </summary>
        public override void Connect()
        {
            try
            {
                //如果连接
                if (IsConnected)
                    return;

                DisConnect();

                //连接对象
                busTcpClient = new ModbusTcpNet(eCom.RemoteIP, eCom.RemotePort, (byte)SlaveAddress);

                //首地址从0开始
                //busTcpClient.AddressStartWithZero = Address;

                //设置ABCD
                switch (format)
                {
                    case DataFormat.ABCD:
                        busTcpClient.DataFormat = HslCommunication.Core.DataFormat.ABCD;
                        break;
                    case DataFormat.BADC:
                        busTcpClient.DataFormat = HslCommunication.Core.DataFormat.BADC;
                        break;
                    case DataFormat.CDAB:
                        busTcpClient.DataFormat = HslCommunication.Core.DataFormat.CDAB;
                        break;
                    case DataFormat.DCBA:
                        busTcpClient.DataFormat = HslCommunication.Core.DataFormat.DCBA;
                        break;
                    default:
                        break;
                }

                OperateResult connect = busTcpClient.ConnectServer();

                if (connect.IsSuccess)
                {
                    IsConnected = true;
                    Log.Info(HslCommunication.StringResources.Language.ConnectedSuccess);
                }
                else
                {
                    IsConnected = false;
                    Log.Error(HslCommunication.StringResources.Language.ConnectedFailed + connect.Message);
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                throw ex;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public override void DisConnect()
        {
            // 断开连接
            busTcpClient?.ConnectClose();
            IsConnected = false;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
            if (IsConnected)
            {
                try
                {
                    int length = 0;
                    int start = 0;
                    string read;
                    for (int i = 0; i < VarLength.Count; i++)
                    {
                        switch (VarLength[i])
                        {
                            case VarType.Bit:
                                length = length + 1;
                                break;
                            case VarType.Int16:
                                length = length + 1;
                                break;
                            case VarType.Int32:
                                length = length + 2;
                                break;
                            case VarType.Int64:
                                length = length + 4;
                                break;
                            case VarType.Float:
                                length = length + 2;
                                break;
                            case VarType.Double:
                                length = length + 4;
                                break;
                            default:
                                break;
                        }
                    }

                    //读取数据
                    BulkReadRenderResult(busTcpClient, Address.ToString(), (ushort)length, out read);

                    //清除数据
                    varResult.Clear();

                    if (read.Length != 0)
                    {
                        //获取数据长度
                        for (int i = 0; i < VarLength.Count; i++)
                        {
                            switch (VarLength[i])
                            {
                                case VarType.Bit://bool数值的读取
                                    string str1 = read.Substring(start, 4);
                                    int index1 = Convert.ToInt16(str1, 16);
                                    if (index1 > 0)
                                    {
                                        varResult.Add(1);
                                    }
                                    else
                                    {
                                        varResult.Add(0);
                                    }
                                    start += 4;
                                    break;
                                case VarType.Int16:
                                    string str2 = read.Substring(start, 4);
                                    int index2 = Convert.ToInt16(str2, 16);
                                    varResult.Add(index2);
                                    start += 4;
                                    break;
                                case VarType.Int32:
                                    string str3 = read.Substring(start, 8);
                                    int index3 = Convert.ToInt32(str3, 16);
                                    varResult.Add(index3);
                                    start += 8;
                                    break;
                                case VarType.Int64:
                                    string str4 = read.Substring(start, 8);
                                    long index4 = Convert.ToInt64(str4, 16);
                                    varResult.Add(index4);
                                    start += 8;
                                    break;
                                case VarType.Float:
                                    string hexString = read.Substring(start, 8);
                                    uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);

                                    byte[] floatVals = BitConverter.GetBytes(num);
                                    float f = BitConverter.ToSingle(floatVals, 0);

                                    varResult.Add(f);
                                    start += 8;
                                    break;
                                case VarType.Double:

                                    string hexString1 = read.Substring(start, 8);
                                    uint num1 = uint.Parse(hexString1, System.Globalization.NumberStyles.AllowHexSpecifier);

                                    byte[] floatVals1 = BitConverter.GetBytes(num1);
                                    double f1 = BitConverter.ToDouble(floatVals1, 0);

                                    varResult.Add(f1);
                                    start += 8;

                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        IsConnected = true;
                    }
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        public override void Write()
        {
            if (IsConnected)
            {
                try
                {
                    List<byte> Writelist = new List<byte>();

                    int start = 0;

                    for (int i = 0; i < VarLength.Count; i++)
                    {
                        switch (VarLength[i])
                        {
                            case VarType.Bit:
                                short num;
                                if (Convert.ToBoolean(writeVar[i]))
                                {
                                    num = 1;
                                }
                                else
                                {
                                    num = 0;
                                }
                                //前置位
                                Writelist.Add(0);
                                //实际数字
                                Writelist.Add((byte)num);
                                break;
                            case VarType.Int16:

                                short num1 = short.Parse(writeVar[i].ToString());
                                //前置位
                                Writelist.Add(0);
                                //实际数字
                                Writelist.Add((byte)num1);
                                break;
                            case VarType.Int32:

                                string Int32str = Convert.ToString(Convert.ToInt32(writeVar[i]), 16).ToUpper().PadLeft(8, '0');

                                string Int32String1 = Int32str.Substring(0, 2);
                                string Int32String2 = Int32str.Substring(2, 2);
                                string Int32String3 = Int32str.Substring(4, 2);
                                string Int32String4 = Int32str.Substring(6, 2);

                                int index1 = Convert.ToInt32(Int32String1, 16);
                                int index2 = Convert.ToInt32(Int32String2, 16);
                                int index3 = Convert.ToInt32(Int32String3, 16);
                                int index4 = Convert.ToInt32(Int32String4, 16);

                                Writelist.Add(byte.Parse(index1.ToString()));
                                Writelist.Add(byte.Parse(index2.ToString()));
                                Writelist.Add(byte.Parse(index3.ToString()));
                                Writelist.Add(byte.Parse(index4.ToString()));

                                break;
                            case VarType.Int64:
                                break;
                            case VarType.Float:
                                //浮点转16进制

                                byte[] bytes = BitConverter.GetBytes(float.Parse(writeVar[i].ToString()));

                                Array.Reverse(bytes);

                                Writelist.Add(bytes[0]);
                                Writelist.Add(bytes[1]);
                                Writelist.Add(bytes[2]);
                                Writelist.Add(bytes[3]);

                                break;
                            default:
                                break;
                        }
                    }

                    //10进制转16进制

                    //单独的两位转10进制

                    //10进制添加至byte位

                    byte[] values = Writelist.ToArray();
                    OperateResult write = busTcpClient.WriteRegister(Address.ToString(), values);
                    if (write.IsSuccess)
                    {
                        // success
                    }
                    else
                    {
                        // failed
                        string err = write.Message;
                    }

                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 批量读取数据
        /// </summary>
        /// <param name="readWrite">通讯对象</param>
        /// <param name="addTextBox">起始地址</param>
        /// <param name="lengthTextBox">长度</param>
        /// <param name="resultTextBox">结果</param>
        public void BulkReadRenderResult(HslCommunication.Core.IReadWriteNet readWrite, string addTextBox, ushort lengthTextBox, out string Result)
        {
            Result = string.Empty;
            try
            {
                OperateResult<byte[]> read = readWrite.Read(addTextBox, lengthTextBox);
                if (read.IsSuccess)
                {
                    Result = HslCommunication.BasicFramework.SoftBasic.ByteToHexString(read.Content);
                }
                else
                {
                    Log.Error("Read Failed：" + read.ToMessageShowString());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Read Failed：" + ex.Message);
            }
        }

    }
}
