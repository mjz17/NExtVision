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
    /// TitleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class TitleFrm : UserControl
    {
        public TitleFrm()
        {
            InitializeComponent();
        }

        #region 列表名称

        public string HeadName
        {
            get { return (string)this.GetValue(HeaderNameProperty); }
            set { this.SetValue(HeaderNameProperty, value); }
        }

        public static readonly DependencyProperty HeaderNameProperty =
            DependencyProperty.Register("HeadName", typeof(string), typeof(TitleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 最小化按钮是否隐藏

        public Visibility MinVis
        {
            get { return (Visibility)this.GetValue(MinVisProperty); }
            set { this.SetValue(MinVisProperty, value); }
        }

        public static readonly DependencyProperty MinVisProperty =
            DependencyProperty.Register("MinVis", typeof(Visibility), typeof(TitleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

        #region 最大化按钮是否隐藏

        public Visibility MaxVis
        {
            get { return (Visibility)this.GetValue(MaxVisProperty); }
            set { this.SetValue(MaxVisProperty, value); }
        }

        public static readonly DependencyProperty MaxVisProperty =
            DependencyProperty.Register("MaxVis", typeof(Visibility), typeof(TitleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

        #region 退出按钮是否隐藏

        public Visibility CloseVis
        {
            get { return (Visibility)this.GetValue(CloseVisProperty); }
            set { this.SetValue(CloseVisProperty, value); }
        }

        public static readonly DependencyProperty CloseVisProperty =
            DependencyProperty.Register("CloseVis", typeof(Visibility), typeof(TitleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

        #region 关闭按钮的路由事件

        public static readonly RoutedEvent CloseWindowEvent = EventManager.RegisterRoutedEvent("CloseWindow",
            RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(TitleFrm));

        public event RoutedEventHandler CloseWindow
        {
            add { AddHandler(CloseWindowEvent, value); }
            remove { RemoveHandler(CloseWindowEvent, value); }
        }

        #endregion

        private void btn_Min_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void btn_Max_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = Window.GetWindow(this).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window window = Window.GetWindow(this);
            RoutedEventArgs args = new RoutedEventArgs(CloseWindowEvent, window);
            this.RaiseEvent(args);
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //按下拖动
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this).DragMove();
            }
            // 双击放大
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this).WindowState = Window.GetWindow(this).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

    }
}
