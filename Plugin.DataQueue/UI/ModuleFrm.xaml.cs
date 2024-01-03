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

namespace Plugin.DataQueue
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
            m_Limitlength = 1;
            Chk_DeleteData.IsChecked = true;
            Chk_LinitLength.IsChecked = true;
        }

        /// <summary>
        /// 非首次打开
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();

            Chk_Wait.IsChecked = frm_ModuleObj.IsWait;
            Chk_DeleteData.IsChecked = frm_ModuleObj.IsDeleteData;
            Chk_LinitLength.IsChecked = frm_ModuleObj.IsLimitLength;
            m_Limitlength = frm_ModuleObj.LimitLength;

            dgv_DataOut.ItemsSource = frm_ModuleObj.DataInInfo;
        }

        #region 限制的长度

        public int m_Limitlength
        {
            get { return (int)this.GetValue(m_LimitlengthProperty); }
            set { this.SetValue(m_LimitlengthProperty, value); }
        }

        public static readonly DependencyProperty m_LimitlengthProperty =
            DependencyProperty.Register("m_Limitlength", typeof(int), typeof(ModuleFrm), new PropertyMetadata(default(int)));

        #endregion

        /// <summary>
        /// 是否阻塞
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chk_Wait_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            frm_ModuleObj.IsWait = chk.IsChecked == true ? true : false;
        }

        /// <summary>
        /// 是否删除出队数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chk_DeleteData_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            frm_ModuleObj.IsDeleteData = chk.IsChecked == true ? true : false;
        }

        /// <summary>
        /// 是否限制长度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chk_IsLimitLength_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            frm_ModuleObj.IsLimitLength = chk.IsChecked == true ? true : false;
        }

        private void btn_Addbool_Click(object sender, RoutedEventArgs e)
        {
            if (frm_ModuleObj.DataInInfo.Count < 1)
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.Bool,
                    DataName = "Vaule0",
                    m_DataQueueIn = 0,
                });
            }
            else
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.Bool,
                    DataName = "Vaule" + frm_ModuleObj.DataInInfo.Count,
                    m_DataQueueIn = 0,
                });
            }

            RefreshList();
        }

        private void btn_Addint_Click(object sender, RoutedEventArgs e)
        {
            if (frm_ModuleObj.DataInInfo.Count < 1)
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.Int,
                    DataName = "Vaule0",
                    m_DataQueueIn = 0,
                });
            }
            else
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.Int,
                    DataName = "Vaule" + frm_ModuleObj.DataInInfo.Count,
                    m_DataQueueIn = 0,
                });
            }
            RefreshList();
        }

        private void btn_AddString_Click(object sender, RoutedEventArgs e)
        {
            if (frm_ModuleObj.DataInInfo.Count < 1)
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.String,
                    DataName = "Vaule0",
                    m_DataQueueIn = 0,
                });
            }
            else
            {
                frm_ModuleObj.DataInInfo.Add(new DataInInfo
                {
                    m_DataTypeIn = DataQueueType.String,
                    DataName = "Vaule" + frm_ModuleObj.DataInInfo.Count,
                    m_DataQueueIn = 0,
                });
            }
            RefreshList();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataOut.SelectedItem != null)
            {
                DataInInfo inInfo = (DataInInfo)dgv_DataOut.SelectedItem;

                int Index = frm_ModuleObj.DataInInfo.FindIndex(c => c.DataID == inInfo.DataID && c.DataTip == inInfo.DataTip);

                frm_ModuleObj.DataInInfo.RemoveAt(Index);

                RefreshList();
            }
        }

        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_DataOut.SelectedItem != null)
            {
                DataInInfo inInfo = dgv_DataOut.SelectedItem as DataInInfo;

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
            if (dgv_DataOut.SelectedItem != null)
            {
                DataInInfo inInfo = dgv_DataOut.SelectedItem as DataInInfo;

                int index = frm_ModuleObj.DataInInfo.IndexOf(inInfo);

                if (index == dgv_DataOut.Items.Count - 1)
                    return;

                frm_ModuleObj.DataInInfo.RemoveAt(index);
                index = index + 1;
                frm_ModuleObj.DataInInfo.Insert(index, inInfo);

                RefreshList();
            }
        }

        public void RefreshList()
        {
            dgv_DataOut.ItemsSource = null;
            dgv_DataOut.ItemsSource = frm_ModuleObj.DataInInfo;
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                //判断是否有重复变量名称
                bool isRepeat = frm_ModuleObj.DataInInfo.GroupBy(i => i.DataID).Where(g => g.Count() > 1).Count() > 0;

                if (isRepeat)
                {
                    System.Windows.Forms.MessageBox.Show("变量名称存在重复！");
                    return;
                }

                frm_ModuleObj.LimitLength = m_Limitlength;
                frm_ModuleObj.m_DataOut = new DataOutQueue(frm_ModuleObj.m_ModuleProject.ProjectInfo.m_ProjectName + "." + frm_ModuleObj.ModuleParam.ModuleName);
                for (int i = 0; i < frm_ModuleObj.DataInInfo.Count; i++)
                {
                    switch (frm_ModuleObj.DataInInfo[i].m_DataTypeIn)
                    {
                        case DataQueueType.Bool:
                            frm_ModuleObj.m_DataOut.DefineBoolQueue();
                            break;
                        case DataQueueType.Int:
                            frm_ModuleObj.m_DataOut.DefineIntQueue();
                            break;
                        case DataQueueType.IntArr:
                            break;
                        case DataQueueType.String:
                            break;
                        case DataQueueType.StringArr:
                            break;
                        default:
                            break;
                    }
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
