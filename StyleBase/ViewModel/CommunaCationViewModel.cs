using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using CommunaCation;
using System.Windows.Controls;

namespace StyleBase
{
    /// <summary>
    /// 变量配置
    /// </summary>
    public class CommunaCationViewModel : NoitifyBase
    {

        /// <summary>
        /// 通讯连接集合
        /// </summary>
        public ObservableCollection<ComunaInfo> CommunaCationLst { get; set; } = new ObservableCollection<ComunaInfo>();

        /// <summary>
        /// 界面显示的状态
        /// </summary>
        public List<SysHelper.DeviceInfo> CommunaInfo = new List<SysHelper.DeviceInfo>();

        /// <summary>
        /// 当前通讯连接的对象
        /// </summary>
        public string m_ComunCation { get; set; } = string.Empty;

        /// <summary>
        /// 关闭窗体
        /// </summary>
        public CommandBase CloseThisFrm { get; set; }

        /// <summary>
        /// 新增通讯
        /// </summary>
        public CommandBase AddCommuna { get; set; }

        /// <summary>
        /// 新增TCP客户端
        /// </summary>
        public CommandBase Add_Tcp_Client { get; set; }

        /// <summary>
        /// 新增TCP服务器
        /// </summary>
        public CommandBase Add_Tcp_Server { get; set; }

        /// <summary>
        /// 新增串口通讯
        /// </summary>
        public CommandBase Add_Serialport { get; set; }

        /// <summary>
        /// 新增Udp通讯
        /// </summary>
        public CommandBase Add_Udp { get; set; }

        /// <summary>
        /// 选中通讯栏
        /// </summary>
        public CommandBase Select_Commun { get; set; }

        /// <summary>
        /// 开关通讯
        /// </summary>
        public CommandBase IsCheck_Commun { get; set; }

        /// <summary>
        /// 发送数据
        /// </summary>
        public CommandBase Send_Mess_Commun { get; set; }

        /// <summary>
        /// 删除通讯列表数据
        /// </summary>
        public CommandBase DeleteCom { get; set; }

        /// <summary>
        /// 当前选定流程名称
        /// </summary>
        private string _selectProName;

