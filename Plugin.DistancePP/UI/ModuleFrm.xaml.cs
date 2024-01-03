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
using HalconDotNet;
using DefineImgRoI;
using ModuleDataVar;

namespace Plugin.DistancePP
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private HXLDCont hxld_Point1 = new HXLDCont();

        private HXLDCont hxld_Point2 = new HXLDCont();

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
        }

        //第二次打开
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
            #region 输入点1.x

            Point1_x = frm_ModuleObj.m_Point1_x;
            if (frm_ModuleObj.m_Point1_x != null)
            {
                if (frm_ModuleObj.m_Point1_x.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_Pont1_x_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Pont1_x_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_Pont1_x_Data.m_DataModuleID);
                    frm_ModuleObj.Pont1_x_Data = (double)frm_ModuleObj.Link_Pont1_x_Data.m_DataValue;
                }
            }

            #endregion
            #region 输入点1.y

            Point1_y = frm_ModuleObj.m_Point1_y;
            if (frm_ModuleObj.m_Point1_y != null)
            {
                if (frm_ModuleObj.m_Point1_y.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_Pont1_y_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Pont1_y_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_Pont1_y_Data.m_DataModuleID);
                    frm_ModuleObj.Pont1_y_Data = (double)frm_ModuleObj.Link_Pont1_y_Data.m_DataValue;
                }
            }

            #endregion
            #region 输入点2.x

            Point2_x = frm_ModuleObj.m_Point2_x;
            if (frm_ModuleObj.m_Point2_x != null)
            {
                if (frm_ModuleObj.m_Point2_x.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_Pont2_x_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Pont2_x_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_Pont2_x_Data.m_DataModuleID);
                    frm_ModuleObj.Pont2_x_Data = (double)frm_ModuleObj.Link_Pont2_x_Data.m_DataValue;
                }
            }

            #endregion
            #region 输入点2.y

            Point2_y = frm_ModuleObj.m_Point2_y;
            if (frm_ModuleObj.m_Point2_y != null)
            {
                if (frm_ModuleObj.m_Point2_y.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_Pont2_y_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Pont2_y_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_Pont2_y_Data.m_DataModuleID);
                    frm_ModuleObj.Pont2_y_Data = (double)frm_ModuleObj.Link_Pont2_y_Data.m_DataValue;
                }
            }

            #endregion
            #region 是否转换

            converValue = frm_ModuleObj.ConverValue;

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

        #region 输入点1.x

        public string Point1_x
        {
            get { return (string)this.GetValue(Point1_xProperty); }
            set { this.SetValue(Point1_xProperty, value); }
        }

        public static readonly DependencyProperty Point1_xProperty =
            DependencyProperty.Register("Point1_x", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Point1_x(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_Pont1_x_Data = data;
                    frm_ModuleObj.Pont1_x_Data = (double)frm_ModuleObj.Link_Pont1_x_Data.m_DataValue;//点1.X坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入点1.y

        public string Point1_y
        {
            get { return (string)this.GetValue(Point1_yProperty); }
            set { this.SetValue(Point1_yProperty, value); }
        }

        public static readonly DependencyProperty Point1_yProperty =
            DependencyProperty.Register("Point1_y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Point1_y(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_Pont1_y_Data = data;
                    frm_ModuleObj.Pont1_y_Data = (double)frm_ModuleObj.Link_Pont1_y_Data.m_DataValue;//点1.X坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入点2.x

        public string Point2_x
        {
            get { return (string)this.GetValue(Point2_xProperty); }
            set { this.SetValue(Point2_xProperty, value); }
        }

        public static readonly DependencyProperty Point2_xProperty =
            DependencyProperty.Register("Point2_x", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Point2_x(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_Pont2_x_Data = data;
                    frm_ModuleObj.Pont2_x_Data = (double)frm_ModuleObj.Link_Pont2_x_Data.m_DataValue;//点1.X坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入点2.y

        public string Point2_y
        {
            get { return (string)this.GetValue(Point2_yProperty); }
            set { this.SetValue(Point2_yProperty, value); }
        }

        public static readonly DependencyProperty Point2_yProperty =
            DependencyProperty.Register("Point2_y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Point2_y(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_Pont2_y_Data = data;
                    frm_ModuleObj.Pont2_y_Data = (double)frm_ModuleObj.Link_Pont2_y_Data.m_DataValue;//点1.X坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输出结果

        //中心
        public string centerResult
        {
            get { return (string)this.GetValue(centerResultProperty); }
            set { this.SetValue(centerResultProperty, value); }
        }

        public static readonly DependencyProperty centerResultProperty =
            DependencyProperty.Register("centerResult", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        //角度
        public string phiResult
        {
            get { return (string)this.GetValue(phiResultProperty); }
            set { this.SetValue(phiResultProperty, value); }
        }

        public static readonly DependencyProperty phiResultProperty =
            DependencyProperty.Register("phiResult", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        //距离
        public string disResult
        {
            get { return (string)this.GetValue(disResultProperty); }
            set { this.SetValue(disResultProperty, value); }
        }

        public static readonly DependencyProperty disResultProperty =
            DependencyProperty.Register("disResult", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 是否使用转换工具

        public bool converValue
        {
            get { return (bool)this.GetValue(converValueProperty); }
            set { this.SetValue(converValueProperty, value); }
        }

        public static readonly DependencyProperty converValueProperty =
            DependencyProperty.Register("converValue", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

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
                    //点1.x链接
                    ((ModuleObj)m_ModuleObjBase).m_Point1_x = Point1_x;
                    //点1.y链接
                    ((ModuleObj)m_ModuleObjBase).m_Point1_y = Point1_y;
                    //点2.x链接
                    ((ModuleObj)m_ModuleObjBase).m_Point2_x = Point2_x;
                    //点2.y链接
                    ((ModuleObj)m_ModuleObjBase).m_Point2_y = Point2_y;

                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    //执行模块
                    frm_ModuleObj.ExeModule();
                    //点点距离
                    disResult = frm_ModuleObj.disResult.ToString();
                    //点点中心
                    centerResult = frm_ModuleObj.centerResult;
                    //点点为线，线的角度
                    phiResult = ((180 / Math.PI) * frm_ModuleObj.phiResult).ToString();

                    //显示图片
                    Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                    //输入点坐标
                    hxld_Point1.GenCrossContourXld(frm_ModuleObj.pointX1, frm_ModuleObj.pointY1, 100, new HTuple(45).TupleRad());
                    Main_HalconView.DispObj(hxld_Point1, "blue");

                    //输入点坐标
                    hxld_Point1.GenCrossContourXld(frm_ModuleObj.pointX2, frm_ModuleObj.pointY2, 100, new HTuple(45).TupleRad());
                    Main_HalconView.DispObj(hxld_Point1, "cyan");

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
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
        /// 是否转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.IsChecked == true)
            {
                frm_ModuleObj.ConverValue = true;
            }
            else
            {
                frm_ModuleObj.ConverValue = false;
            }
        }

        /// <summary>
        /// 参数保护
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

            if (Point1_x.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点1.x链接未选择！");
                return false;
            }
            //点1.x链接
            ((ModuleObj)m_ModuleObjBase).m_Point1_x = Point1_x;

            if (Point1_y.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点1.y链接未选择！");
                return false;
            }

            //点1.y链接
            ((ModuleObj)m_ModuleObjBase).m_Point1_y = Point1_y;

            if (Point2_x.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点2.x链接未选择！");
                return false;
            }

            //点2.x链接
            ((ModuleObj)m_ModuleObjBase).m_Point2_x = Point2_x;

            if (Point2_y.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点2.y链接未选择！");
                return false;
            }

            //点2.y链接
            ((ModuleObj)m_ModuleObjBase).m_Point2_y = Point2_y;

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

    }
}
