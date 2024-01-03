using StyleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionCore;

namespace NExtVison
{
    /// <summary>
    /// 加载资源
    /// </summary>
    public class LoadResources
    {
        CommonConfig config = new CommonConfig();

        /// <summary>
        /// 初始化资源
        /// </summary>
        /// <param name="grid"></param>
        public void InitResourse(Grid grid)
        {
            try
            {
                if (Convert.ToBoolean(config.ReadConfig("ProjectConfig", "是否加载")))
                {
                    string files = config.ReadConfig("ProjectConfig", "加载路径");

                    //加载窗体
                    LoadWindow dlg = new LoadWindow(new LoadSolution(files));
                    dlg.ShowInTaskbar = false;
                    dlg.ShowDialog();
                }
                else
                {
                    //不需要加载
                    SysLayout.CreateLayoutFrm(grid);
                }

                SysProcessPro.SysInterval = Convert.ToInt32(config.ReadConfig("ProjectConfig", "流程间隔"));
                SysProcessPro.SaveUpValue = Convert.ToBoolean(config.ReadConfig("ProjectConfig", "保存更新的值"));

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

    }

    /// <summary>
    /// 软件自动加载流程
    /// </summary>
    public class LoadSolution : LongTimeTask
    {
        private Thread m_threadWorking;
        private LoadWindow m_dlgWaiting;
        private string files;
        private CommonBase common = new CommonBase();//根据值显示一个流程
        public void Start(LoadWindow load)
        {
            m_dlgWaiting = load;
            m_threadWorking = new Thread(Working);
            m_threadWorking.Start();
        }

        public LoadSolution(string str)
        {
            files = str;
        }

        private void Working()
        {
            SysProcessPro.InitialVisionPram(files);

            if (SysProcessPro.g_ProjectList == null)
            {
                SysProcessPro.g_ProjectList = new List<Project>();
            }
            if (SysProcessPro.g_ProjectList.Count < 1)
            {
                SysProcessPro.g_ProjectList.Add(new Project());
            }

            SysProcessPro.Cur_Project = SysProcessPro.g_ProjectList[0];

            SysProcessPro.SolutionPath = files.ToLowerInvariant();

            SysProcessPro.SysFliePath = files.Substring(0, files.LastIndexOf("\\"));

            string filename = System.IO.Path.GetFileNameWithoutExtension(files);

            SysProcessPro.SolutionName = filename;

            SysHelper.DataEventChange.Footdata = SysProcessPro.SolutionPath;//更新底部窗体显示

            SysHelper.DataEventChange.FrmNum = SysLayout.LayoutFrmNum;//窗体布局的更新

            SysHelper.DataEventChange.ProjectData = true;//更新流程窗体显示

            common.RefreshToolList(SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName);//委托刷新Process，UI界面

            SysHelper.DataEventChange.DeviceFrmStatus = true;//更新通讯界面

            m_dlgWaiting.TaskEnd(null);
        }

    }
}
