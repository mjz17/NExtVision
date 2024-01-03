using Common;
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

namespace Plugin.NpointsCalibration
{
    /// <summary>
    /// TwoPointMethod.xaml 的交互逻辑
    /// </summary>
    public partial class TwoPointMethod : UserControl
    {

        private ModuleObj frm_ModuleObj;

        public TwoPointMethod(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;
            //加载参数
            frm_ModuleObj = (ModuleObj)obj;

            if (!frm_ModuleObj.blnNewModule)
            {
                theFirsttime();
            }
            else
            {
                theSecondTime();
            }

        }

        private void theFirsttime()
        {

        }

        private void theSecondTime()
        {
            PostionX1 = frm_ModuleObj.InputX1;
            PostionY1 = frm_ModuleObj.InputY1;
            PostionX2 = frm_ModuleObj.InputX2;
            PostionY1 = frm_ModuleObj.InputY2;
            PostionPhi = frm_ModuleObj.InputPhi;
        }

        #region 图像坐标X1

        public double PostionX1
        {
            get { return (double)this.GetValue(PostionX1Property); }
            set { this.SetValue(PostionX1Property, value); }
        }

        public static readonly DependencyProperty PostionX1Property =
            DependencyProperty.Register("PostionX1", typeof(double), typeof(TwoPointMethod), new PropertyMetadata(default(double)));

        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.InputX1 = PostionX1;
        }

        #endregion

        #region 图像坐标Y1

        public double PostionY1
        {
            get { return (double)this.GetValue(PostionY1Property); }
            set { this.SetValue(PostionY1Property, value); }
        }

        public static readonly DependencyProperty PostionY1Property =
            DependencyProperty.Register("PostionY1", typeof(double), typeof(TwoPointMethod), new PropertyMetadata(default(double)));

        private void UcTxtAddandSub_TextValueEvent_1(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.InputY1 = PostionY1;
        }

        #endregion

        #region 图像坐标X2

        public double PostionX2
        {
            get { return (double)this.GetValue(PostionX2Property); }
            set { this.SetValue(PostionX2Property, value); }
        }

        public static readonly DependencyProperty PostionX2Property =
            DependencyProperty.Register("PostionX2", typeof(double), typeof(TwoPointMethod), new PropertyMetadata(default(double)));

        private void UcTxtAddandSub_TextValueEvent_2(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.InputX2 = PostionX2;
        }

        #endregion

        #region 图像坐标Y2

        public double PostionY2
        {
            get { return (double)this.GetValue(PostionY2Property); }
            set { this.SetValue(PostionY2Property, value); }
        }

        public static readonly DependencyProperty PostionY2Property =
            DependencyProperty.Register("PostionY2", typeof(double), typeof(TwoPointMethod), new PropertyMetadata(default(double)));

        private void UcTxtAddandSub_TextValueEvent_3(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.InputY2 = PostionY2;
        }

        #endregion

        #region 旋转角度

        public double PostionPhi
        {
            get { return (double)this.GetValue(PostionPhiProperty); }
            set { this.SetValue(PostionPhiProperty, value); }
        }

        public static readonly DependencyProperty PostionPhiProperty =
            DependencyProperty.Register("PostionPhi", typeof(double), typeof(TwoPointMethod), new PropertyMetadata(default(double)));

        private void UcTxtAddandSub_TextValueEvent_4(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.InputPhi = PostionPhi;
        }

        #endregion

        #region 旋转角度X

        public string PostionCenterX
        {
            get { return (string)this.GetValue(PostionCenterXProperty); }
            set { this.SetValue(PostionCenterXProperty, value); }
        }

        public static readonly DependencyProperty PostionCenterXProperty =
            DependencyProperty.Register("PostionCenterX", typeof(string), typeof(TwoPointMethod), new PropertyMetadata(default(string)));

        #endregion

        #region 旋转角度Y

        public string PostionCenterY
        {
            get { return (string)this.GetValue(PostionCenterYProperty); }
            set { this.SetValue(PostionCenterYProperty, value); }
        }

        public static readonly DependencyProperty PostionCenterYProperty =
            DependencyProperty.Register("PostionCenterY", typeof(string), typeof(TwoPointMethod), new PropertyMetadata(default(string)));

        #endregion

        private void SuperButton_Click(object sender, RoutedEventArgs e)
        {
            double CenterX, CenterY;
            NpointCommon npoint = new NpointCommon();
            //输入为角度，需要自己转为弧度
            npoint.TwoPointMethod(PostionX1, PostionX2, PostionY1, PostionY2, (Math.PI / 180) * PostionPhi, out CenterX, out CenterY);
            PostionCenterX = CenterX.ToString("f5");
            PostionCenterY = CenterY.ToString("f5");

            frm_ModuleObj.RotationCenter_X = CenterX;
            frm_ModuleObj.RotationCenter_Y = CenterY;

        }

    }
}
