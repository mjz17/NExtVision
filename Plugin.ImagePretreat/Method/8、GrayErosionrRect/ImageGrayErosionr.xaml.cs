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

namespace Plugin.ImagePretreat
{
    /// <summary>
    /// ImageGrayErosionr.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGrayErosionr : UserControl
    {

        public GrayErosionr m_GrayErosionr;

        public ImageGrayErosionr(BaseMethod Eethod)
        {
            InitializeComponent();
            m_GrayErosionr = (GrayErosionr)Eethod;
            this.Loaded += ImageGrayErosionr_Loaded;
        }

        private void ImageGrayErosionr_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_GrayErosionr.m_MaskWidth;
            this.slider2.Value = m_GrayErosionr.m_MaskHeight;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;

            slider_ValueChanged(null, null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_GrayErosionr.m_MaskWidth = (int)slider1.Value;
            m_GrayErosionr.m_MaskHeight = (int)slider2.Value;
            m_GrayErosionr.Execute();
        }

    }
}
