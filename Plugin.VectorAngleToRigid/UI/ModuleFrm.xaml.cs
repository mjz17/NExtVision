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

namespace Plugin.VectorAngleToRigid
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

                InitCmbHomMat2d();
                if (!m_ModuleObjBase.blnNewModule)
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

        private void InitCmbHomMat2d()
        {
            List<string> list = new List<string>()
            {
                "[点->点]",
                "[点、角度->点、角度]",
                "[旋转中心角度]",
            };

            Cmb_HomMat2d.ItemsSource = list;
        }

        /// <summary>
        /// 首次打开
        /// </summary>
        public override void theFirsttime()
        {
            base.theFirsttime();
            Cmb_HomMat2d.SelectedIndex= 0;
            ShowPag();
        }

        /// <summary>
        /// 非首次打开
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();
        }

        /// <summary>
        /// 选择仿射方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmb_HomMat2d_DropDownClosed(object sender, EventArgs e)
        {
            ShowPag();
        }

        private void ShowPag()
        {
            Page_Change.Content = new Frame()
            {
                Content = null
            };
            switch (Cmb_HomMat2d.Text)
            {
                case "[点->点]":
                    PointToPoint pointTo = new PointToPoint(frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = pointTo
                    };
                    break;
                case "[点、角度->点、角度]":
                    PointAngleToPointAngle pointAngleTo = new PointAngleToPointAngle(frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = pointAngleTo
                    };
                    break;
                case "[旋转中心角度]":
                    RotationCenter rotation = new RotationCenter(frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = rotation
                    };
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 运行模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                if (ProtectModuel())
                {
                    frm_ModuleObj.ExeModule();

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
        /// 模块保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            ////链接N点标定
            //if (Cmb_Npc.Text.Length == 0)
            //{
            //    System.Windows.Forms.MessageBox.Show("未设置N点标定链接！");
            //    return false;
            //}

            ////拍照方式
            //if (Cmb_CameraType.Text.Length == 0)
            //{
            //    System.Windows.Forms.MessageBox.Show("未设置拍照方式！");
            //    return false;
            //}

            ////输入图像坐标X
            //if (frm_ModuleObj.Link_InputImgX.m_DataValue == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("输入图像坐标X,未设置！");
            //    return false;
            //}

            ////输入图像坐标Y
            //if (frm_ModuleObj.Link_InputImgY.m_DataValue == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("输入图像坐标Y,未设置！");
            //    return false;
            //}

            //////输入角度
            ////if (frm_ModuleObj.Link_InputPhi.m_DataValue == null)
            ////{
            ////    System.Windows.Forms.MessageBox.Show("输入角度,未设置！");
            ////    return false;
            ////}

            //frm_ModuleObj.ProcessName = Cmb_Npc.Text;
            //frm_ModuleObj.m_CameraType = (CameraType)Cmb_CameraType.SelectedIndex;

            ////frm_ModuleObj.m_InputImgX = InputImage_x;
            ////frm_ModuleObj.m_InputImgY = InputImage_y;
            ////frm_ModuleObj.m_InputPhi = InPutPhi;

            return true;
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }
    }
}
