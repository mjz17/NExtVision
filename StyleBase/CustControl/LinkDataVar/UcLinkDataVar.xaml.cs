using ModuleDataVar;
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
    /// UcLinkDataVar.xaml 的交互逻辑
    /// </summary>
    public partial class UcLinkDataVar : UserControl
    {
        public UcLinkDataVar()
        {
            InitializeComponent();
        }

        #region 列表名称

        //列表名称
        public string CustName
        {
            get { return (string)this.GetValue(CustNameProperty); }
            set { this.SetValue(CustNameProperty, value); }
        }

        public static readonly DependencyProperty CustNameProperty =
            DependencyProperty.Register("CustName", typeof(string), typeof(UcLinkDataVar), new PropertyMetadata(default(string)));

        #endregion

        #region 变量类型（Type）

        public string DataType
        {
            get { return (string)this.GetValue(DataTypeProperty); }
            set { this.SetValue(DataTypeProperty, value); }
        }

        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register("DataType", typeof(string), typeof(UcLinkDataVar), new PropertyMetadata(default(string)));

        #endregion

        #region 模块ID地址

        public string ModuleID
        {
            get { return (string)this.GetValue(ModuleIDProperty); }
            set { this.SetValue(ModuleIDProperty, value); }
        }

        public static readonly DependencyProperty ModuleIDProperty =
            DependencyProperty.Register("ModuleID", typeof(string), typeof(UcLinkDataVar), new PropertyMetadata(default(string)));

        #endregion

        #region 显示链接变量名称

        public string DispVar
        {
            get { return (string)this.GetValue(DispVarProperty); }
            set { this.SetValue(DispVarProperty, value); }
        }

        public static readonly DependencyProperty DispVarProperty =
            DependencyProperty.Register("DispVar", typeof(string), typeof(UcLinkDataVar), new PropertyMetadata(default(string)));

        #endregion

        #region 路由事件

        public static readonly RoutedEvent LinkValueEvent = EventManager.RegisterRoutedEvent("LinkValue",
            RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcLinkDataVar));

        public event RoutedEventHandler LinkValue
        {
            add { AddHandler(LinkValueEvent, value); }
            remove { RemoveHandler(LinkValueEvent, value); }
        }

        #endregion


        //数据变量
        ModuleDataVar.DataVar DataVar;

        //委托方法
        public void Recevie(DataVar value)
        {
            if (value.m_DataValue != null)
            {
                DataVar = value;
                DispVar = value.m_DataTip + "." + value.m_DataName;
                Disp_Name.Content = DispVar;
            }
            else
            {
                DispVar = string.Empty;
                Disp_Name.Content = DispVar;
            }
        }

        //链接数据变量
        private void Link_Btn_Click(object sender, RoutedEventArgs e)
        {
            ShowDataVarFrm showData = new ShowDataVarFrm();
            ShowDataVarViewModel showDataView = new ShowDataVarViewModel(DataType, ModuleID);
            showData.DataContext = showDataView;
            showDataView.sendMessage = Recevie;
            bool? conStatus = showData.ShowDialog();
            if (DispVar != string.Empty && conStatus == true)
            {
                RoutedEventArgs args = new RoutedEventArgs(LinkValueEvent, DataVar);
                this.RaiseEvent(args);
            }
        }

        //取消链接
        private void Cancel_Btn_Click(object sender, RoutedEventArgs e)
        {
            DispVar = string.Empty;
            Disp_Name.Content = DispVar;
            DataVar.m_DataValue = null;
            RoutedEventArgs args = new RoutedEventArgs(LinkValueEvent, DataVar);
            this.RaiseEvent(args);
        }

    }
}
