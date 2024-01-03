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
    /// ImageFill_up.xaml 的交互逻辑
    /// </summary>
    public partial class ImageFill_up : UserControl
    {

        public Fill_upImage fill_UpImage;

        public ImageFill_up(BaseMethod method)
        {
            InitializeComponent();
            this.DataContext = this;
            fill_UpImage = (Fill_upImage)method;
            this.Loaded += ImageFill_up_Loaded;
            this.Cmb_SelectIndex.DropDownClosed += ComboBox_DropDownClosed;
        }

        private void ImageFill_up_Loaded(object sender, RoutedEventArgs e)
        {
            //置于最前端
            if (fill_UpImage.m_Index == 0)
            {
                //控件隐藏
                m_Control = Visibility.Hidden;
            }
            else
            {
                //控件隐藏
                m_Control = Visibility.Visible;
                //数据写入
                Cmb_SelectIndex.ItemsSource = fill_UpImage.InitCmb();
                //当前的选择为前面一个
                this.Cmb_SelectIndex.SelectedIndex = fill_UpImage.m_LinkIndex == "上一个区域" ? 0 : fill_UpImage.m_Index - 1;

            }
        }

        #region 索引状态

        public Visibility m_Control
        {
            get { return (Visibility)this.GetValue(m_ControlProperty); }
            set { this.SetValue(m_ControlProperty, value); }
        }

        public static readonly DependencyProperty m_ControlProperty =
            DependencyProperty.Register("m_Control", typeof(Visibility), typeof(ImageFill_up), new PropertyMetadata(default(Visibility)));

        #endregion

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            fill_UpImage.m_LinkIndex = this.Cmb_SelectIndex.Text;
        }

    }
}
