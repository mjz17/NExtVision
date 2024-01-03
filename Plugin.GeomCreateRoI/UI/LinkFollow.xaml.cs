using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
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

namespace Plugin.GeomCreateRoI
{
    /// <summary>
    /// LinkFollow.xaml 的交互逻辑
    /// </summary>
    public partial class LinkFollow : UserControl
    {
        //后台程序
        private ModuleObj frm_ModuleObj;
        //构造函数
        public LinkFollow(ModuleObj obj)
        {
            InitializeComponent();
            this.DataContext = this;

            this.frm_ModuleObj = obj;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            if (frm_ModuleObj.blnNewModule)
            {
                theSecondtime();
            }
        }

        public void theSecondtime()
        {
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
                }
            }

            #endregion
        }

        /// <summary>
        /// 当前模块ID
        /// </summary>
        public string CurrentModelID
        {
            get { return (string)this.GetValue(CurrentModelIDProperty); }
            set { this.SetValue(CurrentModelIDProperty, value); }
        }

        public static readonly DependencyProperty CurrentModelIDProperty =
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(LinkFollow), new PropertyMetadata(default(string)));

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
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        #endregion

    }
}
