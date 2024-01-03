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
using CommunaCation;

namespace Plugin.TcpSend
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
            this.DataContext= this;
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

                InitCmbEcomun();

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

        public override void theSecondTime()
        {
            base.theSecondTime();
            Cmb_EcomunCation.Text = frm_ModuleObj.m_ComunCation;
            LinkData = frm_ModuleObj.m_CurentLinkName;
            txt_char.Text = frm_ModuleObj.m_ResultChar;
            Txt_Remarks.Text = frm_ModuleObj.m_Remarks;//通讯备注
        }

        private void InitCmbEcomun()
        {
            List<string> EcommunInfo = new List<string>();
            foreach (var item in EComManageer.s_ECommunacationDic)
            {
                EcommunInfo.Add(Name = item.Value.Key);
            }
            Cmb_EcomunCation.ItemsSource = EcommunInfo;
        }

        #region 链接的数据,只链接String

        public string LinkData
        {
            get { return (string)this.GetValue(LinkDataProperty); }
            set { this.SetValue(LinkDataProperty, value); }
        }

        public static readonly DependencyProperty LinkDataProperty =
            DependencyProperty.Register("LinkData", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_DataInfo_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            try
            {
                if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.String)
                {
                    frm_ModuleObj.Link_Data = data;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                if (ProtectModuel())
                {
                    frm_ModuleObj.m_ComunCation = Cmb_EcomunCation.Text;    //保存通讯链接名称
                    frm_ModuleObj.m_CurentLinkName = LinkData;
                    frm_ModuleObj.m_ResultChar = txt_char.Text;             //间隔符
                    frm_ModuleObj.m_Remarks = Txt_Remarks.Text;//通讯备注
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
            //通讯链接对象
            if (Cmb_EcomunCation.SelectedIndex== -1) 
            {
                System.Windows.Forms.MessageBox.Show("未设备通讯链接对象！");
                return false;
            }

            //未设置链接数据
            if (frm_ModuleObj.Link_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("未设置链接数据！");
                return false;
            }

            return true;
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
