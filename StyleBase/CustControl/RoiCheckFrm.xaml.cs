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
    /// RoiCheckFrm.xaml 的交互逻辑
    /// </summary>
    public partial class RoiCheckFrm : UserControl
    {
        public RoiCheckFrm()
        {
            InitializeComponent();
        }

        #region 矩形1

        //列表名称
        public string Rect1
        {
            get { return (string)this.GetValue(Rect1Property); }
            set { this.SetValue(Rect1Property, value); }
        }

        public static readonly DependencyProperty Rect1Property =
            DependencyProperty.Register("Rect1", typeof(string), typeof(RoiCheckFrm), new PropertyMetadata(default(string)));

        //列表名称
        public bool Rect1Check
        {
            get { return (bool)this.GetValue(Rect1CheckProperty); }
            set { this.SetValue(Rect1CheckProperty, value); }
        }

        public static readonly DependencyProperty Rect1CheckProperty =
            DependencyProperty.Register("Rect1Check", typeof(bool), typeof(RoiCheckFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 矩形2

        //列表名称
        public string Rect2
        {
            get { return (string)this.GetValue(Rect2Property); }
            set { this.SetValue(Rect2Property, value); }
        }

        public static readonly DependencyProperty Rect2Property =
            DependencyProperty.Register("Rect2", typeof(string), typeof(RoiCheckFrm), new PropertyMetadata(default(string)));

        //列表名称
        public bool Rect2Check
        {
            get { return (bool)this.GetValue(Rect2CheckProperty); }
            set { this.SetValue(Rect2CheckProperty, value); }
        }

        public static readonly DependencyProperty Rect2CheckProperty =
            DependencyProperty.Register("Rect2Check", typeof(bool), typeof(RoiCheckFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 圆形

        //列表名称
        public string Circle
        {
            get { return (string)this.GetValue(CircleProperty); }
            set { this.SetValue(CircleProperty, value); }
        }

        public static readonly DependencyProperty CircleProperty =
            DependencyProperty.Register("Circle", typeof(string), typeof(RoiCheckFrm), new PropertyMetadata(default(string)));

        //列表名称
        public bool CircleCheck
        {
            get { return (bool)this.GetValue(CircleCheckProperty); }
            set { this.SetValue(CircleCheckProperty, value); }
        }

        public static readonly DependencyProperty CircleCheckProperty =
            DependencyProperty.Register("CircleCheck", typeof(bool), typeof(RoiCheckFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 模式选中

        public static readonly RoutedEvent RoiEvent = EventManager.RegisterRoutedEvent("RoiSelectEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(RoiCheckFrm));

        public event RoutedEventHandler RoiSelectEvent
        {
            add { AddHandler(RoiEvent, value); }
            remove { RemoveHandler(RoiEvent, value); }
        }

        #endregion

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            string select = radio.Content.ToString();

            RoutedEventArgs args = new RoutedEventArgs(RoiEvent, select);

            this.RaiseEvent(args);
        }

    }
}
