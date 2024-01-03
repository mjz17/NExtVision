using ClassLibBase;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 流程单独控制
    /// </summary>
    public class ControlProcessViewModel : NoitifyBase
    {

        /// <summary>
        /// 当前流程名称
        /// </summary>
        private string _processName;

        public string ProcessName
        {
            get { return _processName; }
            set { _processName = value; this.DoNitify(); }
        }

        //运行一次
        public CommandBase RunOnceCom { get; set; }

        //执行连续运行
        public CommandBase RunCycleCom { get; set; }

        //停止连续运行
        public CommandBase StopCycleCom { get; set; }

        #region 运行状态控件的Enabel

        /// <summary>
        /// 控件的enable
        /// </summary>
        private bool _ProcesscontrolIsEnabled = true;

        public bool ProcessControlIsEnabled
        {
            get { return _ProcesscontrolIsEnabled; }
            set { _ProcesscontrolIsEnabled = value; this.DoNitify(); }
        }

        #endregion

        #region 停止状态控件的Enabel

        /// <summary>
        /// 控件的enable
        /// </summary>
        private bool _stopIsEnabled = false;

        public bool StopIsEnabled
        {
            get { return _stopIsEnabled; }
            set { _stopIsEnabled = value; this.DoNitify(); }
        }

        #endregion

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        public ControlProcessViewModel()
        {
            //来自Nva窗体，点击运行一次
            //来自Nva窗体，点击循环运行
            //只控制控件使能开启
            SysHelper.DataEventChange.NvaControlEnabelChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    ProcessControlIsEnabled = e ? true : false;
                    StopIsEnabled = e ? false : true;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ProcessControlIsEnabled = e ? true : false;
                        StopIsEnabled = e ? false : true;
                    }));
                }
            };

            //模块来显示启动按钮是否运行中
            SysHelper.DataEventChange.ProjectChangedEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    ProcessControlIsEnabled = e ? true : false;
                    StopIsEnabled = e ? false : true;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {

                        ProcessControlIsEnabled = e ? true : false;
                        StopIsEnabled = e ? false : true;
                    }));
                }
            };

            //模块来显示启动按钮是否运行中
            SysHelper.DataEventChange.PrecessChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    ProcessControlIsEnabled = e ? true : false;
                    StopIsEnabled = e ? false : true;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {

                        ProcessControlIsEnabled = e ? true : false;
                        StopIsEnabled = e ? false : true;
                    }));
                }
            };

            //模块来显示启动按钮是否运行中
            SysHelper.DataEventChange.SingleProjectChangedEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    ProcessControlIsEnabled = e ? true : false;
                    StopIsEnabled = e ? false : true;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {

                        ProcessControlIsEnabled = e ? true : false;
                        StopIsEnabled = e ? false : true;
                    }));
                }
            };

            //切换流程显示
            DataChange.propertyChanged += (val) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    if (SysProcessPro.Cur_Project != null)
                    {
                        ProcessName = SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName;
                        ProcessControlIsEnabled = !SysProcessPro.Cur_Project.m_ThreadStatus;
                        StopIsEnabled = SysProcessPro.Cur_Project.m_ThreadStatus;
                    }
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (SysProcessPro.Cur_Project != null)
                        {
                            ProcessName = SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName;
                            ProcessControlIsEnabled = !SysProcessPro.Cur_Project.m_ThreadStatus;
                            StopIsEnabled = SysProcessPro.Cur_Project.m_ThreadStatus;
                        }
                    }));
                }
            };

            this.RunOnceCom = new CommandBase();
            this.RunOnceCom.DoExecute = new Action<object>(Run_Once);//运行一次
            this.RunOnceCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.RunCycleCom = new CommandBase();
            this.RunCycleCom.DoExecute = new Action<object>(Run_Cycle);//循环运行
            this.RunCycleCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.StopCycleCom = new CommandBase();
            this.StopCycleCom.DoExecute = new Action<object>(Stop_Cycle);//停止运行
            this.StopCycleCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

        /// <summary>
        /// 当前流程运行一次
        /// </summary>
        /// <param name="obj"></param>
        private void Run_Once(object obj)
        {
            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            if (SysProcessPro.Cur_Project == null)
            {
                Log.Info("当前流程为空！");
                return;
            }

            if (SysProcessPro.Cur_Project.m_ModuleObjList.Count <= 0)
            {
                Log.Info("当前流程无可运行模块！");
                return;
            }

            SysProcessPro.Cur_Project.run = RunMode.执行一次;
            SysProcessPro.Cur_Project.Thread_Start();

            ProcessControlIsEnabled = false;
            StopIsEnabled = true;
            SysHelper.DataEventChange.ProcessChangeEnabel = ProcessControlIsEnabled;

        }

        /// <summary>
        /// 当前流程循环运行
        /// </summary>
        /// <param name="obj"></param>
        private void Run_Cycle(object obj)
        {
            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            if (SysProcessPro.Cur_Project == null)
            {
                Log.Info("当前流程为空！");
                return;
            }

            if (SysProcessPro.Cur_Project.m_ModuleObjList.Count <= 0)
            {
                Log.Info("当前流程无可运行模块！");
                return;
            }

            if (!SysProcessPro.Cur_Project.m_ThreadStatus)
            {
                SysProcessPro.Cur_Project.run = RunMode.循环运行;
                SysProcessPro.Cur_Project.Thread_Start();

                ProcessControlIsEnabled = false;
                StopIsEnabled = true;
                SysHelper.DataEventChange.ProcessChangeEnabel = ProcessControlIsEnabled;

            }

        }

        /// <summary>
        /// 停止当前流程
        /// </summary>
        /// <param name="obj"></param>
        private void Stop_Cycle(object obj)
        {
            if (SysProcessPro.g_ProjectList.Count == 0)
            {
                Log.Info("无可运行解决方案！");
                return;
            }

            if (SysProcessPro.Cur_Project == null)
            {
                Log.Info("当前流程为空！");
                return;
            }

            if (SysProcessPro.Cur_Project.m_ModuleObjList.Count <= 0)
            {
                Log.Info("当前流程无可运行模块！");
                return;
            }

            if (SysProcessPro.Cur_Project.m_ThreadStatus)
            {
                int index = SysProcessPro.g_ProjectList.FindIndex(c => c.CurModuleID == SysProcessPro.Cur_Project.CurModuleID);
                SysProcessPro.Sys_Stop(index);
                ProcessControlIsEnabled = true;
                StopIsEnabled = false;
                SysHelper.DataEventChange.ProcessChangeEnabel = StopIsEnabled;
            }
        }

    }
}
