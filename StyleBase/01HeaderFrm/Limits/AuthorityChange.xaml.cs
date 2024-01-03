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
using System.Windows.Shapes;

namespace StyleBase
{
    /// <summary>
    /// AuthorityChange.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorityChange : Window
    {
        public AuthorityChange()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += AuthorityChange_Loaded;
        }

        private void AuthorityChange_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleFrm_CloseWindow(object sender, RoutedEventArgs e)
        {
            //btn_Confirm_Click(null, null);
        }

    }
}
