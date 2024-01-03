using Common;
using ModuleDataVar;
using StyleBase;
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

namespace Plugin.DataEntry
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        //窗体对应参数
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

                InitCmbDataQueue();

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

        /// <summary>
        /// 首次打开
        /// </summary>
        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        /// <summary>
        /// 非首次打开
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();

            Cmb_DataQueue.Text = frm_ModuleObj.LinkDataOut;//链接数据出列
            this.dgv_DataIn.ItemsSource = frm_ModuleObj.DataInInfo;
            m_Index = frm_ModuleObj.m_QueueIndex.ToString();//起始索引
        }

        #region 初始化

        public void InitCmbDataQueue()
        {
            List<string> info = new List<string>();
            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                List<ModuleObjBase> prj = item.m_ModuleObjList.FindAll(c => c.ModuleParam.PluginName.Contains("数据出队"));

                foreach (ModuleObjBase var in prj)
                {
                    info.Add(var.m_ModuleProject.ProjectInfo.m_ProjectName + "." + var.ModuleParam.ModuleName);
                }

            }
            Cmb_DataQueue.ItemsSource = info;

        }

        #endregion

        #region 起始索引

        public string m_Index
        {
            get { return (string)this.GetValue(m_IndexProperty); }
            set { this.SetValue(m_IndexProperty, value); }
        }

        public static readonly DependencyProperty m_IndexProperty =
            DependencyProperty.Register("m_Index", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel();

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (m_DataVar.m_DataValue != null && conStatus == true)
            {
                DataInInfo info = new DataInInfo();

                info.DataID = m_DataVar.m_DataModuleID;
                info.DataName = m_DataVar.m_DataName;
                info.DataTip = m_DataVar.m_DataTip;

                if (m_DataVar.m_DataType.ToString().Contains("Bool"))
                {
                    info.m_DataTypeIn = DataQueueType.Bool;
                }
                else if (m_DataVar.m_DataType.ToString().Contains("Int"))
                {
                    info.m_DataTypeIn = DataQueueType.Int;//Double
                }
                else if (m_DataVar.m_DataType.ToString().Contains("Double"))
                {
                    info.m_DataTypeIn = DataQueueType.Double;//Double
                }
                frm_ModuleObj.AddQueue(info);//添加入队数据
            }

            RefreshList();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataIn.SelectedItem != null)
            {
                DataInInfo inInfo = (DataInInfo)dgv_DataIn.SelectedItem;
                int Index = frm_ModuleObj.DataInInfo.FindIndex(c => c.DataID == inInfo.DataID && c.DataName == inInfo.DataName && c.DataTip == inInfo.DataTip);
                frm_ModuleObj.DataInInfo.RemoveAt(Index);
                RefreshList();
            }
        }

        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataIn.SelectedItem != null)
            {
                DataInInfo inInfo = dgv_DataIn.SelectedItem as DataInInfo;

                int index = frm_ModuleObj.DataInInfo.IndexOf(inInfo);

                if (index == 0)
                    return;

                frm_ModuleObj.DataInInfo.RemoveAt(index);
                index = index - 1;
                frm_ModuleObj.DataInInfo.Insert(index, inInfo);

                RefreshList();
            }
        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataIn.SelectedItem != null)
            {
                DataInInfo inInfo = dgv_DataIn.SelectedItem as DataInInfo;

                int index = frm_ModuleObj.DataInInfo.IndexOf(inInfo);

                if (index == dgv_DataIn.Items.Count - 1)
                    return;

                frm_ModuleObj.DataInInfo.RemoveAt(index);
                index = index + 1;
                frm_ModuleObj.DataInInfo.Insert(index, inInfo);

                RefreshList();
            }
        }

        public void RefreshList()
        {
            dgv_DataIn.ItemsSource = null;
            dgv_DataIn.ItemsSource = frm_ModuleObj.DataInInfo;
        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
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
                    frm_ModuleObj.LinkDataOut = Cmb_DataQueue.Text;//链接数据出列
                    frm_ModuleObj.m_QueueIndex = Convert.ToInt32(m_Index);//起始索引

                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    frm_ModuleObj.ExeModule();
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
                    frm_ModuleObj.LinkDataOut = Cmb_DataQueue.Text;//链接数据出列
                    frm_ModuleObj.m_QueueIndex = Convert.ToInt32(m_Index);//起始索引
                }

                ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
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
            base.ProtectModuel();

            if (Cmb_DataQueue.Text.Length < 1)
            {
                System.Windows.Forms.MessageBox.Show("未选择出列！");
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
