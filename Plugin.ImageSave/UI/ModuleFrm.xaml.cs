using Common;
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

namespace Plugin.ImageSave
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

                InitCmbFormat();
                rad_SaveImage.IsChecked = frm_ModuleObj.m_SaveInfo == SaveInfo.保存图片 ? true : false;
                rad_SaveHwindow.IsChecked = frm_ModuleObj.m_SaveInfo == SaveInfo.截取窗体 ? true : false;

                if (frm_ModuleObj.blnNewModule)
                    theSecondTime();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 非首次使用
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();

            //链接图片
            CurrentImage = frm_ModuleObj.m_CurentImgName;

            //保存路径
            this.txt_SavePath.Text = frm_ModuleObj.SavePath;

            //是否清理文件
            Chk_FileStatus.IsChecked = frm_ModuleObj.m_IsClearFile;

            //清除时间
            ClearFileTime = frm_ModuleObj.m_ClearTime.ToString();

            //是否等待图片保存完成
            this.Chk_ImageSave.IsChecked = frm_ModuleObj.m_WaitImageSave;

            //图像根目录E
            ImageDirectory = frm_ModuleObj.m_SaveName;

        }

        private void InitCmbFormat()
        {
            this.Cmb_SaveFormat.ItemsSource = Enum.GetNames(typeof(Imageformat));
            this.Cmb_SaveFormat.SelectedIndex = (int)frm_ModuleObj.m_Imageformat;
        }

        #region 链接图片

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
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 保存文件夹清除时间

        public string ClearFileTime
        {
            get { return (string)this.GetValue(ClearFileTimeProperty); }
            set { this.SetValue(ClearFileTimeProperty, value); }
        }

        public static readonly DependencyProperty ClearFileTimeProperty =
            DependencyProperty.Register("ClearFileTime", typeof(string), typeof(ModuleFrm), new PropertyMetadata("5"));

        #endregion

        #region 图像根目录名称

        public string ImageDirectory
        {
            get { return (string)this.GetValue(ImageDirectoryProperty); }
            set { this.SetValue(ImageDirectoryProperty, value); }
        }

        public static readonly DependencyProperty ImageDirectoryProperty =
            DependencyProperty.Register("ImageDirectory", typeof(string), typeof(ModuleFrm), new PropertyMetadata("图像"));

        #endregion

        /// <summary>
        /// 文件夹搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SearchFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_SavePath.Text = frm_ModuleObj.SavePath = dialog.SelectedPath;
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
                    this.Close();
                }
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

            //判断图像类型
            if (!(frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>))
            {
                System.Windows.Forms.MessageBox.Show("图像类型错误！");
                return false;
            }

            //图像名称
            frm_ModuleObj.m_CurentImgName = CurrentImage;

            //是否清理文件
            frm_ModuleObj.m_IsClearFile = Chk_FileStatus.IsChecked == true ? true : false;

            //清除时间
            if (!int.TryParse(ClearFileTime, out int Filenum))
            {
                System.Windows.Forms.MessageBox.Show("请正确设置清除时间！");
                return false;
            }

            //清除时间设置
            frm_ModuleObj.m_ClearTime = Filenum;

            //图像根目录
            if (ImageDirectory.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("图像根目录未设置！");
                return false;
            }

            //图像根目录
            frm_ModuleObj.m_SaveName = ImageDirectory;

            //图片是否保存完成
            frm_ModuleObj.m_WaitImageSave = (bool)this.Chk_ImageSave.IsChecked ? true : false;

            //图像格式
            frm_ModuleObj.m_Imageformat = (Imageformat)Cmb_SaveFormat.SelectedIndex;

            return true;
        }

        private void Chk_Click(object sender, RoutedEventArgs e)
        {
            RadioButton chk = (RadioButton)sender;
            if (chk.Content.ToString().Contains("保存图片"))
            {
                frm_ModuleObj.m_SaveInfo = chk.IsChecked == true ? SaveInfo.保存图片 : SaveInfo.截取窗体;
            }
            else
            {
                frm_ModuleObj.m_SaveInfo = chk.IsChecked == true ? SaveInfo.截取窗体 : SaveInfo.保存图片;
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
