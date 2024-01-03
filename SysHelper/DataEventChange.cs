using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysHelper
{
    /// <summary>
    /// 跨窗体传数据
    /// </summary>
    public static class DataEventChange
    {

        #region 主窗体显示窗体的布局变更

        /// <summary>
        /// 主窗体布局刷新/窗体布局使用，序列化后窗体使用
        /// </summary>
        /// <param name="e"></param>
        public delegate void PropertyLayFrmChange(int e);

        static public event PropertyLayFrmChange LayFrmChangeHandler;

        static private int m_frmNum;

        static public int FrmNum
        {
            get
            {
                return m_frmNum;
            }
            set
            {
                m_frmNum = value;
                LayFrmChangeHandler(m_frmNum);
            }
        }

        #endregion

        #region Nav窗体,点击运行后控制其余窗体控件的使能状态

        /// <summary>
        /// Nav窗体点击运行后，控制其余窗体控件的使能状态
        /// </summary>
        /// <param name="e"></param>
        public delegate void PropertyEnabelChange(bool e);

        static public event PropertyEnabelChange NvaControlEnabelChangeHandler;

        static private bool m_NvaControlEnabel;

        static public bool NvaControlEnabel
        {
            get
            {
                return m_NvaControlEnabel;
            }
            set
            {
                m_NvaControlEnabel = value;
                NvaControlEnabelChangeHandler(m_NvaControlEnabel);
            }
        }

        #endregion

        #region Nav窗体,点击急速模式后窗体控件的显示状态

        /// <summary>
        /// Nav窗体点击运行后，控制其余窗体控件的使能状态
        /// </summary>
        /// <param name="e"></param>
        public delegate void QuickProjectChange(string e);

        static public event QuickProjectChange QuickProjectChangeHandler;

        static private string _QuickProject;

        static public string QuickProject
        {
            get
            {
                return _QuickProject;
            }
            set
            {
                _QuickProject = value;
                QuickProjectChangeHandler(_QuickProject);
            }
        }

        #endregion

        #region 流程运行事件，控制Precess启动按钮使能状态

        /// <summary>
        /// 模块窗体，控制Nva启动按钮使能状态
        /// </summary>
        /// <param name="e"></param>
        public delegate void PropertyNvaStatusChange(bool e);

        static public event PropertyNvaStatusChange PrecessChangeHandler;

        static private bool m_processChangeEnabel;

        static public bool ProcessChangeEnabel
        {
            get
            {
                return m_processChangeEnabel;
            }
            set
            {
                m_processChangeEnabel = value;
                PrecessChangeHandler(m_processChangeEnabel);
            }
        }

        #endregion

        #region 通讯监控界面刷新（重新显示整个界面）

        /// <summary>
        /// 将泛型委托过去刷新界面
        /// </summary>
        /// <param name="e"></param>
        public delegate void PropertyDeviceStatus(bool e);

        static public event PropertyDeviceStatus DeviceFrmStatusChangeHandler;

        static private bool m_DeviceFrmStatus;

        static public bool DeviceFrmStatus
        {
            get
            {
                return m_DeviceFrmStatus;
            }
            set
            {
                m_DeviceFrmStatus = value;
                DeviceFrmStatusChangeHandler(m_DeviceFrmStatus);
            }
        }

        #endregion

        #region 刷新主窗体通讯窗体

        /// <summary>
        /// DeviceConnect,窗体使用
        /// 用于后台数据刷新UI，显示连接状态
        /// </summary>
        /// <param name="e"></param>
        public delegate void PropertyDeviceChange(DeviceInfo e);

        static public event PropertyDeviceChange DeviceFrmChangeHandler;

        static private DeviceInfo m_DeviceChange;

        static public DeviceInfo DeviceChange
        {
            get
            {
                return m_DeviceChange;
            }
            set
            {
                m_DeviceChange = value;
                DeviceFrmChangeHandler(m_DeviceChange);
            }
        }

        #endregion

        #region 更新底部窗体显示的路径名称

        public delegate void FootChangeEvent(string e);

        static public event FootChangeEvent footChangeEvent;

        static private string _footdata;

        static public string Footdata
        {
            get
            {
                return _footdata;
            }
            set
            {
                _footdata = value;
                footChangeEvent(_footdata);
            }
        }

        #endregion

        #region 更新项目流程

        public delegate void ChangedProject(bool e);

        static public event ChangedProject changedProjectEvent;

        static private bool _projectData;

        static public bool ProjectData
        {
            get
            {
                return _projectData;
            }
            set
            {
                _projectData = value;
                changedProjectEvent(ProjectData);
            }
        }

        #endregion

        #region Project流程触发UI的Enabel

        public delegate void ProjectChangedEnabel(bool e);

        static public event ProjectChangedEnabel ProjectChangedEvent;

        static private bool _ProjectChangedEnabelData;

        static public bool ProjectChangedEnabelData
        {
            get
            {
                return _ProjectChangedEnabelData;
            }
            set
            {
                _ProjectChangedEnabelData = value;
                ProjectChangedEvent(ProjectChangedEnabelData);
            }
        }

        #endregion

        #region Single流程触发UI的Enabel

        public delegate void SingleProjectChangedEnabel(bool e);

        static public event SingleProjectChangedEnabel SingleProjectChangedEvent;

        static private bool _SingleProjectChangedEnabelData;

        static public bool SingleProjectChangedEnabelData
        {
            get
            {
                return _SingleProjectChangedEnabelData;
            }
            set
            {
                _SingleProjectChangedEnabelData = value;
                SingleProjectChangedEvent(SingleProjectChangedEnabelData);
            }
        }

        #endregion

    }

    public class DeviceInfo
    {
        //驱动名称
        public string DeviceName { get; set; }
        //驱动类型
        public string DeviceType { get; set; }
        //显示图标
        public object IconImage { get; set; }
        //背景色
        public string ImageColor { get; set; }
        //是否链接
        public bool IsConnected { get; set; }

    }

}
