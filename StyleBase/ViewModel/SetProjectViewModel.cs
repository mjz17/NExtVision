using ClassLibBase;
using ModuleCamera;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 选择流程
    /// </summary>
    public class SetProjectViewModel : NoitifyBase
    {
        public ObservableCollection<ProjectInfo> ProName { get; set; } = new ObservableCollection<ProjectInfo>();

        /// <summary>
        /// 确认
        /// </summary>
        public CommandBase ConfirmCom { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase CancelFrmCom { get; set; }

        /// <summary>
        /// 选中流程
        /// </summary>
        public CommandBase SelectCom { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SetProjectViewModel()
        {
            InitProjectInfo();

            this.ConfirmCom = new CommandBase();
            this.ConfirmCom.DoExecute = new Action<object>(Confirm);
            this.ConfirmCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CancelFrmCom = new CommandBase();
            this.CancelFrmCom.DoExecute = new Action<object>(CancelFrm);
            this.CancelFrmCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.SelectCom = new CommandBase();
            this.SelectCom.DoExecute = new Action<object>(Select);
            this.SelectCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

        /// <summary>
        /// 初始化流程信息
        /// </summary>
        private void InitProjectInfo()
        {
            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                ProName.Add(item.ProjectInfo);
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="obj"></param>
        private void Confirm(object obj)
        {
            Window win = obj as Window;

            if (win != null)
            {
                win.Close();
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="obj"></param>
        private void CancelFrm(object obj)
        {
            Window win = obj as Window;

            if (win != null)
            {
                win.Close();
            }
        }

        /// <summary>
        /// 选中项
        /// </summary>
        /// <param name="obj"></param>
        private void Select(object obj)
        {

        }



    }

}
