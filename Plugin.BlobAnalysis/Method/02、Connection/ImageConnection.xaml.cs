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

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// ImageConnection.xaml 的交互逻辑
    /// </summary>
    public partial class ImageConnection : UserControl
    {

        public ConnectionImage Connectimage;

        public ImageConnection(BaseMethod method)
        {
            InitializeComponent();
            this.DataContext = this;
            Connectimage = (ConnectionImage)method;
            this.Loaded += ImageConnection_Loaded;
            this.Cmb_SelectIndex.DropDownClosed += ComboBox_DropDownClosed;
        }

        private void ImageConnection_Loaded(object sender, RoutedEventArgs e)
        {
            //置于最前端
            if (Connectimage.m_Index == 0)
            {
                //控件隐藏
                m_Control = Visibility.Hidden;
            }
            else
            {
                //控件隐藏
                m_Control = Visibility.Visible;
                //数据写入
                Cmb_SelectIndex.ItemsSource = Connectimage.InitCmb();
                //当前的选择为前面一个
                this.Cmb_SelectIndex.SelectedIndex = Connectimage.m_LinkIndex == "上一个区域" ? 0 : Connectimage.m_Index - 1;
            }
        }

        #region 索引状态

        public Visibility m_Control
        {
            get { return (Visibility)this.GetValue(m_ControlProperty); }
            set { this.SetValue(m_ControlProperty, value); }
        }

        public static readonly DependencyProperty m_ControlProperty =
            DependencyProperty.Register("m_Control", typeof(Visibility), typeof(ImageConnection), new PropertyMetadata(default(Visibility)));

        #endregion

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            Connectimage.m_LinkIndex = this.Cmb_SelectIndex.Text;
        }
    }
}
