using ClassLibBase;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 重命名
    /// </summary>
    public class ReNameToolViewModel : NoitifyBase
    {
        /// <summary>
        /// 旧名称
        /// </summary>
        private string _oldName;

        public string m_oldName
        {
            get { return _oldName; }
            set { _oldName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 新名称
        /// </summary>
        private string _newName;

        public string m_newName
        {
            get { return _newName; }
            set { _newName = value; this.DoNitify(); }
        }

        private int _ID;

        public int m_ID
        {
            get { return _ID; }
            set { _ID = value; }
        }


        /// <summary>
        /// 确定
        /// </summary>
        public CommandBase ConfirmCom { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase CancelfirmCom { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReNameToolViewModel(string FrmName, int id)
        {
            m_oldName = FrmName;
            m_ID = id;

            this.ConfirmCom = new CommandBase();
            this.ConfirmCom.DoExecute = new Action<object>(Confirm);
            this.ConfirmCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.CancelfirmCom = new CommandBase();
            this.CancelfirmCom.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();

            });
            this.CancelfirmCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="obj"></param>
        private void Confirm(object obj)
        {
            //判断输入是否为空
            if (m_newName == null || m_newName.Length == 0)
            {

                System.Windows.Forms.MessageBox.Show("输入为空！");
                return;
            }

            //判断是否有重复名称
            int index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == m_newName);
            if (index > -1)
            {
                System.Windows.Forms.MessageBox.Show("存在重复的流程名称！");
                return;
            }

            SysProcessPro.Cur_Project.ProjectInfo.m_ProjectName = m_newName;

            (obj as System.Windows.Window).Close();

        }



    }
}