        public string SelectProName
        {
            get { return _selectProName; }
            set { _selectProName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 发送的数据
        /// </summary>
        private string _sendMeg;

        public string SendMeg
        {
            get { return _sendMeg; }
            set { _sendMeg = value; this.DoNitify(); }
        }

        /// <summary>
        /// 清空发送的数据
        /// </summary>
        public CommandBase EmptySendData { get; set; }

        /// <summary>
        /// 接受的数据
        /// </summary>
        private string _receiveMeg;

        public string ReceiveMeg
        {
            get { return _receiveMeg; }
            set { _receiveMeg = value; this.DoNitify(); }
        }

        /// <summary>
        /// 是否16进制发送
        /// </summary>
        private bool _SendByHex;

        public bool SendByHex
        {
            get { return _SendByHex; }
            set { _SendByHex = value; this.DoNitify(); }
        }

        /// <summary>
        /// 是否16进制发送
        /// </summary>
        public CommandBase SendByHexCom { get; set; }

        /// <summary>
        /// 是否16进制接收
        /// </summary>
        private bool _ReceivedByHex;

        public bool ReceivedByHex
        {
            get { return _ReceivedByHex; }
            set { _ReceivedByHex = value; this.DoNitify(); }
        }

        /// <summary>
        /// 是否16进制接收
        /// </summary>
        public CommandBase ReceivedByHexCom { get; set; }

        /// <summary>
        /// 清空接收的数据
        /// </summary>
        public CommandBase EmptyReceiveData { get; set; }

        public System.Windows.Controls.ContentControl m_Pag { get; set; }

        public CommunaCationViewModel(System.Windows.Controls.ContentControl content)
        {
            InitCommun();
            m_Pag = content;

            this.CloseThisFrm = new CommandBase();
            this.CloseThisFrm.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();
            });
            this.CloseThisFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.AddCommuna = new CommandBase();
            this.AddCommuna.DoExecute = new Action<object>(AddNewProject);
            this.AddCommuna.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Add_Tcp_Client = new CommandBase();
            this.Add_Tcp_Client.DoExecute = new Action<object>(AddTcpClient);
            this.Add_Tcp_Client.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Add_Tcp_Server = new CommandBase();
            this.Add_Tcp_Server.DoExecute = new Action<object>(AddTcpServer);
            this.Add_Tcp_Server.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Add_Serialport = new CommandBase();
            this.Add_Serialport.DoExecute = new Action<object>(AddSerialport);
            this.Add_Serialport.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Add_Udp = new CommandBase();
            this.Add_Udp.DoExecute = new Action<object>(AddUdp);
            this.Add_Udp.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Select_Commun = new CommandBase();
            this.Select_Commun.DoExecute = new Action<object>(SelectCommun);
            this.Select_Commun.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.IsCheck_Commun = new CommandBase();
            this.IsCheck_Commun.DoExecute = new Action<object>(IsCheckCommun);
            this.IsCheck_Commun.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Send_Mess_Commun = new CommandBase();
            this.Send_Mess_Commun.DoExecute = new Action<object>(SendMessCommun);
            this.Send_Mess_Commun.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.EmptyReceiveData = new CommandBase();
            this.EmptyReceiveData.DoExecute = new Action<object>((o) =>
            {
                ReceiveMeg = string.Empty;
            });
            this.EmptyReceiveData.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.EmptySendData = new CommandBase();
            this.EmptySendData.DoExecute = new Action<object>((o) =>
            {
                SendMeg = string.Empty;
            });
            this.EmptySendData.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.DeleteCom = new CommandBase();
            this.DeleteCom.DoExecute = new Action<object>(DeleteCommun);
            this.DeleteCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SendByHexCom = new CommandBase();
            this.SendByHexCom.DoExecute = new Action<object>(SendByHexCommun);
            this.SendByHexCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.ReceivedByHexCom = new CommandBase();
            this.ReceivedByHexCom.DoExecute = new Action<object>(ReceivedByHexCommun);
            this.ReceivedByHexCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 通讯窗体列表
        /// </summary>
        private void InitCommun()
        {
            foreach (var item in EComManageer.s_ECommunacationDic)
            {
                CommunaCationLst.Add(new ComunaInfo { Name = item.Value.Key, isSelect = item.Value.IsConnected });
                item.Value.ReceiveString += ECom_ReceiveString;
            }
        }

        /// <summary>
        /// 添加新的流程
        /// </summary>
        /// <param name="obj"></param>
        private void AddNewProject(object obj)
        {
            System.Windows.Controls.Button stack = (System.Windows.Controls.Button)obj;

            System.Windows.Window targetWindow = System.Windows.Window.GetWindow(stack);

            System.Windows.Controls.ContextMenu cm = targetWindow.FindResource("ContextMenu") as System.Windows.Controls.ContextMenu;

            cm.PlacementTarget = obj as System.Windows.Controls.Button;

            cm.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

            cm.IsOpen = true;
        }

        /// <summary>
        /// 添加服务器
        /// </summary>
        /// <param name="obj"></param>
        private void AddTcpServer(object obj)
        {
            try
            {
                //创建tcp服务端
                m_ComunCation = EComManageer.CreateECom(CommunicationModel.TcpServer);
                ECommunacation ECom = EComManageer.GetECommunacation(m_ComunCation);
                ECom.ReceiveString += ECom_ReceiveString;

                //设置通讯参数
                //不需要设置监听的ip 默认是0.0.0.0 就可以监听所有ip段
                //设置端口
                ECom.LocalPort = 8000;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            ShowFrmEComInfo();
        }

        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="obj"></param>
        private void AddTcpClient(object obj)
        {
            try
            {
                //创建tcp客户端
                m_ComunCation = EComManageer.CreateECom(CommunicationModel.TcpClient);

                ECommunacation ECom = EComManageer.GetECommunacation(m_ComunCation);

                ECom.ReceiveString += ECom_ReceiveString;

                //设置通讯参数
                ECom.RemoteIP = "127.0.0.1";
                ECom.RemotePort = 8001;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            ShowFrmEComInfo();
        }

        /// <summary>
        /// 添加串口通讯
        /// </summary>
        /// <param name="obj"></param>
        private void AddSerialport(object obj)
        {
            try
            {
                //创建串口
                m_ComunCation = EComManageer.CreateECom(CommunicationModel.COM);
                ECommunacation eCommunacation = EComManageer.GetECommunacation(m_ComunCation);
                eCommunacation.ReceiveString += ECom_ReceiveString;

                //设置通讯参数
                eCommunacation.PortName = "COM1";
                eCommunacation.BaudRate = "9600";
                eCommunacation.Parity = "None";
                eCommunacation.DataBits = "8";
                eCommunacation.StopBits = "One";

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            ShowFrmEComInfo();
        }

        /// <summary>
        /// 添加Ddp通讯
        /// </summary>
        /// <param name="obj"></param>
        private void AddUdp(object obj)
        {
            try
            {
                //创建UDP
                m_ComunCation = EComManageer.CreateECom(CommunicationModel.UDP);
                ECommunacation eCommunacation = EComManageer.GetECommunacation(m_ComunCation);
                eCommunacation.ReceiveString += ECom_ReceiveString;

                //设置通讯参数
                eCommunacation.RemoteIP = "127.0.0.1";//
                eCommunacation.RemotePort = 8002;
                eCommunacation.LocalPort = 8003;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            ShowFrmEComInfo();
        }

        /// <summary>
        /// 选中listbox集合
        /// </summary>
        /// <param name="obj"></param>
        private void SelectCommun(object obj)
        {
            try
            {
                System.Collections.IList items = (System.Collections.IList)obj;

                var collection = items.Cast<ComunaInfo>();

                var selectedItems = collection.ToList();

                if (selectedItems.Count == 0)
                {
                    SelectProName = string.Empty;
                    return;
                }

                ComunaInfo info = selectedItems[0];

                SelectProName = info.Name;
                //UpdateEcomIno(info.Name);

                if (SelectProName.Contains("TCP服务端"))
                {
                    CommunTcpServer folderList = new CommunTcpServer(SelectProName);
                    m_Pag.Content = new System.Windows.Controls.Frame()
                    {
                        Content = folderList
                    };
                }
                else if (SelectProName.Contains("TCP客户端"))
                {
                    CommunTcpClient folderList = new CommunTcpClient(SelectProName);
                    m_Pag.Content = new System.Windows.Controls.Frame()
                    {
                        Content = folderList
                    };
                }
                else if (SelectProName.Contains("UDP"))
                {
                    CommunUdpClient folderList = new CommunUdpClient(SelectProName);
                    m_Pag.Content = new System.Windows.Controls.Frame()
                    {
                        Content = folderList
                    };
                }
                else if (SelectProName.Contains("串口"))
                {
                    CommunCom folderList = new CommunCom(SelectProName);
                    m_Pag.Content = new System.Windows.Controls.Frame()
                    {
                        Content = folderList
                    };
                }

                //SendByHex

                SendByHex = EComManageer.s_ECommunacationDic[SelectProName].IsSendByHex == true ? true : false;

                //ReceivedByHex

                ReceivedByHex = EComManageer.s_ECommunacationDic[SelectProName].IsReceivedByHex == true ? true : false;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessCommun(object obj)
        {
            try
            {
                if (SelectProName == null)
                {
                    System.Windows.Forms.MessageBox.Show("选择端口！");
                    return;
                }
                EComManageer.SendStr(SelectProName, SendMeg);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private void IsCheckCommun(object obj)
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }

        /// <summary>
        /// 显示通讯列表数据
        /// </summary>
        private void ShowFrmEComInfo()
        {
            CommunaCationLst.Clear();
            CommunaInfo.Clear();
            foreach (var item in EComManageer.s_ECommunacationDic)
            {
                CommunaCationLst.Add(new ComunaInfo { Name = item.Value.Key, isSelect = item.Value.IsConnected });
                CommunaInfo.Add(new SysHelper.DeviceInfo
                {
                    DeviceName = item.Value.Key,
                    IsConnected = item.Value.IsConnected,
                    IconImage = "\xe62c",
                    ImageColor = item.Value.IsConnected ? "#00CB9A" : "#C21B05"
                });
            }

            //更新通讯界面
            SysHelper.DataEventChange.DeviceFrmStatus = true;
        }

        /// <summary>
        /// 接收到数据后的回调事件
        /// </summary>
        /// <param name="str"></param>
        private void ECom_ReceiveString(string str)
        {
            EComManageer.SendStr(m_ComunCation, "这是返回数据" + str);
            ReceiveMeg = ReceiveMeg + m_ComunCation + "：" + str + "\n";
        }

        /// <summary>
        /// 菜单列表删除Lst通讯数据
        /// </summary>
        /// <param name="obj"></param>
        private void DeleteCommun(object obj)
        {
            try
            {
                EComManageer.DeleteECom(SelectProName);
                ShowFrmEComInfo();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 选中是否16进制发送
        /// </summary>
        /// <param name="obj"></param>
        private void SendByHexCommun(object obj)
        {
            CheckBox chk = (CheckBox)obj;
            if (SelectProName != null)
            {
                EComManageer.s_ECommunacationDic[SelectProName].IsSendByHex = chk.IsChecked == true ? true : false;
            }
        }

        /// <summary>
        /// 选中是否16进制接收
        /// </summary>
        /// <param name="obj"></param>
        private void ReceivedByHexCommun(object obj)
        {
            CheckBox chk = (CheckBox)obj;
            if (SelectProName != null)
            {
                EComManageer.s_ECommunacationDic[SelectProName].IsReceivedByHex = chk.IsChecked == true ? true : false;
            }
        }

    }

    public class ComunaInfo
    {
        public string Name { get; set; }
        public bool isSelect { get; set; }

    }

}
