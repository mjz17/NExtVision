using CommunaCation;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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
    /// CommunCom.xaml 的交互逻辑
    /// </summary>
    public partial class CommunCom : UserControl
    {

        ECommunacation m_Com;//对应的通讯集数据
        public CommunCom(string ComName)
        {
            InitializeComponent();
            m_Com = EComManageer.s_ECommunacationDic.FirstOrDefault(c => c.Key == ComName).Value;
            InitPortName();//串口号
            InitBaudRate();//波特率
            InitParity();//校验位
            InitDataBits();//数据位
            InitStopBits();//停止位
            this.txt_reMarks.Text = m_Com.Remarks;//备注
        }

        #region 初始化串口参数

        /// <summary>
        /// 串口号
        /// </summary>
        private void InitPortName()
        {
            cmb_PortName.ItemsSource = SerialPort.GetPortNames();
            cmb_PortName.Text = m_Com.PortName;
        }

        /// <summary>
        /// 波特率
        /// </summary>
        private void InitBaudRate()
        {
            List<string> Info = new List<string>()
            {
                "2400",
                "4800",
                "9600",
                "19200",
                "38400",
                "57600",
                "115200",
            };
            cmb_BaudRate.ItemsSource = Info;
            cmb_BaudRate.Text = m_Com.BaudRate;
        }

        /// <summary>
        /// 校验位
        /// </summary>
        private void InitParity()
        {
            cmb_Parity.ItemsSource = Enum.GetNames(typeof(Parity));
            cmb_Parity.Text = m_Com.Parity;
        }

        /// <summary>
        /// 数据位
        /// </summary>
        private void InitDataBits()
        {
            List<string> Info = new List<string>()
            {
                "5",
                "6",
                "7",
                "8",
            };
            cmb_DataBits.ItemsSource = Info;
            cmb_DataBits.Text = m_Com.DataBits;
        }

        /// <summary>
        /// 停止位
        /// </summary>
        private void InitStopBits()
        {
            cmb_StopBits.ItemsSource = Enum.GetNames(typeof(StopBits)); 
            cmb_StopBits.Text = m_Com.StopBits;
        }

        #endregion

        private void cmb_PortName_DropDownClosed(object sender, EventArgs e)
        {
            m_Com.PortName = this.cmb_PortName.Text;
        }

        private void cmb_BaudRate_DropDownClosed(object sender, EventArgs e)
        {
            m_Com.BaudRate = this.cmb_BaudRate.Text;
        }

        private void cmb_Parity_DropDownClosed(object sender, EventArgs e)
        {
            m_Com.Parity = this.cmb_Parity.Text;
        }

        private void cmb_DataBits_DropDownClosed(object sender, EventArgs e)
        {
            m_Com.DataBits = this.cmb_DataBits.Text;
        }

        private void cmb_StopBits_DropDownClosed(object sender, EventArgs e)
        {
            m_Com.StopBits = this.cmb_StopBits.Text;
        }

        private void txt_reMarks_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_Com.Remarks = this.txt_reMarks.Text;
        }
    }
}
