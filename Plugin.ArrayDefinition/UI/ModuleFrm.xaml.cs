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

namespace Plugin.ArrayDefinition
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        //窗体对应参数
        private ModuleObj frm_ModuleObj;

        List<DataVarTool> frm_DataVar = new List<DataVarTool>();

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

        //初次打开
        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        //非初次打开
        public override void theSecondTime()
        {
            base.theSecondTime();
            frm_ModuleObj.QueryData();//查询数据
            frm_DataVar = frm_ModuleObj.m_DataVar;
            this.dgv_DataVar.ItemsSource = frm_DataVar;
        }

        /// <summary>
        /// 添加整数类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Addint_Click(object sender, RoutedEventArgs e)
        {
            frm_DataVar.Add(new DataVarTool 
            { 
                m_DataType = DataQueueType.IntArr,
                m_DataName = "Vaule" + frm_DataVar.Count,
                m_DataTip = "Null", 
            });
            RefreshList();
        }

        /// <summary>
        /// 添加Double类型的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddDouble_Click(object sender, RoutedEventArgs e)
        {
            frm_DataVar.Add(new DataVarTool 
            { 
                m_DataType = DataQueueType.DoubleArr, 
                m_DataName = "Vaule" + frm_DataVar.Count,
                m_DataTip = "Null", 
            });
            RefreshList();
        }

        /// <summary>
        /// 添加字符类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddString_Click(object sender, RoutedEventArgs e)
        {
            frm_DataVar.Add(new DataVarTool 
            { 
                m_DataType = DataQueueType.StringArr,
                m_DataName = "Vaule" + frm_DataVar.Count, 
                m_DataTip = "Null",
            });
            RefreshList();
        }

        /// <summary>
        /// 添加布尔类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Addbool_Click(object sender, RoutedEventArgs e)
        {
            frm_DataVar.Add(new DataVarTool
            {
                m_DataType = DataQueueType.BoolArr,
                m_DataName = "Vaule" + frm_DataVar.Count,
                m_DataTip = "Null",
            });
            RefreshList();
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataVar.SelectedItem != null)
            {
                DataVarTool m_Info = (DataVarTool)dgv_DataVar.SelectedItem;
                int Index = frm_DataVar.FindIndex(c => c.m_DataName == m_Info.m_DataName && c.m_DataType == m_Info.m_DataType);
                frm_DataVar.RemoveAt(Index);
                DelectValue(frm_ModuleObj.ModuleParam.ModuleName + "." + m_Info.m_DataName, frm_ModuleObj.ModuleParam.ModuleID);
                RefreshList();
            }
        }

        private void DelectValue(string VarName, int ModuleID)
        {
            int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
            if (proIndex > -1)
            {
                int Index = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataTip == VarName && c.m_DataModuleID == ModuleID);
                if (Index > -1)
                {
                    SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAt(Index);
                }
            }
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataVar.SelectedItem != null)
            {
                DataVarTool inInfo = dgv_DataVar.SelectedItem as DataVarTool;
                int index = frm_DataVar.IndexOf(inInfo);
                if (index == 0)
                    return;

                frm_DataVar.RemoveAt(index);
                index = index - 1;
                frm_DataVar.Insert(index, inInfo);
                RefreshList();
            }
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataVar.SelectedItem != null)
            {
                DataVarTool inInfo = dgv_DataVar.SelectedItem as DataVarTool;
                int index = frm_DataVar.IndexOf(inInfo);
                if (index == dgv_DataVar.Items.Count - 1)
                    return;

                frm_DataVar.RemoveAt(index);
                index = index + 1;
                frm_DataVar.Insert(index, inInfo);
                RefreshList();
            }
        }

        public void RefreshList()
        {
            dgv_DataVar.ItemsSource = null;
            dgv_DataVar.ItemsSource = frm_DataVar;
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                if (ProtectModuel())
                    return;

                frm_ModuleObj.m_DataVar = frm_DataVar;//数据存储
                frm_ModuleObj.ExeModule();//执行一次模块
                ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
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
        public override bool ProtectModuel()
        {
            //判断是否有重复变量名称
            bool isRepeat = frm_DataVar.GroupBy(i => i.m_DataName).Where(g => g.Count() > 1).Count() > 0;
            if (isRepeat)
            {
                System.Windows.Forms.MessageBox.Show("变量定义,名称存在重复！");
                return true;
            }

            //删除本ID的变量
            int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
            if (proIndex > -1)
            {
                SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
            }


            return false;
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
