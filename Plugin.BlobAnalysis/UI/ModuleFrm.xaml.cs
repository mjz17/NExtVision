using Common;
using ModuleDataVar;
using PublicDefine;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

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

                //二值化类型
                InitCmbThreshold();
                InitCmbShape();
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
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        public override void theFirsttime()
        {
            base.theFirsttime();
            #region 查询该流程中为图像的模块.

            if (frm_ModuleObj.GenModuleIndex(out string str, out frm_ModuleObj.m_Image, out frm_ModuleObj.Link_Image_Data))
            {
                CurrentImage = str;
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
            //图像模式
            InitImageModel();
            //ROI类型
            InitRoiType();
            ShowPagFrm((ThresholdModel)Cmb_Threshold.SelectedIndex);
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            #region 读取链接图像并显示

            CurrentImage = frm_ModuleObj.m_CurentImgName;
            if (CurrentImage != null && CurrentImage.Length > 0)
            {
                DataVar var = frm_ModuleObj.m_ModuleProject.GetCurLocalVarValue(frm_ModuleObj.Link_Image_Data);

                if (var.m_DataValue is List<HImageExt>)
                {
                    frm_ModuleObj.m_Image = ((List<HImageExt>)var.m_DataValue)[0];
                    DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                }
            }

            #endregion
            CurrentROI = frm_ModuleObj.m_CurentRoiName;
            CurrentIndex = frm_ModuleObj.m_CurentIndexName;
            //图像模式
            InitImageModel();
            //ROI类型
            InitRoiType();
            //筛选模式
            cmb_shapeModel.SelectedIndex = (int)cmb_shapeModel.SelectedIndex;
            ShowPagFrm(frm_ModuleObj.m_ThresholdModel);
            RefreshDgvblob();
            InitBaseMethod();
            RefreshDgvSelect();//特征筛选
        }

        /// <summary>
        /// 图像模式
        /// </summary>
        void InitImageModel()
        {
            //模式
            switch (frm_ModuleObj.m_WorkStation)
            {
                case WorkStation.图像模式:
                    Image_Model.IsChecked = true;
                    Region_Model.IsChecked = false;
                    break;
                case WorkStation.区域模式:
                    Image_Model.IsChecked = false;
                    Region_Model.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ROI类型
        /// </summary>
        void InitRoiType()
        {
            //RoI方式
            switch (frm_ModuleObj.m_ROIInfo)
            {
                case ROIInfo.ROI绘制:
                    Draw_RoI.IsChecked = true;
                    Link_RoI.IsChecked = false;
                    m_Visibility = Visibility.Collapsed;
                    break;
                case ROIInfo.ROI链接:
                    Draw_RoI.IsChecked = false;
                    Link_RoI.IsChecked = true;
                    m_Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化Cmb
        /// </summary>
        void InitCmbThreshold()
        {
            Cmb_Threshold.ItemsSource = Enum.GetNames(typeof(ThresholdModel));
            Cmb_Threshold.SelectedIndex = (int)frm_ModuleObj.m_ThresholdModel;
        }

        void InitCmbShape()
        {
            cmb_shapeModel.ItemsSource = Enum.GetNames(typeof(shapeModel));
            cmb_shapeModel.SelectedIndex = (int)frm_ModuleObj.m_shapeModel;
        }

        private void Cmb_Threshold_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_ThresholdModel = (ThresholdModel)Cmb_Threshold.SelectedIndex;
            ShowPagFrm((ThresholdModel)Cmb_Threshold.SelectedIndex);
        }

        private void cmb_shapeModel_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_shapeModel = (shapeModel)cmb_shapeModel.SelectedIndex;
        }

        void ShowPagFrm(ThresholdModel threshold)
        {
            Page_Change2.Content = new Frame()
            {
                Content = null
            };
            switch (threshold)
            {
                case ThresholdModel.二值化:
                    FrmThreshold frmThreadhold = new FrmThreshold(frm_ModuleObj);
                    Page_Change2.Content = new Frame()
                    {
                        Content = frmThreadhold
                    };
                    break;
                case ThresholdModel.自动二值化:
                    BinaryThreshold binThreshold = new BinaryThreshold(frm_ModuleObj);
                    Page_Change2.Content = new Frame()
                    {
                        Content = binThreshold
                    };
                    break;
                case ThresholdModel.均值二值化:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitBaseMethod()
        {
            foreach (BaseMethod item in frm_ModuleObj.m_Blob)
            {
                switch (item.m_blobMethod)
                {
                    case BlobMethod.连通:
                        item.m_Control = new ImageConnection(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.合并:
                        item.m_Control = new ImageUnion(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.补集:

                        break;
                    case BlobMethod.相减:

                        break;
                    case BlobMethod.相交:

                        break;
                    case BlobMethod.空洞填充:
                        item.m_Control = new ImageFill_up(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.闭运算:
                        item.m_Control = new ImageClosed(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.开运算:
                        item.m_Control = new ImageOpening(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.腐蚀:
                        item.m_Control = new ImageErosion(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.膨胀:
                        item.m_Control = new ImageDilation(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.特征筛选:
                        item.m_Control = new ImageSelect_shape(item);
                        item.frm_Obj = this;
                        break;
                    case BlobMethod.转换:
                        break;
                    case BlobMethod.矩形分割:
                        break;
                    case BlobMethod.动态分割:
                        break;
                    default:
                        break;
                }
            }
        }

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

        #region 模式设置

        private void Rad_Modelbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton radio = (System.Windows.Controls.RadioButton)sender;
            if (radio.Content.ToString().Contains("图像模式"))
            {
                frm_ModuleObj.m_WorkStation = WorkStation.图像模式;
            }
            else
            {
                frm_ModuleObj.m_WorkStation = WorkStation.区域模式;
            }
        }

        private void Rad_Drawbtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.RadioButton radio = (System.Windows.Controls.RadioButton)sender;
            if (radio.Content.ToString().Contains("绘制"))
            {
                frm_ModuleObj.m_ROIInfo = ROIInfo.ROI绘制;
                m_Visibility = Visibility.Collapsed;
            }
            else
            {
                frm_ModuleObj.m_ROIInfo = ROIInfo.ROI链接;
                m_Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region ROI

        public string CurrentROI
        {
            get { return (string)this.GetValue(CurrentROIProperty); }
            set { this.SetValue(CurrentROIProperty, value); }
        }

        public static readonly DependencyProperty CurrentROIProperty =
            DependencyProperty.Register("CurrentROI", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Roi_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.区域)
            {
                try
                {
                    frm_ModuleObj.Link_Roi_Data = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 索引

        public string CurrentIndex
        {
            get { return (string)this.GetValue(CurrentIndexProperty); }
            set { this.SetValue(CurrentIndexProperty, value); }
        }

        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register("CurrentIndex", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Index_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Int)
            {
                try
                {
                    frm_ModuleObj.Link_Index_Data = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 控件状态

        public Visibility m_Visibility
        {
            get { return (Visibility)this.GetValue(m_VisibilityProperty); }
            set { this.SetValue(m_VisibilityProperty, value); }
        }

        public static readonly DependencyProperty m_VisibilityProperty =
            DependencyProperty.Register("m_Visibility", typeof(Visibility), typeof(ModuleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

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

                this.dgv_RegionFeature.ItemsSource = null;
                this.dgv_RegionFeature.ItemsSource = frm_ModuleObj.m_OutRegionInfo;

                Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                Main_HalconView.DispObj(frm_ModuleObj.m_OutRegion);

                string out_str = frm_ModuleObj.Out_Result ? "OK" : "NG";
                string out_Color = frm_ModuleObj.Out_Result ? "blue" : "red";

                Main_HalconView.DispTxt("结果：" + out_str, out_Color);

                Main_HalconView.DispObj(frm_ModuleObj.m_DispRegion);
                Main_HalconView.DispObj(frm_ModuleObj.m_InputRegion);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        public override void SaveModuleParam()
        {
            try
            {
                if (ProtectModuel())
                {
                    //保存参数
                    ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
                }
                else
                {
                    return;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public override bool ProtectModuel()
        {
            //图像未设置
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像未设置！");
                return false;
            }

            ((ModuleObj)m_ModuleObjBase).m_CurentRoiName = CurrentROI;

            ((ModuleObj)m_ModuleObjBase).m_CurentIndexName = CurrentIndex;

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称
            return true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.MenuItem menu = sender as System.Windows.Controls.MenuItem;
                if (menu == null)
                    return;

                BlobMethod blob = (BlobMethod)Convert.ToInt32(menu.Tag);

                switch (blob)
                {
                    case BlobMethod.连通:
                        frm_ModuleObj.m_Blob.Add(new ConnectionImage(this));
                        break;
                    case BlobMethod.合并:
                        //frm_ModuleObj.m_Blob.Add(new UnionImage(this));
                        break;
                    case BlobMethod.补集:
                        break;
                    case BlobMethod.相减:
                        break;
                    case BlobMethod.相交:
                        break;
                    case BlobMethod.空洞填充:
                        frm_ModuleObj.m_Blob.Add(new Fill_upImage(this));
                        break;
                    case BlobMethod.闭运算:
                        frm_ModuleObj.m_Blob.Add(new ClosedImage(this));
                        break;
                    case BlobMethod.开运算:
                        frm_ModuleObj.m_Blob.Add(new OpeningImage(this));
                        break;
                    case BlobMethod.腐蚀:
                        frm_ModuleObj.m_Blob.Add(new ErosionImage(this));
                        break;
                    case BlobMethod.膨胀:
                        frm_ModuleObj.m_Blob.Add(new DilationImage(this));
                        break;
                    case BlobMethod.特征筛选:
                        frm_ModuleObj.m_Blob.Add(new Select_shapeImage(this));
                        break;
                    case BlobMethod.转换:
                        break;
                    case BlobMethod.矩形分割:
                        break;
                    case BlobMethod.动态分割:
                        break;
                    default:
                        break;
                }
                int index = dgv_Blob.SelectedIndex;
                RefreshDgvblob();
                if (index < 0)
                {
                    if (frm_ModuleObj.m_Blob.Count > 0)
                        index = dgv_Blob.SelectedIndex = 0;
                }
                else
                {
                    if (frm_ModuleObj.m_Blob.Count > 0)
                        dgv_Blob.SelectedIndex = index + 1;
                }

                if (index > -1)
                {
                    frm_ModuleObj.m_Blob[index].m_Index = index;//模块所在的索引位置
                    frm_ModuleObj.m_Blob[index].m_IndexLength = frm_ModuleObj.m_Blob.Count();//数据长度
                    //显示控件窗体
                    Page_Change.Content = new Frame()
                    {
                        Content = frm_ModuleObj.m_Blob[index].m_Control
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Delet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_Blob.SelectedIndex > -1)
                {
                    int index = dgv_Blob.SelectedIndex;
                    frm_ModuleObj.m_Blob.RemoveAt(dgv_Blob.SelectedIndex);
                    RefreshDgvblob();
                    if (index < 0)
                    {
                        if (frm_ModuleObj.m_Blob.Count > 0)
                            dgv_Blob.SelectedIndex = 0;
                    }
                    else
                    {
                        if (frm_ModuleObj.m_Blob.Count > 0)
                            dgv_Blob.SelectedIndex = index - 1;
                    }

                    if (dgv_Blob.SelectedIndex > -1)
                    {
                        //显示控件窗体
                        Page_Change.Content = new Frame()
                        {
                            Content = frm_ModuleObj.m_Blob[dgv_Blob.SelectedIndex].m_Control
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        private void RefreshDgvblob()
        {
            this.dgv_Blob.ItemsSource = null;
            this.dgv_Blob.ItemsSource = frm_ModuleObj.m_Blob;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.MenuItem menu = sender as System.Windows.Controls.MenuItem;
                if (menu == null)
                    return;

                SelectedshapeType type = (SelectedshapeType)Convert.ToInt32(menu.Tag);

                switch (type)
                {
                    case SelectedshapeType.总面积:
                        frm_ModuleObj.m_shapeInfo.Add(new SelectedShapeInfo() { m_shapeType = type });
                        break;
                    case SelectedshapeType.各个面积:
                        frm_ModuleObj.m_shapeInfo.Add(new SelectedShapeInfo() { m_shapeType = type });
                        break;
                    case SelectedshapeType.个数:
                        frm_ModuleObj.m_shapeInfo.Add(new SelectedShapeInfo() { m_shapeType = type });
                        break;
                    default:
                        break;
                }

                int index = dgv_Select.SelectedIndex;
                RefreshDgvSelect();
                if (index < 0)
                {
                    if (frm_ModuleObj.m_shapeInfo.Count > 0)
                        index = dgv_Select.SelectedIndex = 0;
                }
                else
                {
                    if (frm_ModuleObj.m_shapeInfo.Count > 0)
                        dgv_Select.SelectedIndex = index + 1;
                }


            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Select_Detele_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_Select.SelectedIndex > -1)
                {
                    int index = dgv_Select.SelectedIndex;
                    frm_ModuleObj.m_shapeInfo.RemoveAt(dgv_Select.SelectedIndex);
                    RefreshDgvSelect();
                    if (index < 0)
                    {
                        if (frm_ModuleObj.m_shapeInfo.Count > 0)
                            dgv_Select.SelectedIndex = 0;
                    }
                    else
                    {
                        if (frm_ModuleObj.m_shapeInfo.Count > 0)
                            dgv_Select.SelectedIndex = index - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        private void RefreshDgvSelect()
        {
            this.dgv_Select.ItemsSource = null;
            this.dgv_Select.ItemsSource = frm_ModuleObj.m_shapeInfo;
        }

        private void dgv_Blob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dgv_Blob.SelectedIndex > -1)
            {
                int index = dgv_Blob.SelectedIndex;
                frm_ModuleObj.m_Blob[index].m_Index = index;//模块所在的索引位置
                frm_ModuleObj.m_Blob[index].m_IndexLength = frm_ModuleObj.m_Blob.Count();//数据长度
                Page_Change.Content = new Frame()
                {
                    Content = frm_ModuleObj.m_Blob[index].m_Control
                };

                //判断是否启用
                Int32 Col = this.dgv_Blob.Columns.IndexOf(this.dgv_Blob.CurrentColumn);//获取列位置
                if (Col > -1)
                {
                    string name = dgv_Blob.Columns[Col].Header.ToString();
                    if (name.Contains("启用"))
                    {
                        frm_ModuleObj.m_Blob[index].m_EnableOrnot = frm_ModuleObj.m_Blob[index].m_EnableOrnot ? false : true;
                    }
                }

                RefreshDgvblob();
                dgv_Blob.SelectedIndex = index;
            }
            e.Handled = true;
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

        /// <summary>
        /// 是否排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chk_IsPort_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            frm_ModuleObj.IsSort = chk.IsChecked == true ? true : false;
        }

        /// <summary>
        /// 正序/反序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chk_Order_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            frm_ModuleObj.Order = chk.IsChecked == true ? true : false;
        }


    }
}

