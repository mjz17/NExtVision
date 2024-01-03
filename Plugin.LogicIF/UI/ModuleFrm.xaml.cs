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

namespace Plugin.LogicIF
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
            Logic_True.IsChecked = true;
            frm_ModuleObj.m_LogicInfo = LogicInfo.True;
            ShowNewFrm(LogicInfo.True);
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            InitRadio(frm_ModuleObj.m_LogicInfo);
        }

        #region 初始化

        /// <summary>
        /// 初始化RadioButton
        /// </summary>
        /// <param name="logic"></param>
        public void InitRadio(LogicInfo logic)
        {
            switch (logic)
            {
                case LogicInfo.True:
                    Logic_True.IsChecked = true;
                    ShowNewFrm(logic);
                    break;
                case LogicInfo.False:
                    Logic_False.IsChecked = true;
                    ShowNewFrm(logic);
                    break;
                case LogicInfo.上升沿:
                    Logic_And.IsChecked = true;
                    ShowNewFrm(logic);
                    break;
                case LogicInfo.下降沿:
                    Logic_Or.IsChecked = true;
                    ShowNewFrm(logic);
                    break;
                default:
                    break;
            }
        }

        void ShowNewFrm(LogicInfo logic)
        {
            Page_Change.Content = new Frame()
            {
                Content = null
            };

            switch (logic)
            {
                case LogicInfo.True:
                case LogicInfo.False:
                    ChooseModel1 choose1 = new ChooseModel1(frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = choose1
                    };
                    break;
                case LogicInfo.上升沿:
                case LogicInfo.下降沿:
                    ChooseModel2 choose2 = new ChooseModel2(frm_ModuleObj);
                    Page_Change.Content = new Frame()
                    {
                        Content = choose2
                    };
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Radiu显示窗体

        private void Logic_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            switch (radio.Name)
            {
                case "Logic_True":
                    frm_ModuleObj.m_LogicInfo = LogicInfo.True;
                    ShowNewFrm(LogicInfo.True);
                    break;
                case "Logic_False":
                    frm_ModuleObj.m_LogicInfo = LogicInfo.False;
                    ShowNewFrm(LogicInfo.False);
                    break;
                case "Logic_And":
                    frm_ModuleObj.m_LogicInfo = LogicInfo.上升沿;
                    ShowNewFrm(LogicInfo.上升沿);
                    break;
                case "Logic_Or":
                    frm_ModuleObj.m_LogicInfo = LogicInfo.下降沿;
                    ShowNewFrm(LogicInfo.下降沿);
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 执行模块
        /// </summary>
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
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
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
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 参数保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            base.ProtectModuel();

            switch (frm_ModuleObj.m_LogicInfo)
            {
                case LogicInfo.True:
                case LogicInfo.False:
                    if (frm_ModuleObj.m_Link_Data1.Length == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("链接1未设置！");
                        return false;
                    }
                    break;
                case LogicInfo.上升沿:
                case LogicInfo.下降沿:
                    if (frm_ModuleObj.m_Link_Data1.Length == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("链接1未设置！");
                        return false;
                    }
                    if (frm_ModuleObj.m_Link_Data2.Length == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("链接2未设置！");
                        return false;
                    }
                    break;
                default:
                    break;
            }
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
