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
    /// CommunUdpClient.xaml 的交互逻辑
    /// </summary>
    public partial class CommunUdpClient : UserControl
    {

        ECommunacation m_Udp;//对应的通讯集数据

        public CommunUdpClient(string ComName)
        {
            InitializeComponent();
            this.DataContext = this;
            m_Udp = EComManageer.s_ECommunacationDic.FirstOrDefault(c => c.Key == ComName).Value;
            TcpClientIp = m_Udp.RemoteIP;//IP
            LocalPort = m_Udp.LocalPort.ToString();//本地端口
            RemotePort = m_Udp.RemotePort.ToString();//远程端口
            this.txt_reMarks.Text = m_Udp.Remarks;//备注
        }

        #region IP

        public string TcpClientIp
        {
            get { return (string)this.GetValue(TcpClientIpProperty); }
            set { this.SetValue(TcpClientIpProperty, value); }
        }

        public static readonly DependencyProperty TcpClientIpProperty =
            DependencyProperty.Register("TcpClientIp", typeof(string), typeof(CommunUdpClient), new PropertyMetadata(default(string)));

        #endregion

        #region 本地端口

        public string LocalPort
        {
            get { return (string)this.GetValue(LocalPortPortProperty); }
            set { this.SetValue(LocalPortPortProperty, value); }
        }

        public static readonly DependencyProperty LocalPortPortProperty =
            DependencyProperty.Register("LocalPort", typeof(string), typeof(CommunUdpClient), new PropertyMetadata(default(string)));

        #endregion

        #region 远程端口

        public string RemotePort
        {
            get { return (string)this.GetValue(RemotePortPortProperty); }
            set { this.SetValue(RemotePortPortProperty, value); }
        }

        public static readonly DependencyProperty RemotePortPortProperty =
            DependencyProperty.Register("RemotePort", typeof(string), typeof(CommunUdpClient), new PropertyMetadata(default(string)));

        #endregion

        //本地端口
        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            m_Udp.LocalPort = Convert.ToInt32(LocalPort);
        }

        //远程端口
        private void UcTxtAddandSub_TextValueEvent_1(object sender, RoutedEventArgs e)
        {
            m_Udp.RemotePort = Convert.ToInt32(RemotePort);
        }

        private void txt_reMarks_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_Udp.Remarks = this.txt_reMarks.Text;
        }
    }
}
