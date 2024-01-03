using ClassLibBase;
using HalconDotNet;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 给窗体使用链接
    /// </summary>
    public class LinkDataVarViewModel : NoitifyBase
    {
        /// <summary>
        /// 模块链接名称
        /// </summary>
        public ObservableCollection<ModuleToolNode> MoudelName { get; set; } = new ObservableCollection<ModuleToolNode>();

        /// <summary>
        /// 变量链接名称
        /// </summary>
        public ObservableCollection<DataVar> LinkDataVar { get; set; } = new ObservableCollection<DataVar>();

        /// <summary>
        /// 确定
        /// </summary>
        public CommandBase ConfrimFrm { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase CancelFrm { get; set; }

        /// <summary>
        /// 选中流程
        /// </summary>
        public CommandBase SelectModuelList { get; set; }

        /// <summary>
        /// 选中流程
        /// </summary>
        public CommandBase SelectDgvCommand { get; set; }

        /// <summary>
        /// 选中Dgv的标志位
        /// </summary>
        private bool SelectDgvStstus = false;

        /// <summary>
        /// 当前选定流程名称
        /// </summary>
        private string _selectModuelName;

        public string SelectModuleName
        {
            get { return _selectModuelName; }
            set { _selectModuelName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 数据
        /// </summary>
        private DataVar data_var;

        public delegate void SendMessage(DataVar info);

        public SendMessage sendMessage;

        /// <summary>
        /// 全局变量/局部变量
        /// </summary>
        public string m_setValue { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataVarType.DataType m_dataType { get; set; }

        /// <summary>
        /// 构造函数1，不监控数据类型
        /// </summary>
        public LinkDataVarViewModel()
        {
            RefreshListBox();
        }

        /// <summary>
        /// 是否需要显示全局变量类型模块
        /// </summary>
        /// <param name="setValue"></param>
        public LinkDataVarViewModel(string setValue)
        {
            m_setValue = setValue;
            RefreshListBox();
        }

        /// <summary>
        /// 是否需要显示全局变量类型模块
        /// 显示数据类型
        /// </summary>
        /// <param name="setValue"></param>
        /// <param name="DataType"></param>
        public LinkDataVarViewModel(string setValue, DataVarType.DataType DataType)
        {
            m_setValue = setValue;//变量类型
            m_dataType = DataType;//数据类型
            RefreshListBox();
        }

        private void RefreshListBox()
        {
            InitListBox();

            this.ConfrimFrm = new CommandBase();
            this.ConfrimFrm.DoExecute = new Action<object>(Cnfrim);
            this.ConfrimFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CancelFrm = new CommandBase();
            this.CancelFrm.DoExecute = new Action<object>((o) =>
            {
                SelectModuleName = string.Empty;
                sendMessage(new DataVar());
                (o as System.Windows.Window).DialogResult = false;
                (o as System.Windows.Window).Close();

            });
            this.CancelFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SelectModuelList = new CommandBase();
            this.SelectModuelList.DoExecute = new Action<object>(SelectModuleVar);
            this.SelectModuelList.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SelectDgvCommand = new CommandBase();
            this.SelectDgvCommand.DoExecute = new Action<object>(SelectDgvVar);
            this.SelectDgvCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 初始化模块列表
        /// </summary>
        private void InitListBox()
        {
            MoudelName.Clear();
            if (m_setValue != null)
            {
                if (m_setValue.Contains("全局变量"))
                {
                    MoudelName.Add(new ModuleToolNode()//将全局变量添加至ListBox
                    {
                        Name = "全局变量",
                        IconImage = ModuleProject.GetImageByName("工具箱"),
                    });

                    foreach (ModuleObjBase item in SysProcessPro.Cur_Project.m_ModuleObjList)
                    {
                        MoudelName.Add(new ModuleToolNode()
                        {
                            Name = item.ModuleParam.ModuleName,
                            IconImage = ModuleProject.GetImageByName(item.ModuleParam.ModuleName.Substring(0, item.ModuleParam.ModuleName.Length - 1))
                        });
                    }
                }
                else if (m_setValue.Contains("采集图像"))
                {
                    foreach (ModuleObjBase item in SysProcessPro.Cur_Project.m_ModuleObjList)
                    {
                        if (item.ModuleParam.ModuleName.Contains("采集图像"))
                        {
                            MoudelName.Add(new ModuleToolNode()
                            {
                                Name = item.ModuleParam.ModuleName,
                                IconImage = ModuleProject.GetImageByName(item.ModuleParam.ModuleName.Substring(0, item.ModuleParam.ModuleName.Length - 1))
                            });
                        }
                    }
                }
                else
                {
                    foreach (ModuleObjBase item in SysProcessPro.Cur_Project.m_ModuleObjList)
                    {
                        MoudelName.Add(new ModuleToolNode()
                        {
                            Name = item.ModuleParam.ModuleName,
                            IconImage = ModuleProject.GetImageByName(item.ModuleParam.PluginName)
                        });
                    }
                }
            }
            else
            {
                foreach (ModuleObjBase item in SysProcessPro.Cur_Project.m_ModuleObjList)
                {
                    MoudelName.Add(new ModuleToolNode()
                    {
                        Name = item.ModuleParam.ModuleName,
                        IconImage = ModuleProject.GetImageByName(item.ModuleParam.PluginName)
                    });
                }
            }
        }

        /// <summary>
        /// 确定链接
        /// </summary>
        /// <param name="obj"></param>
        private void Cnfrim(object obj)
        {

            if (SelectModuleName == null || SelectModuleName.Length < 0)
            {
                System.Windows.Forms.MessageBox.Show("未选中模块！");
                return;
            }

            if (data_var.m_DataName == null)
            {
                System.Windows.Forms.MessageBox.Show("未选中模块对应变量！");
                return;
            }

            sendMessage(data_var);

            (obj as System.Windows.Window).DialogResult = true;

            (obj as System.Windows.Window).Close();
        }

        /// <summary>
        /// 选中的链接
        /// </summary>
        /// <param name="obj"></param>
        private void SelectModuleVar(object obj)
        {

            System.Collections.IList items = (System.Collections.IList)obj;

            var collection = items.Cast<ModuleToolNode>();

            var selectedItems = collection.ToList();

            if (selectedItems.Count == 0)
            {
                SelectModuleName = string.Empty;
                return;
            }

            //获取模块名称
            SelectModuleName = selectedItems[0].Name;

            RefreshDgv(SelectModuleName);
        }

        /// <summary>
        /// 根据模块名称显示可使用变量
        /// </summary>
        /// <param name="ModuelName"></param>
        private void RefreshDgv(string ModuelName)
        {
            LinkDataVar.Clear();
            if (m_setValue != null)
            {
                if (m_setValue.Contains("全局变量") && ModuelName.Contains("全局变量"))
                {
                    List<DataVar> SysDataVar = SysProcessPro.g_VarList.FindAll(c => c.m_DataAtrr == DataVarType.DataAtrribution.全局变量);
                    foreach (DataVar item in SysDataVar)
                    {
                        LinkDataVar.Add(item);
                    }
                }
                else
                {
                    List<DataVar> SysDataVar = SysProcessPro.Cur_Project.m_Var_List.FindAll(c => c.m_DataAtrr == DataVarType.DataAtrribution.局部变量 && c.m_DataTip.Contains(ModuelName));
                    foreach (DataVar item in SysDataVar)
                    {
                        LinkDataVar.Add(item);
                    }
                }
            }
            else
            {
                List<DataVar> SysDataVar = SysProcessPro.Cur_Project.m_Var_List.FindAll(c => c.m_DataAtrr == DataVarType.DataAtrribution.局部变量 && c.m_DataTip.Contains(ModuelName));
                foreach (DataVar item in SysDataVar)
                {
                    LinkDataVar.Add(item);
                }
            }
        }

        /// <summary>
        /// 选中dgv窗体
        /// </summary>
        /// <param name="obj"></param>
        private void SelectDgvVar(object obj)
        {
            SelectDgvStstus = false;

            System.Collections.IList items = (System.Collections.IList)obj;

            var collection = items.Cast<DataVar>();

            var selectedItems = collection.ToList();

            if (selectedItems.Count == 0)
                return;

            data_var = (DataVar)selectedItems[0];//选中行转为数据

            SelectDgvStstus = true;
        }

    }
}
