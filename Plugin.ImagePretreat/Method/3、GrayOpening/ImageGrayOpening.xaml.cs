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
    /// ImageGrayOpening.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGrayOpening : UserControl
    {

        public GrayOpening m_Opening;

        public ImageGrayOpening(BaseMethod Eethod)
        {
            InitializeComponent();
            m_Opening = (GrayOpening)Eethod;
            this.Loaded += ImageGrayOpening_Loaded;
        }

        private void ImageGrayOpening_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_Opening.m_MaskWidth;
            this.slider2.Value = m_Opening.m_MaskHeight;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;

            slider_ValueChanged(null, null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_Opening.m_MaskWidth = (int)slider1.Value;
            m_Opening.m_MaskHeight = (int)slider2.Value;
            m_Opening.Execute();
        }

    }
}
