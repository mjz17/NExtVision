using StyleBase;
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
    /// ImageEmpha.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEmpha : UserControl
    {
        public EmphaImage m_Empha;
        public ImageEmpha(BaseMethod Eethod)
        {
            InitializeComponent();
            m_Empha = (EmphaImage)Eethod;
            this.Loaded += ImageEmpha_Loaded;
        }

        private void ImageEmpha_Loaded(object sender, RoutedEventArgs e)
        {
            this.slider1.Value = m_Empha.m_MaskWidth;
            this.slider2.Value = m_Empha.m_MaskHeight;
            this.slider3.Value = m_Empha.m_Factor;

            this.slider1.ValueChanged += slider_ValueChanged;
            this.slider2.ValueChanged += slider_ValueChanged;
            this.slider3.ValueChanged += slider_ValueChanged;

            slider_ValueChanged(null,null);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_Empha.m_MaskWidth = (int)slider1.Value;
            m_Empha.m_MaskHeight = (int)slider2.Value;
            m_Empha.m_Factor = (double)slider3.Value;
            m_Empha.Execute();
        }
    }
}
