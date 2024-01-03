using ClassLibBase;
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
    public class ShowDataVarViewModel : NoitifyBase
    {
        /// <summary>
        /// 模块列表
        /// </summary>
        public ObservableCollection<ModuleToolNode> MoudelName { get; set; } = new ObservableCollection<ModuleToolNode>();

        /// <summary>
        /// 模块变量名称
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
        /// 选中模块
        /// </summary>
        public CommandBase SelectModuelList { get; set; }

        /// <summary>
        /// 选中列表
        /// </summary>
        public CommandBase SelectDgvCommand { get; set; }

        /// <summary>
        /// 双击列表
        /// </summary>
        public CommandBase DoubleDgvCommand { get; set; }

        //委托
        public delegate void SendMessage(DataVar info);
        //委托变量
        public SendMessage sendMessage;

        /// <summary>
        /// 查询数据类型
        /// </summary>
        private string _DataType;
        public string m_DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        /// <summary>
        /// 模块所属ID
        /// </summary>
        private string _ModuleID;
        public string m_ModuleID
        {
            get { return _ModuleID; }
            set { _ModuleID = value; }
        }

        /// <summary>
        /// 数据
        /// </summary>
        private DataVar data_var;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataType">数据类型</param>
        /// <param name="ModuleID">模块所属ID</param>
        public ShowDataVarViewModel(string DataType, string ModuleID)
        {
            _DataType = DataType;
            _ModuleID = ModuleID;

            InitListBox(m_ModuleID);
            Opeate();
        }

        /// <summary>
        /// 初始化流程列表
        /// </summary>
        /// <param name="ID">模块ID</param>
        void InitListBox(string ID)
        {
            MoudelName.Clear();
            //查询模块ID
            int index = SysProcessPro.Cur_Project.m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleName.Contains(ID));
            //获取位于ID之前的ModuleObjBase的数据
            List<ModuleObjBase> list = SysProcessPro.Cur_Project.m_ModuleObjList.Take(index).ToList();

            foreach (ModuleObjBase item in list)
            {
                MoudelName.Add(new ModuleToolNode()
                {
                    Name = item.ModuleParam.ModuleName,
                    IconImage = ModuleProject.GetImageByName(item.ModuleParam.ModuleName.Substring(0, item.ModuleParam.ModuleName.Length - 1))
                });
            }

        }

        /// <summary>
        /// 操作
        /// </summary>
        void Opeate()
        {
            this.ConfrimFrm = new CommandBase();
            this.ConfrimFrm.DoExecute = new Action<object>(Cnfrim);
            this.ConfrimFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CancelFrm = new CommandBase();
            this.CancelFrm.DoExecute = new Action<object>((o) =>
            {
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
        /// 确定
        /// </summary>
        /// <param name="obj"></param>
        void Cnfrim(object obj)
        {
            sendMessage(data_var);
            (obj as System.Windows.Window).DialogResult = true;
            (obj as System.Windows.Window).Close();
        }

        /// <summary>
        /// 选中模块，显示模块下的变量
        /// </summary>
        /// <param name="obj"></param>
        void SelectModuleVar(object obj)
        {
            int index = (int)obj;
            if (index > -1)
            {
                ModuleObjBase moduleObj = SysProcessPro.Cur_Project.m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == MoudelName[index].Name);
                DispMouleID(moduleObj.ModuleParam.ModuleID);
            }
        }

        /// <summary>
        /// 显示模块的变量
        /// </summary>
        /// <param name="ID"></param>
        void DispMouleID(int ID)
        {
            //根据数据类型转为Enum
            DataVarType.DataType data = (DataVarType.DataType)Enum.Parse(typeof(DataVarType.DataType), m_DataType, true);
            //根据ID查询集合
            List<DataVar> SysDataVar = SysProcessPro.Cur_Project.m_Var_List.FindAll(c => c.m_DataModuleID == ID
           && c.m_DataType == data);
            //清空集合
            LinkDataVar.Clear();
            foreach (DataVar item in SysDataVar)
            {
                LinkDataVar.Add(item);
            }
        }

        /// <summary>
        /// 选中模块所属的变量
        /// </summary>
        /// <param name="obj"></param>
        void SelectDgvVar(object obj)
        {
            int index = (int)obj;
            if (index > -1)
            {
                data_var = LinkDataVar[index];//选中行的数据
            }
        }

        /// <summary>
        /// 双击选中变量
        /// </summary>
        /// <param name="obj"></param>
        void DoubleSelectDgvVar(object obj)
        {
            if (data_var.m_DataValue != null)
            {
                sendMessage(data_var);
                (obj as System.Windows.Window).DialogResult = true;
                (obj as System.Windows.Window).Close();
            }
        }

    }
}
