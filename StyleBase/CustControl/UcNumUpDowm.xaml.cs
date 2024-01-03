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
    /// UcNumUpDowm.xaml 的交互逻辑
    /// </summary>
    public partial class UcNumUpDowm : UserControl
    {
        public UcNumUpDowm()
        {
            InitializeComponent();
        }

        #region 列表名称

        //列表名称
        public string numHeadName
        {
            get { return (string)this.GetValue(numNameProperty); }
            set { this.SetValue(numNameProperty, value); }
        }

        public static readonly DependencyProperty numNameProperty =
            DependencyProperty.Register("numHeadName", typeof(string), typeof(UcNumUpDowm), new PropertyMetadata(default(string)));

        #endregion

        #region 最小值

        //列表名称
        public double minSet
        {
            get { return (int)this.GetValue(minSetProperty); }
            set { this.SetValue(minSetProperty, value); }
        }

        public static readonly DependencyProperty minSetProperty =
            DependencyProperty.Register("minSet", typeof(double), typeof(UcNumUpDowm), new PropertyMetadata(default(double)));

        #endregion

        #region 最大值

        //列表名称
        public double maxSet
        {
            get { return (int)this.GetValue(maxSetProperty); }
            set { this.SetValue(maxSetProperty, value); }
        }

        public static readonly DependencyProperty maxSetProperty =
            DependencyProperty.Register("maxSet", typeof(double), typeof(UcNumUpDowm), new PropertyMetadata(default(double)));

        #endregion

        #region 滑动间距

        //列表名称
        public double smallSet
        {
            get { return (double)this.GetValue(smallSetProperty); }
            set { this.SetValue(smallSetProperty, value); }
        }

        public static readonly DependencyProperty smallSetProperty =
            DependencyProperty.Register("smallSet", typeof(double), typeof(UcNumUpDowm), new PropertyMetadata(default(double)));

        #endregion

        #region 滑动栏值

        /// <summary>
        /// 滑动栏的Value值
        /// </summary>
        public double SliderValue
        {
            get { return (double)GetValue(SliderValueContent); }
            set { SetValue(SliderValueContent, value); }
        }

        public static readonly DependencyProperty SliderValueContent =
           DependencyProperty.Register("SliderValue", typeof(double), typeof(UcNumUpDowm), new PropertyMetadata(default(double)));

        #endregion

        #region 路由事件

        public static readonly RoutedEvent ValueEvent = EventManager.RegisterRoutedEvent("SoliderValueEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcNumUpDowm));

        public event RoutedEventHandler SoliderValueEvent
        {
            add { AddHandler(ValueEvent, value); }
            remove { RemoveHandler(ValueEvent, value); }
        }

        #endregion

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ValueEvent, SliderValue);
            this.RaiseEvent(args);
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (SliderValue >= slider.Maximum)
                return;
            SliderValue=SliderValue+ smallSet;
        }

        private void btn_Reduce_Click(object sender, RoutedEventArgs e)
        {
            if (SliderValue <= slider.Minimum)
                return;
            SliderValue = SliderValue - smallSet;
        }
    
    }
}
