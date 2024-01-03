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

namespace Plugin.ImageAreaReg
{
    /// <summary>
    /// CameraMode.xaml 的交互逻辑
    /// </summary>
    public partial class CameraMode : UserControl
    {
        public CameraMode()
        {
            InitializeComponent();
        }

        #region 指定图片

        //列表名称
        public bool ImageCheck
        {
            get { return (bool)this.GetValue(ImageCheckProperty); }
            set { this.SetValue(ImageCheckProperty, value); }
        }

        public static readonly DependencyProperty ImageCheckProperty =
            DependencyProperty.Register("ImageCheck", typeof(bool), typeof(CameraMode), new PropertyMetadata(default(bool)));

        #endregion

        #region 文件目录

        //指定图片
        public bool FileCheck
        {
            get { return (bool)this.GetValue(FileCheckProperty); }
            set { this.SetValue(FileCheckProperty, value); }
        }

        public static readonly DependencyProperty FileCheckProperty =
            DependencyProperty.Register("FileCheck", typeof(bool), typeof(CameraMode), new PropertyMetadata(default(bool)));

        #endregion

        #region 指定相机

        //指定相机
        public bool CameraCheck
        {
            get { return (bool)this.GetValue(CameraCheckProperty); }
            set { this.SetValue(CameraCheckProperty, value); }
        }

        public static readonly DependencyProperty CameraCheckProperty =
            DependencyProperty.Register("CameraCheck", typeof(bool), typeof(CameraMode), new PropertyMetadata(default(bool)));

        #endregion

        #region 相机采集模式

        public static readonly RoutedEvent CameraSelectEvent = EventManager.RegisterRoutedEvent("CameraModelEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(CameraMode));

        public event RoutedEventHandler CameraModelEvent
        {
            add { AddHandler(CameraSelectEvent, value); }
            remove { RemoveHandler(CameraSelectEvent, value); }
        }

        #endregion

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            string select = radio.Content.ToString();

            RoutedEventArgs args = new RoutedEventArgs(CameraSelectEvent, select);

            this.RaiseEvent(args);
        }

    }
}
