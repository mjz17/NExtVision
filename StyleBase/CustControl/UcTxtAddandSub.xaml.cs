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
    /// UcTxtAddandSub.xaml 的交互逻辑
    /// </summary>
    public partial class UcTxtAddandSub : UserControl
    {
        public UcTxtAddandSub()
        {
            InitializeComponent();
            Timer1.Tick += Timer1_Tick;
            Timer2.Tick += Timer2_Tick;
            Timer3.Tick += Timer3_Tick;
            Timer4.Tick += Timer4_Tick;

            btn_Add.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.btn_Add_MouseLeftButtonDown), true);
            btn_Add.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.btn_Add_MouseLeftButtonUp), true);
            btn_Reduce.AddHandler(Button.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.btn_Reduce_MouseLeftButtonDown), true);
            btn_Reduce.AddHandler(Button.MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.btn_Reduce_MouseLeftButtonUp), true);

        }

        System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
        System.Windows.Forms.Timer Timer2 = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
        System.Windows.Forms.Timer Timer3 = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };
        System.Windows.Forms.Timer Timer4 = new System.Windows.Forms.Timer() { Interval = 1000, Enabled = false };

        #region 列表名称

        //列表名称
        public string numHeadName
        {
            get { return (string)this.GetValue(numNameProperty); }
            set { this.SetValue(numNameProperty, value); }
        }

        public static readonly DependencyProperty numNameProperty =
            DependencyProperty.Register("numHeadName", typeof(string), typeof(UcTxtAddandSub), new PropertyMetadata(default(string)));

        #endregion

        #region 最小值

        public double minValue
        {
            get { return (double)this.GetValue(minValueProperty); }
            set { this.SetValue(minValueProperty, value); }
        }

        public static readonly DependencyProperty minValueProperty =
            DependencyProperty.Register("minValue", typeof(double), typeof(UcTxtAddandSub), new PropertyMetadata(default(double)));

        #endregion

        #region 最大值

        public double maxValue
        {
            get { return (double)this.GetValue(maxValueProperty); }
            set { this.SetValue(maxValueProperty, value); }
        }

        public static readonly DependencyProperty maxValueProperty =
            DependencyProperty.Register("maxValue", typeof(double), typeof(UcTxtAddandSub), new PropertyMetadata(default(double)));

        #endregion

        #region 区间值

        public double midValue
        {
            get { return (double)this.GetValue(midValueProperty); }
            set { this.SetValue(midValueProperty, value); }
        }

        public static readonly DependencyProperty midValueProperty =
            DependencyProperty.Register("midValue", typeof(double), typeof(UcTxtAddandSub), new PropertyMetadata(default(double)));

        #endregion

        #region 控件值

        /// <summary>
        /// 控件值
        /// </summary>
        public double ControlValue
        {
            get { return (double)GetValue(ControlValueContent); }
            set { SetValue(ControlValueContent, value); }
        }

        public static readonly DependencyProperty ControlValueContent =
           DependencyProperty.Register("ControlValue", typeof(double), typeof(UcTxtAddandSub), new PropertyMetadata(default(double)));

        #endregion

        #region 路由事件

        public static readonly RoutedEvent ValueEvent = EventManager.RegisterRoutedEvent("TextValueEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(UcTxtAddandSub));

        public event RoutedEventHandler TextValueEvent
        {
            add { AddHandler(ValueEvent, value); }
            remove { RemoveHandler(ValueEvent, value); }
        }

        #endregion

        private void btn_Add_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ControlValue >= maxValue)
            {
                ControlValue = maxValue;
            }
            else
            {
                ControlValue = ControlValue + midValue;
            }

            Timer1.Interval = 2000;
            Timer1.Enabled = true;
        }

        private void btn_Add_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Timer1.Enabled = false;
            Timer2.Enabled = false;
        }

        private void btn_Reduce_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ControlValue <= minValue)
            {
                ControlValue = minValue;
            }
            else
            {
                ControlValue = ControlValue - midValue;
            }
            Timer3.Interval = 2000;
            Timer3.Enabled = true;
        }

        private void btn_Reduce_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Timer3.Enabled = false;
            Timer4.Enabled = false;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Timer2.Interval = 50;
            Timer2.Enabled = true;
            Timer1.Enabled = false;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (ControlValue >= maxValue)
            {
                ControlValue = maxValue;
            }
            else
            {
                ControlValue = ControlValue + midValue;
            }
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            Timer4.Interval = 50;
            Timer4.Enabled = true;
            Timer3.Enabled = false;
        }

        private void Timer4_Tick(object sender, EventArgs e)
        {
            if (ControlValue <= minValue)
            {
                ControlValue = minValue;
            }
            else
            {
                ControlValue = ControlValue - midValue;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ValueEvent, ControlValue);
            this.RaiseEvent(args);
        }

    }
}
