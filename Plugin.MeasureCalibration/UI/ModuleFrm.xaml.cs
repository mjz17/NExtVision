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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.MeasureCalibration
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        //搜索区域
        List<ViewWindow.Model.ROI> SeachRegion;

        //当前窗体
        private string TabName = string.Empty;

        //绘制的ROI
        private RepaintROI matchRepaint = new RepaintROI();

        private HXLDCont xldMark = new HXLDCont();

        public ModuleFrm()
        {
            InitializeComponent();
            this.DataContext = this;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
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

                //拖拽事件
                this.Main_HalconView.hWindowControl.MouseUp += HWindowControl_MouseUp;
                //初始化模式
                InitCmbModel();
                if (!frm_ModuleObj.blnNewModule)
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
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 初次打开
        /// </summary>
        public override void theFirsttime()
        {
            base.theFirsttime();
            ThresholdValue = "0";
            Distance = "0";
            Pixel_x = "0";
            Pixel_y = "0";

            #region 查询该流程中为图像的模块.

            if (frm_ModuleObj.GenModuleIndex(out string str, out frm_ModuleObj.m_Image, out frm_ModuleObj.Link_Image_Data))
            {
                CurrentImage = str;
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
        }

        /// <summary>
        /// 非初次打开
        /// </summary>
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

            matchRepaint.MatchRoIInfo = frm_ModuleObj.MatchRoIInfo;

            //标定模式选择//标定板类型
            CmbModel.Text = frm_ModuleObj.m_Calibration.ToString();

            //搜索区域

            //屏蔽区域

            //梯度阈值
            ThresholdValue = frm_ModuleObj.ThresholdValue.ToString();
            //物理间距
            Distance = frm_ModuleObj.Distance.ToString();
            //像素当量X
            Pixel_x = frm_ModuleObj.ModuleImageParam.ScaleX.ToString();
            //像素当量Y
            Pixel_y = frm_ModuleObj.ModuleImageParam.ScaleY.ToString();

            InitCom();

        }

        #region 选择标定模式

        private void InitCmbModel()
        {
            CmbModel.ItemsSource = Enum.GetNames(typeof(CalibrationModel));
            CmbModel.SelectedIndex = (int)frm_ModuleObj.m_Calibration;
        }

        private void CmbModel_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_Calibration = (CalibrationModel)CmbModel.SelectedIndex;
            InitCom();
        }

        private void InitCom()
        {
            switch (frm_ModuleObj.m_Calibration)
            {
                case CalibrationModel.孔板模式:
                    Page_Change.Content = new Frame()
                    {
                        Content = null
                    };
                    break;
                case CalibrationModel.孔板效正图像模式:
                    Page_Change.Content = new Frame()
                    {
                        Content = null
                    };
                    break;
                case CalibrationModel.多相机孔板模式:
                    MultCameraMapping camera = new MultCameraMapping(this.frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = camera
                    };
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 显示图像

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_ImgPath_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Image)
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

        #region 物理间距

        public string Distance
        {
            get { return (string)this.GetValue(DistanceProperty); }
            set { this.SetValue(DistanceProperty, value); }
        }

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("Distance", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 梯度阈值

        public string ThresholdValue
        {
            get { return (string)this.GetValue(ThresholdValueProperty); }
            set { this.SetValue(ThresholdValueProperty, value); }
        }

        public static readonly DependencyProperty ThresholdValueProperty =
            DependencyProperty.Register("ThresholdValue", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 像素当量

        public string Pixel_x
        {
            get { return (string)this.GetValue(Pixel_xProperty); }
            set { this.SetValue(Pixel_xProperty, value); }
        }

        public static readonly DependencyProperty Pixel_xProperty =
            DependencyProperty.Register("Pixel_x", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Pixel_y
        {
            get { return (string)this.GetValue(Pixel_yProperty); }
            set { this.SetValue(Pixel_yProperty, value); }
        }

        public static readonly DependencyProperty Pixel_yProperty =
            DependencyProperty.Register("Pixel_y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Rms
        {
            get { return (string)this.GetValue(Rms_Property); }
            set { this.SetValue(Rms_Property, value); }
        }

        public static readonly DependencyProperty Rms_Property =
            DependencyProperty.Register("Rms", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox radio = (System.Windows.Controls.CheckBox)sender;
            try
            {
                if (radio.IsChecked == true)
                {
                    if (frm_ModuleObj.Link_Image_Data.m_DataValue != null)
                    {
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                        List<double> data = new List<double>();
                        if (matchRepaint.QueryRoI(RoIModel.搜索区域, RoIType.方行, ref data))
                        {
                            Main_HalconView.viewWindow.removeActiveROI(ref SeachRegion);
                            Main_HalconView.viewWindow.genRect1(data[0], data[1], data[2], data[3], ref this.SeachRegion);
                            frm_ModuleObj.m_SearchRegion = new Rectangle_INFO(data[0], data[1], data[2], data[3]);//将参数保存
                        }
                        else
                        {
                            Main_HalconView.viewWindow.genRect1(110.0, 110.0, 250.0, 250.0, ref this.SeachRegion);
                            data.Add(110.0);
                            data.Add(110.0);
                            data.Add(250);
                            data.Add(250.0);
                            matchRepaint.UpdateROI(RoIModel.搜索区域, RoIType.方行, data);
                            frm_ModuleObj.m_SearchRegion = new Rectangle_INFO(110.0, 110.0, 250.0, 250.0);//将参数保存
                        }
                    }
                }
                if (radio.IsChecked == false)
                {
                    if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                    {
                        return;
                    }
                    Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                    //显示搜索区域
                    List<double> searchdata = new List<double>();
                    if (matchRepaint.QueryRoI(RoIModel.搜索区域, RoIType.方行, ref searchdata))
                    {
                        Rectangle_INFO rectangle = new Rectangle_INFO(searchdata[0], searchdata[1], searchdata[2], searchdata[3]);
                        Main_HalconView.DispObj(rectangle.GenRegion(), "blue");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private void HWindowControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (TabName.Contains("基本参数") && Region_Check.IsChecked == true)
                {
                    //搜索区域  
                    List<double> Seach_data = new List<double>();
                    //搜索区域  
                    List<double> clickSeach_data = new List<double>();
                    int index;
                    Main_HalconView.viewWindow.smallestActiveROI(out Seach_data, out index);
                    if (index >= 0)
                    {
                        matchRepaint.UpdateROI(RoIModel.搜索区域, RoIType.方行, Seach_data);
                        frm_ModuleObj.m_SearchRegion = new Rectangle_INFO(Seach_data[0], Seach_data[1], Seach_data[2], Seach_data[3]);//将参数保存
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        private void tab_Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var TabItemdata = this.tab_Control.SelectedItem as TabItem;
                TabName = TabItemdata.Header.ToString();
            }));
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                frm_ModuleObj.Calibration = false;

                switch (frm_ModuleObj.m_Calibration)
                {
                    case CalibrationModel.孔板模式:
                        Plate plate = new Plate(frm_ModuleObj.m_Image, frm_ModuleObj.m_SearchRegion.GenRegion(),
                            Convert.ToDouble(Distance), Convert.ToInt32(ThresholdValue));
                        plate.CalibrationMeasure(ref frm_ModuleObj.ModuleImageParam);

                        Pixel_x = frm_ModuleObj.ModuleImageParam.ScaleX.ToString("f6");
                        Pixel_y = frm_ModuleObj.ModuleImageParam.ScaleY.ToString("f6");

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                        Main_HalconView.DispObj(plate.m_xldMark, "red");

                        frm_ModuleObj.Calibration = true;

                        break;
                    case CalibrationModel.孔板效正图像模式:









                        break;
                    case CalibrationModel.多相机孔板模式:

                        PlateMult plateMult = new PlateMult(frm_ModuleObj.m_Image, frm_ModuleObj.m_SearchRegion.GenRegion(),
                          Convert.ToDouble(Distance), Convert.ToInt32(ThresholdValue), Convert.ToInt32(frm_ModuleObj.BigWorldOriginX),
                          Convert.ToInt32(frm_ModuleObj.BigWorldOriginY));

                        plateMult.CalibrationImage(ref frm_ModuleObj.ModuleImageParam);

                        Pixel_x = frm_ModuleObj.ModuleImageParam.ScaleX.ToString("f6");
                        Pixel_y = frm_ModuleObj.ModuleImageParam.ScaleY.ToString("f6");
                        Rms = plateMult.m_Rms.ToString("f6");

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                        Main_HalconView.DispObj(plateMult.m_xldMark, "red");

                        frm_ModuleObj.Calibration = true;

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                frm_ModuleObj.Calibration = false;
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                if (ProtectModuel())
                {
                    //保存参数
                    ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 数据保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            //图像未设置
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像未设置！");
                return false;
            }

            //判断图像类型
            if (!(frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>))
            {
                System.Windows.Forms.MessageBox.Show("图像类型错误！");
                return false;
            }

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称

            //物理间距未设置
            if (Distance.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("物理间距未设置！");
                return false;
            }

            //阈值未设置
            if (ThresholdValue.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("阈值未设置！");
                return false;
            }


            ((ModuleObj)m_ModuleObjBase).Link_Image_Data = frm_ModuleObj.Link_Image_Data;

            frm_ModuleObj.MatchRoIInfo = matchRepaint.MatchRoIInfo;//保存参数

            //梯度阈值
            frm_ModuleObj.ThresholdValue = int.Parse(ThresholdValue);
            //物理间距
            frm_ModuleObj.Distance = double.Parse(Distance);

            //像素当量X
            frm_ModuleObj.ModuleImageParam.ScaleX = double.Parse(Pixel_x);
            //像素当量Y
            frm_ModuleObj.ModuleImageParam.ScaleY = double.Parse(Pixel_y);

            return true;
        }

        /// <summary>
        /// 取消
        /// </summary>
        public override void CancelModuleParam()
        {
            this.Main_HalconView.hWindowControl.MouseUp -= HWindowControl_MouseUp;
            base.CancelModuleParam();
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
