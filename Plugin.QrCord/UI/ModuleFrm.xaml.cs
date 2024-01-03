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

namespace Plugin.QrCord
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        //绘制的搜索框
        private List<ViewWindow.Model.ROI> DrawRect;

        //当前窗体
        private string TabName = string.Empty;

        //绘制的ROI
        private RepaintROI matchRepaint = new RepaintROI();

        /// <summary>
        /// 绘制矩形信息
        /// </summary>
        public Rectangle_INFO m_DrawInRect = new Rectangle_INFO();

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

                this.Main_HalconView.hWindowControl.MouseUp += HWindowControl_MouseUp;

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

        public override void theSecondTime()
        {
            base.theSecondTime();
            try
            {
                #region 读取链接图像并显示

                CurrentImage = frm_ModuleObj.m_CurentImgName;
                if (CurrentImage != null && CurrentImage.Length > 0)
                {
                    DataVar var = frm_ModuleObj.m_ModuleProject.GetCurLocalVarValue(frm_ModuleObj.Link_Image_Data);
                    frm_ModuleObj.m_Image = ((List<HImageExt>)var.m_DataValue)[0];
                    DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                }

                #endregion
                #region 位置补正信息

                AffineImage = frm_ModuleObj.m_LinkDataName;

                if (frm_ModuleObj.m_LinkDataName != null)
                {
                    if (frm_ModuleObj.m_LinkDataName.Length > 0)
                    {
                        //查询坐标系
                        frm_ModuleObj.Link_Affine_data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Affine_data.m_DataName &&
                        c.m_DataModuleID == frm_ModuleObj.Link_Affine_data.m_DataModuleID);

                        frm_ModuleObj.LinkAffineCorre = ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0];

                        HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                        Main_HalconView.DispObj(CoorXLD, "yellow");
                    }
                }

                #endregion
                matchRepaint.MatchRoIInfo = frm_ModuleObj.MatchRoIInfo;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #region 矩形ROI拖拽事件

        private void HWindowControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (TabName.Contains("参数设置") && DrawRect != null)
            {
                List<double> Line_data = new List<double>();
                int index;
                Main_HalconView.viewWindow.smallestActiveROI(out Line_data, out index);

                if (index >= 0)
                {
                    Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                    //Main_HalconView.viewWindow.removeActiveROI(ref DrawLine);
                    DrawRect.Clear();
                    Main_HalconView.viewWindow.genRect1(Line_data[0], Line_data[1], Line_data[2], Line_data[3], ref DrawRect);
                    m_DrawInRect = new Rectangle_INFO(Line_data[0], Line_data[1], Line_data[2], Line_data[3]);

                    if (frm_ModuleObj.m_LinkDataName != null)
                    {
                        if (frm_ModuleObj.m_LinkDataName.Length > 0)
                        {
                            HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                            Main_HalconView.DispObj(CoorXLD, "blue");
                        }
                    }
                    UpdateParam(m_DrawInRect);
                    matchRepaint.UpdateROI(RoIType.矩形, Line_data);
                }

            }
        }

        #endregion

        #region 窗体切换

        private void tab_Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var data = this.tab_Control.SelectedItem as TabItem;
                var datas = data.Header; //主要是在后端获取到当前的TabItem的Heade 
                TabName = datas.ToString();
                try
                {
                    if (datas.ToString().Contains("基本参数"))
                    {
                        #region 基本参数

                        if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                        {
                            return;
                        }

                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);//显示图片

                        if (frm_ModuleObj.Link_Affine_data.m_DataValue != null)
                        {
                            HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, ((List<Coordinate_INFO>)frm_ModuleObj.Link_Affine_data.m_DataValue)[0]);
                            Main_HalconView.DispObj(CoorXLD, "blue");//显示位置补正
                        }

                        #endregion
                    }
                    else if (datas.ToString().Contains("参数设置"))
                    {
                        #region 参数设置

                        if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
                        {
                            return;
                        }

                        Main_HalconView.viewWindow.removeActiveROI(ref DrawRect);
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                        //绘制的直线
                        List<double> Line_data = new List<double>();
                        if (matchRepaint.QueryRoI(RoIType.矩形, ref Line_data))
                        {
                            Main_HalconView.viewWindow.genRect1(Line_data[0], Line_data[1], Line_data[2], Line_data[3], ref DrawRect);
                        }
                        else
                        {
                            Main_HalconView.viewWindow.genRect1(100.0, 100.0, 200.0, 200.0, ref this.DrawRect);
                            Line_data.Add(100.0);
                            Line_data.Add(100.0);
                            Line_data.Add(200.0);
                            Line_data.Add(200.0);
                            matchRepaint.UpdateROI(RoIType.矩形, Line_data);

                            m_DrawInRect = new Rectangle_INFO(100.0, 100.0, 200.0, 200.0);
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }));
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
                    frm_ModuleObj.m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                    frm_ModuleObj.Link_Image_Data = data;
                    DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 链接位置补正

        public string AffineImage
        {
            get { return (string)this.GetValue(AffineImageImageProperty); }
            set { this.SetValue(AffineImageImageProperty, value); }
        }

        public static readonly DependencyProperty AffineImageImageProperty =
            DependencyProperty.Register("AffineImage", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Affine_EValueAlarm(object sender, RoutedEventArgs e)
        {
            try
            {
                DataVar data = (DataVar)e.OriginalSource;
                //数据不为空，且是图像类型
                if (data.m_DataValue != null && data.m_DataType == DataVarType.DataType.坐标系)
                {
                    frm_ModuleObj.m_LinkDataName = AffineImage;
                    frm_ModuleObj.Link_Affine_data = data;
                    if (frm_ModuleObj.m_Image != null)
                    {
                        frm_ModuleObj.LinkAffineCorre = ((List<Coordinate_INFO>)data.m_DataValue)[0];
                        HXLDCont CoorXLD = SysVisionCore.GetCoord(frm_ModuleObj.m_Image, frm_ModuleObj.LinkAffineCorre);
                        Main_HalconView.DispObj(CoorXLD);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region 结果显示

        public string QrResult
        {
            get { return (string)this.GetValue(QrResultProperty); }
            set { this.SetValue(QrResultProperty, value); }
        }

        public static readonly DependencyProperty QrResultProperty =
            DependencyProperty.Register("QrResult", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        //执行
        public override void ExModule()
        {
            try
            {

                frm_ModuleObj.VerifyParam(m_DrawInRect);

                if (ProtectModuel())
                {
                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    ((ModuleObj)m_ModuleObjBase).m_Model = new HDataCode2D();

                    ((ModuleObj)m_ModuleObjBase).ExeModule();

                    Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
                    Main_HalconView.DispObj(frm_ModuleObj.m_ResultXld);
                    QrResult = frm_ModuleObj.m_DataCode;
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

            if (DrawRect == null)
            {
                System.Windows.Forms.MessageBox.Show("请设置搜索区域！");
                return false;
            }

            //保存ROI的参数
            ((ModuleObj)m_ModuleObjBase).MatchRoIInfo = matchRepaint.MatchRoIInfo;

            return true;
        }

        //取消
        public override void CancelModuleParam()
        {
            this.Main_HalconView.hWindowControl.MouseUp -= HWindowControl_MouseUp;
            base.CancelModuleParam();
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

        public void UpdateParam(Rectangle_INFO in_INFO)
        {
            frm_ModuleObj.VerifyParam(in_INFO);
            Main_HalconView.DispObj(frm_ModuleObj.m_ResultXld);
            QrResult = frm_ModuleObj.m_DataCode;
        }

    }
}
