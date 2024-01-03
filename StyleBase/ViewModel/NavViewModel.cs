using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using Common;
using CommunaCation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.InteropServices;

namespace StyleBase
{
    /// <summary>
    /// 导航栏
    /// </summary>
    public class NavViewModel : NoitifyBase
    {
        //设置窗体
        public CommandBase SetFrmCom { get; set; }

        //解决方案
        public CommandBase FrmSolution { get; set; }

        //运行一次
        public CommandBase RunOnceCom { get; set; }

        //启动连续运行
        public CommandBase RunStartCycleCom { get; set; }

        //停止连续运行
        public CommandBase RunStopCycleCom { get; set; }

        //新建项目
        public CommandBase CreateProjectCom { get; set; }

        //相机编辑
        public CommandBase CmeraCom { get; set; }

        //变量设定
        public CommandBase VarSetCom { get; set; }

        //保存项目
        public CommandBase SaveProject { get; set; }

        //另存项目
        public CommandBase SaveAsProject { get; set; }

        //读取项目
        public CommandBase ReadProject { get; set; }

        //软件权限
        public CommandBase PermProject { get; set; }

        //急速模式
        public CommandBase QuickProject { get; set; }

        //布局窗体
        public CommandBase ShowLayoutFrm { get; set; }

        //通讯配置
        public CommandBase CommunaCation { get; set; }


        #region 控件的Enabel

        /// <summary>
        /// 控件的enable
        /// </summary>
        private bool _controlIsEnabled = true;

        public bool ControlIsEnabled
        {
            get { return _controlIsEnabled; }
            set { _controlIsEnabled = value; this.DoNitify(); }
        }

        #endregion

        #region 控件的Enabel

        /// <summary>
        /// 控件的enable
        /// </summary>
        private bool _stopControIsEnabled = false;

        public bool StopControIsEnabled
        {
            get { return _stopControIsEnabled; }
            set { _stopControIsEnabled = value; this.DoNitify(); }
        }

        #endregion

        #region 急速状态的背景色切换

        /// <summary>
        /// 急速状态的背景色切换
        /// </summary>
        private object _runModelBackGround;

        public object RunModelBackGround
        {
            get
            {
                if (SysProcessPro.RushMode)
                {
                    _runModelBackGround = "green";
                }
                else
                {
                    _runModelBackGround = "#00CC99";
                }
                return _runModelBackGround;
            }
            set
            {
                _runModelBackGround = value;
                this.DoNitify();
            }
        }

        #endregion

        /// <summary>
        /// 界面显示的状态
        /// </summary>
        public List<SysHelper.DeviceInfo> CommunaInfo = new List<SysHelper.DeviceInfo>();

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// 构造函数
        /// </summary>
        public NavViewModel()
        {

            //事件来自Project
            SysHelper.DataEventChange.ProjectChangedEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    if (e)
                    {
                        ControlIsEnabled = true;//所有使能关闭
                        StopControIsEnabled = false;//循环运行停止使能打开
                    }
                    else
                    {
                        ControlIsEnabled = false;//所有使能关闭
                        StopControIsEnabled = true;//循环运行停止使能打开
                    }
                }
                else
                {
                    if (e)
                    {
                        ControlIsEnabled = true;//所有使能关闭
                        StopControIsEnabled = false;//循环运行停止使能打开
                    }
                    else
                    {
                        ControlIsEnabled = false;//所有使能关闭
                        StopControIsEnabled = true;//循环运行停止使能打开
                    }
                }
            };

