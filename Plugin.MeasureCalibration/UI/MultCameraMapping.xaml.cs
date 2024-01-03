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

namespace Plugin.MeasureCalibration
{
    /// <summary>
    /// MultCameraMapping.xaml 的交互逻辑
    /// </summary>
    public partial class MultCameraMapping : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public MultCameraMapping(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;

            //加载参数
            frm_ModuleObj = (ModuleObj)obj;

            this.Loaded += MultCameraMapping_Loaded;
        }

        private void MultCameraMapping_Loaded(object sender, RoutedEventArgs e)
        {
            if (frm_ModuleObj.blnNewModule)
            {
                theSecondTime();
            }
            Right_X.TextValueEvent += Right_X_TextValueEvent;
            Right_Y.TextValueEvent += Right_Y_TextValueEvent;
        }

        private void theSecondTime()
        {
            UpperleftX = frm_ModuleObj.BigWorldOriginX;
            UpperleftY = frm_ModuleObj.BigWorldOriginY;
        }

        #region 左上坐标X

        public double UpperleftX
        {
            get { return (double)this.GetValue(UpperleftXProperty); }
            set { this.SetValue(UpperleftXProperty, value); }
        }

        public static readonly DependencyProperty UpperleftXProperty =
            DependencyProperty.Register("UpperleftX", typeof(double), typeof(MultCameraMapping), new PropertyMetadata(default(double)));

        #endregion

        #region 左上坐标Y

        public double UpperleftY
        {
            get { return (double)this.GetValue(UpperleftYProperty); }
            set { this.SetValue(UpperleftYProperty, value); }
        }

        public static readonly DependencyProperty UpperleftYProperty =
            DependencyProperty.Register("UpperleftY", typeof(double), typeof(MultCameraMapping), new PropertyMetadata(default(double)));

        #endregion

        private void Right_X_TextValueEvent(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.BigWorldOriginX = UpperleftX;
        }

        private void Right_Y_TextValueEvent(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.BigWorldOriginY = UpperleftY;
        }
    }
}
