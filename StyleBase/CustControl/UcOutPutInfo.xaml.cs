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
    /// UcOutPutInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UcOutPutInfo : UserControl
    {
        public UcOutPutInfo()
        {
            InitializeComponent();
        }

        #region 列表名称

        //列表名称
        public string HeadName
        {
            get { return (string)this.GetValue(HeadNameNameProperty); }
            set { this.SetValue(HeadNameNameProperty, value); }
        }

        public static readonly DependencyProperty HeadNameNameProperty =
            DependencyProperty.Register("HeadName", typeof(string), typeof(UcOutPutInfo), new PropertyMetadata(default(string)));

        #endregion

        #region 显示内容

        //列表名称
        public string DispName
        {
            get { return (string)this.GetValue(DispNameProperty); }
            set { this.SetValue(DispNameProperty, value); }
        }

        public static readonly DependencyProperty DispNameProperty =
            DependencyProperty.Register("DispName", typeof(string), typeof(UcOutPutInfo), new PropertyMetadata(default(string)));

        #endregion

        #region 是否只读

        //列表名称
        public bool ReadOnly
        {
            get { return (bool)this.GetValue(ReadOnlyProperty); }
            set { this.SetValue(ReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(UcOutPutInfo), new PropertyMetadata(default(bool)));

        #endregion

        #region 路由事件

        public static readonly RoutedEvent ValueEvent = EventManager.RegisterRoutedEvent("TxtValueEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcOutPutInfo));

        public event RoutedEventHandler TxtValueEvent
        {
            add { AddHandler(ValueEvent, value); }
            remove { RemoveHandler(ValueEvent, value); }
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ValueEvent, DispName);
            this.RaiseEvent(args);
        }
    }
}
