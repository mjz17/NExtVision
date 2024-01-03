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

namespace Plugin.IntersectionPoint
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private HXLDCont m_Line1Cross = new HXLDCont();

        private HXLDCont m_Line2Cross = new HXLDCont();

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

                DispOutPoint = frm_ModuleObj.m_IsDispOutPoint;

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
            #region 输入直线1

            Line1 = frm_ModuleObj.m_Line1;

            if (frm_ModuleObj.m_Image.IsInitialized() && frm_ModuleObj.m_Line1 != null)
            {
                if (frm_ModuleObj.m_Line1.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_m_Line1_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_m_Line1_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_m_Line1_Data.m_DataModuleID);

                    Line_INFO line = (Line_INFO)frm_ModuleObj.Link_m_Line1_Data.m_DataValue;

                    HXLDCont m_ResultXLD = new HXLDCont();
                    m_ResultXLD.GenContourPolygonXld(new HTuple(line.StartY, line.EndY), new HTuple(line.StartX, line.EndX));

                    Main_HalconView.DispObj(m_ResultXLD, "red");

                }
            }

            #endregion
            #region 输入直线2

            Line2 = frm_ModuleObj.m_Line2;
            if (frm_ModuleObj.m_Image.IsInitialized() && frm_ModuleObj.m_Line2 != null)
            {
                if (frm_ModuleObj.m_Line2.Length > 0)
                {
                    //查询坐标系
                    frm_ModuleObj.Link_m_Line2_Data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_m_Line2_Data.m_DataName &&
                    c.m_DataModuleID == frm_ModuleObj.Link_m_Line2_Data.m_DataModuleID);
                    Line_INFO line = (Line_INFO)frm_ModuleObj.Link_m_Line2_Data.m_DataValue;

                    HXLDCont m_ResultXLD = new HXLDCont();
                    m_ResultXLD.GenContourPolygonXld(new HTuple(line.StartY, line.EndY), new HTuple(line.StartX, line.EndX));

                    Main_HalconView.DispObj(m_ResultXLD, "cyan");
                }
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

        #region 输入直线1

        public string Line1
        {
            get { return (string)this.GetValue(Line1Property); }
            set { this.SetValue(Line1Property, value); }
        }

        public static readonly DependencyProperty Line1Property =
            DependencyProperty.Register("Line1", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Line1_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Line)
            {
                try
                {
                    //导入数据
                    frm_ModuleObj.Link_m_Line1_Data = data;
                    frm_ModuleObj._Line1 = (Line_INFO)frm_ModuleObj.Link_m_Line1_Data.m_DataValue;

                    HXLDCont m_ResultXLD = new HXLDCont();
                    m_ResultXLD.GenContourPolygonXld(new HTuple(frm_ModuleObj._Line1.StartY, frm_ModuleObj._Line1.EndY),
                        new HTuple(frm_ModuleObj._Line1.StartX, frm_ModuleObj._Line1.EndX));

                    Main_HalconView.DispObj(m_ResultXLD, "yellow");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 输入直线2

        public string Line2
        {
            get { return (string)this.GetValue(Line2Property); }
            set { this.SetValue(Line2Property, value); }
        }

        public static readonly DependencyProperty Line2Property =
            DependencyProperty.Register("Line2", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Line2_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Line)
            {
                try
                {
                    //导入数据
                    frm_ModuleObj.Link_m_Line2_Data = data;
                    frm_ModuleObj._Line2 = (Line_INFO)frm_ModuleObj.Link_m_Line2_Data.m_DataValue;

                    HXLDCont m_ResultXLD = new HXLDCont();
                    m_ResultXLD.GenContourPolygonXld(new HTuple(frm_ModuleObj._Line2.StartY, frm_ModuleObj._Line2.EndY),
                        new HTuple(frm_ModuleObj._Line2.StartX, frm_ModuleObj._Line2.EndX));

                    Main_HalconView.DispObj(m_ResultXLD, "cyan");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 结果显示

        public string ResultRowCol
        {
            get { return (string)this.GetValue(ResultRowColProperty); }
            set { this.SetValue(ResultRowColProperty, value); }
        }

        public static readonly DependencyProperty ResultRowColProperty =
            DependencyProperty.Register("ResultRowCol", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string ResultPhi
        {
            get { return (string)this.GetValue(ResultPhiProperty); }
            set { this.SetValue(ResultPhiProperty, value); }
        }

        public static readonly DependencyProperty ResultPhiProperty =
            DependencyProperty.Register("ResultPhi", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        public string ResultStatus
        {
            get { return (string)this.GetValue(ResultStatusProperty); }
            set { this.SetValue(ResultStatusProperty, value); }
        }

        public static readonly DependencyProperty ResultStatusProperty =
            DependencyProperty.Register("ResultStatus", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        #region 显示的设置

        public bool DispOutPoint
        {
            get { return (bool)this.GetValue(DispOutPointProperty); }
            set { this.SetValue(DispOutPointProperty, value); }
        }

        public static readonly DependencyProperty DispOutPointProperty =
            DependencyProperty.Register("DispOutPoint", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

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

                    ResultRowCol = frm_ModuleObj.m_Col.ToString("0.000") + "/" + frm_ModuleObj.m_Row.ToString("0.000");

                    ResultPhi = ((180 / Math.PI) * frm_ModuleObj.m_phi).ToString();

                    if (frm_ModuleObj.isParallel == 0)
                    {
                        ResultStatus = "不平行";
                    }
                    else if (frm_ModuleObj.isParallel == 1)
                    {
                        ResultStatus = "平行";
                    }

                    Main_HalconView.DispObj(frm_ModuleObj.m_PointCross, "cyan");
                }
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
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            switch (chk.Content)
            {
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
                default:
                    break;
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

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称

            //输入点2未设置
            if (frm_ModuleObj.m_Line1 == null)
            {
                System.Windows.Forms.MessageBox.Show("输入直线1未设置！");
                return false;
            }
            //点1链接
            ((ModuleObj)m_ModuleObjBase).m_Line1 = Line1;

            //输入点2未设置
            if (frm_ModuleObj.m_Line2 == null)
            {
                System.Windows.Forms.MessageBox.Show("输入直线2未设置！");
                return false;
            }
            //点2链接
            ((ModuleObj)m_ModuleObjBase).m_Line2 = Line2;

            return true;
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
