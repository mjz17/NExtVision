using ClassLibBase;
using HalconDotNet;
using StyleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using VisionCore;

namespace NExtVison
{
    public class LoadFrmViewModel : NoitifyBase, LongTimeTask
    {
        #region 加载流程名称

        /// <summary>
        /// 加载流程名称
        /// </summary>
        private string _loadTaskName;

        public string LoadTaskName
        {
            get { return _loadTaskName; }
            set { _loadTaskName = value; this.DoNitify(); }
        }

        #endregion

        //判断程序是否重新打开
        public static Mutex mutex;
        //线程
        private Thread m_threadWorking;
        public LoadFrmViewModel()
        {
            mutex = new Mutex(true, "onlyRun");
            LoadTaskName = "加载工具：";
            if (!mutex.WaitOne(0, false))
            {
                System.Windows.Forms.MessageBox.Show("该程序正在运行", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                LoadWork();
            }

        }

        public void LoadWork()
        {
            m_threadWorking = new Thread(LoadTask);
            m_threadWorking.Start();
        }

        void LoadTask()
        {
            LoadVistion();
        }

        void LoadVistion()
        {
            //初始化Halcon
            HOperatorSet.SetSystem("clip_region", "false");
            //初始化
            SysProcessPro.s_HDevEngine.IsInitialized();
            //设置临时目录为路径
            SysProcessPro.s_HDevEngine.SetProcedurePath(Environment.GetEnvironmentVariable("TEMP"));
            //增加预编译,在脚本里有大量的循环的时候 速度会提示,否则没什么效果
            SysProcessPro.s_HDevEngine.SetEngineAttribute("execute_procedures_jit_compiled", "true");
            //加载插件
            PluginService.InitPlugin();
        }

        public void Start(LoadWindow load)
        {

        }
    }
}
