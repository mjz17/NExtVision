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
    /// ImageGrayClosed.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGrayClosed : UserControl
    {

        public GrayClosed m_GrayClosed;

        public ImageGrayClosed(BaseMethod Eethod)
        {
            InitializeComponent();
            m_GrayClosed = (GrayClosed)Eethod;
            this.Loaded += ImageGrayClosed_Loaded;
        }

        private void ImageGrayClosed_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_GrayClosed.m_MaskWidth;
            this.slider2.Value = m_GrayClosed.m_MaskHeight;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;

            slider_ValueChanged(null, null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_GrayClosed.m_MaskWidth = (int)slider1.Value;
            m_GrayClosed.m_MaskHeight = (int)slider2.Value;
            m_GrayClosed.Execute();
        }

    }
}
