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
    /// ImageGauss.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGauss : UserControl
    {

        public GaussImage m_GaussImage;

        public ImageGauss(BaseMethod Eethod)
        {
            InitializeComponent();
            m_GaussImage = (GaussImage)Eethod;
            this.Loaded += ImageGauss_Loaded;
        }

        private void ImageGauss_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_GaussImage.m_Size;
            this.slider1.ValueChanged += slider_ValueChanged;
            slider_ValueChanged(null, null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_GaussImage.m_Size = (int)slider1.Value;
            m_GaussImage.Execute();
        }

    }
}
