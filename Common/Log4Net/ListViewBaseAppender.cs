using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common
{
    /// <summary>
    /// 同时写入显示控件
    /// </summary>
    public class ListViewBaseAppender : AppenderSkeleton
    {
        public ListView m_listView { get; set; }

        static ListViewBaseAppender()
        {

        }
        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            PatternLayout patternLayout = (PatternLayout)this.Layout;
            string str = string.Empty;

            if (patternLayout != null)
            {
                str = patternLayout.Format(loggingEvent);

                if (loggingEvent.ExceptionObject != null)
                    str += loggingEvent.ExceptionObject.ToString() + Environment.NewLine;
            }
            else
                str = loggingEvent.LoggerName + "-" + loggingEvent.RenderedMessage + Environment.NewLine;

            // 打印
            printf(str);
        }

        private bool m_Flag = false;//线程开启

        private List<string> m_LogStrList = new List<string>();

        private Object m_LockObj = new Object();

        private void printf(string str)
        {
            lock (m_LockObj)
            {
                Debug.WriteLine(str);
                m_LogStrList.Add(str);
            }

            if (m_Flag == false)
            {
                m_Flag = true;
                Task.Run(
                    () =>
                    {
                        while (true)
                        {
                            Thread.Sleep(1000);//日志刷新周期是300ms 避免 native刷新太块 抢占了主线程
                            if (m_LogStrList.Count == 0) continue;
                            List<string> tempList = new List<string>();
                            lock (m_LockObj)
                            {
                                for (int i = 0; i < m_LogStrList.Count; i++)
                                {
                                    tempList.Add(m_LogStrList[i]);
                                }
                                m_LogStrList.Clear();
                            }
                            if (tempList.Count > 0)
                            {
                                try
                                {
                                    m_listView.Invoke(
                                    new Action(() =>
                                    {
                                        try
                                        {
                                            if (m_listView.Items.Count > 100)
                                            {
                                                m_listView.Items.Clear();
                                            }
                                            m_listView.BeginUpdate();
                                            foreach (var strTemp in tempList)
                                            {
                                                ListViewItem item = new ListViewItem();
                                                item.Text = strTemp.ToString();
                                                m_listView.Items.Add(item);
                                            }
                                            m_listView.Items[m_listView.Items.Count - 1].EnsureVisible();//滚动到最后  
                                            m_listView.EndUpdate();
                                        }
                                        catch (Exception e)
                                        {
                                            throw e;
                                        }
                                    }));
                                }
                                catch (TaskCanceledException)
                                {
                                    //TaskCanceledException 异常是关闭时候可能出现 直接忽略 yoga  2019-9-8 06:46:55 
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                    });
            }
        }
    }
}
