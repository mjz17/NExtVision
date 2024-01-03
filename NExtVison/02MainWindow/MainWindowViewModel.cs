using ClassLibBase;
using HalconDotNet;
using StyleBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using VisionCore;

namespace NExtVison
{
    public class MainWindowViewModel : NoitifyBase
    {
        public static Mutex mutex;

        /// <summary>
        /// 显示的窗体
        /// </summary>
        private Grid _dispHindow;
        public Grid DispHindow
        {
            get { return _dispHindow; }
            set { _dispHindow = value; this.DoNitify(); }
        }

        public CommandBase MinmizeCommand { get; set; }
        public CommandBase MaxmizeCommand { get; set; }
        public CommandBase CloseCommand { get; set; }

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        public MainWindowViewModel(Grid grid)
        {
            DispHindow = grid;

            //布局窗体，变更事件
            SysHelper.DataEventChange.LayFrmChangeHandler += (e) =>
            {
                if (!m_Dispatcher.CheckAccess())
                {
                    m_Dispatcher.Invoke(new Action(() =>
                    {
                        CreateLayOutFrm(e);
                    }));
                }
                else
                {
                    CreateLayOutFrm(e);
                }
            };

            this.MinmizeCommand = new CommandBase();
            this.MinmizeCommand.DoExecute = new Action<object>(MinmizeWindow);
            this.MinmizeCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.MaxmizeCommand = new CommandBase();
            this.MaxmizeCommand.DoExecute = new Action<object>(MaxmizeWindow);
            this.MaxmizeCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CloseCommand = new CommandBase();
            this.CloseCommand.DoExecute = new Action<object>(CloseWindow);
            this.CloseCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            //加载配置文件
            InitConfig();
        }

        #region 窗体最小化事件

        /// <summary>
        /// 窗体最小化事件
        /// </summary>
        /// <param name="obj"></param>
        private void MinmizeWindow(object obj)
        {
            SystemCommands.MinimizeWindow(obj as Window);
        }

        #endregion

        #region 窗体最大化事件

        /// <summary>
        /// 窗体最大化事件
        /// </summary>
        /// <param name="obj"></param>
        private void MaxmizeWindow(object obj)
        {
            //((Window)obj).WindowState = WindowState.Maximized == ((Window)obj).WindowState ? WindowState.Normal : WindowState.Maximized;

            Window mainWindow = obj as Window;

            if (mainWindow.WindowState == WindowState.Maximized)
            {
                mainWindow.WindowState = WindowState.Normal;
            }
            else if (mainWindow.WindowState == WindowState.Normal)
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
        }

        #endregion

        #region 关闭窗体事件

        /// <summary>
        /// 关闭窗体事件
        /// </summary>
        /// <param name="obj"></param>
        private void CloseWindow(object obj)
        {
            StyleBase.FrmClosed frm = new StyleBase.FrmClosed();
            frm.ShowDialog();
            MessageBoxResult boxResult = frm.result;

            if (boxResult == MessageBoxResult.Yes)
            {
                //加载窗体
                LoadWindow dlg = new LoadWindow(new SaveProjectInfo());
                dlg.ShowInTaskbar = false;
                dlg.ShowDialog();

                SystemCommands.CloseWindow(obj as Window);
                Process.GetCurrentProcess().Kill();
            }
            else if (boxResult == MessageBoxResult.No)
            {
                //加载窗体
                LoadWindow dlg = new LoadWindow(new DisposejectInfo());
                dlg.ShowInTaskbar = false;
                dlg.ShowDialog();

                SystemCommands.CloseWindow(obj as Window);
                Process.GetCurrentProcess().Kill();
            }
        }

        #endregion

        /// <summary>
        /// 根据窗体布局创建窗体
        /// </summary>
        private void CreateLayOutFrm(int FrmNum)
        {
            SysLayout.LayoutFrmNum = FrmNum;
            SysLayout.CreateLayoutFrm(DispHindow);
        }

        /// <summary>
        /// 初始化系统
        /// </summary>
        void InitConfig()
        {
            LoadResources load = new LoadResources();
            load.InitResourse(DispHindow);
        }

    }

    public class SaveProjectInfo : LongTimeTask
    {
        private Thread m_threadWorking;
        private LoadWindow m_dlgWaiting;

        public void Start(LoadWindow load)
        {
            m_dlgWaiting = load;
            m_threadWorking = new Thread(Working);
            m_threadWorking.Start();
        }

        private void Working()
        {
            //保存当前项目数据
            SysProcessPro.SaveConfig(SysProcessPro.ConfigPath);
            //关闭当前项目连接
            SysProcessPro.DisposeVisionProgram();

            m_dlgWaiting.TaskEnd(null);
        }

    }

    public class DisposejectInfo : LongTimeTask
    {
        private Thread m_threadWorking;
        private LoadWindow m_dlgWaiting;

        public void Start(LoadWindow load)
        {
            m_dlgWaiting = load;
            m_threadWorking = new Thread(Working);
            m_threadWorking.Start();
        }

        private void Working()
        {
            //关闭当前项目连接
            SysProcessPro.DisposeVisionProgram();
            m_dlgWaiting.TaskEnd(null);
        }

    }

}
