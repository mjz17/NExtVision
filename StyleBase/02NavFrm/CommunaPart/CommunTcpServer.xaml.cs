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
    /// CommunTcpServer.xaml 的交互逻辑
    /// </summary>
    public partial class CommunTcpServer : UserControl
    {

        ECommunacation m_TcpServer;//对应的通讯集数据

        public CommunTcpServer(string ComName)
        {
            InitializeComponent();
            this.DataContext = this;
            m_TcpServer = EComManageer.s_ECommunacationDic.FirstOrDefault(c => c.Key == ComName).Value;
            LocalPort = m_TcpServer.LocalPort.ToString();//本地端口
            this.txt_reMarks.Text = m_TcpServer.Remarks;//备注
        }

        #region 本地端口

        public string LocalPort
        {
            get { return (string)this.GetValue(LocalPortPortProperty); }
            set { this.SetValue(LocalPortPortProperty, value); }
        }

        public static readonly DependencyProperty LocalPortPortProperty =
            DependencyProperty.Register("LocalPort", typeof(string), typeof(CommunTcpServer), new PropertyMetadata(default(string)));

        #endregion

        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            m_TcpServer.LocalPort = Convert.ToInt32(LocalPort);
        }
        private void txt_reMarks_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_TcpServer.Remarks = this.txt_reMarks.Text;
        }
    }
}
