using Common;
using CommunaCation;
using CommunaCationPLC;
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

namespace Plugin.PlcMessage
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

                InitPort();//通讯端口
                InitCommunType();//通讯类型
                InitDataFormat();//解析格式

                if (!m_ModuleObjBase.blnNewModule)
                {
                    theFirsttime();
                }
                else
                {
                    theSecondTime();
                }
                this.txt_adress.TextValueEvent += UcTxtAddandSub_TextValueEvent;
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
            Int_32 = true;
            Float_32 = true;
        }

        /// <summary>
        /// 非首次打开
        /// </summary>
        public override void theSecondTime()
        {
            base.theSecondTime();

            cmb_Port.Text = frm_ModuleObj.Communa_Port;
            cmb_CommunType.Text = frm_ModuleObj.m_CommunaType.ToString();
            cmb_DataFormat.Text = frm_ModuleObj.m_Format.ToString();

            SlaveAddress = frm_ModuleObj.SlaveAddress;

            switch (frm_ModuleObj.m_IntType)
            {
                case CommunaCationPLC.VarType.Int16:
                    Int_16 = true;
                    break;
                case CommunaCationPLC.VarType.Int32:
                    Int_32 = true;
                    break;
                case CommunaCationPLC.VarType.Int64:
                    Int_64 = true;
                    break;
                default:
                    break;
            }

            switch (frm_ModuleObj.m_FloatType)
            {
                case CommunaCationPLC.VarType.Float:
                    Float_32 = true;
                    break;
                case CommunaCationPLC.VarType.Double:
                    Double_64 = true;
                    break;
                default:
                    break;
            }
        }

        #region 初始化

        /// <summary>
        /// 通讯端口
        /// </summary>
        private void InitPort()
        {
            List<string> info = new List<string>();
            foreach (var item in EComManageer.s_ECommunacationDic)
            {
                if (item.Value.Key.Contains("串口") || item.Value.Key.Contains("客户端"))
                {
                    info.Add(item.Value.Key);
                }
            }
            cmb_Port.ItemsSource = info;
        }

        /// <summary>
        /// 通讯类型
        /// </summary>
        private void InitCommunType()
        {
            cmb_CommunType.ItemsSource = Enum.GetNames(typeof(CommunaCationPLC.CommunaType));
            cmb_CommunType.SelectedIndex = 0;
        }

        /// <summary>
        /// 解析格式
        /// </summary>
        private void InitDataFormat()
        {
            cmb_DataFormat.ItemsSource = Enum.GetNames(typeof(CommunaCationPLC.DataFormat));
            cmb_DataFormat.SelectedIndex = 0;
        }

        #endregion

        /// <summary>
        /// 通讯端口设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_Port_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.Communa_Port = cmb_Port.Text;
        }

        /// <summary>
        /// 通讯类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_CommunType_DropDownClosed(object sender, EventArgs e)
        {

            switch ((CommunaCationPLC.CommunaType)cmb_CommunType.SelectedIndex)
            {
                case CommunaCationPLC.CommunaType.ModbusRtu:
                    frm_ModuleObj.m_CommunaType = CommunaCationPLC.CommunaType.ModbusRtu;
                    break;
                case CommunaCationPLC.CommunaType.ModbusTcp:
                    frm_ModuleObj.m_CommunaType = CommunaCationPLC.CommunaType.ModbusTcp;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 解码格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_DataFormat_DropDownClosed(object sender, EventArgs e)
        {
            frm_ModuleObj.m_Format = (CommunaCationPLC.DataFormat)cmb_DataFormat.SelectedIndex;
        }

        #region 从站地址

        public int SlaveAddress
        {
            get { return (int)this.GetValue(SlaveAddressProperty); }
            set { this.SetValue(SlaveAddressProperty, value); }
        }

        public static readonly DependencyProperty SlaveAddressProperty =
            DependencyProperty.Register("SlaveAddress", typeof(int), typeof(ModuleFrm), new PropertyMetadata(default(int)));

        #endregion

        private void UcTxtAddandSub_TextValueEvent(object sender, RoutedEventArgs e)
        {
            frm_ModuleObj.SlaveAddress = (int)e.OriginalSource;
        }

        #region Int类型

        public bool Int_16
        {
            get { return (bool)this.GetValue(Int_16Property); }
            set { this.SetValue(Int_16Property, value); }
        }

        public static readonly DependencyProperty Int_16Property =
            DependencyProperty.Register("Int_16", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool Int_32
        {
            get { return (bool)this.GetValue(Int_32Property); }
            set { this.SetValue(Int_32Property, value); }
        }

        public static readonly DependencyProperty Int_32Property =
            DependencyProperty.Register("Int_32", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool Int_64
        {
            get { return (bool)this.GetValue(Int_64Property); }
            set { this.SetValue(Int_64Property, value); }
        }

        public static readonly DependencyProperty Int_64Property =
            DependencyProperty.Register("Int_64", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        private void RoiCheckFrm_RoiSelectEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();
            switch (info)
            {
                case "Int16":
                    frm_ModuleObj.m_IntType = CommunaCationPLC.VarType.Int16;
                    break;
                case "Int32":
                    frm_ModuleObj.m_IntType = CommunaCationPLC.VarType.Int32;
                    break;
                case "Int64":
                    frm_ModuleObj.m_IntType = CommunaCationPLC.VarType.Int64;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Double类型

        public bool Float_32
        {
            get { return (bool)this.GetValue(Float_32Property); }
            set { this.SetValue(Float_32Property, value); }
        }

        public static readonly DependencyProperty Float_32Property =
            DependencyProperty.Register("Float_32", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        public bool Double_64
        {
            get { return (bool)this.GetValue(Double_64Property); }
            set { this.SetValue(Double_64Property, value); }
        }

        public static readonly DependencyProperty Double_64Property =
            DependencyProperty.Register("Double_64", typeof(bool), typeof(ModuleFrm), new PropertyMetadata(default(bool)));

        private void UcRadDoubleFrm_ModelSelectEvent(object sender, RoutedEventArgs e)
        {
            string info = e.OriginalSource.ToString();
            switch (info)
            {
                case "Float(32)":
                    frm_ModuleObj.m_FloatType = CommunaCationPLC.VarType.Float;
                    break;
                case "Double(64)":
                    frm_ModuleObj.m_FloatType = CommunaCationPLC.VarType.Double;
                    break;
                default:
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 执行一次
        /// </summary>
        public override void ExModule()
        {
            try
            {
                //端口信息
                if (frm_ModuleObj.Communa_Port == null)
                {
                    System.Windows.Forms.MessageBox.Show("端口信息错误！");
                    return;
                }

                ECommunacation E_Communa = EComManageer.s_ECommunacationDic.First(c => c.Key == frm_ModuleObj.Communa_Port).Value;
                if (E_Communa == null)
                {
                    System.Windows.Forms.MessageBox.Show("端口信息错误！");
                    return;
                }

                //通讯类型
                if (frm_ModuleObj.m_CommunaType == CommunaCationPLC.CommunaType.None)
                {
                    System.Windows.Forms.MessageBox.Show("通讯类型未设置！");
                    return;
                }

                switch (frm_ModuleObj.m_CommunaType)
                {
                    case CommunaType.ModbusRtu:

                        break;
                    case CommunaType.ModbusTcp:
                        frm_ModuleObj.m_Communa = new CommunaModbusTcp(frm_ModuleObj.m_CommunaType, E_Communa);
                        break;
                    default:
                        break;
                }

                if (frm_ModuleObj.m_Communa == null)
                {
                    System.Windows.Forms.MessageBox.Show("通讯类型错误！");
                    return;
                }

                //删除本ID的变量
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                }

                frm_ModuleObj.ExeModule();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public override void SaveModuleParam()
        {
            ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
            this.txt_adress.TextValueEvent -= UcTxtAddandSub_TextValueEvent;
            this.Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        public override void CancelModuleParam()
        {
            this.txt_adress.TextValueEvent -= UcTxtAddandSub_TextValueEvent;
            this.Close();
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
