using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common
{
    public class Log
    {
        public static bool s_IsStarting = false;//程序正在加载中 

        public static List<string> s_ErrMsg = new List<string>();//加载过程中是错误信息

        private static ILog log4Net = log4net.LogManager.GetLogger("logLogger");//全局日志 "logLogger"与log4net.config配置里的名称一致

        private static readonly ILog LogModify = LogManager.GetLogger("modifyLogger");//模块参数修改记录

        public static void RegisterLog()
        {
            s_IsStarting = true;
            //
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(System.Windows.Forms.Application.StartupPath + "\\log4net.config"));

            //按理说可以通过配置log4net来达到天数控制,但是这个有时候会调整天数,故自己写逻辑
            DeleteLog(10);//删除10天的日志
        }

        #region 初始化listView控件

        /// <summary>
        /// /只需要在窗口初始化的时候注册listview就可以了,意思是让listview这个控件也显示日志信息
        /// </summary>
        /// <param name="richTextBox_log"></param>
        public static void InitializeListView(ListView listView)
        {
            if (listView == null) return;

            // 设置listview打印日志
            var logPattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
            var list_logAppender = new ListViewBaseAppender();
            list_logAppender.m_listView = listView;
            list_logAppender.Layout = new PatternLayout(logPattern);

            Logger log4NetLogger = log4Net.Logger as Logger;//单独配置appender
            log4NetLogger.AddAppender(list_logAppender);

        }

        #endregion

        #region 初始化textBox控件

        /// <summary>
        /// /只需要在窗口初始化的时候注册TextBox就可以了,意思是让TextBox这个控件也显示日志信息
        /// </summary>
        /// <param name="richTextBox_log"></param>
        public static void InitializeTextBox(TextBox textBox)
        {
            if (textBox == null) return;

            // 设置listview打印日志
            var logPattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
            var list_logAppender = new TextBoxBaseAppender();
            list_logAppender.TextBox = textBox;
            list_logAppender.Layout = new PatternLayout(logPattern);

            Logger log4NetLogger = log4Net.Logger as Logger;//单独配置appender
            log4NetLogger.AddAppender(list_logAppender);

        }

        #endregion

        #region 初始化textBox控件

        /// <summary>
        /// /只需要在窗口初始化的时候注册TextBox就可以了,意思是让TextBox这个控件也显示日志信息
        /// </summary>
        /// <param name="richTextBox_log"></param>
        public static void InitializeWPFTextBox(System.Windows.Controls.TextBox textBox)
        {
            if (textBox == null) return;

            // 设置listview打印日志
            var logPattern = "%d{yyyy-MM-dd HH:mm:ss} --%-5p-- %m%n";
            var list_logAppender = new WpfTextBoxAppender();

            list_logAppender.TextBox = textBox;
            list_logAppender.Layout = new PatternLayout(logPattern);

            Logger log4NetLogger = log4Net.Logger as Logger;//单独配置appender
            log4NetLogger.AddAppender(list_logAppender);


        }

        #endregion

        public static void Debug(string str)
        {
            log4Net.Debug(str);
        }
        public static void Info(string str)
        {
            log4Net.Info(str);
        }
        public static void Warn(string str)
        {
            if (s_IsStarting)
            {
                if (!s_ErrMsg.Contains(str)) s_ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }

            log4Net.Warn(str);
        }
        public static void Error(string str) // error为关键字
        {
            if (s_IsStarting)
            {
                if (!s_ErrMsg.Contains(str)) s_ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }
            log4Net.Error(str);
        }
        public static void Fatal(string str)
        {
            if (s_IsStarting)
            {
                if (!s_ErrMsg.Contains(str)) s_ErrMsg.Add(str);//窗体加载过程中 需要弹窗提示
            }
            log4Net.Fatal(str);
        }

        #region 记录模块参数变更记录

        /// <summary>
        /// 记录模块参数变更记录  用于记录额外的一些重要记录,比如关键参数修改记录-->记录在log/modify文件下
        /// </summary>
        /// <param name="str"></param>
        public static void ModullParamModify(string str)
        {
            LogModify.Info(str);
        }

        #endregion

        public static void Print(LogLevel LogLevel, string str)
        {
            switch (LogLevel)
            {
                case LogLevel.Debug:
                    {
                        Log.Debug(str);
                        break;
                    }
                case LogLevel.Info:
                    {
                        Log.Info(str);
                        break;
                    }
                case LogLevel.Warn:
                    {
                        Log.Warn(str);
                        break;
                    }
                case LogLevel.Error:
                    {
                        Log.Error(str);
                        break;
                    }
                case LogLevel.Fatal:
                    {
                        Log.Fatal(str);
                        break;
                    }
                default:
                    {
                        Log.Debug(str);
                        break;
                    }
            }
        }

        #region 获取日志存储文件夹

        /// <summary>
        /// 获取日志存储文件夹
        /// </summary>
        /// <returns></returns>
        private static string getFolder()
        {
            Logger log4NetLogger = log4Net.Logger as Logger;//

            var appender = log4NetLogger.GetAppender("LogFile") as RollingFileAppender;
            return Path.GetDirectoryName(appender.File);
        }

        #endregion

        #region 删除制定日期前的日志

        public static void DeleteLog(int dayNum)
        {
            Task.Run(() =>
            {
                try
                {
                    // 删除制定日期前的日志
                    DateTime tempDate;
                    DirectoryInfo dir = new DirectoryInfo(getFolder());
                    FileInfo[] fileInfo = dir.GetFiles();
                    // 遍历
                    foreach (FileInfo NextFile in fileInfo)
                    {
                        tempDate = NextFile.LastWriteTime;
                        int days = (DateTime.Now - tempDate).Days;
                        // 删除30天前
                        if (days > dayNum)
                            File.Delete(NextFile.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Error(ex.ToString());
                }
            });
        }

        #endregion

        #region 更新日志等级

        /// <summary>
        /// 更新日志等级
        /// </summary>
        /// <param name="level"></param>
        public static void UpdateLogLevel(LogLevel logLevel)
        {
            try
            {
                Level level = Level.Debug;//log4net 自带的类
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        {
                            level = Level.Debug;
                            break;
                        }
                    case LogLevel.Info:
                        {
                            level = Level.Info;
                            break;
                        }
                    case LogLevel.Warn:
                        {
                            level = Level.Warn;
                            break;
                        }
                    case LogLevel.Error:
                        {
                            level = Level.Error;
                            break;
                        }
                    case LogLevel.Fatal:
                        {
                            level = Level.Fatal;
                            break;
                        }
                    default:
                        {
                            level = Level.Debug;
                            break;
                        }
                }

                Logger log4NetLogger = log4Net.Logger as Logger;//
                log4NetLogger.Level = level;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw;
            }
        }

        #endregion

    }
}
