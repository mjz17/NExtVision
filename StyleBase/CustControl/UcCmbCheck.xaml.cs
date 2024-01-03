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
    /// UcCmbCheck.xaml 的交互逻辑
    /// </summary>
    public partial class UcCmbCheck : UserControl
    {
        public UcCmbCheck()
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
            DependencyProperty.Register("HeadName", typeof(string), typeof(UcCmbCheck), new PropertyMetadata(default(string)));

        #endregion

        #region 列表名称

        //列表名称
        public List<string> CmbSourse
        {
            get { return (List<string>)this.GetValue(CmbItemProperty); }
            set { this.SetValue(CmbItemProperty, value); }
        }

        public static readonly DependencyProperty CmbItemProperty =
            DependencyProperty.Register("CmbSourse", typeof(List<string>), typeof(UcCmbCheck), new PropertyMetadata(default(List<string>)));

        #endregion

        #region 选中项

        //列表名称
        public string CmbSelectItem
        {
            get { return (string)this.GetValue(CmbSelectItemProperty); }
            set { this.SetValue(CmbSelectItemProperty, value); }
        }

        public static readonly DependencyProperty CmbSelectItemProperty =
            DependencyProperty.Register("CmbSelectItem", typeof(string), typeof(UcCmbCheck), new PropertyMetadata(default(string)));

        #endregion

        #region cmb选择路由事件

        public static readonly RoutedEvent CmbEvent = EventManager.RegisterRoutedEvent("CmbSelectEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcCmbCheck));

        public event RoutedEventHandler CmbSelectEvent
        {
            add { AddHandler(CmbEvent, value); }
            remove { RemoveHandler(CmbEvent, value); }
        }

        #endregion

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox  cmb = (ComboBox)sender;

            RoutedEventArgs args = new RoutedEventArgs(CmbEvent, cmb.SelectedValue);

            this.RaiseEvent(args);
        }
    }
}