            //事件来自Control
            SysHelper.DataEventChange.PrecessChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    if (e)
                    {
                        ControlIsEnabled = true;//所有使能关闭
                        StopControIsEnabled = false;//循环运行停止使能打开
                    }
                    else
                    {
                        ControlIsEnabled = false;//所有使能关闭
                        StopControIsEnabled = true;//循环运行停止使能打开
                    }
                }
                else
                {
                    if (e)
                    {
                        ControlIsEnabled = true;//所有使能关闭
                        StopControIsEnabled = false;//循环运行停止使能打开
                    }
                    else
                    {
                        ControlIsEnabled = false;//所有使能关闭
                        StopControIsEnabled = true;//循环运行停止使能打开
                    }
                }
            };

            this.SetFrmCom = new CommandBase();
            this.SetFrmCom.DoExecute = new Action<object>(SetFrmPram);//设置窗体
            this.SetFrmCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.FrmSolution = new CommandBase();
            this.FrmSolution.DoExecute = new Action<object>(FrmSolutioCom);//解决方案列表
            this.FrmSolution.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.RunOnceCom = new CommandBase();
            this.RunOnceCom.DoExecute = new Action<object>(Start_Run_Once);//所有项目运行一次
            this.RunOnceCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.RunStartCycleCom = new CommandBase();
            this.RunStartCycleCom.DoExecute = new Action<object>(Start_Run_Cycle);//所有项目循环运行启动
            this.RunStartCycleCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.RunStopCycleCom = new CommandBase();
            this.RunStopCycleCom.DoExecute = new Action<object>(Stop_Run_Cycle);//所有项目循环运行停止
            this.RunStopCycleCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CreateProjectCom = new CommandBase();
            this.CreateProjectCom.DoExecute = new Action<object>(ShowCreateProject);
            this.CreateProjectCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CmeraCom = new CommandBase();
            this.CmeraCom.DoExecute = new Action<object>(ShowCmeraCom);
            this.CmeraCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.VarSetCom = new CommandBase();
            this.VarSetCom.DoExecute = new Action<object>(ShowVarSetCom);
            this.VarSetCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.ReadProject = new CommandBase();
            this.ReadProject.DoExecute = new Action<object>(ReadSolution);
            this.ReadProject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SaveProject = new CommandBase();
            this.SaveProject.DoExecute = new Action<object>(SaveSolution);
            this.SaveProject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SaveAsProject = new CommandBase();
            this.SaveAsProject.DoExecute = new Action<object>(SaveAsSolution);
            this.SaveAsProject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.QuickProject = new CommandBase();
            this.QuickProject.DoExecute = new Action<object>(RunModelProject);
            this.QuickProject.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.ShowLayoutFrm = new CommandBase();
            this.ShowLayoutFrm.DoExecute = new Action<object>(ShowLayout);
            this.ShowLayoutFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CommunaCation = new CommandBase();
            this.CommunaCation.DoExecute = new Action<object>(ShowCommunaCation);
            this.CommunaCation.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });


        }

        #region 设置窗体

        /// <summary>
        /// 设置窗体
        /// </summary>
        /// <param name="obj"></param>
        private void SetFrmPram(object obj)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "设置窗体" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmSysSet load = new FrmSysSet();
            load.ShowDialog();
        }

        #endregion

        #region 解决方案列表

        /// <summary>
        /// 解决方案列表
        /// </summary>
        /// <param name="obj"></param>
        private void FrmSolutioCom(object obj)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "解决方案列表" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmSolutionList solutionList = new FrmSolutionList();
            solutionList.ShowDialog();
        }

        #endregion

        #region 所有项目运行一次

        /// <summary>
        /// 所有项目运行一次
        /// </summary>
        /// <param name="obj"></param>
        private void Start_Run_Once(object obj)
        {
            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "所有项目运行一次" };
            notify.ShowInTaskbar = false;
            notify.Show();

            //判断是否有主动执行
            bool ExecttionStatus = false;

            foreach (var item in SysProcessPro.g_ProjectList)
            {
                if (item.ProjectInfo.m_Execution == Execution.主动执行)
                {
                    ExecttionStatus = true;
                }
            }

            //有主动执行流程
            if (ExecttionStatus)
            {
                ControlIsEnabled = false;//使能关闭
                StopControIsEnabled = true;//循环运行停止使能打开

                SysProcessPro.g_SysStatus.m_RunMode = RunMode.执行一次;//解决方案运行方式
                SysProcessPro.Sys_Run_Once();//解决方案所有项目执行一次

                SysHelper.DataEventChange.NvaControlEnabel = ControlIsEnabled;////将使能关闭
            }

        }

        #endregion

        #region 解决方案所有项目循环运行启动

        /// <summary>
        /// 解决方案所有项目循环运行启动
        /// </summary>
        /// <param name="obj"></param>
        private void Start_Run_Cycle(object obj)
        {

            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "所有项目循环运行启动" };
            notify.ShowInTaskbar = false;
            notify.Show();

            //判断是否有主动执行
            bool ExecttionStatus = false;

            foreach (var item in SysProcessPro.g_ProjectList)
            {
                if (item.ProjectInfo.m_Execution == Execution.主动执行)
                {
                    ExecttionStatus = true;
                }
            }

            //有主动执行流程
            if (ExecttionStatus)
            {
                ControlIsEnabled = false;//所有使能关闭
                StopControIsEnabled = true;//循环运行停止使能打开

                SysProcessPro.g_SysStatus.m_RunMode = RunMode.循环运行;//解决方案运行方式
                SysHelper.DataEventChange.NvaControlEnabel = ControlIsEnabled;//将使能关闭

                SysProcessPro.Sys_Run_Cycle();
            }

        }

        #endregion

        #region 解决方案所有项目循环运行停止

        /// <summary>
        /// 解决方案所有项目循环运行停止
        /// </summary>
        /// <param name="obj"></param>
        private void Stop_Run_Cycle(object obj)
        {

            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            if (SysProcessPro.g_SysStatus.m_RunMode == RunMode.循环运行)
            {
                //判断是否有停止时执行
                foreach (var item in SysProcessPro.g_ProjectList)
                {
                    if (item.ProjectInfo.m_Execution == Execution.停止时执行)
                    {
                        if (item.m_ThreadStatus)
                        {
                            return;
                        }
                    }
                }

                //判断是否有停止时执行
                foreach (var item in SysProcessPro.g_ProjectList)
                {
                    if (item.ProjectInfo.m_Execution == Execution.停止时执行)
                    {
                        if (!item.m_ThreadStatus)
                        {
                            item.run = RunMode.执行一次;
                            item.Thread_Start();
                        }
                    }
                }
            }

            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "所有项目循环运行停止" };
            notify.ShowInTaskbar = false;
            notify.Show();

            //停止系统运行
            SysProcessPro.Sys_Stop_Run();
        }

        //显示创建项目窗体
        private void ShowCreateProject(object o)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "创建项目" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmCreateProject frmCreate = new FrmCreateProject();
            frmCreate.ShowDialog();
        }

        #endregion

        #region 显示相机列表窗体

        /// <summary>
        /// 显示相机列表窗体
        /// </summary>
        /// <param name="o"></param>
        private void ShowCmeraCom(object o)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "显示相机列表" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmCamera frmCamera = new FrmCamera();
            frmCamera.ShowDialog();
        }

        #endregion

        #region 显示变量设备窗体

        /// <summary>
        /// 显示变量设备窗体
        /// </summary>
        /// <param name="o"></param>
        private void ShowVarSetCom(object o)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "变量设备窗体" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmDataSet frmDataSet = new FrmDataSet();
            frmDataSet.ShowDialog();
        }

        #endregion

        #region 读取项目解决方案

        /// <summary>
        /// 读取项目解决方案
        /// </summary>
        /// <param name="obj"></param>
        private void ReadSolution(object obj)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "工程文件|*.nv";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //加载窗体
                LoadWindow dlg = new LoadWindow(new LoadSolution(openFileDialog));
                dlg.ShowInTaskbar = false;
                dlg.ShowDialog();

                //消息提示窗体
                NotifyBox notify = new NotifyBox() { NotifyMessage = SysProcessPro.SolutionPath };
                notify.ShowInTaskbar = false;
                notify.Show();

                System.GC.Collect();//主动回收下系统未使用的资源
            }
        }

        #endregion

        #region 保存项目解决方案

        /// <summary>
        /// 保存项目解决方案
        /// </summary>
        /// <param name="obj"></param>
        private void SaveSolution(object obj)
        {
            try
            {
                if (SysProcessPro.g_ProjectList != null && SysProcessPro.g_ProjectList.Count != 0)
                {
                    string str = SysProcessPro.SolutionPath;

                    //加载窗体
                    LoadWindow dlg = new LoadWindow(new SaveSolution(str));
                    dlg.ShowInTaskbar = false;
                    dlg.ShowDialog();

                    //消息提示窗体
                    NotifyBox notify = new NotifyBox() { NotifyMessage = SysProcessPro.SolutionPath };
                    notify.ShowInTaskbar = false;
                    notify.Show();

                    Log.Info(SysProcessPro.SolutionName + ".nv" + "项目保存完成");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region 另存项目解决方案

        /// <summary>
        /// 另存项目解决方案
        /// </summary>
        /// <param name="obj"></param>
        private void SaveAsSolution(object obj)
        {
            string files;

            System.Windows.Forms.SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog();
            saveFile.Filter = "工程文件|*.nv";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                files = saveFile.FileName;

                //加载窗体
                LoadWindow dlg = new LoadWindow(new SaveSolution(files));
                dlg.ShowInTaskbar = false;
                dlg.ShowDialog();

                //消息提示窗体
                NotifyBox notify = new NotifyBox() { NotifyMessage = "另存项目解决方案,完成！" };
                notify.ShowInTaskbar = false;
                notify.Show();

                Log.Info(SysProcessPro.SolutionName + "保存完成。");

            }
        }

        #endregion

        #region 布局窗体

        /// <summary>
        /// 布局窗体
        /// </summary>
        /// <param name="obj"></param>
        private void ShowLayout(object obj)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "布局窗体" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmLayout layout = new FrmLayout();
            layout.ShowDialog();
        }

        #endregion

        #region 显示变量配置列表

        /// <summary>
        /// 显示变量配置列表
        /// </summary>
        private void ShowCommunaCation(object obj)
        {
            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "变量配置列表" };
            notify.ShowInTaskbar = false;
            notify.Show();

            FrmCommunaCation frmCommuna = new FrmCommunaCation();
            frmCommuna.ShowDialog();
        }

        #endregion

        #region 急速模式

        /// <summary>
        /// 急速模式
        /// </summary>
        /// <param name="obj"></param>
        public void RunModelProject(object obj)
        {
            SysProcessPro.RushMode = !SysProcessPro.RushMode;
            RunModelBackGround = SysProcessPro.RushMode ? "green" : "#00CC99";

            string str = SysProcessPro.RushMode ? "开启" : "关闭";

            SysHelper.DataEventChange.QuickProject = str;

            //消息提示窗体
            NotifyBox notify = new NotifyBox() { NotifyMessage = "急速模式" + str };
            notify.ShowInTaskbar = false;
            notify.Show();
        }

        #endregion

    }

    public class LoadSolution : LongTimeTask
    {
        private Thread m_threadWorking;
        private LoadWindow m_dlgWaiting;

        private System.Windows.Forms.OpenFileDialog openFileDialog;

        private CommonBase common = new CommonBase();//根据值显示一个流程
        public void Start(LoadWindow load)
        {
            m_dlgWaiting = load;
            m_threadWorking = new Thread(Working);
            m_threadWorking.Start();
        }

        public LoadSolution(System.Windows.Forms.OpenFileDialog opf)
        {
            openFileDialog = opf;
        }

        private void Working()
        {
            //获取选中文件文件夹名称
            string FolderName = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

            //程序名称
            string NvName = System.IO.Path.GetFullPath(openFileDialog.FileName);

            //程序初始化
            SysProcessPro.InitialVisionPram(NvName);

            if (SysProcessPro.g_ProjectList == null)
            {
                SysProcessPro.g_ProjectList = new List<Project>();
            }

            if (SysProcessPro.g_ProjectList.Count < 1)
            {
                SysProcessPro.g_ProjectList.Add(new Project());
            }

            SysProcessPro.Cur_Project = SysProcessPro.g_ProjectList[0];

            SysProcessPro.SolutionPath = NvName.ToLowerInvariant();

            SysProcessPro.SysFliePath = FolderName;

            string filename = System.IO.Path.GetFileNameWithoutExtension(NvName);

            SysProcessPro.SolutionName = filename;

            SysHelper.DataEventChange.Footdata = SysProcessPro.SolutionPath;//更新底部窗体显示

            SysHelper.DataEventChange.FrmNum = SysLayout.LayoutFrmNum;//窗体布局的更新

            SysHelper.DataEventChange.ProjectData = true;//更新流程窗体显示

            common.RefreshToolList(SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName);//委托刷新Process，UI界面

            SysHelper.DataEventChange.DeviceFrmStatus = true;//更新通讯界面

            m_dlgWaiting.TaskEnd(null);
        }

    }

    public class SaveSolution : LongTimeTask
    {
        private Thread m_threadWorking;
        private LoadWindow m_dlgWaiting;
        private string files;
        public void Start(LoadWindow load)
        {
            m_dlgWaiting = load;
            m_threadWorking = new Thread(Working);
            m_threadWorking.Start();
        }

        public SaveSolution(string str)
        {
            files = str;
        }

        private void Working()
        {
            if (files.Length != 0)
            {
                SysProcessPro.SaveConfig(files);
                System.GC.Collect();//主动回收下系统未使用的资源
            }
            m_dlgWaiting.TaskEnd(null);
        }

    }

}
