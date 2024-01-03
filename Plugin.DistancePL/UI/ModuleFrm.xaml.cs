using Common;
using DefineImgRoI;
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
using HalconControl;
using HalconDotNet;
using ModuleDataVar;

namespace Plugin.DistancePL
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private HXLDCont hxld_Point1 = new HXLDCont();

        private HXLDCont hxld_Point2 = new HXLDCont();

        private HXLDCont hxld_Dp = new HXLDCont();

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
            #region 输入直线中心.x

            LinePoint_x = frm_ModuleObj.m_LinePoint_x;
            if (frm_ModuleObj.m_LinePoint_x != null)
            {
                if (frm_ModuleObj.m_LinePoint_x.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_LinePont_x_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_LinePont_x_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_LinePont_x_Data.m_DataModuleID);
                    frm_ModuleObj.LinePont_x_Data = (double)frm_ModuleObj.Link_LinePont_x_Data.m_DataValue;
                }
            }

            #endregion
            #region 输入直线中心.y

            LinePoint_y = frm_ModuleObj.m_LinePoint_y;
            if (frm_ModuleObj.m_LinePoint_y != null)
            {
                if (frm_ModuleObj.m_LinePoint_y.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_LinePont_y_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_LinePont_y_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_LinePont_y_Data.m_DataModuleID);
                    frm_ModuleObj.LinePont_y_Data = (double)frm_ModuleObj.Link_LinePont_y_Data.m_DataValue;
                }
            }

            #endregion
            #region 输入直线角度

            LinePhi = frm_ModuleObj.m_LinePhi;
            if (frm_ModuleObj.m_LinePhi != null)
            {
                if (frm_ModuleObj.m_LinePhi.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_m_LinePhi_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_m_LinePhi_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_m_LinePhi_Data.m_DataModuleID);
                    frm_ModuleObj.LinePhi_Data = (double)frm_ModuleObj.Link_m_LinePhi_Data.m_DataValue;
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

        #region 输入直线.x

        public string LinePoint_x
        {
            get { return (string)this.GetValue(LinePoint_xProperty); }
            set { this.SetValue(LinePoint_xProperty, value); }
        }

        public static readonly DependencyProperty LinePoint_xProperty =
            DependencyProperty.Register("LinePoint_x", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_LinePoint_x(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_LinePont_x_Data = data;
                    frm_ModuleObj.LinePont_x_Data = (double)frm_ModuleObj.Link_LinePont_x_Data.m_DataValue;//直线X中点坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入直线.y

        public string LinePoint_y
        {
            get { return (string)this.GetValue(LinePoint_yProperty); }
            set { this.SetValue(LinePoint_yProperty, value); }
        }

        public static readonly DependencyProperty LinePoint_yProperty =
            DependencyProperty.Register("LinePoint_y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_LinePoint_y(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是数值类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_LinePont_y_Data = data;
                    frm_ModuleObj.LinePont_y_Data = (double)frm_ModuleObj.Link_LinePont_y_Data.m_DataValue;//直线Y中点坐标
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 直线角度

        //直线为世界坐标系

        public string LinePhi
        {
            get { return (string)this.GetValue(LinePhiProperty); }
            set { this.SetValue(LinePhiProperty, value); }
        }

        public static readonly DependencyProperty LinePhiProperty =
            DependencyProperty.Register("LinePhi", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Line1_phi(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.Link_m_LinePhi_Data = data;
                    frm_ModuleObj.LinePhi_Data = (double)frm_ModuleObj.Link_m_LinePhi_Data.m_DataValue;//直线角度
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

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

        #region 结果显示

        public string Dp_X
        {
            get { return (string)this.GetValue(Dp_XProperty); }
            set { this.SetValue(Dp_XProperty, value); }
        }

        public static readonly DependencyProperty Dp_XProperty =
            DependencyProperty.Register("Dp_X", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Dp_Y
        {
            get { return (string)this.GetValue(Dp_YProperty); }
            set { this.SetValue(Dp_YProperty, value); }
        }

        public static readonly DependencyProperty Dp_YProperty =
            DependencyProperty.Register("Dp_Y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string Dp_Rstult
        {
            get { return (string)this.GetValue(Dp_RstultProperty); }
            set { this.SetValue(Dp_RstultProperty, value); }
        }

        public static readonly DependencyProperty Dp_RstultProperty =
            DependencyProperty.Register("Dp_Rstult", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        //执行模块
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

                    if (frm_ModuleObj.ConverValue)
                    {
                        Dp_X = frm_ModuleObj.Droopfoot_dis_y.ToString("0.000");
                        Dp_Y = frm_ModuleObj.Droopfoot_dis_x.ToString("0.000");
                        Dp_Rstult = frm_ModuleObj.Droopfoot_dis.ToString("0.000");
                    }
                    else
                    {
                        Dp_X = frm_ModuleObj.Droopfoot_dis_y_w.ToString("0.000");
                        Dp_Y = frm_ModuleObj.Droopfoot_dis_x_w.ToString("0.000");
                        Dp_Rstult = frm_ModuleObj.Droopfoot_dis.ToString("0.000");
                    }

                    if (Dp_X != null)
                    {
                        //显示图片
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        //输入点坐标
                        hxld_Point1.GenCrossContourXld(frm_ModuleObj.Pont1_ImageX_Data, frm_ModuleObj.Pont1_ImageY_Data, 100, new HTuple(45).TupleRad());
                        Main_HalconView.DispObj(hxld_Point1, "blue");

                        //输入直线中心点坐标
                        hxld_Point2.GenCrossContourXld(frm_ModuleObj.LinePont_x_Data, frm_ModuleObj.LinePont_y_Data, 100, new HTuple(45).TupleRad());
                        Main_HalconView.DispObj(hxld_Point2, "cyan");

                        //Main_HalconView.getHWindowControl().HalconWindow.SetLineStyle(0);

                        //输入直线中心点坐标，转直线XLD
                        Main_HalconView.DispObj(frm_ModuleObj.Line.genXLD());

                        //垂足
                        hxld_Dp.GenCrossContourXld(frm_ModuleObj.Droopfoot_dis_x, frm_ModuleObj.Droopfoot_dis_y, 100, new HTuple(45).TupleRad());
                        Main_HalconView.DispObj(hxld_Dp, "red");

                        //Main_HalconView.getHWindowControl().HalconWindow.SetLineStyle(5);

                        //Line_INFO line =new Line_INFO(frm_ModuleObj.Pont1_ImageY_Data,frm_ModuleObj.Pont1_ImageX_Data, 
                        //     frm_ModuleObj.Droopfoot_dis_y, frm_ModuleObj.Droopfoot_dis_x);
                        //Main_HalconView.DispObj(line.genXLD());

                    }
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
        /// 保护
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

            //点1.X未设置
            if (Point1_x.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点1.x链接未选择！");
                return false;
            }
            //点1.X未设置
            if (Point1_y.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("点1.y链接未选择！");
                return false;
            }
            //输入直线中心点1.X未设置
            if (LinePoint_x.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("输入直线中心点1.X未设置！");
                return false;
            }
            //输入直线中心点1.Y未设置
            if (LinePoint_y.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("输入直线中心点1.Y未设置！");
                return false;
            }
            //输入直线角度未设置
            if (LinePhi.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("输入直线角度未设置");
                return false;
            }

            frm_ModuleObj.m_Point1_x = Point1_x;
            frm_ModuleObj.m_Point1_y = Point1_y;
            frm_ModuleObj.m_LinePoint_x = LinePoint_x;
            frm_ModuleObj.m_LinePoint_y = LinePoint_y;
            frm_ModuleObj.m_LinePhi = LinePhi;

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
