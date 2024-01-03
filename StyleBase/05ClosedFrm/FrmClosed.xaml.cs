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

namespace StyleBase
{
    /// <summary>
    /// FrmClosed.xaml 的交互逻辑
    /// </summary>
    public partial class FrmClosed : Window
    {
        public FrmClosed()
        {
            InitializeComponent();
        }

        public MessageBoxResult result = new MessageBoxResult();

        private void btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Yes;
            this.Close();
        }

        private void btn_No_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.No;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            this.Close();
        }

        private void TitleFrm_CloseWindow(object sender, RoutedEventArgs e)
        {
            result = MessageBoxResult.Cancel;
            this.Close();
        }

    }
}
