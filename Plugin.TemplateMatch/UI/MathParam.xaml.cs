using Common;
using DefineImgRoI;
using HalconDotNet;
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
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.TemplateMatch
{
    /// <summary>
    /// MathParam.xaml 的交互逻辑
    /// </summary>
    public partial class MathParam : Window
    {
        private ModuleObj frm_ModuleObj;

        //涂抹
        private MatchTool matchTool = new MatchTool();

        //模板匹配的XLD
        private HXLDCont contour_xld;

        //需要涂抹的区域
        private HRegion hRegion;

        public MathParam(ModuleObj m_ModuleObj)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.DataContext = this;
            frm_ModuleObj = m_ModuleObj;
            this.Loaded += MathParam_Loaded;
        }

        private void MathParam_Loaded(object sender, RoutedEventArgs e)
        {
            InitCmbNumLevels();
            InitCmbMetric();

            try
            {
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

        #region 参数设置

        private void InitCmbNumLevels()
        {
            List<string> NumLevels = new List<string>()
            {
                "0",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
            };
            Cmb_numLevels.ItemsSource = NumLevels;
        }

        private void InitCmbMetric()
        {
            List<string> NumLevels = new List<string>()
            {
                "黑白对比度一致",
                "黑白对比度不一致",
                "黑白对比局部不一致"
            };
            Cmb_Metric.ItemsSource = NumLevels;
        }

        #endregion

        private void theFirsttime()
        {
            CreateMatch = new CreateMatchPram();
            FindMatch = new FindMatchPram();
            InitFrm();
            Main_Halcon.HobjectToHimage(frm_ModuleObj.m_ModelImage);

            //模板区域
            HTuple hv_width;
            HTuple hv_heigth;
            HOperatorSet.GetImageSize(frm_ModuleObj.m_ModelImage, out hv_width, out hv_heigth);
            hRegion = new HRegion(0, 0, hv_heigth - 1, hv_width - 1);
        }

        private void theSecondTime()
        {
            CreateMatch = frm_ModuleObj.CreateMatch;
            FindMatch = frm_ModuleObj.FindMatch;
            InitFrm();
            #region 模板图像

            int index = SysProcessPro.Cur_Project.m_RegistImg.FindIndex(c => c.m_Image_ID == SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName + frm_ModuleObj.ModuleParam.ModuleName);
            if (index > -1)
            {
                frm_ModuleObj.m_ModelImage = SysProcessPro.Cur_Project.m_RegistImg[index].m_Image;
                DispImage();
            }

            #endregion
            hRegion = frm_ModuleObj.MatchdisableRegion.GenRegion();
        }

        #region 创建模板参数

        public CreateMatchPram CreateMatch
        {
            get { return (CreateMatchPram)this.GetValue(CreateMatchProperty); }
            set { this.SetValue(CreateMatchProperty, value); }
        }

        public static readonly DependencyProperty CreateMatchProperty =
            DependencyProperty.Register("CreateMatch", typeof(CreateMatchPram), typeof(MathParam), new PropertyMetadata(default(CreateMatchPram)));

        #endregion

        #region 查询模板参数

        public FindMatchPram FindMatch
        {
            get { return (FindMatchPram)this.GetValue(FindMatchProperty); }
            set { this.SetValue(FindMatchProperty, value); }
        }

        public static readonly DependencyProperty FindMatchProperty =
            DependencyProperty.Register("FindMatch", typeof(FindMatchPram), typeof(MathParam), new PropertyMetadata(default(FindMatchPram)));

        #endregion

        #region 使用极性

        private void InitFrm()
        {
            Cmb_numLevels.Text = CreateMatch.numLevels.ToString();
            switch (CreateMatch.metric.ToString())
            {
                case "use_polarity":
                    Cmb_Metric.Text = "黑白对比度一致"; //极性控制
                    break;
                case "ignore_global_polarity":
                    Cmb_Metric.Text = "黑白对比度不一致"; //极性控制
                    break;
                case "ignore_local_polarity":
                    Cmb_Metric.Text = "黑白对比局部不一致"; //极性控制
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 金字塔层数设置

        private void Cmb_numLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateMatch.numLevels = int.Parse(Cmb_numLevels.SelectedValue.ToString());
        }

        #endregion

        #region 使用极性设置

        private void Cmb_Metric_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Cmb_Metric.SelectedValue.ToString())
            {
                case "黑白对比度一致":
                    CreateMatch.metric = "use_polarity";
                    break;
                case "黑白对比度不一致":
                    CreateMatch.metric = "ignore_global_polarity";
                    break;
                case "黑白对比局部不一致":
                    CreateMatch.metric = "ignore_local_polarity";
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 选择笔画类型

        private void Set_BrushType_ModelSelectEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();
            try
            {
                if (frm_ModuleObj.m_ModelImage != null)
                {
                    if (info.Contains("矩形"))
                    {
                        matchTool = new MatchTool(Main_Halcon, frm_ModuleObj.m_ModelImage, BrushType.矩形, hRegion);
                        matchTool.set_hwindow_brush();
                    }
                    else if (info.Contains("圆形"))
                    {
                        matchTool = new MatchTool(Main_Halcon, frm_ModuleObj.m_ModelImage, BrushType.圆形, hRegion);
                        matchTool.set_hwindow_brush();
                    }
                }
                else
                {
                    Log.Info("未输入模板图片！");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 选择涂抹方式

        private void Paint_ModelSelectEvent(object sender, RoutedEventArgs e)
        {
            try
            {

                string info = e.OriginalSource.ToString();
                if (info.Contains("涂抹"))
                {
                    matchTool.set_hwindow_paint(BrushAction.涂抹, contour_xld);
                }
                else if (info.Contains("擦除"))
                {
                    matchTool.set_hwindow_paint(BrushAction.擦除, contour_xld);
                }

                hRegion = new HRegion(matchTool.final_region);

                ModuleROI roi屏蔽范围 = new ModuleROI(frm_ModuleObj.ModuleParam.ModuleID.ToString(), frm_ModuleObj.ModuleParam.ModuleName.ToString(),
                       frm_ModuleObj.ModuleParam.ModuleDesCribe, ModuleRoiType.屏蔽范围.ToString(), "red", new HObject(hRegion));
                frm_ModuleObj.m_ModelImage.UpdateRoiList(roi屏蔽范围);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 重新学习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reStudy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateMath();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存屏蔽区域
                frm_ModuleObj.MatchdisableRegion = new UserDefinedShape_INFO(hRegion);
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 取消参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        private void CreateMath()
        {

            if (frm_ModuleObj.m_SearchRegion == null)
            {
                System.Windows.Forms.MessageBox.Show("请设置搜索区域！");
                return;
            }
            if (frm_ModuleObj.m_ModelRegion == null)
            {
                System.Windows.Forms.MessageBox.Show("请设置模板区域！");
                return;
            }
            if (frm_ModuleObj.m_ModelType == ModelType.None)
            {
                System.Windows.Forms.MessageBox.Show("未选择模板类型！");
                return;
            }

            try
            {

                //创建模板参数
                frm_ModuleObj.CreateMatch = CreateMatch;

                //查询模板参数
                frm_ModuleObj.FindMatch = FindMatch;
                frm_ModuleObj.FindMatch.angleStart = frm_ModuleObj.CreateMatch.angleStart;
                frm_ModuleObj.FindMatch.angleExtent = frm_ModuleObj.CreateMatch.angleExtent;
                frm_ModuleObj.FindMatch.scaleMax = frm_ModuleObj.CreateMatch.scaleMax;
                frm_ModuleObj.FindMatch.scaleMin = frm_ModuleObj.CreateMatch.scaleMin;


                if (frm_ModuleObj.m_ModelType == ModelType.形状模板)
                {
                    frm_ModuleObj.m_Model = new HShapeModel();
                }
                else if (frm_ModuleObj.m_ModelType == ModelType.灰度模板)
                {
                    frm_ModuleObj.m_Model = new HNCCModel();
                }

                HImage m_Image;

                if (hRegion != null)
                {
                    m_Image = frm_ModuleObj.m_ModelImage.ReduceDomain(new HRegion(hRegion));
                }
                else
                {
                    m_Image = frm_ModuleObj.m_ModelImage;
                }

                if (!frm_ModuleObj.m_Model.IsInitialized())
                {

                    //创建模板
                    SysVisionCore.CreateMatchModel(ModelType.形状模板, m_Image, ref frm_ModuleObj.m_Model, frm_ModuleObj.CreateMatch);
                    //查询模板
                    SysVisionCore.FindMatchModel(ModelType.形状模板, frm_ModuleObj.m_ModelImage, frm_ModuleObj.m_Model, 
                        frm_ModuleObj.FindMatch, out frm_ModuleObj.out_FindMathch);

                    frm_ModuleObj.m_PositionInfo.X = frm_ModuleObj.out_FindMathch.row;
                    frm_ModuleObj.m_PositionInfo.Y = frm_ModuleObj.out_FindMathch.column;
                    frm_ModuleObj.m_PositionInfo.Phi = frm_ModuleObj.out_FindMathch.angle;
                }

                //查询模板
                SysVisionCore.FindMatchModel(ModelType.形状模板, frm_ModuleObj.m_ModelImage, frm_ModuleObj.m_Model, frm_ModuleObj.FindMatch, out frm_ModuleObj.out_FindMathch);

                if (frm_ModuleObj.m_ModelType == ModelType.形状模板)
                {
                    //模板匹配，模板轮廓使用
                    HHomMat2D tempMat2D = new HHomMat2D();
                    tempMat2D.VectorAngleToRigid(0, 0, 0,
                        frm_ModuleObj.out_FindMathch.row.D, frm_ModuleObj.out_FindMathch.column.D, frm_ModuleObj.out_FindMathch.angle.D);

                    contour_xld = ((HShapeModel)this.frm_ModuleObj.m_Model).GetShapeModelContours(1);
                    contour_xld = contour_xld.AffineTransContourXld(tempMat2D);

                    ModuleROI roi检测结果 = new ModuleROI(frm_ModuleObj.ModuleParam.ModuleID.ToString(), frm_ModuleObj.ModuleParam.ModuleName.ToString(),
                        frm_ModuleObj.ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red", new HObject(contour_xld));
                    frm_ModuleObj.m_ModelImage.UpdateRoiList(roi检测结果);

                }

                DispImage();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 绘制窗体
        /// </summary>
        private void DispImage()
        {
            try
            {
                Main_Halcon.HobjectToHimage(frm_ModuleObj.m_ModelImage);
                List<ModuleROI> roiList = frm_ModuleObj.m_ModelImage.moduleROIlist.Where(c => c.ModuleID == frm_ModuleObj.ModuleParam.ModuleID.ToString()).ToList();
                foreach (ModuleROI roi in roiList)
                {
                    if (roi != null && roi.hObject.IsInitialized())
                    {
                        Main_Halcon.DispObj(roi.hObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 清除涂抹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Paint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //模板区域
                HTuple hv_width;
                HTuple hv_heigth;
                HOperatorSet.GetImageSize(frm_ModuleObj.m_ModelImage, out hv_width, out hv_heigth);
                hRegion = new HRegion(0, 0, hv_heigth - 1, hv_width - 1);
                matchTool.final_region = hRegion;

                ModuleROI roi屏蔽范围 = new ModuleROI(frm_ModuleObj.ModuleParam.ModuleID.ToString(), frm_ModuleObj.ModuleParam.ModuleName.ToString(),
                       frm_ModuleObj.ModuleParam.ModuleDesCribe, ModuleRoiType.屏蔽范围.ToString(), "red", new HObject(hRegion));
                frm_ModuleObj.m_ModelImage.UpdateRoiList(roi屏蔽范围);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
