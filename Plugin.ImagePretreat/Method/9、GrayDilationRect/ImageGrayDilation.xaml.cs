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
    /// ImageGrayDilation.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGrayDilation : UserControl
    {
        public GrayDilation m_GrayDilation;

        public ImageGrayDilation(BaseMethod Eethod)
        {
            InitializeComponent();
            m_GrayDilation = (GrayDilation)Eethod;
            this.Loaded += ImageGrayDilation_Loaded;
        }

        private void ImageGrayDilation_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_GrayDilation.m_MaskWidth;
            this.slider2.Value = m_GrayDilation.m_MaskHeight;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;

            slider_ValueChanged(null, null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_GrayDilation.m_MaskWidth = (int)slider1.Value;
            m_GrayDilation.m_MaskHeight = (int)slider2.Value;
            m_GrayDilation.Execute();
        }

    }
}
