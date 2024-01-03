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
    /// UcRadDoubleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class UcRadDoubleFrm : UserControl
    {
        public UcRadDoubleFrm()
        {
            InitializeComponent();
        }

        #region 左侧序列名称

        //列表名称
        public string Leftside
        {
            get { return (string)this.GetValue(LeftsideProperty); }
            set { this.SetValue(LeftsideProperty, value); }
        }

        public static readonly DependencyProperty LeftsideProperty =
            DependencyProperty.Register("Leftside", typeof(string), typeof(UcRadDoubleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 左侧序列是否选中

        //列表名称
        public bool LeftCheck
        {
            get { return (bool)this.GetValue(LeftCheckProperty); }
            set { this.SetValue(LeftCheckProperty, value); }
        }

        public static readonly DependencyProperty LeftCheckProperty =
            DependencyProperty.Register("LeftCheck", typeof(bool), typeof(UcRadDoubleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 右侧序列名称

        //列表名称
        public string Rightside
        {
            get { return (string)this.GetValue(RightsideProperty); }
            set { this.SetValue(RightsideProperty, value); }
        }

        public static readonly DependencyProperty RightsideProperty =
            DependencyProperty.Register("Rightside", typeof(string), typeof(UcRadDoubleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 右侧序列是否选中

        //列表名称
        public bool RightCheck
        {
            get { return (bool)this.GetValue(RightCheckProperty); }
            set { this.SetValue(RightCheckProperty, value); }
        }

        public static readonly DependencyProperty RightCheckProperty =
            DependencyProperty.Register("RightCheck", typeof(bool), typeof(UcRadDoubleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 模式选中

        public static readonly RoutedEvent ModelEvent = EventManager.RegisterRoutedEvent("ModelSelectEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcRadDoubleFrm));

        public event RoutedEventHandler ModelSelectEvent
        {
            add { AddHandler(ModelEvent, value); }
            remove { RemoveHandler(ModelEvent, value); }
        }

        #endregion

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            string select = radio.Content.ToString();

            RoutedEventArgs args = new RoutedEventArgs(ModelEvent, select);

            this.RaiseEvent(args);
        }
    }
}
