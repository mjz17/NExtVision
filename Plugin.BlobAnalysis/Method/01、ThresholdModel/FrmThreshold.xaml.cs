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
    /// FrmThreshold.xaml 的交互逻辑
    /// </summary>
    public partial class FrmThreshold : UserControl
    {
        private ModuleObj frm_ModuleObj;

        private double minvalue;

        private double maxvalue;

        public FrmThreshold(ModuleObj obj)
        {
            InitializeComponent();
            //加载参数
            frm_ModuleObj = (ModuleObj)obj;

            this.Loaded += FrmThreshold_Loaded;
        }

        private void FrmThreshold_Loaded(object sender, RoutedEventArgs e)
        {
            minvalue = this.Min_slider.Value = frm_ModuleObj.MinValue;
            maxvalue = this.Max_slider.Value = frm_ModuleObj.MaxValue;

            this.Min_slider.ValueChanged += slider_ValueChanged;
            this.Max_slider.ValueChanged += slider_ValueChanged;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double midMinValue = Min_slider.Value;
            double midMaxValue = Max_slider.Value;

            if (midMinValue > midMaxValue)
            {
                Min_slider.Value = minvalue;
                Max_slider.Value = maxvalue;    
            }

            frm_ModuleObj.MinValue = this.Min_slider.Value;
            frm_ModuleObj.MaxValue = this.Max_slider.Value;

            minvalue = this.Min_slider.Value;
            maxvalue = this.Max_slider.Value;
        }

    }
}
