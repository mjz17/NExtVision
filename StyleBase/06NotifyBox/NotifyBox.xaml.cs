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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StyleBase
{
    /// <summary>
    /// NotifyBox.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyBox : Window
    {

        //#region 窗体对象实例

        //private static NotifyBox _instance;
        //public static NotifyBox Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            _instance = new NotifyBox();
        //        _instance.ShowInTaskbar = false;
        //        return _instance;
        //    }
        //}

        //#endregion

        //public void ShowMessage(string Msg)
        //{
        //    NotifyMessage = Msg;
        //    Show();
        //}

        public NotifyBox()
        {
            InitializeComponent();
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height - 25;
        }

        async private void Storyboard_Completed(object sender, EventArgs e)
        {
            await Task.Delay(1500);
            BeginStoryboard(FindResource("CloseStoryboard") as Storyboard);
        }

        private void Storyboard_Completed_1(object sender, EventArgs e)
        {
            Close();
            //Hide();
        }

        public string NotifyMessage { get; set; }

        private void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Text = NotifyMessage;
        }

    }
}
