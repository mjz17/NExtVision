using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Plugin.LapseTime
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
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        public override void theSecondTime()
        {
            txy_Lapse.Text = frm_ModuleObj.DelayTime.ToString();
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            if (ModuleProtect())
                return;
            ((ModuleObj)frm_ModuleObj).DelayTime = int.Parse(this.txy_Lapse.Text);
            ((ModuleObj)m_ModuleObjBase).blnNewModule = true;//参数已经保存

            this.Close();
        }

        private bool ModuleProtect()
        {
            //判断输入是否为空
            if (string.IsNullOrEmpty(this.txy_Lapse.Text))
            {
                System.Windows.Forms.MessageBox.Show("输入为空！");
                string str = string.Format($"模块名称：{frm_ModuleObj.ModuleParam.ModuleName}{"，故障信息："}{"输入为空！"}");
                Log.Error(str);
                return true;
            }

            //判断输入的是否为整数
            Regex regex = new Regex("^[1-9]\\d*$");
            if (!regex.IsMatch(this.txy_Lapse.Text))
            {
                System.Windows.Forms.MessageBox.Show("输入不为整数！");
                string str = string.Format($"模块名称：{frm_ModuleObj.ModuleParam.ModuleName}{"，故障信息："}{"输入不为整数！"}");
                Log.Error(str);
                return true;
            }

            return false;
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
