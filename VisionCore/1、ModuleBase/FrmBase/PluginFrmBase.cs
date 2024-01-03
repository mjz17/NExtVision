using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common;

namespace VisionCore
{
    /// <summary>
    /// 窗体类
    /// </summary>
    public class PluginFrmBase : Window
    {
        public ModuleFrmBase ModuleFrmBase { get; set; }//底部控件

        /// <summary>
        /// 窗体对应的obj数据
        /// </summary>
        public ModuleObjBase m_ModuleObjBase { get; set; }

        /// <summary>
        /// 备份取消时 还原
        /// </summary>
        public ModuleObjBase m_ModuleObjBaseBack;

        /// <summary>
        /// 初次打开模块
        /// </summary>
        public virtual void theFirsttime()
        {

        }

        /// <summary>
        /// 第二次打开模块
        /// </summary>
        public virtual void theSecondTime()
        {
            //m_ModuleObjBaseBack = CloneObject.DeepCopy(m_ModuleObjBase);
            m_ModuleObjBaseBack = m_ModuleObjBase;
            m_ModuleObjBase.blnNewModule = true;
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        public string CurrentImage
        {
            get { return (string)this.GetValue(CurrentImageProperty); }
            set { this.SetValue(CurrentImageProperty, value); }
        }

        public static readonly DependencyProperty CurrentImageProperty =
            DependencyProperty.Register("CurrentImage", typeof(string), typeof(PluginFrmBase), new PropertyMetadata(default(string)));

        /// <summary>
        /// 当前模块ID
        /// </summary>
        public string CurrentModelID
        {
            get { return (string)this.GetValue(CurrentModelIDProperty); }
            set { this.SetValue(CurrentModelIDProperty, value); }
        }

        public static readonly DependencyProperty CurrentModelIDProperty =
            DependencyProperty.Register("CurrentModelID", typeof(string), typeof(PluginFrmBase), new PropertyMetadata(default(string)));

        /// <summary>
        /// 执行模块的程序保护
        /// </summary>
        public virtual bool ProtectModuel() { return true; }

        /// <summary>
        /// 运行模块一次
        /// </summary>
        public virtual void ExModule() { }

        /// <summary>
        /// 保存模块参数
        /// </summary>
        public virtual void SaveModuleParam() { }

        /// <summary>
        /// 取消模块参数
        /// </summary>
        public virtual void CancelModuleParam()
        {
            m_ModuleObjBase = m_ModuleObjBaseBack;//返还原参数
            this.Close();
        }

    }
}
