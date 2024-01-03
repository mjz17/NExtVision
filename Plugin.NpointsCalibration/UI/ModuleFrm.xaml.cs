using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using StyleBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace Plugin.NpointsCalibration
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        //窗体对应的后台数据
        private ModuleObj frm_ModuleObj;

        //机械和图像坐标
        public List<PointCoord> PonitList = new List<PointCoord>();

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

            Rad_Rotation.Click += Rad_Rotation_Click;
            X_Move.TextValueEvent += TxtAddandSub_TextValueEvent;
            Y_Move.TextValueEvent += TxtAddandSub_TextValueEvent;
            X_Criterion.TextValueEvent += TxtAddandSub_TextValueEvent;
            Y_Criterion.TextValueEvent += TxtAddandSub_TextValueEvent;

        }

        public override void theFirsttime()
        {
            base.theFirsttime();
            this.rad_Auto.IsChecked = true;//自动标定
            this.rad_Manu.IsChecked = false;//手动标定

            this.Rad_CameraFix.IsChecked = true;//相机固定
            this.Rad_CameraMove.IsChecked = false;//相机移动

            this.Rad_Rotation.IsChecked = true;//是否启用旋转中心

            this.Rad_PhiEn.IsChecked = false;//角度是否取反

            frm_ModuleObj.m_XDis = X_Move_Postion;//X轴移动距离
            frm_ModuleObj.m_YDis = Y_Move_Postion;//Y轴移动距离
            frm_ModuleObj.m_XCriterion = X_Stand_Postion;//X轴基准距离
            frm_ModuleObj.m_YCriterion = Y_Stand_Postion;//Y轴基准距离

            for (int i = 0; i < 14; i++)
            {
                frm_ModuleObj.AddNpointsPram();
            }

            PonitList = frm_ModuleObj.PonitList.FindAll(c => c.m_Index > 0);
            RefreshDgv();
        }
        public override void theSecondTime()
        {
            base.theSecondTime();

            #region 读取链接图像

            CurrentImage = frm_ModuleObj.m_CurentImgName;

            if (CurrentImage != null && CurrentImage.Length > 0)
            {
                int CurrImgIndex = SysProcessPro.Cur_Project.m_Var_List.FindIndex(c => c.m_DataName == frm_ModuleObj.Link_Image_Data.m_DataName
                  && c.m_DataModuleID == frm_ModuleObj.Link_Image_Data.m_DataModuleID);

                if (CurrImgIndex > -1)
                {
                    frm_ModuleObj.m_Image = ((List<HImageExt>)SysProcessPro.Cur_Project.m_Var_List[CurrImgIndex].m_DataValue)[0];
                }
            }

            #endregion

            #region 标定方式

            if (frm_ModuleObj.m_CalibraMethod == CalibraMethod.自动)
            {
                this.rad_Auto.IsChecked = true;//自动标定
                this.rad_Manu.IsChecked = false;//手动标定
            }
            else
            {
                this.rad_Auto.IsChecked = false;//自动标定
                this.rad_Manu.IsChecked = true;//手动标定
            }

            #endregion

            Image_x = frm_ModuleObj.m_ImgX;//图像X坐标
            Image_y = frm_ModuleObj.m_ImgY;//图像Y坐标

            #region 相机安装方式

            if (frm_ModuleObj.m_CameraInstall == CameraInstall.相机固定)
            {
                this.Rad_CameraFix.IsChecked = true;//相机固定
                this.Rad_CameraMove.IsChecked = false;//相机移动
            }
            else
            {
                this.Rad_CameraFix.IsChecked = false;//相机固定
                this.Rad_CameraMove.IsChecked = true;//相机移动
            }

            #endregion

            Rad_Rotation.IsChecked = frm_ModuleObj.m_EnableorNot ? true : false;//是否启用旋转中心

            X_Move_Postion = frm_ModuleObj.m_XDis;//X移动距离
            Y_Move_Postion = frm_ModuleObj.m_YDis;//Y移动距离

            X_Stand_Postion = frm_ModuleObj.m_XCriterion;//X轴基准距离
            Y_Stand_Postion = frm_ModuleObj.m_YCriterion;//Y轴基准距离

            Phi_Stand_Postion = frm_ModuleObj.m_PhiCriterion;//基准角度
            Rad_PhiEn.IsChecked = frm_ModuleObj.m_PhiNegation ? true : false;//角度是否取反

            PonitList = frm_ModuleObj.PonitList.FindAll(c => c.m_Index > 0);

            RefreshDgv();
            FindNpointCartion();

            Chk_Two.IsChecked = frm_ModuleObj.m_EnableTwoPoint;
            ChkIsCheck(frm_ModuleObj.m_EnableTwoPoint);
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
                    frm_ModuleObj.m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                    frm_ModuleObj.Link_Image_Data = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 图像坐标X

        public string Image_x
        {
            get { return (string)this.GetValue(Image_xProperty); }
            set { this.SetValue(Image_xProperty, value); }
        }

        public static readonly DependencyProperty Image_xProperty =
            DependencyProperty.Register("Image_x", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Image_X_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_ImgX = data.m_DataName;
                    frm_ModuleObj.Link_ImgX = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 图像坐标Y

        public string Image_y
        {
            get { return (string)this.GetValue(Image_yProperty); }
            set { this.SetValue(Image_yProperty, value); }
        }

        public static readonly DependencyProperty Image_yProperty =
            DependencyProperty.Register("Image_y", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_Image_Y_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Double)
            {
                try
                {
                    frm_ModuleObj.m_ImgY = data.m_DataName;
                    frm_ModuleObj.Link_ImgY = data;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region X轴移动距离

        public double X_Move_Postion
        {
            get { return (double)this.GetValue(X_Move_PostionProperty); }
            set { this.SetValue(X_Move_PostionProperty, value); }
        }

        public static readonly DependencyProperty X_Move_PostionProperty =
            DependencyProperty.Register("X_Move_Postion", typeof(double), typeof(ModuleFrm), new PropertyMetadata((double)25));

        #endregion

        #region Y轴移动距离

        public double Y_Move_Postion
        {
            get { return (double)this.GetValue(Y_Move_Postionroperty); }
            set { this.SetValue(Y_Move_Postionroperty, value); }
        }

        public static readonly DependencyProperty Y_Move_Postionroperty =
            DependencyProperty.Register("Y_Move_Postion", typeof(double), typeof(ModuleFrm), new PropertyMetadata((double)25));

        #endregion

        #region X轴基准距离

        public double X_Stand_Postion
        {
            get { return (double)this.GetValue(X_Stand_PostionProperty); }
            set { this.SetValue(X_Stand_PostionProperty, value); }
        }

        public static readonly DependencyProperty X_Stand_PostionProperty =
            DependencyProperty.Register("X_Stand_Postion", typeof(double), typeof(ModuleFrm), new PropertyMetadata((double)100));

        #endregion

        #region Y轴基准距离

        public double Y_Stand_Postion
        {
            get { return (double)this.GetValue(Y_Stand_PostionProperty); }
            set { this.SetValue(Y_Stand_PostionProperty, value); }
        }

        public static readonly DependencyProperty Y_Stand_PostionProperty =
            DependencyProperty.Register("Y_Stand_Postion", typeof(double), typeof(ModuleFrm), new PropertyMetadata((double)50));

        #endregion

        #region 标定结果

        //平移X
        public double Tran_X
        {
            get { return (double)this.GetValue(Tran_XProperty); }
            set { this.SetValue(Tran_XProperty, value); }
        }

        public static readonly DependencyProperty Tran_XProperty =
            DependencyProperty.Register("Tran_X", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //平移Y
        public double Tran_Y
        {
            get { return (double)this.GetValue(Tran_YProperty); }
            set { this.SetValue(Tran_YProperty, value); }
        }

        public static readonly DependencyProperty Tran_YProperty =
            DependencyProperty.Register("Tran_Y", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //像素当量X
        public double Pixel_X
        {
            get { return (double)this.GetValue(Pixel_XProperty); }
            set { this.SetValue(Pixel_XProperty, value); }
        }

        public static readonly DependencyProperty Pixel_XProperty =
            DependencyProperty.Register("Pixel_X", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //像素当量Y
        public double Pixel_Y
        {
            get { return (double)this.GetValue(Pixel_YProperty); }
            set { this.SetValue(Pixel_YProperty, value); }
        }

        public static readonly DependencyProperty Pixel_YProperty =
            DependencyProperty.Register("Pixel_Y", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //旋转角度
        public double RotationAngle
        {
            get { return (double)this.GetValue(RotationAngleProperty); }
            set { this.SetValue(RotationAngleProperty, value); }
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //倾斜角度
        public double DipAngle
        {
            get { return (double)this.GetValue(DipAngleProperty); }
            set { this.SetValue(DipAngleProperty, value); }
        }

        public static readonly DependencyProperty DipAngleProperty =
            DependencyProperty.Register("DipAngle", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //Rms误差
        public double RmsError
        {
            get { return (double)this.GetValue(RmsErrorProperty); }
            set { this.SetValue(RmsErrorProperty, value); }
        }

        public static readonly DependencyProperty RmsErrorProperty =
            DependencyProperty.Register("RmsError", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //旋转中心X
        public double RotationCenterX
        {
            get { return (double)this.GetValue(RotationCenterXProperty); }
            set { this.SetValue(RotationCenterXProperty, value); }
        }

        public static readonly DependencyProperty RotationCenterXProperty =
            DependencyProperty.Register("RotationCenterX", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        //旋转中心Y
        public double RotationCenterY
        {
            get { return (double)this.GetValue(RotationCenterYProperty); }
            set { this.SetValue(RotationCenterYProperty, value); }
        }

        public static readonly DependencyProperty RotationCenterYProperty =
            DependencyProperty.Register("RotationCenterY", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        #endregion

        #region 轴基准角度

        public double Phi_Stand_Postion
        {
            get { return (double)this.GetValue(Phi_Stand_PostionProperty); }
            set { this.SetValue(Phi_Stand_PostionProperty, value); }
        }

        public static readonly DependencyProperty Phi_Stand_PostionProperty =
            DependencyProperty.Register("Phi_Stand_Postion", typeof(double), typeof(ModuleFrm), new PropertyMetadata((double)0));

        #endregion

        /// <summary>
        /// 是否启用标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rad_Rotation_Click(object sender, RoutedEventArgs e)
        {
            CheckBox rad = (CheckBox)sender;
            if (rad.IsChecked == true)
            {
                frm_ModuleObj.m_EnableorNot = true;
                //9点标定+旋转中心
                PonitList = frm_ModuleObj.PonitList.FindAll(c => c.m_Index < 15);
            }
            else
            {
                frm_ModuleObj.m_EnableorNot = false;
                //9点标定
                PonitList = frm_ModuleObj.PonitList.FindAll(c => c.m_Index < 10);
            }
            RefreshDgv();
        }

        private void TxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.m_XDis = X_Move_Postion;//X轴移动距离
            frm_ModuleObj.m_YDis = Y_Move_Postion;//Y轴移动距离
            frm_ModuleObj.m_XCriterion = X_Stand_Postion;//X轴基准距离
            frm_ModuleObj.m_YCriterion = Y_Stand_Postion;//Y轴基准距离

            frm_ModuleObj.PonitList.Clear();
            for (int i = 0; i < 14; i++)
            {
                frm_ModuleObj.AddNpointsPram();
            }
            PonitList = frm_ModuleObj.PonitList.FindAll(c => c.m_Index > 0);
            RefreshDgv();
        }

        #region 手动清空

        /// <summary>
        /// 手动清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ClearClick(object sender, RoutedEventArgs e)
        {
            try
            {
                frm_ModuleObj.m_Index = 0;

                PonitList.Clear();
                frm_ModuleObj.PonitList.Clear();
                for (int i = 0; i < 14; i++)
                {
                    frm_ModuleObj.AddNpointsPram();
                }
                PonitList = frm_ModuleObj.PonitList;
                RefreshDgv();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        public void RefreshDgv()
        {
            this.dgv_Mat_Data.ItemsSource = null;
            this.dgv_Mat_Data.ItemsSource = PonitList;
        }

        /// <summary>
        /// 执行标定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcuCalibration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NpointCommon npoint = new NpointCommon();
                //启用旋转中心
                if (frm_ModuleObj.m_EnableorNot)
                {
                    //2023.5.30,修改一次旋转中心X,Y,有可能因为角度是递增导致，后续继续测试
                    frm_ModuleObj.m_Mat2D = npoint.NpointTranRotation(PonitList, out frm_ModuleObj.RotationCenter_X, out frm_ModuleObj.RotationCenter_Y, out frm_ModuleObj.RotationCenter_X_W, out frm_ModuleObj.RotationCenter_Y_W);
                    FindNpointCartion();
                }
                else
                {
                    frm_ModuleObj.m_Mat2D = npoint.NpointTran(PonitList);
                    RotationCenterX = frm_ModuleObj.RotationCenter_Y = 0;
                    RotationCenterY = frm_ModuleObj.RotationCenter_X = 0;
                    FindNpointCartion();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
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
                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    frm_ModuleObj.SavePram();

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

            //图像坐标X
            if (frm_ModuleObj.Link_ImgX.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像坐标X未设置！");
                return false;
            }

            //图像坐标Y
            if (frm_ModuleObj.Link_ImgY.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像坐标Y未设置！");
                return false;
            }

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称
            frm_ModuleObj.m_ImgX = Image_x;//图像坐标X
            frm_ModuleObj.m_ImgY = Image_y;//图像坐标Y
            frm_ModuleObj.m_PhiCriterion = Phi_Stand_Postion;//基准角度

            frm_ModuleObj.m_EnableorNot = Rad_Rotation.IsChecked == true ? true : false;//是否启用旋转中心

            return true;
        }

        public void FindNpointCartion()
        {
            if (frm_ModuleObj.m_Mat2D != null)
            {
                double sx = frm_ModuleObj.m_Mat2D.HomMat2dToAffinePar(out double sy, out double phi, out double theta, out double tx, out double ty);
                //像素当量X
                Pixel_X = sx;
                //像素当量Y
                Pixel_Y = sy;
                //旋转角度
                RotationAngle = (180 / Math.PI) * phi;

                //倾斜角度
                DipAngle = (180 / Math.PI) * theta;

                //X偏移
                Tran_X = tx;
                //Y偏移
                Tran_Y = ty;

                RotationCenterX = frm_ModuleObj.RotationCenter_X;
                RotationCenterY = frm_ModuleObj.RotationCenter_Y;
            }

            //求取Rms
            if (frm_ModuleObj.m_Mat2D != null)
            {
                //9点标定的数据
                List<PointCoord> PonitList1 = PonitList.FindAll(c => c.m_Index < 10);

                double[] row = PonitList1.AsEnumerable().Select(r => r.m_ImageRow).ToArray();
                double[] col = PonitList1.AsEnumerable().Select(r => r.m_ImageCol).ToArray();
                double[] x = PonitList1.AsEnumerable().Select(r => r.m_Mach_x).ToArray();
                double[] y = PonitList1.AsEnumerable().Select(r => r.m_Mach_y).ToArray();

                RmsError = CalculateRMS(frm_ModuleObj.m_Mat2D, row, col, x, y);
            }
        }

        //计算rms误差
        public static double CalculateRMS(HHomMat2D hom2d, HTuple x_Image, HTuple y_Image, HTuple x_Robot, HTuple y_Robot)
        {
            try
            {
                double count = 0;
                for (int i = 0; i < x_Image.Length; i++)
                {
                    double tempX, tempY;
                    tempX = hom2d.HomMat2dInvert().AffineTransPoint2d(x_Robot[i].D, y_Robot[i].D, out tempY);
                    double dis = HMisc.DistancePp(tempY, tempX, y_Image[i], x_Image[i]);
                    count = count + dis * dis;
                }
                double RMS = Math.Sqrt(count / x_Image.Length);
                return RMS;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return -1;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            ChkIsCheck(chk.IsChecked == true ? true : false);
        }

        private void ChkIsCheck(bool _bool)
        {
            if (_bool)
            {
                frm_ModuleObj.m_EnableTwoPoint = true;
                TwoPointMethod folderList = new TwoPointMethod(frm_ModuleObj);
                Page_Change.Content = new Frame()
                {
                    Content = folderList
                };
            }
            else
            {
                frm_ModuleObj.m_EnableTwoPoint = false;
                Page_Change.Content = new Frame()
                {
                    Content = null
                };
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
    }
}
