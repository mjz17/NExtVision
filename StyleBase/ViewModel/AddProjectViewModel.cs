using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using VisionCore;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Reflection;

namespace StyleBase
{
    public class AddProjectViewModel : NoitifyBase
    {
        /// <summary>
        /// 当前选定流程名称
        /// </summary>
        private string _process;

        public string Process
        {
            get { return _process; }
            set { _process = value; this.DoNitify(); }
        }

        /// <summary>
        /// 当前选定流程名称
        /// </summary>
        private string _selectProName;

        public string SelectProName
        {
            get { return _selectProName; }
            set { _selectProName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 新增流程
        /// </summary>
        public CommandBase Addproject { get; set; }

        /// <summary>
        /// 删除流程
        /// </summary>
        public CommandBase Deteproject { get; set; }

        /// <summary>
        /// 选中流程
        /// </summary>
        public CommandBase Selectproject { get; set; }

        /// <summary>
        /// 鼠标右击
        /// </summary>
        public CommandBase RightButtonDownCom { get; set; }

        /// <summary>
        /// 复制
        /// </summary>
        public CommandBase CopyCom { get; set; }

        /// <summary>
        /// 粘贴
        /// </summary>
        public CommandBase PasteCom { get; set; }

        /// <summary>
        /// 重命名
        /// </summary>
        public CommandBase ReNameCom { get; set; }

        /// <summary>
        /// 删除流程
        /// </summary>
        public CommandBase DeleteCom { get; set; }

        /// <summary>
        /// 拷贝的流程
        /// </summary>
        private Project CopyPrj;

        #region 控件的Enabel

        /// <summary>
        /// 控件的enable
        /// </summary>
        private bool _addcontrolIsEnabled = true;

        public bool AddControlIsEnabled
        {
            get { return _addcontrolIsEnabled; }
            set { _addcontrolIsEnabled = value; this.DoNitify(); }
        }

        #endregion

        public ObservableCollection<ProjectModel> ProName { get; set; } = new ObservableCollection<ProjectModel>();

        private CommonBase common = new CommonBase();

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        public AddProjectViewModel()
        {
            //项目流程界面更新
            SysHelper.DataEventChange.changedProjectEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    RefreshListBox();
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RefreshListBox();
                    }));
                }
            };

            //Nva窗体，运行按钮控制添加窗体控件使能状态
            //Nva窗体，所有项目执行一次
            SysHelper.DataEventChange.NvaControlEnabelChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    AddControlIsEnabled = e;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        AddControlIsEnabled = e;
                    }));
                }
            };

            //模块来显示启动按钮是否运行中
            SysHelper.DataEventChange.ProjectChangedEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    if (e)
                    {
                        AddControlIsEnabled = e;
                    }
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (e)
                        {
                            AddControlIsEnabled = e;
                        }
                    }));
                }
            };

            //事件来自Control
            SysHelper.DataEventChange.PrecessChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    AddControlIsEnabled = e;
                }
                else
                {

                    AddControlIsEnabled = e;
                }
            };

            this.Addproject = new CommandBase();
            this.Addproject.DoExecute = new Action<object>(AddNewProject);
            this.Addproject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Deteproject = new CommandBase();
            this.Deteproject.DoExecute = new Action<object>(DelectProject);
            this.Deteproject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.Selectproject = new CommandBase();
            this.Selectproject.DoExecute = new Action<object>(SelectLstProject);
            this.Selectproject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CopyCom = new CommandBase();
            this.CopyCom.DoExecute = new Action<object>(Copy);
            this.CopyCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.PasteCom = new CommandBase();
            this.PasteCom.DoExecute = new Action<object>(Paste);
            this.PasteCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.ReNameCom = new CommandBase();
            this.ReNameCom.DoExecute = new Action<object>(ReName);
            this.ReNameCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.DeleteCom = new CommandBase();
            this.DeleteCom.DoExecute = new Action<object>(Delete);
            this.DeleteCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.RightButtonDownCom = new CommandBase();
            this.RightButtonDownCom.DoExecute = new Action<object>(RightButtonDown);
            this.RightButtonDownCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 添加新的流程
        /// </summary>
        /// <param name="obj"></param>
        private void AddNewProject(object obj)
        {
            //项目名称，项目路径地址
            if (SysProcessPro.SolutionName == string.Empty || SysProcessPro.SolutionPath == string.Empty)
            {
                System.Windows.MessageBox.Show("未创建解决方案！");
                Log.Info("未创建解决方案！");
                return;
            }

            FrmAddPro frmAddPro = new FrmAddPro();
            frmAddPro.ShowDialog();
            SysProcessPro.Cur_Project = null;
            RefreshListBox();
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="obj"></param>
        private void DelectProject(object obj)
        {
            try
            {
                System.Windows.Controls.ListBox listBox = obj as System.Windows.Controls.ListBox;
                if (obj != null)
                {
                    int selectIndex = listBox.SelectedIndex;
                    //int index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == SelectProName);
                    if (selectIndex > -1)
                    {
                        SysProcessPro.g_ProjectList.RemoveAt(selectIndex);
                        SysProcessPro.Cur_Project = null;
                        RefreshListBox(listBox);
                        listBox.SelectedIndex = selectIndex - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 选中列表
        /// </summary>
        /// <param name="obj"></param>
        private void SelectLstProject(object obj)
        {
            if (obj != null)
            {
                ProjectModel project = (ProjectModel)obj;
                common.RefreshToolList(project.m_ProjectName);
                SelectProName = project.m_ProjectName;
            }
        }

        /// <summary>
        /// 鼠标右击事件
        /// </summary>
        /// <param name="obj"></param>
        private void RightButtonDown(object obj)
        {
            if (obj != null)
            {
                SetProject set = new SetProject();
                set.ShowDialog();
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="obj"></param>
        private void Copy(Object obj)
        {
            try
            {
                System.Windows.Controls.ListBox listBox = obj as System.Windows.Controls.ListBox;
                if (listBox != null)
                {
                    int selectIndex = listBox.SelectedIndex;
                    if (selectIndex > -1)
                    {
                        CopyPrj = null;
                        CopyPrj = (Project)SysProcessPro.g_ProjectList[selectIndex].Clone();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="obj"></param>
        private void Paste(Object obj)
        {
            try
            {
                if (SelectProName != null && CopyPrj != null)
                {
                    System.Windows.Controls.ListBox listBox = obj as System.Windows.Controls.ListBox;
                    int selectIndex = listBox.SelectedIndex;
                    int index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ProName[selectIndex].m_ProjectID &&
                    c.ProjectInfo.m_ProjectName == ProName[selectIndex].m_ProjectName);
                    if (index > -1)
                    {
                        Project project = new Project();

                        //流程ID
                        CopyPrj.ProjectInfo.m_ProjectID = project.ProjectInfo.m_ProjectID;

                        //清空变量列表
                        CopyPrj.m_Var_List.Clear();

                        foreach (ModuleObjBase item in CopyPrj.m_ModuleObjList)
                        {
                            item.ModuleParam.ProjectID = CopyPrj.ProjectInfo.m_ProjectID;
                        }

                        //名称
                        CopyPrj.ProjectInfo.m_ProjectName = CopyPrj.ProjectInfo.m_ProjectName + "备份";
                        //添加数据
                        SysProcessPro.g_ProjectList.Insert(selectIndex + 1, CopyPrj);
                        RefreshListBox(listBox);
                        listBox.SelectedIndex = selectIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="obj"></param>
        private void ReName(object obj)
        {
            try
            {
                System.Windows.Controls.ListBox listBox = obj as System.Windows.Controls.ListBox;
                if (listBox != null)
                {
                    int selectIndex = listBox.SelectedIndex;
                    if (selectIndex > -1)
                    {
                        FrmReName frmReName = new FrmReName(SelectProName, ProName[selectIndex].m_ProjectID);
                        frmReName.ShowDialog();

                        SysProcessPro.Cur_Project = null;
                        RefreshListBox(listBox);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        private void Delete(Object obj)
        {
            try
            {
                System.Windows.Controls.ListBox listBox = obj as System.Windows.Controls.ListBox;
                if (obj != null)
                {
                    int selectIndex = listBox.SelectedIndex;
                    if (selectIndex > -1)
                    {
                        SysProcessPro.g_ProjectList.RemoveAt(selectIndex);
                        SysProcessPro.Cur_Project = null;
                        RefreshListBox(listBox);
                        listBox.SelectedIndex = selectIndex - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 刷新ListBox
        /// </summary>
        private void RefreshListBox()
        {
            ProName.Clear();
            for (int i = 0; i < SysProcessPro.g_ProjectList.Count; i++)
            {
                ProName.Add(new ProjectModel
                {
                    m_ID = i + ".",
                    m_ProjectName = SysProcessPro.g_ProjectList[i].ProjectInfo.m_ProjectName,
                    m_ProjectID = SysProcessPro.g_ProjectList[i].ProjectInfo.m_ProjectID,
                });
            }
        }

        /// <summary>
        /// 刷新ListBox
        /// </summary>
        private void RefreshListBox(System.Windows.Controls.ListBox listBox)
        {
            int index = 0;
            if (listBox != null)
            {
                index = listBox.SelectedIndex;
            }
            ProName.Clear();
            for (int i = 0; i < SysProcessPro.g_ProjectList.Count; i++)
            {
                ProName.Add(new ProjectModel
                {
                    m_ID = i + ".",
                    m_ProjectName = SysProcessPro.g_ProjectList[i].ProjectInfo.m_ProjectName,
                    m_ProjectID = SysProcessPro.g_ProjectList[i].ProjectInfo.m_ProjectID,
                });
            }
            if (listBox != null)
            {
                listBox.SelectedIndex = index;
            }
        }

    }

    public class ProjectModel : NoitifyBase
    {
        /// <summary>
        /// 显示ID
        /// </summary>
        private string _ID;

        public string m_ID
        {
            get { return _ID; }
            set { _ID = value; this.DoNitify(); }
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        private string _ProjectName;

        public string m_ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 项目ID
        /// </summary>
        private int _ProjectID;

        public int m_ProjectID
        {
            get { return _ProjectID; }
            set { _ProjectID = value; this.DoNitify(); }
        }

        /// <summary>
        /// 注释
        /// </summary>
        private string _ProjectTip;

        public string m_ProjectTip
        {
            get { return _ProjectTip; }
            set { _ProjectTip = value; this.DoNitify(); }
        }

    }

}
