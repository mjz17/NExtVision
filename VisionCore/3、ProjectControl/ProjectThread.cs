using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization;

namespace VisionCore
{
    /// <summary>
    /// 项目线程
    /// </summary>
    [Serializable]
    public class ProjectThread : IDisposable
    {

        [NonSerialized]
        protected Thread _thread = null;//线程

        [NonSerialized]
        protected bool _threadStatus = false;//线程状态

        [NonSerialized]
        public AutoResetEvent ProjectEventWait = new AutoResetEvent(false);

        public int ThreadID = 0;

        private static int _threadID = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectThread()
        {
            _threadID++;
            ThreadID = _threadID;
        }

        //线程状态
        public bool m_ThreadStatus
        {
            get { return _threadStatus; }
        }

        /// <summary>
        /// 线程启动
        /// </summary>
        public virtual void Thread_Start()
        {
            _threadStatus = true;
        }

        /// <summary>
        /// 线程停止
        /// </summary>
        public virtual void Thread_Stop()
        {
            _threadStatus = false;
        }

        /// <summary>
        /// 获取线程状态
        /// </summary>
        public virtual ThreadState Thread_Status()
        {
            return _thread != null ? _thread.ThreadState : ThreadState.Stopped;
        }

        /// <summary>
        /// 释放线程资源
        /// </summary>
        public void Dispose()
        {
            if (_thread != null && _thread.ThreadState != ThreadState.Aborted)
            {
                _threadStatus = false;
                _thread.Abort();
                _thread = null;
            }
        }

        [OnDeserializing()]
        internal void OnDeSerializingMethod(StreamingContext context)
        {
            ProjectEventWait = new AutoResetEvent(false);
        }

    }
}
