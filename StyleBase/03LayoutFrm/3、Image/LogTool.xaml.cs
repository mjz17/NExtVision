using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;

namespace StyleBase
{
    /// <summary>
    /// LogTool.xaml 的交互逻辑
    /// </summary>
    public partial class LogTool : UserControl
    {
        public LogTool()
        {
            InitializeComponent();
            Log.RegisterLog();
            Log.InitializeWPFTextBox(Logtxt);
            Log.Info("欢迎进入");
        }
    }
}
