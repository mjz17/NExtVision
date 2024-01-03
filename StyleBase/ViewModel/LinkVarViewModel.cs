using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using ModuleDataVar;
using Common;

namespace StyleBase
{
    /// <summary>
    /// 控件使用
    /// </summary>
    public class LinkVarViewModel : NoitifyBase
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
        /// 双击流程
        /// </summary>
        public CommandBase DoubleDgvCommand { get; set; }

        /// <summary>
        /// 选中Dgv的标志位
        /// </summary>
        private bool SelectDgvStstus = false;

        /// <summary>
        /// 当前选定流程名称
        /// </summary>
        private string _selectModuelName;

        /// <summary>
        /// 数据
        /// </summary>
        private DataVar data_var;

        public string SelectModuleName
        {
            get { return _selectModuelName; }
            set { _selectModuelName = value; this.DoNitify(); }
        }

        public delegate void SendMessage(DataVar info);
        public SendMessage sendMessage;

        public LinkVarViewModel()
        {
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

            this.DoubleDgvCommand = new CommandBase();
            this.DoubleDgvCommand.DoExecute = new Action<object>(DoubleSelectDgvVar);
            this.DoubleDgvCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 初始化模块列表
        /// </summary>
        private void InitListBox()
        {
            foreach (ModuleObjBase item in SysProcessPro.Cur_Project.m_ModuleObjList)
            {
                MoudelName.Add(new ModuleToolNode()
                {
                    Name = item.ModuleParam.ModuleName,
                    IconImage = ModuleProject.GetImageByName(item.ModuleParam.ModuleName.Substring(0, item.ModuleParam.ModuleName.Length - 1))
                });
            }
        }

        /// <summary>
        /// 确定链接
        /// </summary>
        /// <param name="obj"></param>
        private void Cnfrim(object obj)
        {
            if (SelectModuleName.Length < 0)
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

            ModuleObjBase moduleObj = SysProcessPro.Cur_Project.m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == SelectModuleName);

            RefreshDgv(moduleObj.ModuleParam.ModuleID);
        }

        /// <summary>
        /// 根据模块名称显示可使用变量
        /// </summary>
        /// <param name="ModuelID">模块ID</param>
        private void RefreshDgv(int ModuelID)
        {
            List<DataVar> SysDataVar = SysProcessPro.Cur_Project.m_Var_List.FindAll(c => c.m_DataModuleID == ModuelID);

            LinkDataVar.Clear();

            foreach (DataVar item in SysDataVar)
            {
                LinkDataVar.Add(item);
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

        /// <summary>
        /// 双击选中dgv窗体
        /// </summary>
        /// <param name="obj"></param>
        private void DoubleSelectDgvVar(object obj)
        {


            //SelectDgvStstus = false;

            //System.Collections.IList items = (System.Collections.IList)obj;

            //var collection = items.Cast<DataVar>();

            //var selectedItems = collection.ToList();

            //if (selectedItems.Count == 0)
            //    return;

            //data_var = (DataVar)selectedItems[0];//选中行转为数据

            //SelectDgvStstus = true;

            //if (SelectModuleName.Length < 0)
            //{
            //    System.Windows.Forms.MessageBox.Show("未选中模块！");
            //    return;
            //}

            //if (data_var.m_DataName == null)
            //{
            //    System.Windows.Forms.MessageBox.Show("未选中模块对应变量！");
            //    return;
            //}

            //sendMessage(data_var);

            //(obj as System.Windows.Window).DialogResult = true;

            //(obj as System.Windows.Window).Close();
        }

    }
}
