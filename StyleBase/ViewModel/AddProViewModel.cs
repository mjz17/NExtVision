using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using VisionCore;

namespace StyleBase
{
    public class AddProViewModel : NoitifyBase
    {
        //流程名称
        private string _projectName;

        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 创建一个项目流程
        /// </summary>
        public CommandBase CreatePro { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase CancelFrm { get; set; }

        public AddProViewModel()
        {
            this.CreatePro = new CommandBase();
            this.CreatePro.DoExecute = new Action<object>(AddProject);
            this.CreatePro.DoCanExecute = new Func<object, bool>((o) =>
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

        private void AddProject(object obj)
        {

            if (ProjectName == null || ProjectName.Trim().Replace(" ", "").ToLowerInvariant().Length == 0)
            {
                System.Windows.MessageBox.Show("请输入流程名称！");
                Log.Info("请输入流程名称！");
                return;
            }

            if (SameNmae(ProjectName))
            {
                System.Windows.MessageBox.Show("请输入流程相同,“清修改”！");
                Log.Info("请输入流程相同,“清修改”！");
                return;
            }

            //新建一个项目流程
            Project P = new Project();

            P.ProjectInfo.m_ProjectName = ProjectName;

            SysProcessPro.g_ProjectList.Add(P);

            (obj as System.Windows.Window).Close();

        }

        private bool SameNmae(string Name)
        {
            int index = -1;
            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Name);
                if (index >= 0)
                {
                    break;
                }
            }
            return index >= 0 ? true : false;
        }

    }
}
