using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleDataVar;
using System.Collections.ObjectModel;
using VisionCore;
using System.Windows.Controls;

namespace StyleBase
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class DataSetViewModel : NoitifyBase
    {

        /// <summary>
        /// 全局变量Datagridviw的ItemsSource
        /// </summary>
        public ObservableCollection<DataVarModel> Dgv_DataSet { get; set; } = new ObservableCollection<DataVarModel>();

        /// <summary>
        /// 保存更新后的值
        /// </summary>
        private bool _SysSaveUpValue;

        public bool SysSaveUpValue
        {
            get { return _SysSaveUpValue; }
            set { _SysSaveUpValue = value; this.DoNitify(); }
        }

        #region 删除选中行

        /// <summary>
        /// 删除选中行
        /// </summary>
        public CommandBase DeleteDgvCom { get; set; }

        #endregion

        #region 选中行上移

        /// <summary>
        /// 选中行上移
        /// </summary>
        public CommandBase UpDgvCom { get; set; }

        #endregion

        #region 选中行下移

        /// <summary>
        /// 选中行下移
        /// </summary>
        public CommandBase DownDgvCom { get; set; }

        #endregion

        #region 添加bool类型

        /// <summary>
        /// 添加bool类型
        /// </summary>
        public CommandBase AppentBoolTypeCom { get; set; }

        #endregion

        #region 添加Int类型

        /// <summary>
        /// 添加Int类型
        /// </summary>
        public CommandBase AppentIntTypeCom { get; set; }

        #endregion

        #region 添加Double类型

        /// <summary>
        /// 添加Int类型
        /// </summary>
        public CommandBase AppentDoubleTypeCom { get; set; }

        #endregion

        #region 添加String类型

        /// <summary>
        /// 添加String类型
        /// </summary>
        public CommandBase AppentStringTypeCom { get; set; }

        #endregion

        #region 确定按钮

        /// <summary>
        /// 确定按钮
        /// </summary>
        public CommandBase BtnConfirmeCom { get; set; }

        #endregion

        #region 取消按钮

        /// <summary>
        /// 取消按钮
        /// </summary>
        public CommandBase BtnCancelCom { get; set; }

        #endregion

        #region 保存更新后的值

        /// <summary>
        /// 保存更新后的值
        /// </summary>
        public CommandBase SaveUpValueCom { get; set; }

        #endregion

        private CommonConfig config = new CommonConfig();

        public DataSetViewModel()
        {
            InitDgv();
            UpValue();

            this.DeleteDgvCom = new CommandBase();
            this.DeleteDgvCom.DoExecute = new Action<object>(DeleteDgv);
            this.DeleteDgvCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.UpDgvCom = new CommandBase();
            this.UpDgvCom.DoExecute = new Action<object>(UpDgv);
            this.UpDgvCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.DownDgvCom = new CommandBase();
            this.DownDgvCom.DoExecute = new Action<object>(DownDgv);
            this.DownDgvCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.AppentBoolTypeCom = new CommandBase();
            this.AppentBoolTypeCom.DoExecute = new Action<object>(AppentBoolType);
            this.AppentBoolTypeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.AppentIntTypeCom = new CommandBase();
            this.AppentIntTypeCom.DoExecute = new Action<object>(AppentIntType);
            this.AppentIntTypeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.AppentDoubleTypeCom = new CommandBase();
            this.AppentDoubleTypeCom.DoExecute = new Action<object>(AppentDoubleType);
            this.AppentDoubleTypeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.AppentStringTypeCom = new CommandBase();
            this.AppentStringTypeCom.DoExecute = new Action<object>(AppentStringType);
            this.AppentStringTypeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnConfirmeCom = new CommandBase();
            this.BtnConfirmeCom.DoExecute = new Action<object>(BtnConfirme);
            this.BtnConfirmeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnCancelCom = new CommandBase();
            this.BtnCancelCom.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();
            });
            this.BtnCancelCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SaveUpValueCom = new CommandBase();
            this.SaveUpValueCom.DoExecute = new Action<object>(SaveUpValue);
            this.SaveUpValueCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        //初始化数据列表
        private void InitDgv()
        {
            foreach (DataVar item in SysProcessPro.g_VarList)
            {
                Dgv_DataSet.Add(new DataVarModel
                {
                    m_DataAtrr = item.m_DataAtrr,//变量属性
                    m_DataType = item.m_DataType,//类型
                    m_DataName = item.m_DataName,//名称
                    m_DataValue = item.m_DataValue,//变量值
                    m_DataTip = item.m_DataTip,//注释
                    m_Data_InitValue = item.m_Data_InitValue,//初始值
                });
            }
        }

        private void UpValue()
        {
            SysSaveUpValue = SysProcessPro.SaveUpValue;
        }

        //将数据添加至列表
        private void Refresh()
        {
            SysProcessPro.g_VarList.Clear();
            foreach (DataVarModel item in Dgv_DataSet)
            {
                DataVar var = new DataVar(item.m_DataAtrr, 0, item.m_DataName, item.m_DataType, item.m_DataGroup, item.m_Data_Num,
                    item.m_Data_InitValue, item.m_DataTip, item.m_DataValue);
                SysProcessPro.g_VarList.Add(var);
            }
        }

        //删除选中行
        private void DeleteDgv(object obj)
        {
            LayDatagrid lay = obj as LayDatagrid;
            if (lay != null)
            {
                int index = lay.SelectedIndex;
                if (index > -1)
                {
                    Dgv_DataSet.RemoveAt(index);
                    lay.SelectedIndex = index - 1;
                }
            }
        }

        //选中行上移
        private void UpDgv(object obj)
        {
            LayDatagrid lay = obj as LayDatagrid;

            if (lay != null)
            {
                int index = lay.SelectedIndex;
                if (index > -1)
                {
                    DataVarModel var = Dgv_DataSet[index];
                    int addIndex = Dgv_DataSet.IndexOf(var);
                    if (addIndex == 0)
                        return;
                    Dgv_DataSet.RemoveAt(index);
                    index = index - 1;
                    Dgv_DataSet.Insert(index, var);
                    lay.SelectedIndex = index;
                }
            }
        }

        //选中行下移
        private void DownDgv(object obj)
        {
            LayDatagrid lay = obj as LayDatagrid;

            if (lay != null)
            {
                int index = lay.SelectedIndex;
                if (index > -1)
                {
                    DataVarModel var = Dgv_DataSet[index];
                    int DeleteIndex = Dgv_DataSet.IndexOf(var);
                    if (DeleteIndex == Dgv_DataSet.Count - 1)
                        return;
                    Dgv_DataSet.RemoveAt(DeleteIndex);
                    DeleteIndex = DeleteIndex + 1;
                    Dgv_DataSet.Insert(DeleteIndex, var);
                    lay.SelectedIndex = DeleteIndex;
                }
            }
        }

        //添加Bool类型的数据
        private void AppentBoolType(object obj)
        {
            Dgv_DataSet.Add(new DataVarModel
            {
                m_DataAtrr = DataVarType.DataAtrribution.全局变量,
                m_DataModuleID = 0,
                m_DataName = "Value" + Dgv_DataSet.Count(),
                m_DataType = DataVarType.DataType.Bool,
                m_DataGroup = DataVarType.DataGroup.单量,
                m_Data_Num = 1,
                m_DataValue = false,
                m_Data_InitValue = "false",
                m_DataTip = "NULL",
            });
        }

        //添加Int类型的数据
        private void AppentIntType(object obj)
        {
            Dgv_DataSet.Add(new DataVarModel
            {
                m_DataAtrr = DataVarType.DataAtrribution.全局变量,
                m_DataModuleID = 0,
                m_DataName = "Value" + Dgv_DataSet.Count(),
                m_DataType = DataVarType.DataType.Int,
                m_DataGroup = DataVarType.DataGroup.单量,
                m_Data_Num = 1,
                m_DataValue = 0,
                m_Data_InitValue = "0",
                m_DataTip = "NULL",
            });
        }

        //添加Int类型的数据
        private void AppentDoubleType(object obj)
        {
            Dgv_DataSet.Add(new DataVarModel
            {
                m_DataAtrr = DataVarType.DataAtrribution.全局变量,
                m_DataModuleID = 0,
                m_DataName = "Value" + Dgv_DataSet.Count(),
                m_DataType = DataVarType.DataType.Double,
                m_DataGroup = DataVarType.DataGroup.单量,
                m_Data_Num = 1,
                m_DataValue = 0,
                m_Data_InitValue = "0",
                m_DataTip = "NULL",
            });
        }

        //添加String类型的数据
        private void AppentStringType(object obj)
        {
            Dgv_DataSet.Add(new DataVarModel
            {
                m_DataAtrr = DataVarType.DataAtrribution.全局变量,
                m_DataModuleID = 0,
                m_DataName = "Value" + Dgv_DataSet.Count(),
                m_DataType = DataVarType.DataType.String,
                m_DataGroup = DataVarType.DataGroup.单量,
                m_Data_Num = 1,
                m_DataValue = 0,
                m_Data_InitValue = "0",
                m_DataTip = "NULL",
            });
        }

        //确定按钮
        private void BtnConfirme(object obj)
        {
            //判断是否有重复变量名称
            bool isRepeat = Dgv_DataSet.GroupBy(i => i.m_DataName).Where(g => g.Count() > 1).Count() > 0;
            if (isRepeat)
            {
                System.Windows.Forms.MessageBox.Show("变量定义,名称存在重复！");
                return;
            }

            Refresh();
            (obj as System.Windows.Window).Close();
        }

        //保存更新后的值
        private void SaveUpValue(object obj)
        {
            CheckBox chk = (CheckBox)obj;
            if (chk != null)
            {
                SysProcessPro.SaveUpValue = !chk.IsChecked == true ? false : true;
                config.WriteConfig("ProjectConfig", "保存更新的值", SysSaveUpValue.ToString());
            }
        }

    }
}
