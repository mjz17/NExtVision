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

namespace Plugin.ExecutionProcess
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private List<ExecutionType> Project_Module = new List<ExecutionType>();

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

        //第一次打开
        public override void theFirsttime()
        {
            ProListBox.ItemsSource = Project_Module = InitProcess();
            Rad_Order.IsChecked = true;
        }

        //第二次打开
        public override void theSecondTime()
        {
            switch (frm_ModuleObj.m_execution)
            {
                case ExecutionMode.顺序单次执行:
                    Rad_Order.IsChecked = true;
                    Rad_Para.IsChecked = false;
                    break;
                case ExecutionMode.并行单次执行:
                    Rad_Para.IsChecked = true;
                    Rad_Order.IsChecked = false;
                    break;
                default:
                    break;
            }
            //查询所有流程
            Project_Module = InitProcess();
            foreach (var item in frm_ModuleObj.Project_Module)
            {
                ExecutionType var = Project_Module.Find(c => c.ProjectName == item.ProjectName);
                var.SingleType = item.SingleType;
                var.WaitType = item.WaitType;
            }
            ProListBox.ItemsSource = Project_Module;
        }

        private List<ExecutionType> InitProcess()
        {
            List<ExecutionType> lst = new List<ExecutionType>();
            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                lst.Add(new ExecutionType
                {
                    ProjectName = item.ProjectInfo.m_ProjectName
                });
            }
            return lst;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rad_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Name.Contains("Rad_Order"))
            {
                frm_ModuleObj.m_execution = ExecutionMode.顺序单次执行;
            }
            else
            {
                frm_ModuleObj.m_execution = ExecutionMode.并行单次执行;
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
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 流程保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            frm_ModuleObj.Project_Module = Project_Module;
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
