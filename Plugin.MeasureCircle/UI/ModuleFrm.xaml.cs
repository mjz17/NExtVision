using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
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
using VisionCore;

namespace Plugin.MeasureCircle
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {
        //窗体对应的数据
        private ModuleObj frm_ModuleObj;

        //绘制的圆
        private List<ViewWindow.Model.ROI> DrawCircle;

        //当前窗体
        private string TabName = string.Empty;

        //绘制的ROI
        private RepaintROI matchRepaint = new RepaintROI();

        /// <summary>
        /// 绘制圆的信息
        /// </summary>
        public Circle_INFO m_DrawCircle = new Circle_INFO();

        public ModuleFrm()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.DataContext = this;
            this.Loaded += ModuleFrm_Loaded;
        }

        private void ModuleFrm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载参数
                frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
                //窗体名称
                Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;
                //模块当前ID
                CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

                DispOutRang = frm_ModuleObj.m_IsDispOutRang;
                DispOutPoint = frm_ModuleObj.m_IsDispOutPoint;
                DispResult = frm_ModuleObj.m_IsDispResult;

                //模式
                Init_Transition();
                //规格
                Init_Direction();
                //方向
                Init_Screen();

                if (!m_ModuleObjBase.blnNewModule)
                {
                    theFirsttime();
                }
                else
                {
                    theSecondTime();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            this.Main_HalconView.hWindowControl.MouseDown += HWindowControl_MouseDown;
            this.Main_HalconView.hWindowControl.MouseUp += HWindowControl_MouseUp;
            this.tab_Control.SelectionChanged += tab_Control_SelectionChanged;
        }

        public override void theFirsttime()
        {
            base.theFirsttime();
            #region 使用默认参数

            Length1 = 10;
            Length2 = 5;
            Threshold = 50;
            MeasureDis = 15;
            cmb_Transition.Text = "由白到黑";
            cmb_Direction.Text = "默认值";
            cmb_Screen.Text = "first";

            #endregion
            #region 查询该流程中为图像的模块.

            if (frm_ModuleObj.GenModuleIndex(out string str, out frm_ModuleObj.m_Image, out frm_ModuleObj.Link_Image_Data))
            {
                CurrentImage = str;
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            #region 读取链接图像并显示

            CurrentImage = frm_ModuleObj.m_CurentImgName;
            if (CurrentImage != null && CurrentImage.Length > 0)
            {
                DataVar var = frm_ModuleObj.m_ModuleProject.GetCurLocalVarValue(frm_ModuleObj.Link_Image_Data);
                frm_ModuleObj.m_Image = ((List<HImageExt>)var.m_DataValue)[0];
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
            #region 位置补正信息

            AffineImage = frm_ModuleObj.m_LinkDataName;

            if (frm_ModuleObj.m_LinkDataName != null)
            {
                if (frm_ModuleObj.m_LinkDataName.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_Affine_data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Affine_data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_Affine_data.m_DataModuleID);

                    frm_ModuleObj.LinkAffineCorre = ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0];

                    HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                    Main_HalconView.DispObj(CoorXLD, "yellow");
                }
            }

            #endregion
            getMetrologInfo();
            matchRepaint.MatchRoIInfo = frm_ModuleObj.MatchRoIInfo;
        }

        private void HWindowControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void HWindowControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (TabName.Contains("参数设置"))
                {
                    List<double> Circle_data = new List<double>();
                    int index;
                    Main_HalconView.viewWindow.smallestActiveROI(out Circle_data, out index);
                    if (index >= 0)
                    {
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                        matchRepaint.UpdateROI(RoIType.圆形, Circle_data);
                        //Main_HalconView.viewWindow.removeActiveROI(ref DrawCircle);
                        DrawCircle.Clear();

                        Main_HalconView.viewWindow.genCircle(Circle_data[0], Circle_data[1], Circle_data[2], ref this.DrawCircle);
                        m_DrawCircle = new Circle_INFO(Circle_data[0], Circle_data[1], Circle_data[2]);

                        if (frm_ModuleObj.m_LinkDataName != null)
                        {
                            if (frm_ModuleObj.m_LinkDataName.Length > 0)
                            {
                                HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                                Main_HalconView.DispObj(CoorXLD, "blue"); ;
                            }
                        }

                        UpdateParam(m_DrawCircle);

                        CircleRow = frm_ModuleObj.m_OutCircle.CenterX;
                        CircleColum = frm_ModuleObj.m_OutCircle.CenterY;
                        CircleRadius = frm_ModuleObj.m_OutCircle.Radius;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #region 模式

        private void Init_Transition()
        {
            List<string> NumLevels = new List<string>()
            {
                "由白到黑",
                "由黑到白",
                "所有",
                "规格一致",
            };
            cmb_Transition.ItemsSource = NumLevels;
        }

        private void cmb_Transition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        #endregion

        #region 规格

        private void Init_Direction()
        {
            List<string> NumLevels = new List<string>()
            {
                "默认值",
                "顺时针",
                "逆时针",
            };
            cmb_Direction.ItemsSource = NumLevels;
        }

        private void cmb_Direction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        #endregion

        #region 方向

        private void Init_Screen()
        {
            List<string> NumLevels = new List<string>()
            {
                "first",
                "last",
                "all",
                "strongest",
            };
            cmb_Screen.ItemsSource = NumLevels;
        }

        private void cmb_Screen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        #endregion

        #region 链接图像显示

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_ImgPath_EValueAlarm(object sender, RoutedEventArgs e)
        {
            DataVar data = (DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == DataVarType.DataType.Image)
            {
                try
                {
                    if (data.m_DataValue is List<HImageExt>)
                    {
                        frm_ModuleObj.m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                        frm_ModuleObj.Link_Image_Data = data;
                        DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 链接位置补正

        public string AffineImage
        {
            get { return (string)this.GetValue(AffineImageImageProperty); }
            set { this.SetValue(AffineImageImageProperty, value); }
        }

        public static readonly DependencyProperty AffineImageImageProperty =
            DependencyProperty.Register("AffineImage", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Affine_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                DataVar data = (DataVar)e.OriginalSource;
                //数据不为空，且是图像类型
                if (data.m_DataValue != null && data.m_DataType == DataVarType.DataType.坐标系)
                {
                    frm_ModuleObj.m_LinkDataName = AffineImage;
                    frm_ModuleObj.Link_Affine_data = data;
                    if (frm_ModuleObj.m_Image != null)
                    {
                        frm_ModuleObj.LinkAffineCorre = ((List<Coordinate_INFO>)data.m_DataValue)[0];
                        HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, frm_ModuleObj.LinkAffineCorre);
                        Main_HalconView.DispObj(CoorXLD);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 窗体切换

        private void tab_Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var data = this.tab_Control.SelectedItem as TabItem;
                TabName = data.Header.ToString(); //主要是在后端获取到当前的TabItem的Heade 
                try
                {
                    if (TabName.ToString().Contains("基本参数"))
                    {
                        #region 基本参数

                        if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                        {
                            return;
                        }

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        if (frm_ModuleObj.Link_Affine_data.m_DataValue != null)
                        {
                            HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                            Main_HalconView.DispObj(CoorXLD, "blue");
                        }

                        #endregion
                    }
                    else if (TabName.ToString().Contains("参数设置"))
                    {
                        #region 参数设置

                        if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                        {
                            return;
                        }

                        Main_HalconView.viewWindow.removeActiveROI(ref DrawCircle);
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        //绘制的圆
                        List<double> Circledata = new List<double>();
                        if (matchRepaint.QueryRoI(RoIType.圆形, ref Circledata))
                        {
                            Main_HalconView.viewWindow.genCircle(Circledata[0], Circledata[1], Circledata[2], ref this.DrawCircle);
                        }
                        else
                        {
                            Main_HalconView.viewWindow.genCircle(200.0, 200.0, 60.0, ref this.DrawCircle);
                            Circledata.Add(200.0);
                            Circledata.Add(200.0);
                            Circledata.Add(60.0);
                            matchRepaint.UpdateROI(RoIType.圆形, Circledata);
                            frm_ModuleObj.m_DrawCircle = new Circle_INFO(200.0, 200.0, 60.0);
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }));
        }

        #endregion

        #region 圆测量参数

        //宽度
        public double Length1
        {
            get { return (double)this.GetValue(Length1Property); }
            set { this.SetValue(Length1Property, value); }
        }

        public static readonly DependencyProperty Length1Property =
            DependencyProperty.Register("Length1", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //高度
        public double Length2
        {
            get { return (double)this.GetValue(Length2Property); }
            set { this.SetValue(Length2Property, value); }
        }

        public static readonly DependencyProperty Length2Property =
            DependencyProperty.Register("Length2", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //阈值
        public double Threshold
        {
            get { return (double)this.GetValue(ThresholdProperty); }
            set { this.SetValue(ThresholdProperty, value); }
        }

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register("Threshold", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //间隔
        public double MeasureDis
        {
            get { return (double)this.GetValue(MeasureDisdProperty); }
            set { this.SetValue(MeasureDisdProperty, value); }
        }

        public static readonly DependencyProperty MeasureDisdProperty =
            DependencyProperty.Register("MeasureDis", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //圆坐标ROW
        public double CircleRow
        {
            get { return (double)this.GetValue(CircleRowProperty); }
            set { this.SetValue(CircleRowProperty, value); }
        }

        public static readonly DependencyProperty CircleRowProperty =
            DependencyProperty.Register("CircleRow", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //圆坐标Colum
        public double CircleColum
        {
            get { return (double)this.GetValue(CircleColumProperty); }
            set { this.SetValue(CircleColumProperty, value); }
        }

        public static readonly DependencyProperty CircleColumProperty =
            DependencyProperty.Register("CircleColum", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //圆坐标Radius
        public double CircleRadius
        {
            get { return (double)this.GetValue(CircleRadiusProperty); }
            set { this.SetValue(CircleRadiusProperty, value); }
        }

        public static readonly DependencyProperty CircleRadiusProperty =
            DependencyProperty.Register("CircleRadius", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        #endregion

        #region 显示的设置

        public bool DispOutRang
        {
            get { return (bool)this.GetValue(DispOutRangProperty); }
            set { this.SetValue(DispOutRangProperty, value); }
        }

        public static readonly DependencyProperty DispOutRangProperty =
            DependencyProperty.Register("DispOutRang", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool DispOutPoint
        {
            get { return (bool)this.GetValue(DispOutPointProperty); }
            set { this.SetValue(DispOutPointProperty, value); }
        }

        public static readonly DependencyProperty DispOutPointProperty =
            DependencyProperty.Register("DispOutPoint", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool DispResult
        {
            get { return (bool)this.GetValue(DispResultProperty); }
            set { this.SetValue(DispResultProperty, value); }
        }

        public static readonly DependencyProperty DispResultProperty =
            DependencyProperty.Register("DispResult", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        /// <summary>
        /// 执行模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                //删除本ID的变量
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                }

                frm_ModuleObj.ExeModule();
                CircleRow = frm_ModuleObj.m_OutCircle.CenterX;
                CircleColum = frm_ModuleObj.m_OutCircle.CenterY;
                CircleRadius = frm_ModuleObj.m_OutCircle.Radius;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                //图像
                ((ModuleObj)m_ModuleObjBase).m_CurentImgName = CurrentImage;
                ((ModuleObj)m_ModuleObjBase).Link_Image_Data = frm_ModuleObj.Link_Image_Data;
                //链接
                ((ModuleObj)m_ModuleObjBase).m_LinkDataName = AffineImage;
                ((ModuleObj)m_ModuleObjBase).Link_Affine_data = frm_ModuleObj.Link_Affine_data;
                ((ModuleObj)m_ModuleObjBase).m_MetrologyInfo = frm_ModuleObj.m_MetrologyInfo;
                //坐标保存
                ((ModuleObj)m_ModuleObjBase).MatchRoIInfo = matchRepaint.MatchRoIInfo;
                ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public override void CancelModuleParam()
        {
            base.CancelModuleParam();
            this.Main_HalconView.hWindowControl.MouseDown -= HWindowControl_MouseDown;
            this.Main_HalconView.hWindowControl.MouseUp -= HWindowControl_MouseUp;
            this.tab_Control.SelectionChanged -= tab_Control_SelectionChanged;
        }

        public void setMetrologInfo()
        {
            frm_ModuleObj.m_MetrologyInfo.Length1 = Length1;
            frm_ModuleObj.m_MetrologyInfo.Length2 = Length2;
            frm_ModuleObj.m_MetrologyInfo.Threshold = Threshold;
            frm_ModuleObj.m_MetrologyInfo.MeasureDis = MeasureDis;

            string mTransition = "negative";

            if (cmb_Transition.SelectedIndex == 0)
                mTransition = "negative";
            else if (cmb_Transition.SelectedIndex == 1)
                mTransition = "positive";
            else if (cmb_Transition.SelectedIndex == 2)
                mTransition = "all";
            else if (cmb_Transition.SelectedIndex == 3)
                mTransition = "uniform";

            string mSelect = "first";
            if (cmb_Screen.SelectedIndex == 0)
                mSelect = "first";
            else if (cmb_Screen.SelectedIndex == 1)
                mSelect = "last";
            else if (cmb_Screen.SelectedIndex == 2)
                mSelect = "all";
            else if (cmb_Screen.SelectedIndex == 3)
                mSelect = "strongest";//magical 20180405 增加最强边边缘

            frm_ModuleObj.m_MetrologyInfo.ParamName = new HTuple();
            frm_ModuleObj.m_MetrologyInfo.ParamName.Append("measure_transition");
            frm_ModuleObj.m_MetrologyInfo.ParamName.Append("measure_select");
            frm_ModuleObj.m_MetrologyInfo.ParamName.Append("measure_distance");
            //m_MetrologyInfo.ParamName.Append("max_num_iterations");
            //m_MetrologyInfo.ParamName.Append("measure_interpolation");

            frm_ModuleObj.m_MetrologyInfo.ParamValue = new HTuple();
            frm_ModuleObj.m_MetrologyInfo.ParamValue.Append(mTransition);
            frm_ModuleObj.m_MetrologyInfo.ParamValue.Append(mSelect);
            frm_ModuleObj.m_MetrologyInfo.ParamValue.Append(frm_ModuleObj.m_MetrologyInfo.MeasureDis);
            //m_MetrologyInfo.ParamValue.Append(-1);
            //m_MetrologyInfo.ParamValue.Append("nearest_neighbor");
        }

        public void getMetrologInfo()
        {
            Length1 = frm_ModuleObj.m_MetrologyInfo.Length1;
            Length2 = frm_ModuleObj.m_MetrologyInfo.Length2;
            Threshold = frm_ModuleObj.m_MetrologyInfo.Threshold;
            MeasureDis = frm_ModuleObj.m_MetrologyInfo.MeasureDis;

            //模式
            switch (frm_ModuleObj.m_MetrologyInfo.ParamValue[0].S)
            {
                case "negative":
                    cmb_Transition.SelectedIndex = 0;
                    break;
                case "positive":
                    cmb_Transition.SelectedIndex = 1;
                    break;
                case "all":
                    cmb_Transition.SelectedIndex = 2;
                    break;
                case "uniform":
                    cmb_Transition.SelectedIndex = 3;
                    break;
                default:
                    break;
            }

            cmb_Direction.Text = "默认值";

            //模式
            switch (frm_ModuleObj.m_MetrologyInfo.ParamValue[1].S)
            {
                case "first":
                    cmb_Screen.SelectedIndex = 0;
                    break;
                case "last":
                    cmb_Screen.SelectedIndex = 1;
                    break;
                case "all":
                    cmb_Screen.SelectedIndex = 2;
                    break;
                case "strongest":
                    cmb_Screen.SelectedIndex = 3;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        /// <param name="in_Circle">输入圆心坐标</param>
        public void UpdateParam(Circle_INFO in_Circle)
        {
            //设置测量参数
            setMetrologInfo();
            frm_ModuleObj.VerifyParam(in_Circle);
            Main_HalconView.DispObj(frm_ModuleObj.m_MeasureXLD, "blue");//卡尺
            Main_HalconView.DispObj(frm_ModuleObj.m_MeasureCross, "yellow");//点
            Main_HalconView.DispObj(frm_ModuleObj.m_ResultXLD, "red");//拟合圆
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            switch (chk.Content)
            {
                case "显示检测范围":
                    if (chk.IsChecked == true)
                    {
                        frm_ModuleObj.m_IsDispOutRang = true;
                    }
                    if (chk.IsChecked == false)
                    {
                        frm_ModuleObj.m_IsDispOutRang = false;
                    }
                    break;
                case "显示检测点":
                    if (chk.IsChecked == true)
                    {
                        frm_ModuleObj.m_IsDispOutPoint = true;
                    }
                    if (chk.IsChecked == false)
                    {
                        frm_ModuleObj.m_IsDispOutPoint = false;
                    }
                    break;
                case "显示检测结果":
                    if (chk.IsChecked == true)
                    {
                        frm_ModuleObj.m_IsDispResult = true;
                    }
                    if (chk.IsChecked == false)
                    {
                        frm_ModuleObj.m_IsDispResult = false;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
