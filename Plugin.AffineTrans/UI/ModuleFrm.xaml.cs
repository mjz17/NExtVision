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
using ModuleDataVar;

namespace Plugin.AffineTrans
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
                //检测结果
                DispResult = frm_ModuleObj.m_IsDispResult;
                //链接的数据类型
                rad_single.IsChecked = frm_ModuleObj.m_DataType;
                rad_single.IsChecked = !frm_ModuleObj.m_DataType;

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
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
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
            if (frm_ModuleObj.m_DataType)
            {
                IndexVis = Visibility.Visible;
                Link_DataType = "Double_Array";
            }
            else
            {
                IndexVis = Visibility.Collapsed;
                Link_DataType = "Double";
            }
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

            if (frm_ModuleObj.m_DataType)
            {
                rad_array.IsChecked = true;
                rad_single.IsChecked = false;
                IndexVis = Visibility.Visible;
                Link_Index = frm_ModuleObj.m_Index;
                Link_DataType = "Double_Array";
            }
            else
            {
                rad_array.IsChecked = false;
                rad_single.IsChecked = true;
                IndexVis = Visibility.Collapsed;
                Link_DataType = "Double";
            }

            Link_Row = frm_ModuleObj.m_Row;
            Link_Colum = frm_ModuleObj.m_Colum;
            Link_Angle = frm_ModuleObj.m_Phi;
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

        #region 链接数据类型

        public string Link_DataType
        {
            get { return (string)this.GetValue(Link_DataTypeProperty); }
            set { this.SetValue(Link_DataTypeProperty, value); }
        }

        public static readonly DependencyProperty Link_DataTypeProperty =
            DependencyProperty.Register("Link_DataType", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 链接Row

        public string Link_Row
        {
            get { return (string)this.GetValue(Link_RowProperty); }
            set { this.SetValue(Link_RowProperty, value); }
        }

        public static readonly DependencyProperty Link_RowProperty =
            DependencyProperty.Register("Link_Row", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_RowProperty_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                if (data.m_DataValue != null)
                {
                    frm_ModuleObj.Link_Row_data = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 链接Colum

        public string Link_Colum
        {
            get { return (string)this.GetValue(Link_ColumProperty); }
            set { this.SetValue(Link_ColumProperty, value); }
        }

        public static readonly DependencyProperty Link_ColumProperty =
            DependencyProperty.Register("Link_Colum", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_ColumProperty_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                if (data.m_DataValue != null)
                {
                    frm_ModuleObj.Link_Colum_data = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 链接Phi

        public string Link_Angle
        {
            get { return (string)this.GetValue(Link_AngleProperty); }
            set { this.SetValue(Link_AngleProperty, value); }
        }

        public static readonly DependencyProperty Link_AngleProperty =
            DependencyProperty.Register("Link_Angle", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_AngleProperty_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                if (data.m_DataValue != null)
                {
                    frm_ModuleObj.Link_Phi_data = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 链接索引

        public Visibility IndexVis
        {
            get { return (Visibility)this.GetValue(IndexVisProperty); }
            set { this.SetValue(IndexVisProperty, value); }
        }

        public static readonly DependencyProperty IndexVisProperty =
            DependencyProperty.Register("IndexVis", typeof(Visibility), typeof(ModuleFrm), new PropertyMetadata(default(Visibility)));

        public string Link_Index
        {
            get { return (string)this.GetValue(Link_IndexProperty); }
            set { this.SetValue(Link_IndexProperty, value); }
        }

        public static readonly DependencyProperty Link_IndexProperty =
            DependencyProperty.Register("Link_Index", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Link_IndexProperty_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
                if (data.m_DataValue != null)
                {
                    frm_ModuleObj.Link_Index_data = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 显示设置

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
                if (ProtectModuel())
                {
                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    frm_ModuleObj.m_ArrayNum = 0;
                    //运行模块
                    frm_ModuleObj.ExeModule();
                    //刷新窗体
                    DispHwImg.UpdateWindow(frm_ModuleObj, Main_HalconView);
                }
            }
            catch (Exception ex)
            {
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
                    frm_ModuleObj.m_ArrayNum = 0;
                    this.Close();
                }
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

            //Row未设置
            if (frm_ModuleObj.Link_Row_data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("原点X未设置！");
                return false;

            }
            //Col未设置
            if (frm_ModuleObj.Link_Colum_data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("原点Y未设置！");
                return false;

            }
            //Phi未设置
            if (frm_ModuleObj.Link_Phi_data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("角度未设置！");
                return false;
            }

            //位置补正坐标ROW
            ((ModuleObj)m_ModuleObjBase).m_Row = Link_Row;
            //位置补正坐标Col
            ((ModuleObj)m_ModuleObjBase).m_Colum = Link_Colum;
            //位置补正坐标Phi
            ((ModuleObj)m_ModuleObjBase).m_Phi = Link_Angle;
            //位置补正坐标Phi
            ((ModuleObj)m_ModuleObjBase).m_Index = Link_Index;

            return true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            if (chk.IsChecked == true)
            {
                frm_ModuleObj.m_IsDispResult = true;
            }
            if (chk.IsChecked == false)
            {
                frm_ModuleObj.m_IsDispResult = false;
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

        private void rad_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rad = (RadioButton)sender;
            if (rad.Name.Contains("rad_single"))//单量
            {
                Link_Row = string.Empty;
                Link_Colum = string.Empty;
                Link_Angle = string.Empty;
                Link_Index = string.Empty;

                frm_ModuleObj.m_DataType = false;
                Link_DataType = "Double";
                IndexVis = Visibility.Collapsed;
            }
            else//数组
            {
                Link_Row = string.Empty;
                Link_Colum = string.Empty;
                Link_Angle = string.Empty;
                Link_Index = string.Empty;

                frm_ModuleObj.m_DataType = true;
                Link_DataType = "Double_Array";
                IndexVis = Visibility.Visible;
            }
        }




    }
}
