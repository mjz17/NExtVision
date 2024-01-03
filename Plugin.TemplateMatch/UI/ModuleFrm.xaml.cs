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
using Common;
using PublicDefine;
using HalconDotNet;
using DefineImgRoI;
using System.Threading;
using ModuleDataVar;

namespace Plugin.TemplateMatch
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {
        //窗体对应参数
        private ModuleObj frm_ModuleObj;

        //搜索区域
        List<ViewWindow.Model.ROI> SeachRegion;

        //当前窗体
        private string TabName = string.Empty;

        //绘制的ROI
        private RepaintROI matchRepaint = new RepaintROI();

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

                DispSearch = frm_ModuleObj.m_IsDispSearch;
                DispDirect = frm_ModuleObj.m_IsDispDirect;
                DispResult = frm_ModuleObj.m_IsDispResult;

                if (!m_ModuleObjBase.blnNewModule)
                {
                    theFirsttime();
                }
                else
                {
                    theSecondTime();
                }
                this.Main_HalconView.hWindowControl.MouseUp += HWindowControl_MouseUp;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 首次打开
        /// </summary>
        public override void theFirsttime()
        {
            base.theFirsttime();
            #region 使用默认参数

            CreateMatch = new CreateMatchPram();
            FindMatch = new FindMatchPram();

            #endregion
            #region 查询该流程中为图像的模块.

            if (frm_ModuleObj.GenModuleIndex(out string str, out frm_ModuleObj.m_Image, out frm_ModuleObj.Link_Image_Data))
            {
                CurrentImage = str;
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
        }

        /// <summary>
        /// 非首次打开
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();
            CreateMatch = frm_ModuleObj.CreateMatch;//创建模板匹配参数
            FindMatch = frm_ModuleObj.FindMatch;//查询模板匹配参数

            matchRepaint.MatchRoIInfo = frm_ModuleObj.MatchRoIInfo;

            //匹配方式
            switch (frm_ModuleObj.m_ModelType)
            {
                case ModelType.None:
                    break;
                case ModelType.形状模板:
                    ShapeMatch = true;
                    GrayMatch = false;
                    break;
                case ModelType.灰度模板:
                    ShapeMatch = false;
                    GrayMatch = true;
                    break;
                default:
                    break;
            }

            //搜索区域方式
            switch (frm_ModuleObj.SearchModel)
            {
                case RoILink.手动区域:
                    HandSerach = true;
                    LinkSerach = false;
                    break;
                case RoILink.链接区域:
                    HandSerach = false;
                    LinkSerach = true;
                    break;
                default:
                    break;
            }

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

            #region 模板图像

            //路径+项目名称+模块名称
            string str = SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName + frm_ModuleObj.ModuleParam.ModuleName;
            int index = SysProcessPro.Cur_Project.m_RegistImg.FindIndex(c => c.m_Image_ID == str);

            if (index > -1)
            {
                HImage img = SysProcessPro.Cur_Project.m_RegistImg[index].m_Image;
                Main_HalconView1.HobjectToHimage(img);
            }

            #endregion

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

        #region 创建模板参数

        public CreateMatchPram CreateMatch
        {
            get { return (CreateMatchPram)this.GetValue(CreateMatchProperty); }
            set { this.SetValue(CreateMatchProperty, value); }
        }

        public static readonly DependencyProperty CreateMatchProperty =
            DependencyProperty.Register("CreateMatch", typeof(CreateMatchPram), typeof(ModuleFrm), new PropertyMetadata(default(CreateMatchPram)));

        #endregion

        #region 查询模板参数

        public FindMatchPram FindMatch
        {
            get { return (FindMatchPram)this.GetValue(FindMatchProperty); }
            set { this.SetValue(FindMatchProperty, value); }
        }

        public static readonly DependencyProperty FindMatchProperty =
            DependencyProperty.Register("FindMatch", typeof(FindMatchPram), typeof(ModuleFrm), new PropertyMetadata(default(FindMatchPram)));

        #endregion

        #region 选择模板类型

        private void MatchMethod_ModelSelectEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();

            if (info.Contains("形状模板"))
            {
                ShapeMatch = true;
                GrayMatch = false;
                frm_ModuleObj.m_ModelType = ModelType.形状模板;
            }
            else if (info.Contains("灰度模板"))
            {
                ShapeMatch = false;
                GrayMatch = true;
                frm_ModuleObj.m_ModelType = ModelType.灰度模板;
            }
        }

        #endregion

        #region 匹配方式

        public bool ShapeMatch
        {
            get { return (bool)this.GetValue(ShapeMatchProperty); }
            set { this.SetValue(ShapeMatchProperty, value); }
        }

        public static readonly DependencyProperty ShapeMatchProperty =
            DependencyProperty.Register("ShapeMatch", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool GrayMatch
        {
            get { return (bool)this.GetValue(GrayMatchProperty); }
            set { this.SetValue(GrayMatchProperty, value); }
        }

        public static readonly DependencyProperty GrayMatchProperty =
            DependencyProperty.Register("GrayMatch", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region ROI链接方式

        private void MatchRegionInfo_LinkModelEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                return;
            }
            Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
            List<double> data = new List<double>();
            if (info == "手动输入")
            {
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
                frm_ModuleObj.SearchModel = RoILink.手动区域;
                HandSerach = true;
                LinkSerach = false;
            }
            else if (info == "链接区域")
            {
                frm_ModuleObj.SearchModel = RoILink.链接区域;
                HandSerach = false;
                LinkSerach = true;
            }
        }

        #endregion

        #region 搜索区域创建方式

        public bool HandSerach
        {
            get { return (bool)this.GetValue(HandSerachProperty); }
            set { this.SetValue(HandSerachProperty, value); }
        }

        public static readonly DependencyProperty HandSerachProperty =
            DependencyProperty.Register("HandSerach", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool LinkSerach
        {
            get { return (bool)this.GetValue(LinkSerachProperty); }
            set { this.SetValue(LinkSerachProperty, value); }
        }

        public static readonly DependencyProperty LinkSerachProperty =
            DependencyProperty.Register("LinkSerach", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 搜索区域ROI坐标信息

        public string Row_X
        {
            get { return (string)this.GetValue(Row_X_Property); }
            set { this.SetValue(Row_X_Property, value); }
        }

        public static readonly DependencyProperty Row_X_Property =
            DependencyProperty.Register("Row_X", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Row_Y
        {
            get { return (string)this.GetValue(Row_Y_Property); }
            set { this.SetValue(Row_Y_Property, value); }
        }

        public static readonly DependencyProperty Row_Y_Property =
            DependencyProperty.Register("Row_Y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Colum_X
        {
            get { return (string)this.GetValue(Colum_X_Property); }
            set { this.SetValue(Colum_X_Property, value); }
        }

        public static readonly DependencyProperty Colum_X_Property =
            DependencyProperty.Register("Colum_X", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Colum_Y
        {
            get { return (string)this.GetValue(Colum_Y_Property); }
            set { this.SetValue(Colum_Y_Property, value); }
        }

        public static readonly DependencyProperty Colum_Y_Property =
            DependencyProperty.Register("Colum_Y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        private void HWindowControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (TabName.Contains("基本参数"))
            {
                //搜索区域  
                List<double> Seach_data = new List<double>();
                int index;
                Main_HalconView.viewWindow.smallestActiveROI(out Seach_data, out index);
                if (index >= 0)
                {
                    matchRepaint.UpdateROI(RoIModel.搜索区域, RoIType.方行, Seach_data);
                    frm_ModuleObj.m_SearchRegion = new Rectangle_INFO(Seach_data[0], Seach_data[1], Seach_data[2], Seach_data[3]);//将参数保存
                    Row_X = Seach_data[0].ToString("f3");
                    Row_Y = Seach_data[1].ToString("f3");
                    Colum_X = Seach_data[2].ToString("f3");
                    Colum_Y = Seach_data[3].ToString("f3");
                }
            }
            else if (TabName.Contains("参数设置"))
            {
                //搜索区域  
                List<double> Seach_data = new List<double>();
                int index;
                Main_HalconView.viewWindow.smallestActiveROI(out Seach_data, out index);
                if (index >= 0)
                {
                    matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.方行, Seach_data);
                    frm_ModuleObj.m_ModelRegion = new Rectangle_INFO(Seach_data[0], Seach_data[1], Seach_data[2], Seach_data[3]);
                }
            }
        }

        #region 窗体切换

        private void tab_Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var TabItemdata = this.tab_Control.SelectedItem as TabItem;
                TabName = TabItemdata.Header.ToString();
                try
                {
                    if (TabName.ToString().Contains("基本参数"))
                    {
                        #region 基础参数

                        if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                        {
                            return;
                        }

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        if (frm_ModuleObj.SearchModel != RoILink.None)
                        {
                            List<double> data = new List<double>();

                            if (matchRepaint.QueryRoI(RoIModel.搜索区域, RoIType.方行, ref data))
                            {
                                Main_HalconView.viewWindow.removeActiveROI(ref SeachRegion);
                                Main_HalconView.viewWindow.genRect1(data[0], data[1], data[2], data[3], ref this.SeachRegion);
                            }
                            else
                            {
                                Main_HalconView.viewWindow.genRect1(110.0, 110.0, 250.0, 250.0, ref this.SeachRegion);
                                data.Add(110.0);
                                data.Add(110.0);
                                data.Add(250);
                                data.Add(250.0);
                                matchRepaint.UpdateROI(RoIModel.搜索区域, RoIType.方行, data);
                            }
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

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        //显示搜索区域
                        List<double> searchdata = new List<double>();
                        if (matchRepaint.QueryRoI(RoIModel.搜索区域, RoIType.方行, ref searchdata))
                        {
                            Rectangle_INFO rectangle = new Rectangle_INFO(searchdata[0], searchdata[1], searchdata[2], searchdata[3]);
                            Main_HalconView.DispObj(rectangle.GenRegion(), "blue");
                        }

                        //模板区域
                        List<double> modeldata = new List<double>();
                        if (matchRepaint.QueryRoI(RoIModel.模板区域, RoIType.方行, ref modeldata))
                        {
                            Main_HalconView.viewWindow.genRect1(modeldata[0], modeldata[1], modeldata[2], modeldata[3], ref this.SeachRegion);
                        }
                        else
                        {
                            Main_HalconView.viewWindow.genRect1(110.0, 110.0, 250.0, 250.0, ref this.SeachRegion);

                            modeldata.Add(110.0);
                            modeldata.Add(110.0);
                            modeldata.Add(250);
                            modeldata.Add(250.0);
                            matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.方行, modeldata);
                        }

                        #endregion
                    }
                    else if (TabName.ToString().Contains("数据结果"))
                    {
                        #region 数据结果

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

                        //显示模板区域
                        List<double> modeldata = new List<double>();
                        if (matchRepaint.QueryRoI(RoIModel.模板区域, RoIType.方行, ref modeldata))
                        {
                            Rectangle_INFO rectangle = new Rectangle_INFO(modeldata[0], modeldata[1], modeldata[2], modeldata[3]);
                            Main_HalconView.DispObj(rectangle.GenRegion(), "green");
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                    Log.Error(ex.ToString());
                }
            }));
        }

        #endregion

        #region 查询模板后输出坐标信息

        public string Row
        {
            get { return (string)this.GetValue(RowProperty); }
            set { this.SetValue(RowProperty, value); }
        }

        public static readonly DependencyProperty RowProperty =
            DependencyProperty.Register("Row", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Colum
        {
            get { return (string)this.GetValue(ColumProperty); }
            set { this.SetValue(ColumProperty, value); }
        }

        public static readonly DependencyProperty ColumProperty =
            DependencyProperty.Register("Colum", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Angle
        {
            get { return (string)this.GetValue(AngleProperty); }
            set { this.SetValue(AngleProperty, value); }
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Scale
        {
            get { return (string)this.GetValue(ScaleProperty); }
            set { this.SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Score
        {
            get { return (string)this.GetValue(ScoreProperty); }
            set { this.SetValue(ScoreProperty, value); }
        }

        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register("Score", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 模块显示设置

        public bool DispSearch
        {
            get { return (bool)this.GetValue(DispSearchProperty); }
            set { this.SetValue(DispSearchProperty, value); }
        }

        public static readonly DependencyProperty DispSearchProperty =
            DependencyProperty.Register("DispSearch", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool DispDirect
        {
            get { return (bool)this.GetValue(DispDirectProperty); }
            set { this.SetValue(DispDirectProperty, value); }
        }

        public static readonly DependencyProperty DispDirectProperty =
            DependencyProperty.Register("DispDirect", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool DispResult
        {
            get { return (bool)this.GetValue(DispResultProperty); }
            set { this.SetValue(DispResultProperty, value); }
        }

        public static readonly DependencyProperty DispResultProperty =
            DependencyProperty.Register("DispResult", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        public override void ExModule()
        {
            try
            {
                if (ProtectModuel())
                {
                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    frm_ModuleObj.ExeModule();
                    DispHwImg.UpdateWindow(frm_ModuleObj, Main_HalconView);

                    Row = frm_ModuleObj.out_FindMathch.column.ToString();
                    Colum = frm_ModuleObj.out_FindMathch.row.ToString();
                    Angle = frm_ModuleObj.out_FindMathch.angle.ToString();
                    Scale = frm_ModuleObj.out_FindMathch.scale.ToString();
                    Score = frm_ModuleObj.out_FindMathch.score.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //保存参数
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

        /// <summary>
        /// 保护函数
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

            if (frm_ModuleObj.m_SearchRegion == null)
            {
                System.Windows.Forms.MessageBox.Show("请设置搜索区域！");
                return false;
            }

            ((ModuleObj)m_ModuleObjBase).m_SearchRegion = frm_ModuleObj.m_SearchRegion;//设置搜索区域

            if (frm_ModuleObj.m_ModelRegion == null)
            {
                System.Windows.Forms.MessageBox.Show("请设置模板区域！");
                return false;
            }

            ((ModuleObj)m_ModuleObjBase).m_ModelRegion = frm_ModuleObj.m_ModelRegion;//设置模板区域

            if (frm_ModuleObj.m_ModelType == ModelType.None)
            {
                System.Windows.Forms.MessageBox.Show("未选择模板类型！");
                return false;
            }

            if (HandSerach)
            {
                ((ModuleObj)m_ModuleObjBase).SearchModel = RoILink.手动区域;
            }
            if (LinkSerach)
            {
                ((ModuleObj)m_ModuleObjBase).SearchModel = RoILink.链接区域;
            }

            frm_ModuleObj.MatchRoIInfo = matchRepaint.MatchRoIInfo;//保存参数
            frm_ModuleObj.FindMatch = FindMatch;//写入查询模板参数


            return base.ProtectModuel();
        }

        public override void CancelModuleParam()
        {
            this.Main_HalconView.hWindowControl.MouseUp -= HWindowControl_MouseUp;
            base.CancelModuleParam();
        }

        /// <summary>
        /// 模板学习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Match_learn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("点击确定后,执行模板创建",
                "提示信息", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    HImage m_ModuelImage = frm_ModuleObj.m_Image.ReduceDomain(frm_ModuleObj.m_ModelRegion.GenRegion()).CropDomain();
                    frm_ModuleObj.m_ModelImage = new HImageExt(m_ModuelImage);

                    Main_HalconView1.HobjectToHimage(frm_ModuleObj.m_ModelImage);

                    //保存的地址
                    //ID
                    string ID = SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName + frm_ModuleObj.ModuleParam.ModuleName;
                    RegisterIMG_Info regImg = new RegisterIMG_Info(ID, frm_ModuleObj.m_ModelImage, SysProcessPro.SysFliePath);

                    //判断是否需要
                    int index = SysProcessPro.Cur_Project.m_RegistImg.FindIndex(c => c.m_Image_ID == ID);
                    if (index > -1)
                    {
                        SysProcessPro.Cur_Project.m_RegistImg[index] = regImg;
                        //保存图片
                        SysProcessPro.Cur_Project.m_RegistImg[index].m_Image.WriteImageExt(SysProcessPro.SysFliePath + "\\" + ID + ".png");
                        //将原图保存
                        frm_ModuleObj.m_Image.WriteImageExt(SysProcessPro.SysFliePath + "\\原图" + ID + ".png");
                    }
                    else
                    {
                        SysProcessPro.Cur_Project.m_RegistImg.Add(regImg);
                        //保存图片
                        SysProcessPro.Cur_Project.m_RegistImg[0].m_Image.WriteImageExt(SysProcessPro.SysFliePath + "\\" + ID + ".png");
                        //将原图保存
                        frm_ModuleObj.m_Image.WriteImageExt(SysProcessPro.SysFliePath + "\\原图" + ID + ".png");
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                    Log.Error(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 模板编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Match_Editor_Click(object sender, RoutedEventArgs e)
        {
            MathParam math = new MathParam(frm_ModuleObj);
            math.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            switch (chk.Content)
            {
                case "显示搜索区域":
                    if (chk.IsChecked == true)
                    {
                        frm_ModuleObj.m_IsDispSearch = true;
                    }
                    if (chk.IsChecked == false)
                    {
                        frm_ModuleObj.m_IsDispSearch = false;
                    }
                    break;
                case "显示匹配位置及区域":
                    if (chk.IsChecked == true)
                    {
                        frm_ModuleObj.m_IsDispDirect = true;
                    }
                    if (chk.IsChecked == false)
                    {
                        frm_ModuleObj.m_IsDispDirect = false;
                    }
                    break;
                case "显示结果轮廓":
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
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
