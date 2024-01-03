using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace StyleBase
{
    public class LinkProcessViewModel : NoitifyBase
    {
        //流程列表
        public List<string> Process_Lst { get; set; } = new List<string>();

        //combox选择事件
        public CommandBase CmbSelectChange { get; set; }

        //确定
        public CommandBase BtnConfirm { get; set; }

        //取消
        public CommandBase BtnCancel { get; set; }

        //选中的combox的数据
        public string SelectCmb { get; set; } = string.Empty;

        public delegate void SendMessage(string Prpcess);

        public SendMessage sendMessage;

        public LinkProcessViewModel()
        {
            InitType_Lst();

            this.CmbSelectChange = new CommandBase();
            this.CmbSelectChange.DoExecute = new Action<object>(Cmb_SelectChange);
            this.CmbSelectChange.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnConfirm = new CommandBase();
            this.BtnConfirm.DoExecute = new Action<object>(LinkConfirm);
            this.BtnConfirm.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnCancel = new CommandBase();
            this.BtnCancel.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).DialogResult = false;
                (o as System.Windows.Window).Close();
            });
            this.BtnCancel.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 初始化模块列表
        /// </summary>
        private void InitType_Lst()
        {
            foreach (Project item in SysProcessPro.g_ProjectList)
            {
                Process_Lst.Add(item.ProjectInfo.m_ProjectName.ToString());
            }
        }

        //cmb的选中行
        private void Cmb_SelectChange(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (Process_Lst.Count == 0)
            {
                return;
            }

            SelectCmb = obj.ToString();

        }

        //确定
        private void LinkConfirm(object obj)
        {

            if (SelectCmb == string.Empty)
            {
                return;
            }

            (obj as System.Windows.Window).DialogResult = true;

            sendMessage(SelectCmb);

            (obj as System.Windows.Window).Close();

        }

    }
}
