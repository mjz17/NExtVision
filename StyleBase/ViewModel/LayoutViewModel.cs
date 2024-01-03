using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 画布设置的ViewModel
    /// </summary>
    public class LayoutViewModel : NoitifyBase
    {

        #region 画布1

        /// <summary>
        /// 画布1
        /// </summary>
        private bool _canvas_First = true;

        public bool Canvas_First
        {
            get { return _canvas_First; }
            set { _canvas_First = value; this.DoNitify(); }
        }

        #endregion

        #region 画布2

        /// <summary>
        /// 画布2
        /// </summary>
        private bool _canvas_Second;

        public bool Canvas_Second
        {
            get { return _canvas_Second; }
            set { _canvas_Second = value; this.DoNitify(); }
        }

        #endregion

        #region 画布3

        /// <summary>
        /// 画布3
        /// </summary>
        private bool _canvas_Three;

        public bool Canvas_Three
        {
            get { return _canvas_Three; }
            set { _canvas_Three = value; this.DoNitify(); }
        }

        #endregion

        #region 画布4

        /// <summary>
        /// 画布4
        /// </summary>
        private bool _canvas_Four;

        public bool Canvas_Four
        {
            get { return _canvas_Four; }
            set { _canvas_Four = value; this.DoNitify(); }
        }

        #endregion

        #region 画布5

        /// <summary>
        /// 画布4
        /// </summary>
        private bool _canvas_Five;

        public bool Canvas_Five
        {
            get { return _canvas_Five; }
            set { _canvas_Five = value; this.DoNitify(); }
        }

        #endregion

        #region 画布6

        /// <summary>
        /// 画布6
        /// </summary>
        private bool _canvas_Six;

        public bool Canvas_Six
        {
            get { return _canvas_Six; }
            set { _canvas_Six = value; this.DoNitify(); }
        }

        #endregion

        #region 画布8

        /// <summary>
        /// 画布6
        /// </summary>
        private bool _canvas_Eight;

        public bool Canvas_Eight
        {
            get { return _canvas_Eight; }
            set { _canvas_Eight = value; this.DoNitify(); }
        }

        #endregion

        #region 画布设置

        /// <summary>
        /// 画布设置
        /// </summary>
        private int _layout;

        public int Layout
        {
            get { return _layout; }
            set { _layout = value; this.DoNitify(); }
        }

        #endregion

        /// <summary>
        /// 确定
        /// </summary>
        public CommandBase confirmCommand { get; set; }

        /// <summary>
        /// 取消
        /// </summary>
        public CommandBase cancelCommand { get; set; }

        /// <summary>
        /// 选中
        /// </summary>
        public CommandBase checkCommand { get; set; }

        public LayoutViewModel()
        {
            InitLayFrm();
            this.confirmCommand = new CommandBase();
            this.confirmCommand.DoExecute = new Action<object>(confirm_Command);
            this.confirmCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.cancelCommand = new CommandBase();
            this.cancelCommand.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();

            });
            this.cancelCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.checkCommand = new CommandBase();
            this.checkCommand.DoExecute = new Action<object>(check_Command);
            this.checkCommand.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

        /// <summary>
        /// 初始化布局窗体
        /// </summary>
        private void InitLayFrm()
        {
            switch (SysLayout.LayoutFrmNum)
            {
                case 1:
                    Canvas_First = true;
                    Canvas_Second = false;
                    Canvas_Three = false;
                    Canvas_Four = false;
                    Canvas_Five = false;
                    Canvas_Six = false;
                    Canvas_Eight = false;
                    Layout = 1;
                    break;
                case 2:
                    Canvas_First = false;
                    Canvas_Second = true;
                    Canvas_Three = false;
                    Canvas_Four = false;
                    Canvas_Five = false;
                    Canvas_Six = false;
                    Canvas_Eight = false;
                    Layout = 2;
                    break;
                case 3:
                    Canvas_First = false;
                    Canvas_Second = false;
                    Canvas_Three = true;
                    Canvas_Four = false;
                    Canvas_Five = false;
                    Canvas_Six = false;
                    Canvas_Eight = false;
                    Layout = 3;
                    break;
                case 4:
                    Canvas_First = false;
                    Canvas_Second = false;
                    Canvas_Three = false;
                    Canvas_Four = true;
                    Canvas_Five = false;
                    Canvas_Six = false;
                    Canvas_Eight = false;
                    Layout = 4;
                    break;
                case 5:
                    Canvas_First = false;
                    Canvas_Second = false;
                    Canvas_Three = false;
                    Canvas_Four = false;
                    Canvas_Five = true;
                    Canvas_Six = false;
                    Canvas_Eight = false;
                    Layout = 5;
                    break;
                case 6:
                    Canvas_First = false;
                    Canvas_Second = false;
                    Canvas_Three = false;
                    Canvas_Four = false;
                    Canvas_Five = false;
                    Canvas_Six = true;
                    Canvas_Eight = false;
                    Layout = 6;
                    break;
                case 8:
                    Canvas_First = false;
                    Canvas_Second = false;
                    Canvas_Three = false;
                    Canvas_Four = false;
                    Canvas_Five = false;
                    Canvas_Six = false;
                    Canvas_Eight = true;
                    Layout = 8;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="obj"></param>
        public void confirm_Command(object obj)
        {
            //窗体布局的更新
            SysHelper.DataEventChange.FrmNum = Layout;
            (obj as System.Windows.Window).Close();
        }

        /// <summary>
        /// 选择布局
        /// </summary>
        /// <param name="obj"></param>
        public void check_Command(object obj)
        {
            System.Windows.Controls.RadioButton radio = (System.Windows.Controls.RadioButton)obj;
            if (radio.Content.ToString() == "1画布")
            {
                Canvas_First = true;
                Canvas_Second = false;
                Canvas_Three = false;
                Canvas_Four = false;
                Canvas_Five = false;
                Canvas_Six = false;
                Canvas_Eight = false;
                Layout = 1;
            }
            else if (radio.Content.ToString() == "2画布")
            {
                Canvas_First = false;
                Canvas_Second = true;
                Canvas_Three = false;
                Canvas_Four = false;
                Canvas_Five = false;
                Canvas_Six = false;
                Canvas_Eight = false;
                Layout = 2;
            }
            else if (radio.Content.ToString() == "3画布")
            {
                Canvas_First = false;
                Canvas_Second = false;
                Canvas_Three = true;
                Canvas_Four = false;
                Canvas_Five = false;
                Canvas_Six = false;
                Canvas_Eight = false;
                Layout = 3;
            }
            else if (radio.Content.ToString() == "4画布")
            {
                Canvas_First = false;
                Canvas_Second = false;
                Canvas_Three = false;
                Canvas_Four = true;
                Canvas_Five = false;
                Canvas_Six = false;
                Canvas_Eight = false;
                Layout = 4;
            }
            else if (radio.Content.ToString() == "5画布")
            {
                Canvas_First = false;
                Canvas_Second = false;
                Canvas_Three = false;
                Canvas_Four = false;
                Canvas_Five = true;
                Canvas_Six = false;
                Canvas_Eight = false;
                Layout = 5;
            }
            else if (radio.Content.ToString() == "6画布")
            {
                Canvas_First = false;
                Canvas_Second = false;
                Canvas_Three = false;
                Canvas_Four = false;
                Canvas_Five = false;
                Canvas_Six = true;
                Canvas_Eight = false;
                Layout = 6;
            }
            else if (radio.Content.ToString() == "8画布")
            {
                Canvas_First = false;
                Canvas_Second = false;
                Canvas_Three = false;
                Canvas_Four = false;
                Canvas_Five = false;
                Canvas_Six = false;
                Canvas_Eight = true;
                Layout = 8;
            }
        }

    }
}
