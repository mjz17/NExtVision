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

namespace Plugin.ArraySet
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        public List<DataVarTool> frm_DataVar = new List<DataVarTool>();


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
            frm_DataVar = frm_ModuleObj.m_DataVar;
            this.dgv_DataVar.ItemsSource = frm_DataVar;
        }

        private void btn_AddVar_Click(object sender, RoutedEventArgs e)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel();//添加数据，不监控输入数据类型

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (conStatus == true)
            {
                DataVarTool dataVar = new DataVarTool();

                //数据类型
                switch (m_DataVar.m_DataType)
                {
                    case DataVarType.DataType.Int_Array:
                        dataVar.m_DataType = DataQueueType.IntArr;
                        break;
                    case DataVarType.DataType.Double_Array:
                        dataVar.m_DataType = DataQueueType.DoubleArr;
                        break;
                    case DataVarType.DataType.Bool_Array:
                        dataVar.m_DataType = DataQueueType.BoolArr;
                        break;
                    case DataVarType.DataType.String_Array:
                        dataVar.m_DataType = DataQueueType.StringArr;
                        break;
                    default:
                        break;
                }

                //链接读取
                dataVar.Link_DataVar_Read = m_DataVar;

                //链接模块ID
                dataVar.Link_DataVar_Read_Name = m_DataVar.m_DataTip + "." + m_DataVar.m_DataName;

                //dataVar.Link_DataVar_Read_Name = m_DataVar.m_DataName + "." + m_DataVar.m_DataTip;

                //链接写入
                dataVar.Link_DataVar_Write_Name = "NULL";

                //结果

                //注释

                frm_DataVar.Add(dataVar);
            }
            RefreshList();
        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_DataVar.SelectedIndex;
                if (index > -1)
                {
                    frm_DataVar.RemoveAt(index);
                    RefreshList();
                    dgv_DataVar.SelectedIndex = index - 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

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
        /// 点击DGV，显示链接界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_DataVar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgv_DataVar.SelectedItem != null)
            {
                e.Handled = true;
                //Int32 row = this.dgv_DataVar.Items.IndexOf(this.dgv_DataVar.CurrentItem);
                Int32 Col = this.dgv_DataVar.Columns.IndexOf(this.dgv_DataVar.CurrentColumn);//获取列位置
                string name = dgv_DataVar.Columns[Col].Header.ToString();
                if (name.Contains("链接2"))
                {
                    ShowLinfkFrm(dgv_DataVar.SelectedIndex);
                }
            }
        }

        private void ShowLinfkFrm(int index)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("全局变量");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (conStatus == true)
            {
                //链接写入
                frm_DataVar[index].Link_DataVar_Write = m_DataVar;

                if (m_DataVar.m_DataAtrr.ToString().Contains("全局变量"))
                {
                    frm_DataVar[index].Link_DataVar_Write_Name = m_DataVar.m_DataAtrr + "." + m_DataVar.m_DataName.ToString();
                }
                else
                {
                    frm_DataVar[index].Link_DataVar_Write_Name = m_DataVar.m_DataTip + "." + m_DataVar.m_DataName.ToString();
                }
            }
            RefreshList();
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                //删除本ID的变量
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                }

                //变量设置，添加保护模块
                frm_ModuleObj.m_DataVar = frm_DataVar;//数据存储
                frm_ModuleObj.ExeModule();
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
                //变量设置，添加保护模块

                frm_ModuleObj.m_DataVar = frm_DataVar;//数据存储
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
