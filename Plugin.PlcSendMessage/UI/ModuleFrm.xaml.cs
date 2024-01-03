using Common;
using CommunaCationPLC;
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

namespace Plugin.PlcSendMessage
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        List<EcomData> ReceData = new List<EcomData>();//列表对应的数据

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

                InitProcess();//流程

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

        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        public override void theSecondTime()
        {
            base.theSecondTime();
            cmb_Port.Text = frm_ModuleObj.ProcessName;//PLC通讯链接
            Tip = frm_ModuleObj.Tip;//备注
            StartAddress = frm_ModuleObj.startAddress;//寄存器读取起始位置
            ReceData = frm_ModuleObj.ReceData;
            this.dgv_PlcData.ItemsSource = ReceData;
            InitCmb(cmb_Port.Text);//初始化
        }

        #region 初始化

        /// <summary>
        /// 通讯端口
        /// </summary>
        private void InitProcess()
        {
            List<string> info = new List<string>();

            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                int Num = item.m_ModuleObjList.FindIndex(c => c.ModuleParam.PluginName.Contains("PLC通讯"));

                if (Num > -1)
                {
                    info.Add(item.ProjectInfo.m_ProjectName + "." + item.m_ModuleObjList[Num].ModuleParam.PluginName);
                }
            }

            cmb_Port.ItemsSource = info;
        }

        #endregion

        #region 寄存器读取起始位置

        public int StartAddress
        {
            get { return (int)this.GetValue(StartAddressProperty); }
            set { this.SetValue(StartAddressProperty, value); }
        }

        public static readonly DependencyProperty StartAddressProperty =
            DependencyProperty.Register("StartAddress", typeof(int), typeof(ModuleFrm), new PropertyMetadata(default(int)));

        #endregion

        #region 备注

        public string Tip
        {
            get { return (string)this.GetValue(TipProperty); }
            set { this.SetValue(TipProperty, value); }
        }

        public static readonly DependencyProperty TipProperty =
            DependencyProperty.Register("Tip", typeof(string), typeof(ModuleFrm), new PropertyMetadata(default(string)));

        #endregion

        private void cmb_Port_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                InitCmb(cmb_Port.Text);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        void InitCmb(string process)
        {
            try
            {
                string[] Info = process.Split('.');
                int ProcessNum = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Info[0]);
                if (ProcessNum > -1)
                {
                    frm_ModuleObj.m_Communa = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList.Find(c => c.ModuleParam.PluginName.Contains(Info[1])).m_Communa;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        /// <summary>
        /// 添加bool类型变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Addbool_Click(object sender, RoutedEventArgs e)
        {

            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("全局变量");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (m_DataVar.m_DataValue != null && conStatus == true)
            {
                if (ReceData.Count > 0)
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "bool",
                        VarType = CommunaCationPLC.VarType.Bit,
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0,
                    });
                }
                else
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "bool",
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0
                    });
                }

                RefreshList();
            }

        }

        /// <summary>
        /// 添加整数类型变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddInt_Click(object sender, RoutedEventArgs e)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("全局变量");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (m_DataVar.m_DataValue != null && conStatus == true)
            {
                if (ReceData.Count > 0)
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "Int",
                        VarType = CommunaCationPLC.VarType.Int16,
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0,
                    });
                }
                else
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "Int",
                        VarType = CommunaCationPLC.VarType.Int16,
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0
                    });
                }

                RefreshList();
            }
        }

        /// <summary>
        /// 添加Double类型变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddDouble_Click(object sender, RoutedEventArgs e)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("全局变量");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (m_DataVar.m_DataValue != null && conStatus == true)
            {
                if (ReceData.Count > 0)
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "Double",
                        VarType = CommunaCationPLC.VarType.Double,
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0,
                    });
                }
                else
                {
                    ReceData.Add(new EcomData()
                    {
                        DataID = 1,
                        DataAtrr = m_DataVar.m_DataAtrr.ToString(),
                        LinkDataModuleID = m_DataVar.m_DataModuleID,
                        DataType = "Double",
                        VarType = CommunaCationPLC.VarType.Double,
                        DataName = m_DataVar.m_DataName,
                        DataObj = 0
                    });
                }

                RefreshList();
            }
        }

        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DeleteValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dgv_PlcData.SelectedIndex;
                if (index > -1)
                {
                    ReceData.RemoveAt(index);
                    RefreshList();
                    index = index - 1;
                    dgv_PlcData.SelectedIndex = index;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

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
            frm_ModuleObj.ProcessName = cmb_Port.Text;//PLC通讯链接
            frm_ModuleObj.Tip = Tip;//备注

            frm_ModuleObj.startAddress = Convert.ToInt32(StartAddress);//寄存器读取起始位置
            frm_ModuleObj.ReceData = ReceData;

            //PLC通讯方式
            if (frm_ModuleObj.ProcessName.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("未选择PLC通讯方式！");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 刷新DatagridView
        /// </summary>
        public void RefreshList()
        {
            int num = 0;
            for (int i = 0; i < ReceData.Count; i++)
            {
                //链接PLC通讯
                if (frm_ModuleObj.m_Communa != null)
                {
                    if (i == 0)
                    {
                        ReceData[i].DataID = Convert.ToInt32(StartAddress);//起始地址
                        num = Convert.ToInt32(StartAddress);//起始地址
                        if (ReceData[i].DataType.Contains("Int"))
                        {
                            switch (frm_ModuleObj.m_Communa.Int_Type)
                            {
                                case VarType.Int16:
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int16;
                                    break;
                                case VarType.Int32:
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int32;
                                    break;
                                case VarType.Int64:
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int64;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (ReceData[i].DataType.Contains("Double"))
                        {
                            switch (frm_ModuleObj.m_Communa.Float_Type)
                            {
                                case VarType.Float:
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Float;
                                    break;
                                case VarType.Double:
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Double;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (ReceData[i].DataType.Contains("bool"))
                        {
                            ReceData[i].DataID = num + 1;
                            num = num + 1;
                        }
                        else if (ReceData[i].DataType.Contains("Int"))
                        {
                            int Int_value = 0;
                            switch (frm_ModuleObj.m_Communa.Int_Type)
                            {
                                case VarType.Int16:
                                    Int_value = 1;
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int16;
                                    break;
                                case VarType.Int32:
                                    Int_value = 2;
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int32;
                                    break;
                                case VarType.Int64:
                                    Int_value = 4;
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Int64;
                                    break;
                                default:
                                    break;
                            }
                            ReceData[i].DataID = num + Int_value;
                            num = num + Int_value;
                        }
                        else if (ReceData[i].DataType.Contains("Double"))
                        {
                            int Double_value = 0;
                            switch (frm_ModuleObj.m_Communa.Float_Type)
                            {
                                case VarType.Float:
                                    Double_value = 2;
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Float;
                                    break;
                                case VarType.Double:
                                    Double_value = 4;
                                    ReceData[i].VarType = CommunaCationPLC.VarType.Double;
                                    break;
                                default:
                                    break;
                            }
                            ReceData[i].DataID = num + Double_value;
                            num = num + Double_value;
                        }
                    }
                }
                //未链接PLC通讯
                else
                {
                    ReceData[i].DataID = Convert.ToInt32(StartAddress);//起始地址
                }
            }
            dgv_PlcData.ItemsSource = null;
            dgv_PlcData.ItemsSource = ReceData;
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
