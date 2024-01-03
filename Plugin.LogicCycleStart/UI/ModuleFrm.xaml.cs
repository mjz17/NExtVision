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

namespace Plugin.LogicCycleStart
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
                //初始化
                InitCycleInfo();
                cmb_CycleModel.DropDownClosed += Cmb_CycleModel_DropDownClosed;

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
            showPage();
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            cmb_CycleModel.SelectedIndex = (int)frm_ModuleObj.m_logicCycle;
            showPage();
        }

        private void InitCycleInfo()
        {
            cmb_CycleModel.ItemsSource = Enum.GetValues(typeof(LogicCycle)).OfType<LogicCycle>().ToList();
            cmb_CycleModel.SelectedIndex = 0;
        }

        private void Cmb_CycleModel_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_logicCycle = (LogicCycle)cmb_CycleModel.SelectedIndex;
            showPage();
        }

        private void showPage()
        {
            Page_Change1.Content = new Frame()
            {
                Content = null
            };
            switch ((LogicCycle)cmb_CycleModel.SelectedIndex)
            {
                case LogicCycle.从Start到End:
                    StartToEnd camera = new StartToEnd(frm_ModuleObj);
                    Page_Change1.Content = new Frame()
                    {
                        Content = camera
                    };
                    break;
                case LogicCycle.从End到Start:
                    break;
                case LogicCycle.无限循环:
                    break;
                case LogicCycle.遍历数组:
                    QueryArray query = new QueryArray(frm_ModuleObj);
                    Page_Change1.Content = new Frame()
                    {
                        Content = query
                    };
                    break;
                default:
                    break;
            }
        }

        public override void SaveModuleParam()
        {
            base.SaveModuleParam();
            try
            {
                ((ModuleObj)m_ModuleObjBase).ModuleParam.pIndex = -1;

                frm_ModuleObj.ExeModule();

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
            return true;
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
