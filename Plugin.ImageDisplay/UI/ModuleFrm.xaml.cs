using Common;
using ModuleDataVar;
using PublicDefine;
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

namespace Plugin.ImageDisplay
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
            //加载参数
            frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
            //窗体名称
            Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            //显示窗体
            InitCmbHwindow();
            try
            {
                if (frm_ModuleObj.blnNewModule)
                    theSecondTime();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            #region 读取链接图像并显示

            CurrentImage = frm_ModuleObj.m_CurentImgName;

            #endregion
            #region 显示的窗体

            Cmb_DispHwindow.Text = frm_ModuleObj.HwindowName;

            #endregion
            RefreshList();
        }

        #region 窗体显示图像

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_ImgPath_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Image)
            {
                try
                {
                    if (data.m_DataValue is List<HImageExt>)
                    {
                        frm_ModuleObj.m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                        frm_ModuleObj.Link_Image_Data = data;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        #region 显示窗体列表

        private void InitCmbHwindow()
        {
            List<string> Cmb_Hwindow = new List<string>();
            foreach (LayoutInfo item in SysLayout.Frm_Info)
            {
                Cmb_Hwindow.Add(item.Name);
            }
            Cmb_DispHwindow.ItemsSource = Cmb_Hwindow;
            Cmb_DispHwindow.SelectedIndex = 0;
        }

        #endregion

        /// <summary>
        /// 执行一次
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
                    m_ModuleObjBase.ExeModule(true);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
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
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 参数保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            //图像未设置
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像未设置！");
                return false;
            }

            //判断图像类型
            if (!(frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>))
            {
                System.Windows.Forms.MessageBox.Show("图像类型错误！");
                return false;
            }

            //判断图像类型
            if (Cmb_DispHwindow.Text == null)
            {
                System.Windows.Forms.MessageBox.Show("图像显示窗体未设置！");
                return false;
            }

            //图像名称
            frm_ModuleObj.m_CurentImgName = CurrentImage;
            //显示的窗体名称
            frm_ModuleObj.HwindowName = Cmb_DispHwindow.Text;

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

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frm_ModuleObj.LinkInfo.Add(new DispInfo());
                int index = dgv_Link.SelectedIndex;
                RefreshList();
                dgv_Link.SelectedIndex = index;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Delet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_Link.SelectedIndex;
                if (index > -1)
                {
                    frm_ModuleObj.LinkInfo.RemoveAt(index);
                    RefreshList();
                    dgv_Link.SelectedIndex = index - 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 双击DatagridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_DataVar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgv_Link.SelectedItem != null)
            {
                e.Handled = true;
                //Int32 row = this.dgv_DataVar.Items.IndexOf(this.dgv_DataVar.CurrentItem);

                //获取列位置
                Int32 Col = this.dgv_Link.Columns.IndexOf(this.dgv_Link.CurrentColumn);
                string name = dgv_Link.Columns[Col].Header.ToString();
                if (name.Contains("显示数据链接"))
                {
                    LinkVarFrm linkVar = new LinkVarFrm();
                    LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel();//添加数据，不监控输入数据类型
                    linkVar.DataContext = dataviewModel;
                    dataviewModel.sendMessage = Recevie;
                    bool? conStatus = linkVar.ShowDialog();
                    if (conStatus == true)
                    {
                        int index = dgv_Link.SelectedIndex;
                        frm_ModuleObj.LinkInfo[index].m_linkInfo = m_DataVar.m_DataTip + "." + m_DataVar.m_DataName.ToString();
                        frm_ModuleObj.LinkInfo[index].m_DataVar = m_DataVar;
                        RefreshList();
                    }
                }
            }
        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

        public void RefreshList()
        {
            this.dgv_Link.ItemsSource = null;
            this.dgv_Link.ItemsSource = frm_ModuleObj.LinkInfo;
        }



    }
}
