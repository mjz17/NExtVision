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
    /// ImageMean.xaml 的交互逻辑
    /// </summary>
    public partial class ImageMean : UserControl
    {
        public MeanImage m_MeanImage;

        public ImageMean(BaseMethod Eethod)
        {
            InitializeComponent();
            m_MeanImage = (MeanImage)Eethod;
            this.Loaded += ImageMean_Loaded;
        }

        private void ImageMean_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_MeanImage.m_MaskWidth;
            this.slider2.Value = m_MeanImage.m_MaskHeight;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_MeanImage.m_MaskWidth = (int)slider1.Value;
            m_MeanImage.m_MaskHeight = (int)slider2.Value;
            m_MeanImage.Execute();
        }

    }
}
