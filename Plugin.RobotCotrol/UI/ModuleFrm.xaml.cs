using Common;
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

namespace Plugin.RobotCotrol
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        //窗体对应的后台数据
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

                InitCmbNpc();
                InitCmb_CameraType();

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

        //首次打开
        public override void theFirsttime()
        {
            base.theFirsttime();
            showPage();
        }

        //第二次打开
        public override void theSecondTime()
        {
            base.theSecondTime();
            //N点标定
            Cmb_Npc.Text = frm_ModuleObj.ProcessName;
            //拍照方式
            Cmb_CameraType.SelectedIndex = (int)frm_ModuleObj.m_CameraType;
            //显示窗体
            showPage();
        }

        #region 初始化

        /// <summary>
        /// 初始化CmbNpc
        /// </summary>
        private void InitCmbNpc()
        {
            List<string> info = new List<string>();

            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                int Num = item.m_ModuleObjList.FindIndex(c => c.ModuleParam.PluginName.Contains("N点标定"));

                if (Num > -1)
                {
                    info.Add(item.ProjectInfo.m_ProjectName + "." + item.m_ModuleObjList[Num].ModuleParam.PluginName);
                }
            }

            Cmb_Npc.ItemsSource = info;
        }

        /// <summary>
        /// 拍照方式
        /// </summary>
        private void InitCmb_CameraType()
        {
            List<string> info = new List<string>();

            foreach (string item in Enum.GetNames(typeof(CameraType)))
            {
                info.Add(item);
            }

            Cmb_CameraType.ItemsSource = info;
            Cmb_CameraType.SelectedIndex = 0;
        }

        #endregion

        #region 机械坐标X

        public double Mach_X
        {
            get { return (double)this.GetValue(Mach_XProperty); }
            set { this.SetValue(Mach_XProperty, value); }
        }

        public static readonly DependencyProperty Mach_XProperty =
            DependencyProperty.Register("Mach_X", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        #endregion

        #region 机械坐标Y

        public double Mach_Y
        {
            get { return (double)this.GetValue(Mach_YProperty); }
            set { this.SetValue(Mach_YProperty, value); }
        }

        public static readonly DependencyProperty Mach_YProperty =
            DependencyProperty.Register("Mach_Y", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        #endregion

        #region 输出角度

        public double Mach_Phi
        {
            get { return (double)this.GetValue(Mach_PhiProperty); }
            set { this.SetValue(Mach_PhiProperty, value); }
        }

        public static readonly DependencyProperty Mach_PhiProperty =
            DependencyProperty.Register("Mach_Phi", typeof(double), typeof(ModuleFrm), new PropertyMetadata(default(double)));

        #endregion

        private void Cmb_CameraType_DropDownClosed(object sender, EventArgs e)
        {
            showPage();
        }

        private void showPage()
        {
            Page_Change1.Content = new Frame()
            {
                Content = null
            };
            Page_Change2.Content = new Frame()
            {
                Content = null
            };

            switch ((CameraType)Cmb_CameraType.SelectedIndex)
            {
                case CameraType.固定相机先拍再取或放:
                    CameraRobot camera = new CameraRobot(frm_ModuleObj);
                    Page_Change2.Content = new Frame()
                    {
                        Content = camera
                    };
                    break;
                case CameraType.固定相机抓取后拍照:
                    RobotPostion robot = new RobotPostion(frm_ModuleObj);
                    Page_Change1.Content = new Frame()
                    {
                        Content = robot
                    };
                    RobotCamera robot1 = new RobotCamera(frm_ModuleObj);
                    Page_Change2.Content = new Frame()
                    {
                        Content = robot1
                    };
                    break;
                case CameraType.运动相机先拍再取或放:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                if (ProtectModuel())
                {
                    frm_ModuleObj.ExeModule();
                    Mach_X = frm_ModuleObj.m_OutX;
                    Mach_Y = frm_ModuleObj.m_OutY;
                    Mach_Phi = frm_ModuleObj.m_OutPhi;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
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
        /// 保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            //链接N点标定
            if (Cmb_Npc.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("未设置N点标定链接！");
                return false;
            }

            //拍照方式
            if (Cmb_CameraType.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("未设置拍照方式！");
                return false;
            }

            //输入图像坐标X
            if (frm_ModuleObj.Link_InputImgX.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("输入图像坐标X,未设置！");
                return false;
            }

            //输入图像坐标Y
            if (frm_ModuleObj.Link_InputImgY.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("输入图像坐标Y,未设置！");
                return false;
            }

            ////输入角度
            //if (frm_ModuleObj.Link_InputPhi.m_DataValue == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("输入角度,未设置！");
            //    return false;
            //}

            frm_ModuleObj.ProcessName = Cmb_Npc.Text;
            frm_ModuleObj.m_CameraType = (CameraType)Cmb_CameraType.SelectedIndex;

            //frm_ModuleObj.m_InputImgX = InputImage_x;
            //frm_ModuleObj.m_InputImgY = InputImage_y;
            //frm_ModuleObj.m_InputPhi = InPutPhi;

            return true;
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }
    }
}
