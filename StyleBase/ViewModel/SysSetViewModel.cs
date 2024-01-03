using ClassLibBase;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 系统参数设置
    /// </summary>
    public class SysSetViewModel : NoitifyBase
    {

        /// <summary>
        /// 是否启动加载
        /// </summary>
        private bool _Loadornot;

        public bool Loadornot
        {
            get { return _Loadornot; }
            set { _Loadornot = value; this.DoNitify(); }
        }

        /// <summary>
        /// 路径地址的控件状态
        /// </summary>
        private Visibility _PathVisibility;

        public Visibility PathVisibility
        {
            get { return _PathVisibility; }
            set { _PathVisibility = value; this.DoNitify(); }
        }

        /// <summary>
        /// 路径地址
        /// </summary>
        private string _PathAddress;

        public string PathAddress
        {
            get { return _PathAddress; }
            set { _PathAddress = value; this.DoNitify(); }
        }

        /// <summary>
        /// 开机自启
        /// </summary>
        private bool _BootStrapStatus;

        public bool BootStrapStatus
        {
            get { return _BootStrapStatus; }
            set { _BootStrapStatus = value; this.DoNitify(); }
        }

        /// <summary>
        /// 间隔时间
        /// </summary>
        private string _Interval;

        public string Interval
        {
            get { return _Interval; }
            set { _Interval = value; this.DoNitify(); }
        }

        #region 启动是否加载按钮

        /// <summary>
        /// 启动是否加载按钮
        /// </summary>
        public CommandBase LoadOrnotCom { get; set; }

        #endregion

        #region 查询按钮

        /// <summary>
        /// 确定按钮
        /// </summary>
        public CommandBase BtnQueryCom { get; set; }

        #endregion

        #region 开机自启

        /// <summary>
        /// 开机自启
        /// </summary>
        public CommandBase bootStrapCom { get; set; }

        #endregion

        #region 确定按钮

        /// <summary>
        /// 确定按钮
        /// </summary>
        public CommandBase BtnConfirmeCom { get; set; }

        #endregion

        #region 取消按钮

        /// <summary>
        /// 取消按钮
        /// </summary>
        public CommandBase BtnCancelCom { get; set; }

        #endregion

        CommonConfig config = new CommonConfig();

        /// <summary>
        /// 构造函数
        /// </summary>
        public SysSetViewModel()
        {
            InitConfig();

            this.LoadOrnotCom = new CommandBase();
            this.LoadOrnotCom.DoExecute = new Action<object>(LoadOrnot);
            this.LoadOrnotCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.bootStrapCom = new CommandBase();
            this.bootStrapCom.DoExecute = new Action<object>(bootStrap);
            this.bootStrapCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnQueryCom = new CommandBase();
            this.BtnQueryCom.DoExecute = new Action<object>(BtnQuery);
            this.BtnQueryCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnConfirmeCom = new CommandBase();
            this.BtnConfirmeCom.DoExecute = new Action<object>(BtnConfirme);
            this.BtnConfirmeCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.BtnCancelCom = new CommandBase();
            this.BtnCancelCom.DoExecute = new Action<object>((o) =>
            {
                (o as System.Windows.Window).Close();
            });
            this.BtnCancelCom.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitConfig()
        {
            Loadornot = Convert.ToBoolean(config.ReadConfig("ProjectConfig", "是否加载"));

            PathVisibility = Loadornot ? Visibility.Visible : Visibility.Collapsed;

            BootStrapStatus = Convert.ToBoolean(config.ReadConfig("ProjectConfig", "开机自启"));

            PathAddress = config.ReadConfig("ProjectConfig", "加载路径");
            Interval = config.ReadConfig("ProjectConfig", "流程间隔");
        }

        /// <summary>
        /// 是否确定
        /// </summary>
        /// <param name="obj"></param>
        private void LoadOrnot(object obj)
        {
            CheckBox check = obj as CheckBox;
            if (check != null)
            {
                if ((bool)check.IsChecked)
                {
                    Loadornot = true;
                    PathVisibility = Visibility.Visible;
                }
                else
                {
                    Loadornot = false;
                    PathVisibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 开机自启动
        /// </summary>
        /// <param name="obj"></param>
        private void bootStrap(object obj)
        {
            CheckBox check = obj as CheckBox;
            if (check != null)
            {
                if ((bool)check.IsChecked)
                {
                    BootStrapStatus = true;
                    Nginx(true);
                }
                else
                {
                    BootStrapStatus = false;
                    Nginx(false);
                }
            }
        }

        //开机自启动
        public void Nginx(bool bstate)
        {
            if (bstate)
            {
                //此⽅法把启动项加载到注册表中

                //获得应⽤程序路径
                string strAssName = System.Windows.Forms.Application.StartupPath + @"\" + System.Windows.Forms.Application.ProductName + @".exe";

                //获得应⽤程序名
                string ShortFileName = System.Windows.Forms.Application.ProductName;

                RegistryKey rgkRun = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (rgkRun == null)
                {
                    rgkRun = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }
                rgkRun.SetValue(ShortFileName, strAssName);
            }
            else
            {
                //删除注册表中启动项

                //获得应⽤程序名
                string ShortFileName = System.Windows.Forms.Application.ProductName;

                RegistryKey rgkRun = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (rgkRun == null)
                {
                    rgkRun = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }

                rgkRun.DeleteValue(ShortFileName, false);
            }
        }

        //查询按钮
        private void BtnQuery(object obj)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "工程文件|*.nv";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDialog.FileName;

                if (file.Trim() == "" || System.IO.File.Exists(file) == false)
                {
                    System.Windows.Forms.MessageBox.Show("输入文件名错误");
                    return;
                }

                PathAddress = file;
            }
        }

        //确定按钮
        private void BtnConfirme(object obj)
        {
            config.WriteConfig("ProjectConfig", "是否加载", Loadornot.ToString());
            config.WriteConfig("ProjectConfig", "加载路径", PathAddress);
            config.WriteConfig("ProjectConfig", "流程间隔", Interval);
            config.WriteConfig("ProjectConfig", "开机自启", BootStrapStatus.ToString());

            SysProcessPro.SysInterval = Convert.ToInt32(config.ReadConfig("ProjectConfig", "流程间隔"));

            (obj as System.Windows.Window).Close();
        }


    }
}
