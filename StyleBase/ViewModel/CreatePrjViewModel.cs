using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibBase;
using Common;
using VisionCore;

namespace StyleBase
{
    public class CreatePrjViewModel : NoitifyBase
    {

        public CreatePrjModel CreatePrjModel { get; set; } = new CreatePrjModel();

        /// <summary>
        /// 创建一个新的解决方案
        /// </summary>
        public CommandBase CreatePrj { get; set; }

        /// <summary>
        /// 确定
        /// </summary>
        public CommandBase FridPath { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase CancelFrm { get; set; }

        public CreatePrjViewModel()
        {
            this.CreatePrj = new CommandBase();
            this.CreatePrj.DoExecute = new Action<object>(CreateProject);
            this.CreatePrj.DoCanExecute = new Func<object, bool>((o) =>
              {
                  return true;
              });

            this.FridPath = new CommandBase();
            this.FridPath.DoExecute = new Action<object>(FindTxtPath);
            this.FridPath.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CancelFrm = new CommandBase();
            this.CancelFrm.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();

            });
            this.CancelFrm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="o"></param>
        private void CreateProject(object obj)
        {
            if (CreatePrjModel.ProjectName == null || CreatePrjModel.ProjectName.Trim().Replace(" ", "").ToLowerInvariant().Length == 0)
            {
                System.Windows.MessageBox.Show("请输入解决方案名称!");
                Log.Info("请输入解决方案名称!");
                return;
            }

            if (CreatePrjModel.ProjectPath == null || CreatePrjModel.ProjectPath.Trim().Replace(" ", "").ToLowerInvariant().Length == 0)
            {
                System.Windows.MessageBox.Show("请选择解决方案保存路径!");
                Log.Info("请选择解决方案保存路径!");
                return;
            }

            //清空列表
            SysProcessPro.g_ProjectList.Clear();

            //解决方案名称
            SysProcessPro.SolutionName = CreatePrjModel.ProjectName.Trim().Replace(" ", "").ToLowerInvariant();

            //解决方案路径
            SysProcessPro.SolutionPath = CreatePrjModel.ProjectPath 
                + "\\" + CreatePrjModel.ProjectName.Trim().Replace(" ", "").ToLowerInvariant() + ".nv";

            //更新底部窗体显示
            SysHelper.DataEventChange.Footdata = SysProcessPro.SolutionPath;

            Log.Info("解决方案创建成功！");

            (obj as System.Windows.Window).Close();

        }

        /// <summary>
        /// 打开窗体，获取位置
        /// </summary>
        /// <param name="o"></param>
        private void FindTxtPath(object o)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CreatePrjModel.ProjectPath = dialog.SelectedPath;
            }
        }

    }
}
