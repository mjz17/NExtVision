using Common;
using ModuleDataVar;
using PublicDefine;
using StyleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Plugin.ImagesUnit
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
            //标题名称
            Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;

            try
            {
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
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
        }

        public override void ExModule()
        {
            try
            {
                //运行模块
                frm_ModuleObj.ExeModule();
            }
            catch (Exception)
            {

                throw;
            }
        }

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

        public override bool ProtectModuel()
        {
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frm_ModuleObj.ImageUnit.Add(new ImageUnitPram
                {
                    m_UnitName = "####",
                    m_Row1 = 0,
                    m_Col1 = 0,
                    m_Row2 = 0,
                    m_Col2 = 0,
                });
                Refresh();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_ImageUnit.SelectedIndex > -1)
                {
                    int index = dgv_ImageUnit.SelectedIndex;
                    frm_ModuleObj.ImageUnit.RemoveAt(dgv_ImageUnit.SelectedIndex);
                    Refresh();
                    if (index < 0)
                    {
                        if (frm_ModuleObj.ImageUnit.Count > 0)
                            dgv_ImageUnit.SelectedIndex = 0;
                    }
                    else
                    {
                        if (frm_ModuleObj.ImageUnit.Count > 0)
                            dgv_ImageUnit.SelectedIndex = index - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        /// <summary>
        /// 双击显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_ImageUnit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgv_ImageUnit.SelectedItem != null)
                {
                    e.Handled = true;
                    Int32 Col = this.dgv_ImageUnit.Columns.IndexOf(this.dgv_ImageUnit.CurrentColumn);//获取列位置
                    string name = dgv_ImageUnit.Columns[Col].Header.ToString();
                    int index = dgv_ImageUnit.SelectedIndex;
                    if (name.Contains("待合并图像"))
                    {
                        ShowLinfkFrm(index);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

        private void ShowLinfkFrm(int index)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("采集图像");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (conStatus == true)
            {
                frm_ModuleObj.ImageUnit[index].m_UnitName = m_DataVar.m_DataTip + "." + m_DataVar.m_DataName;
            }
            Refresh();
        }

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        private void Refresh()
        {
            this.dgv_ImageUnit.ItemsSource = null;
            this.dgv_ImageUnit.ItemsSource = frm_ModuleObj.ImageUnit;
        }


    }
}
