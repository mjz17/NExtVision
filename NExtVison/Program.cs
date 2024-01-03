using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionCore;

namespace NExtVison
{
    class Program : System.Windows.Application
    {

        public static Mutex mutex;

        /// <summary>
        /// 程序入口
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            mutex = new Mutex(true, "onlyRun");
            if (!mutex.WaitOne(0, false))
            {
                System.Windows.Forms.MessageBox.Show("该程序正在运行", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                System.Windows.Application app = new System.Windows.Application();

                //获取Halcon版本号
                //HOperatorSet.GetSystem("file_version", out HTuple w);

                //Halcon初始化
                HOperatorSet.SetSystem("clip_region", "false");

                //图像脚本初始化
                SysProcessPro.s_HDevEngine.IsInitialized();

                //设置临时目录为路径
                SysProcessPro.s_HDevEngine.SetProcedurePath(Environment.GetEnvironmentVariable("TEMP"));

                //增加预编译,在脚本里有大量的循环的时候 速度会提示,否则没什么效果  magical 2019-5-23 10:46:24
                SysProcessPro.s_HDevEngine.SetEngineAttribute("execute_procedures_jit_compiled", "true");

                //加载插件
                PluginService.InitPlugin();

                //启动窗体
                app.Run(new MainWindow());
            }
        }
    }
}
