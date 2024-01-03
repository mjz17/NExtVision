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
using HalconDotNet;
using PublicDefine;
using Common;

namespace Plugin.ImageAreaReg
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
            //加载参数
            frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
            //标题名称
            Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;
            InitCmbHwindow();//显示窗体
            InitCmbCalibration();//测量标定引用
            InitImageModel();//图像设置
            if (m_ModuleObjBase.blnNewModule)
                theSecondTime();
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            //设置选项
            SetCameraModel();
            //显示窗体
            Cmb_DispHwindow.Text = frm_ModuleObj.m_ModuleProject.ProjectInfo.m_DispHwindowName;
            //测量标定引用
            Cmb_Calibration.Text = frm_ModuleObj.m_MeasureCalibration;

            chk_DispImage.IsChecked = frm_ModuleObj.m_DispImage == Visibility.Visible ? true : false;
            chk_Measure.IsChecked = frm_ModuleObj.m_MeasureVis == Visibility.Visible ? true : false;
            DispImage = frm_ModuleObj.m_DispImage;
            MeasureVis = frm_ModuleObj.m_MeasureVis;
        }

        #region 显示窗体列表

        private void InitCmbHwindow()
        {
            List<string> Cmb_Hwindow = new List<string>();
            foreach (LayoutInfo item in SysLayout.Frm_Info)
            {
                Cmb_Hwindow.Add(item.Name);
            }
            Cmb_DispHwindow.ItemsSource = Cmb_Hwindow;
            Cmb_DispHwindow.SelectedIndex = 0;
        }

        #endregion

        #region 测量标定

        private void InitCmbCalibration()
        {
            List<string> CmbCalibration = new List<string>();

            foreach (Project pro in SysProcessPro.g_ProjectList)
            {
                IEnumerable<string> resultList = from datacell in pro.m_ModuleObjList
                                                 where datacell.ModuleParam.PluginName.Contains("测量标定")
                                                 select pro.ProjectInfo.m_ProjectName + "." + datacell.ModuleParam.ModuleName.ToString();

                CmbCalibration.AddRange(resultList.ToList());
            }

            Cmb_Calibration.ItemsSource = CmbCalibration;
        }

        #endregion

        #region 图像设置

        private void InitImageModel()
        {
            ImgAdjust_List.ItemsSource = Enum.GetNames(typeof(IMG_ADJUST));
            ImgAdjust_List.SelectedIndex = (int)frm_ModuleObj.m_ImgAdjust;
        }

        #endregion

        #region 指定图片

        //列表名称
        public bool Image_Check
        {
            get { return (bool)this.GetValue(Image_CheckProperty); }
            set { this.SetValue(Image_CheckProperty, value); }
        }

        public static readonly DependencyProperty Image_CheckProperty =
            DependencyProperty.Register("Image_Check", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 文件目录

        //指定图片
        public bool File_Check
        {
            get { return (bool)this.GetValue(File_CheckProperty); }
            set { this.SetValue(File_CheckProperty, value); }
        }

        public static readonly DependencyProperty File_CheckProperty =
            DependencyProperty.Register("File_Check", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 指定相机

        //指定相机
        public bool Camera_Check
        {
            get { return (bool)this.GetValue(Camera_CheckProperty); }
            set { this.SetValue(Camera_CheckProperty, value); }
        }

        public static readonly DependencyProperty Camera_CheckProperty =
            DependencyProperty.Register("Camera_Check", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        #endregion

        #region 模式选择

        private void SetCameraModel()
        {
            switch (frm_ModuleObj.m_CameraModel)
            {
                case CameraModel.指定图像:
                    Image_Check = true;
                    File_Check = false;
                    Camera_Check = false;
                    break;
                case CameraModel.文件目录:
                    Image_Check = false;
                    File_Check = true;
                    Camera_Check = false;
                    break;
                case CameraModel.相机:
                    Image_Check = false;
                    File_Check = false;
                    Camera_Check = true;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 采集模式的选择

        private void CameraMode_CameraModelEvent(object sender, RoutedEventArgs e)
        {

            Page_Change.Content = new Frame()
            {
                Content = null
            };

            string info = e.OriginalSource.ToString();

            if (info.Contains("指定图片"))
            {
                frm_ModuleObj.m_CameraModel = CameraModel.指定图像;
                SingleImage singleimage = new SingleImage(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = singleimage
                };
            }
            else if (info.Contains("文件目录"))
            {
                frm_ModuleObj.m_CameraModel = CameraModel.文件目录;
                FileDirectory folderList = new FileDirectory(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = folderList
                };
            }
            else if (info.Contains("相机"))
            {
                frm_ModuleObj.m_CameraModel = CameraModel.相机;
                CameraList camera = new CameraList(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = camera
                };
            }
        }

        #endregion

        #region 显示窗体Visibility

        public Visibility DispImage
        {
            get { return (Visibility)this.GetValue(DispImageProperty); }
            set { this.SetValue(DispImageProperty, value); }
        }

        public static readonly DependencyProperty DispImageProperty =
            DependencyProperty.Register("DispImage", typeof(Visibility), typeof(ModuleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

        #region 显示测量标定Visibility

        public Visibility MeasureVis
        {
            get { return (Visibility)this.GetValue(MeasureVisProperty); }
            set { this.SetValue(MeasureVisProperty, value); }
        }

        public static readonly DependencyProperty MeasureVisProperty =
            DependencyProperty.Register("MeasureVis", typeof(Visibility), typeof(ModuleFrm), new PropertyMetadata(default(Visibility)));

        #endregion

        /// <summary>
        /// 执行模块
        /// </summary>
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

                    m_ModuleObjBase.ExeModule(true);
                    Main_HalconView.HobjectToHimage(((List<HImageExt>)(((ModuleObj)m_ModuleObjBase).data_Var).m_DataValue)[0]);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 保存模块参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                if (ProtectModuel())
                {
                    ((ModuleObj)m_ModuleObjBase).blnNewModule = true; //保存参数
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 参数保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            //读取单张图片
            if (frm_ModuleObj.m_CameraModel == CameraModel.指定图像)
            {
                if (frm_ModuleObj.SingleImagePath == string.Empty)
                {
                    System.Windows.Forms.MessageBox.Show("未选择指定图像！");
                    string str = string.Format($"模块名称：{frm_ModuleObj.ModuleParam.ModuleName}{"，故障信息："}{"未选择指定图像！"}");
                    Log.Error(str);
                    return false;
                }
            }

            //读取文件夹
            if (frm_ModuleObj.m_CameraModel == CameraModel.文件目录)
            {
                if (frm_ModuleObj.files.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("未正确选择文件夹！");
                    string str = string.Format($"模块名称：{frm_ModuleObj.ModuleParam.ModuleName}{"，故障信息："}{"未正确选择文件夹！"}");
                    Log.Error(str);
                    return false;
                }
            }

            //SDK采集方式
            if (frm_ModuleObj.m_CameraModel == CameraModel.相机)
            {
                if (frm_ModuleObj.m_DeviceID == null)
                {
                    System.Windows.Forms.MessageBox.Show("请选择采集类型！");
                    string str = string.Format($"模块名称：{frm_ModuleObj.ModuleParam.ModuleName}{"，故障信息："}{"请选择采集类型！"}");
                    Log.Error(str);
                    return false;
                }
            }

            frm_ModuleObj.m_ModuleProject.ProjectInfo.m_DispHwindowName = Cmb_DispHwindow.Text;//保存显示图像窗体
            frm_ModuleObj.m_MeasureCalibration = Cmb_Calibration.Text;//保存测量标定引用

            frm_ModuleObj.m_DispImage = DispImage;
            frm_ModuleObj.m_IsDispImage = DispImage == Visibility.Visible ? true : false;//窗体是否显示图像

            frm_ModuleObj.m_MeasureVis = MeasureVis;

            return true;
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
        /// 图像调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgAdjust_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            switch (cmb.SelectedValue)
            {
                case "None":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.None;
                    break;
                case "垂直镜像":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.垂直镜像;
                    break;
                case "水平镜像":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.水平镜像;
                    break;
                case "顺时针90度":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.顺时针90度;
                    break;
                case "逆时针90度":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.逆时针90度;
                    break;
                case "旋转180度":
                    frm_ModuleObj.m_ImgAdjust = IMG_ADJUST.旋转180度;
                    break;
                default:
                    break;
            }
        }

        private void Chk_DispImg_Click(object sender, RoutedEventArgs e)
        {
            DispImage = DispImage == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Chk_Measure_Click(object sender, RoutedEventArgs e)
        {
            MeasureVis = MeasureVis == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
