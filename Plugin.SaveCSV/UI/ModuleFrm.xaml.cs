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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Plugin.SaveCSV
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

        //首次打开
        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        //第二次打开
        public override void theSecondTime()
        {
            RefreshList();
            txt_SavePath.Text = frm_ModuleObj.m_SavePath;//保存地址
            Txt_FileName.Text = frm_ModuleObj.m_SaveName;//保存名称
            Chk_FileStatus.IsChecked = frm_ModuleObj.m_IsClearFile;//是否清理
            ClearTime = frm_ModuleObj.m_ClearTime;//清除时间

            Chk_Custom.IsChecked = frm_ModuleObj.m_IsColumnHead;//是否自定义列头
            Txt_ColumnHead.Text = frm_ModuleObj.m_ColumnHead;//自定义数据

        }


        #region 保存文件夹清除时间

        public int ClearTime
        {
            get { return (int)this.GetValue(ClearTimeProperty); }
            set { this.SetValue(ClearTimeProperty, value); }
        }

        public static readonly DependencyProperty ClearTimeProperty =
            DependencyProperty.Register("ClearTime", typeof(int), typeof(ModuleFrm), new PropertyMetadata((int)25));

        #endregion

        /// <summary>
        /// 存储路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SearchFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txt_SavePath.Text = frm_ModuleObj.m_SavePath = dialog.SelectedPath;
            }
        }

        private DataVar m_DataVar;
        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            LinkVarFrm linkVar = new LinkVarFrm();
            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel();
            linkVar.DataContext = dataviewModel;
            dataviewModel.sendMessage = Recevie;
            bool? conStatus = linkVar.ShowDialog();
            if (conStatus == true)
            {
                frm_ModuleObj.m_LinkData.Add(m_DataVar);
                RefreshList();
            }
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_PlcData.SelectedIndex > -1)
                {
                    int index = dgv_PlcData.SelectedIndex;
                    frm_ModuleObj.m_LinkData.RemoveAt(index);
                    RefreshList();
                    dgv_PlcData.SelectedIndex = index;
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
            try
            {
                int index = dgv_PlcData.SelectedIndex;
                if (index > -1)
                {
                    DataVar var = frm_ModuleObj.m_LinkData[index];
                    int addIndex = frm_ModuleObj.m_LinkData.IndexOf(var);
                    if (addIndex == 0)
                        return;
                    frm_ModuleObj.m_LinkData.RemoveAt(index);
                    index = index - 1;
                    frm_ModuleObj.m_LinkData.Insert(index, var);
                    RefreshList();
                    dgv_PlcData.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_PlcData.SelectedIndex;
                if (index > -1)
                {
                    DataVar var = frm_ModuleObj.m_LinkData[index];
                    int DeleteIndex = frm_ModuleObj.m_LinkData.IndexOf(var);
                    if (DeleteIndex == frm_ModuleObj.m_LinkData.Count - 1)
                        return;
                    frm_ModuleObj.m_LinkData.RemoveAt(DeleteIndex);
                    DeleteIndex = DeleteIndex + 1;
                    frm_ModuleObj.m_LinkData.Insert(DeleteIndex, var);
                    RefreshList();
                    dgv_PlcData.SelectedIndex = DeleteIndex;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public void RefreshList()
        {
            dgv_PlcData.ItemsSource = null;
            dgv_PlcData.ItemsSource = frm_ModuleObj.m_LinkData;
        }

        /// <summary>
        /// 保存
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
        /// 保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            if (txt_SavePath.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("路径未设置！");
                return false;
            }

            frm_ModuleObj.m_SavePath = txt_SavePath.Text;

            if (Txt_FileName.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("文件名称未设置！");
                return false;
            }

            frm_ModuleObj.m_SaveName = Txt_FileName.Text;
            frm_ModuleObj.m_IsClearFile = Chk_FileStatus.IsChecked == true ? true : false;//是否清除
            frm_ModuleObj.m_ClearTime = (int)ClearTime;//清除时间
            frm_ModuleObj.m_IsColumnHead = Chk_Custom.IsChecked == true ? true : false;//是否定义列头
            frm_ModuleObj.m_ColumnHead = Txt_ColumnHead.Text;//列头内容

            return true;
        }

        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }
    }
}
