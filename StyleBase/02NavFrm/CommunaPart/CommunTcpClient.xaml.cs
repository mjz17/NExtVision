using CommunaCation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StyleBase
{
    /// <summary>
    /// CommunTcpClient.xaml 的交互逻辑
    /// </summary>
    public partial class CommunTcpClient : UserControl
    {

        ECommunacation m_TcpClient;//对应的通讯集数据

        public CommunTcpClient(string ComName)
        {
            InitializeComponent();
            this.DataContext = this;
            m_TcpClient = EComManageer.s_ECommunacationDic.FirstOrDefault(c => c.Key == ComName).Value;
            TcpClientIp = m_TcpClient.RemoteIP;//目标IP
            TcpClientRemotePort = m_TcpClient.RemotePort.ToString();//目标端口
            this.txt_reMarks.Text = m_TcpClient.Remarks;//备注
        }

        #region 目标IP

        public string TcpClientIp
        {
            get { return (string)this.GetValue(TcpClientIpProperty); }
            set { this.SetValue(TcpClientIpProperty, value); }
        }

        public static readonly DependencyProperty TcpClientIpProperty =
            DependencyProperty.Register("TcpClientIp", typeof(string), typeof(CommunTcpClient), new PropertyMetadata(default(string)));

        #endregion

        #region 目标端口

        public string TcpClientRemotePort
        {
            get { return (string)this.GetValue(TcpClientRemotePortProperty); }
            set { this.SetValue(TcpClientRemotePortProperty, value); }
        }

        public static readonly DependencyProperty TcpClientRemotePortProperty =
            DependencyProperty.Register("TcpClientRemotePort", typeof(string), typeof(CommunTcpClient), new PropertyMetadata(default(string)));

        #endregion

        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            m_TcpClient.RemotePort = Convert.ToInt32(TcpClientRemotePort);
        }

        private void UcOutPutInfo_TxtValueEvent(object sender, RoutedEventArgs e)
        {
            m_TcpClient.RemoteIP = TcpClientIp;
        }

        private void txt_reMarks_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_TcpClient.Remarks = this.txt_reMarks.Text;
        }
    }
}
