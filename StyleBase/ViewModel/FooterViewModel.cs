using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using VisionCore;

namespace StyleBase
{
    public class FooterViewModel : NoitifyBase
    {

        //解决方案名称
        private string _solution;

        public string Solution
        {
            get { return _solution; }
            set { _solution = value; this.DoNitify(); }
        }

        //急速模式
        private string _quickStatus = "关闭";

        public string QuickStatus
        {
            get { return _quickStatus; }
            set { _quickStatus = value; this.DoNitify(); }
        }

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        public FooterViewModel()
        {
            SysHelper.DataEventChange.footChangeEvent += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    Solution = e;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Solution = e;
                    }));
                }
            };

            SysHelper.DataEventChange.QuickProjectChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    QuickStatus = e;
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        QuickStatus = e;
                    }));
                }
            };
        }
    }
}
