using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.GeomCreateRoI
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {
        //对应后台程序
        private ModuleObj frm_ModuleObj;

        //搜索区域
        List<ViewWindow.Model.ROI> ROIRegion;

        //绘制的ROI
        private RepaintROI matchRepaint = new RepaintROI();

        //矩形窗体
        private RectROIInfo RectFrm;

        //圆形窗体
        private CircleROIInfo CirFrm;

        /// <summary>
        /// 绘制ROI的信息
        /// </summary>
        public ROI m_DrawROI;

        public ModuleFrm()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.DataContext = this;
            this.Main_HalconView.hWindowControl.MouseUp += HWindowControl_MouseUp;
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

                // this.ROIType.ModelSelectEvent += UcRadDoubleFrm_ModelSelectEvent;

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

        public override void theSecondTime()
        {
            base.theSecondTime();
            #region 读取链接图像并显示

            CurrentImage = frm_ModuleObj.m_CurentImgName;
            if (CurrentImage != null && CurrentImage.Length > 0)
            {
                DataVar var = frm_ModuleObj.m_ModuleProject.GetCurLocalVarValue(frm_ModuleObj.Link_Image_Data);
                frm_ModuleObj.m_Image = ((List<HImageExt>)var.m_DataValue)[0];
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
            #region ROI
            matchRepaint.MatchRoIInfo = frm_ModuleObj.MatchRoIInfo;
            //ROI区域方式
            switch (frm_ModuleObj.ModelModel)
            {
                case RoIType.矩形:
                    Rect1Model = true;
                    CircleModel = false;

                    //frm_ModuleObj.ModelModel = RoIType.矩形;
                    //RectFrm = new RectROIInfo(frm_ModuleObj);
                    //Page_Change.Content = new Frame()
                    //{
                    //    Content = RectFrm,
                    //};

                    break;
                case RoIType.圆形:
                    Rect1Model = false;
                    CircleModel = true;

                    //frm_ModuleObj.ModelModel = RoIType.圆形;
                    //CirFrm = new CircleROIInfo(frm_ModuleObj);
                    //Page_Change.Content = new Frame()
                    //{
                    //    Content = CirFrm
                    //};
                    break;
                default:
                    break;
            }
            #endregion
            #region 位置补正信息

            Chk_Follow.IsChecked = frm_ModuleObj.IsFollow;

            if (frm_ModuleObj.IsFollow)
            {
                LinkFollow follow = new LinkFollow(frm_ModuleObj);
                Page_Change1.Content = new Frame()
                {
                    Content = follow
                };
            }
            else
            {
                Page_Change1.Content = new Frame()
                {
                    Content = null
                };
            }

            #endregion
            #region 是否显示

            Chk_Disp.IsChecked = frm_ModuleObj.m_IsDispOutPoint;

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

        #region 选择ROI类型

        public bool Rect1Model
        {
            get { return (bool)this.GetValue(Rect1ModelProperty); }
            set { this.SetValue(Rect1ModelProperty, value); }
        }

        public static readonly DependencyProperty Rect1ModelProperty =
            DependencyProperty.Register("Rect1Model", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool CircleModel
        {
            get { return (bool)this.GetValue(CircleModelProperty); }
            set { this.SetValue(CircleModelProperty, value); }
        }

        public static readonly DependencyProperty CircleModelProperty =
            DependencyProperty.Register("CircleModel", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        /// <summary>
        /// 选择ROI类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcRadDoubleFrm_ModelSelectEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();

            //图像为空
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("未链接图像！");
                return;
            }

            //重新显示图像
            Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

            //显示ROI区域
            List<double> data = new List<double>();

            if (info.Contains("矩形"))
            {
                frm_ModuleObj.ModelModel = RoIType.矩形;
                RectFrm = new RectROIInfo(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = RectFrm,
                };

                if (matchRepaint.QueryRoI(RoIModel.模板区域, RoIType.矩形, ref data))
                {
                    Main_HalconView.viewWindow.removeActiveROI(ref ROIRegion);
                    Main_HalconView.viewWindow.genRect2(data[0], data[1], data[2], data[3], data[4], ref this.ROIRegion);
                    frm_ModuleObj.m_DrawROI = new Rectangle2_INFO(data[0], data[1], -data[2], data[3], data[4]);

                    RectFrm.rectDelegate(data[0], data[1], data[2], data[3], data[4]);

                }
                else
                {
                    //清除保存的信息
                    frm_ModuleObj.m_ExternalCenterRow = false;
                    frm_ModuleObj.m_ExternalCenterCol = false;
                    frm_ModuleObj.m_ExternalPhi = false;
                    frm_ModuleObj.m_ExternalLength1 = false;
                    frm_ModuleObj.m_ExternalLength2 = false;

                    Main_HalconView.viewWindow.genRect2(200.0, 200.0, 0.52, 60.0, 30.0, ref this.ROIRegion);
                    data.Add(200.0);
                    data.Add(200.0);
                    data.Add(0.52);
                    data.Add(60.0);
                    data.Add(30.0);
                    matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.矩形, data);
                    frm_ModuleObj.m_DrawROI = new Rectangle2_INFO(200.0, 200.0, -0.52, 60.0, 30.0);

                    RectFrm.rectDelegate(200.0, 200.0, 0.52, 60.0, 30.0);

                }
            }
            else if (info.Contains("圆形"))
            {
                frm_ModuleObj.ModelModel = RoIType.圆形;
                CirFrm = new CircleROIInfo(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = CirFrm
                };

                if (matchRepaint.QueryRoI(RoIModel.模板区域, RoIType.圆形, ref data))
                {
                    Main_HalconView.viewWindow.removeActiveROI(ref ROIRegion);
                    Main_HalconView.viewWindow.genCircle(data[0], data[1], data[2], ref this.ROIRegion);
                    frm_ModuleObj.m_DrawROI = new Circle_INFO(data[0], data[1], data[2]);

                    CirFrm.circleDelegate(data[0], data[1], data[2]);

                }
                else
                {
                    //清除保存的信息
                    frm_ModuleObj.m_ExternalRow = false;
                    frm_ModuleObj.m_ExternalCol = false;
                    frm_ModuleObj.m_ExternalRadius = false;

                    Main_HalconView.viewWindow.genCircle(200.0, 200.0, 60.0, ref this.ROIRegion);
                    data.Add(200.0);
                    data.Add(200.0);
                    data.Add(60.0);
                    matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.圆形, data);
                    frm_ModuleObj.m_DrawROI = new Circle_INFO(200.0, 200.0, 60.0);

                    //坐标保存
                    frm_ModuleObj.m_Row = 200.0.ToString("f3");
                    frm_ModuleObj.m_Col = 200.0.ToString("f3");
                    frm_ModuleObj.m_Radius = 60.0.ToString("f3");

                    CirFrm.circleDelegate(200.0, 200.0, 60.0);

                }
            }
        }

        #endregion

        #region 跟随模式

        private void Follow_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {
                frm_ModuleObj.IsFollow = true;

                frm_ModuleObj.m_ExternalCenterRow = false;
                frm_ModuleObj.m_ExternalCenterCol = false;
                frm_ModuleObj.m_ExternalPhi = false;
                frm_ModuleObj.m_ExternalLength1 = false;
                frm_ModuleObj.m_ExternalLength2 = false;

                LinkFollow follow = new LinkFollow(frm_ModuleObj);
                Page_Change1.Content = new Frame()
                {
                    Content = follow
                };
            }
            else
            {
                frm_ModuleObj.IsFollow = false;

                frm_ModuleObj.m_ExternalRow = false;
                frm_ModuleObj.m_ExternalCol = false;
                frm_ModuleObj.m_ExternalPhi = false;

                Page_Change1.Content = new Frame()
                {
                    Content = null
                };
            }
        }

        #endregion

        #region 是否显示Region轮廓

        private void IsDispChk_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            if (chk.IsChecked == true)
            {
                frm_ModuleObj.m_IsDispOutPoint = true;
            }
            else
            {
                frm_ModuleObj.m_IsDispOutPoint = false;
            }
        }

        #endregion

        private void HWindowControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //图像为空
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null && frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>)
            {
                System.Windows.Forms.MessageBox.Show("未链接图像！");
                return;
            }

            //搜索区域  
            List<double> Seach_data = new List<double>();
            //Mid区域  
            List<double> Mid_data = new List<double>();
            int index;
            Main_HalconView.viewWindow.smallestActiveROI(out Seach_data, out index);
            if (index >= 0)
            {

                //重新显示图像
                Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);

                switch (frm_ModuleObj.ModelModel)
                {
                    case RoIType.矩形:
                        #region 矩形

                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[0]);
                        }
                        else
                        {
                            //CenterRow判断是否链接了外部数据
                            if (frm_ModuleObj.m_ExternalCenterRow)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_CenterCol.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_CenterCol.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[0]);
                            }
                        }

                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[1]);
                        }
                        else
                        {
                            //CenterCol判断是否链接了外部数据
                            if (frm_ModuleObj.m_ExternalCenterRow)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_CenterRow.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_CenterRow.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[1]);
                            }
                        }

                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[2]);
                        }
                        else
                        {
                            //Link_Phi判断是否链接了外部数据
                            if (frm_ModuleObj.m_ExternalPhi)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Phi.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_Phi.m_DataModuleID);
                                Mid_data.Add(-Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[2]);
                            }
                        }

                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[3]);
                        }
                        else
                        {
                            //Length2判断是否链接了外部数据
                            if (frm_ModuleObj.m_ExternalLength2)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Length2.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_Length2.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[3]);
                            }
                        }

                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[4]);
                        }
                        else
                        {
                            //Length1判断是否链接了外部数据
                            if (frm_ModuleObj.m_ExternalLength1)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Length1.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_Length1.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[4]);
                            }
                        }

                        m_DrawROI = new Rectangle2_INFO(Mid_data[0], Mid_data[1], Mid_data[2], Mid_data[3], Mid_data[4]);
                        matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.矩形, Mid_data);

                        Main_HalconView.viewWindow.removeActiveROI(ref ROIRegion);
                        Main_HalconView.viewWindow.genRect2(Mid_data[0], Mid_data[1], Mid_data[2], Mid_data[3], Mid_data[4], ref this.ROIRegion);
                        frm_ModuleObj.m_DrawROI = new Rectangle2_INFO(Mid_data[0], Mid_data[1], Mid_data[2], Mid_data[3], Mid_data[4]);

                        RectFrm.rectDelegate(Seach_data[1], Seach_data[0], Seach_data[2], Seach_data[3], Seach_data[4]);

                        #endregion
                        break;
                    case RoIType.圆形:
                        #region 圆形

                        //是否跟随模式
                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[0]);
                        }
                        else
                        {
                            if (frm_ModuleObj.m_ExternalCol)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Col.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_Col.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[0]);
                            }
                        }

                        //是否跟随模式
                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[1]);
                        }
                        else
                        {
                            if (frm_ModuleObj.m_ExternalRow)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Row.m_DataName &&
                                c.m_DataModuleID == frm_ModuleObj.Link_Row.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[1]);
                            }
                        }

                        //是否跟随模式
                        if (frm_ModuleObj.IsFollow)
                        {
                            Mid_data.Add(Seach_data[2]);
                        }
                        else
                        {
                            if (frm_ModuleObj.m_ExternalRadius)
                            {
                                DataVar data = SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == frm_ModuleObj.Link_Radius.m_DataName &&
                               c.m_DataModuleID == frm_ModuleObj.Link_Radius.m_DataModuleID);
                                Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                            }
                            else
                            {
                                Mid_data.Add(Seach_data[2]);
                            }
                        }

                        m_DrawROI = new Circle_INFO(Mid_data[0], Mid_data[1], Mid_data[2]);
                        matchRepaint.UpdateROI(RoIModel.模板区域, RoIType.圆形, Mid_data);

                        Main_HalconView.viewWindow.removeActiveROI(ref ROIRegion);
                        Main_HalconView.viewWindow.genCircle(Mid_data[0], Mid_data[1], Mid_data[2], ref this.ROIRegion);
                        frm_ModuleObj.m_DrawROI = new Circle_INFO(Mid_data[0], Mid_data[1], Mid_data[2]);

                        CirFrm.circleDelegate(Seach_data[1], Seach_data[0], Seach_data[2]);

                        #endregion
                        break;
                    default:
                        break;
                }
            }
            //跟随模式
            if (frm_ModuleObj.IsFollow)
            {
                //frm_ModuleObj.VerifyParam(m_DrawROI);
            }
        }

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
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
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

            //判断图像类型
            if (!(frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>))
            {
                System.Windows.Forms.MessageBox.Show("图像类型错误！");
                return false;
            }

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称

            //ROI类型
            if (Rect1Model)
            {
                ((ModuleObj)m_ModuleObjBase).ModelModel = RoIType.矩形;
            }
            if (CircleModel)
            {
                ((ModuleObj)m_ModuleObjBase).ModelModel = RoIType.圆形;
            }
            frm_ModuleObj.MatchRoIInfo = matchRepaint.MatchRoIInfo;//保存参数

            return base.ProtectModuel();
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
