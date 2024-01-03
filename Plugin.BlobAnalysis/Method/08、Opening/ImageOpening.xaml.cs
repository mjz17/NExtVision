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
using static VisionCore.VBA_Function;

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// ImageOpening.xaml 的交互逻辑
    /// </summary>
    public partial class ImageOpening : UserControl
    {

        //对应的后台程序
        public OpeningImage m_OpeningImage;

        public ImageOpening(BaseMethod method)
        {
            InitializeComponent();
            m_OpeningImage = (OpeningImage)method;
            this.DataContext = this;
            cmb_Model.DropDownClosed += Cmb_Model_DropDownClosed;
            this.Cmb_SelectIndex.DropDownClosed += ComboBox_DropDownClosed;
            this.Loaded += ImageOpening_Loaded;
        }

        private void ImageOpening_Loaded(object sender, RoutedEventArgs e)
        {
            //置于最前端
            if (m_OpeningImage.m_Index == 0)
            {
                //控件隐藏
                m_Control = Visibility.Hidden;
            }
            else
            {
                //控件隐藏
                m_Control = Visibility.Visible;
                //数据写入
                Cmb_SelectIndex.ItemsSource = m_OpeningImage.InitCmb();
                //当前的选择为前面一个
                this.Cmb_SelectIndex.SelectedIndex = m_OpeningImage.m_LinkIndex == "上一个区域" ? 0 : m_OpeningImage.m_Index - 1;
            }

            cmb_Model.ItemsSource = Enum.GetNames(typeof(StructElement));
            cmb_Model.SelectedIndex = (int)m_OpeningImage.element;

            switch (m_OpeningImage.element)
            {
                case StructElement.圆行:
                    m_Radius = m_OpeningImage.m_Radius;
                    m_Circle = Visibility.Visible;
                    m_Rect = Visibility.Collapsed;
                    break;
                case StructElement.矩形:
                    m_MaskWidth = m_OpeningImage.m_Width;
                    m_MaskHeight = m_OpeningImage.m_Height;
                    m_Circle = Visibility.Collapsed;
                    m_Rect = Visibility.Visible;
                    break;
                default:
                    break;
            }

            txt_Radius.TextValueEvent += Txt_Width_TextValueEvent;
            txt_Width.TextValueEvent += Txt_Width_TextValueEvent;
            txt_Heigth.TextValueEvent += Txt_Width_TextValueEvent;
        }

        private void Cmb_Model_DropDownClosed(object sender, EventArgs e)
        {
            txt_Radius.TextValueEvent -= Txt_Width_TextValueEvent;
            txt_Width.TextValueEvent -= Txt_Width_TextValueEvent;
            txt_Heigth.TextValueEvent -= Txt_Width_TextValueEvent;

            switch ((StructElement)cmb_Model.SelectedIndex)
            {
                case StructElement.圆行:
                    m_Circle = Visibility.Visible;
                    m_Rect = Visibility.Collapsed;
                    m_OpeningImage.element = StructElement.圆行;
                    m_Radius = m_OpeningImage.m_Radius;
                    break;
                case StructElement.矩形:
                    m_Circle = Visibility.Collapsed;
                    m_Rect = Visibility.Visible;
                    m_OpeningImage.element = StructElement.矩形;
                    m_MaskWidth = m_OpeningImage.m_Width;
                    m_MaskHeight = m_OpeningImage.m_Height;
                    break;
                default:
                    break;
            }
            txt_Radius.TextValueEvent += Txt_Width_TextValueEvent;
            txt_Width.TextValueEvent += Txt_Width_TextValueEvent;
            txt_Heigth.TextValueEvent += Txt_Width_TextValueEvent;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            m_OpeningImage.m_LinkIndex = this.Cmb_SelectIndex.Text;
        }

        #region 索引状态

        public Visibility m_Control
        {
            get { return (Visibility)this.GetValue(m_ControlProperty); }
            set { this.SetValue(m_ControlProperty, value); }
        }

        public static readonly DependencyProperty m_ControlProperty =
            DependencyProperty.Register("m_Control", typeof(Visibility), typeof(ImageOpening), new PropertyMetadata(default(Visibility)));

        #endregion

        #region 数据

        public Visibility m_Circle
        {
            get { return (Visibility)this.GetValue(m_CircleProperty); }
            set { this.SetValue(m_CircleProperty, value); }
        }

        public static readonly DependencyProperty m_CircleProperty =
            DependencyProperty.Register("m_Circle", typeof(Visibility), typeof(ImageOpening), new PropertyMetadata(default(Visibility)));

        public Visibility m_Rect
        {
            get { return (Visibility)this.GetValue(m_RectProperty); }
            set { this.SetValue(m_RectProperty, value); }
        }

        public static readonly DependencyProperty m_RectProperty =
            DependencyProperty.Register("m_Rect", typeof(Visibility), typeof(ImageOpening), new PropertyMetadata(default(Visibility)));

        public int m_Radius
        {
            get { return (int)this.GetValue(m_RadiusProperty); }
            set { this.SetValue(m_RadiusProperty, value); }
        }

        public static readonly DependencyProperty m_RadiusProperty =
            DependencyProperty.Register("m_Radius", typeof(int), typeof(ImageOpening), new PropertyMetadata(default(int)));

        public int m_MaskWidth
        {
            get { return (int)this.GetValue(m_MaskWidthProperty); }
            set { this.SetValue(m_MaskWidthProperty, value); }
        }

        public static readonly DependencyProperty m_MaskWidthProperty =
            DependencyProperty.Register("m_MaskWidth", typeof(int), typeof(ImageOpening), new PropertyMetadata(default(int)));

        public int m_MaskHeight
        {
            get { return (int)this.GetValue(m_MaskHeightProperty); }
            set { this.SetValue(m_MaskHeightProperty, value); }
        }

        public static readonly DependencyProperty m_MaskHeightProperty =
            DependencyProperty.Register("m_MaskHeight", typeof(int), typeof(ImageOpening), new PropertyMetadata(default(int)));

        #endregion

        private void Txt_Width_TextValueEvent(object sender, RoutedEventArgs e)
        {
            switch (m_OpeningImage.element)
            {
                case StructElement.圆行:
                    m_OpeningImage.m_Radius = Convert.ToInt32(m_Radius);
                    break;
                case StructElement.矩形:
                    m_OpeningImage.m_Width = Convert.ToInt32(m_MaskWidth);
                    m_OpeningImage.m_Height = Convert.ToInt32(m_MaskHeight);
                    break;
                default:
                    break;
            }
        }

    }
}
