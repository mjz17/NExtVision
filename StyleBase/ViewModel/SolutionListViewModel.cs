using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleBase
{
    /// <summary>
    /// 解决方案列表
    /// </summary>
    public class SolutionListViewModel : NoitifyBase
    {
        /// <summary>
        /// 打开流程
        /// </summary>
        public CommandBase ComOpenSolutionList { get; set; }

        /// <summary>
        /// 默认启动
        /// </summary>
        public CommandBase ComDefaultStart { get; set; }

        /// <summary>
        /// 添加当前解决方案
        /// </summary>
        public CommandBase ComAddDefaultSolutionList { get; set; }

        /// <summary>
        /// 添加
        /// </summary>
        public CommandBase ComAdd { get; set; }

        /// <summary>
        /// 删除
        /// </summary>
        public CommandBase ComDelete { get; set; }

        /// <summary>
        /// 上移
        /// </summary>
        public CommandBase ComUp { get; set; }

        /// <summary>
        /// 下移
        /// </summary>
        public CommandBase ComDown { get; set; }

        /// <summary>
        /// 解决方案数据
        /// </summary>
        private List<SolutionList> solution = new List<SolutionList>();

        /// <summary>
        /// 配置文件操作
        /// </summary>
        private CommonConfig config = new CommonConfig();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SolutionListViewModel()
        {
            #region 打开流程

            this.ComOpenSolutionList = new CommandBase();
            this.ComOpenSolutionList.DoExecute = new Action<object>(OpenSolutionList);
            this.ComOpenSolutionList.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 默认启动

            this.ComDefaultStart = new CommandBase();
            this.ComDefaultStart.DoExecute = new Action<object>(DefaultStart);
            this.ComDefaultStart.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 添加当前解决方案

            this.ComAddDefaultSolutionList = new CommandBase();
            this.ComAddDefaultSolutionList.DoExecute = new Action<object>(AddDefaultSolutionList);
            this.ComAddDefaultSolutionList.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 添加

            this.ComAdd = new CommandBase();
            this.ComAdd.DoExecute = new Action<object>(Add);
            this.ComAdd.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 删除

            this.ComDelete = new CommandBase();
            this.ComDelete.DoExecute = new Action<object>(Delete);
            this.ComDelete.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 上移

            this.ComUp = new CommandBase();
            this.ComUp.DoExecute = new Action<object>(Up);
            this.ComUp.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            #region 上移

            this.ComDown = new CommandBase();
            this.ComDown.DoExecute = new Action<object>(Down);
            this.ComDown.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            #endregion
            InitDataGrid();
        }

        private void InitDataGrid()
        {
            string str = config.ReadConfig("SolutionPath", "保存图像测试");
        }

        /// <summary>
        /// 打开流程
        /// </summary>
        /// <param name="obj"></param>
        private void OpenSolutionList(object obj)
        {

        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void DefaultStart(object obj)
        {

        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void AddDefaultSolutionList(object obj)
        {

        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void Add(object obj)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "工程文件|*.nv";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //获取程序路径
                string solutionPath = openFileDialog.FileName;
                //文件名没有扩展名
                string solutionName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.SafeFileName);
                config.AddConfig("SolutionPath", solutionName, solutionPath);
            }
        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void Delete(object obj)
        {

        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void Up(object obj)
        {

        }

        /// <summary>
        /// 默认启动
        /// </summary>
        /// <param name="obj"></param>
        private void Down(object obj)
        {

        }


    }
}
