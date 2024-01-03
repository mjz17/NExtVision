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

namespace Plugin.TemplateMatch
{
    /// <summary>
    /// MatchRegionInfo.xaml 的交互逻辑
    /// </summary>
    public partial class MatchRegionInfo : UserControl
    {
        public MatchRegionInfo()
        {
            InitializeComponent();
        }

        #region Row_x

        //Row_X
        public string SerachRowX
        {
            get { return (string)this.GetValue(SerachRowXProperty); }
            set { this.SetValue(SerachRowXProperty, value); }
        }

        public static readonly DependencyProperty SerachRowXProperty =
            DependencyProperty.Register("SerachRowX", typeof(string), typeof(MatchRegionInfo), new PropertyMetadata(default(string)));

        //Row_Y
        public string SerachRowY
        {
            get { return (string)this.GetValue(SerachRowYProperty); }
            set { this.SetValue(SerachRowYProperty, value); }
        }

        public static readonly DependencyProperty SerachRowYProperty =
            DependencyProperty.Register("SerachRowY", typeof(string), typeof(MatchRegionInfo), new PropertyMetadata(default(string)));

        //Colum_X
        public string SerachColumX
        {
            get { return (string)this.GetValue(SerachColumXProperty); }
            set { this.SetValue(SerachColumXProperty, value); }
        }

        public static readonly DependencyProperty SerachColumXProperty =
            DependencyProperty.Register("SerachColumX", typeof(string), typeof(MatchRegionInfo), new PropertyMetadata(default(string)));

        //Colum_Y
        public string SerachColumY
        {
            get { return (string)this.GetValue(SerachColumYProperty); }
            set { this.SetValue(SerachColumYProperty, value); }
        }

        public static readonly DependencyProperty SerachColumYProperty =
            DependencyProperty.Register("SerachColumY", typeof(string), typeof(MatchRegionInfo), new PropertyMetadata(default(string)));

        #endregion

        #region Roi方式

        public static readonly RoutedEvent RoiLinkEvent = EventManager.RegisterRoutedEvent("LinkModelEvent",
           RoutingStrategy.Bubble, typeof(EventHandler<RoutedEventArgs>), typeof(MatchRegionInfo));

        public event RoutedEventHandler LinkModelEvent
        {
            add { AddHandler(RoiLinkEvent, value); }
            remove { RemoveHandler(RoiLinkEvent, value); }
        }

        #endregion

        #region 手动输入是否选中

        //列表名称
        public bool HandCheck
        {
            get { return (bool)this.GetValue(HandCheckProperty); }
            set { this.SetValue(HandCheckProperty, value); }
        }

        public static readonly DependencyProperty HandCheckProperty =
            DependencyProperty.Register("HandCheck", typeof(bool), typeof(MatchRegionInfo), new PropertyMetadata(default(bool)));

        #endregion

        #region 外部链接是否选中

        //列表名称
        public bool LinkCheck
        {
            get { return (bool)this.GetValue(LinkCheckProperty); }
            set { this.SetValue(LinkCheckProperty, value); }
        }

        public static readonly DependencyProperty LinkCheckProperty =
            DependencyProperty.Register("LinkCheck", typeof(bool), typeof(MatchRegionInfo), new PropertyMetadata(default(bool)));

        #endregion

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;

            string select = radio.Content.ToString();

            RoutedEventArgs args = new RoutedEventArgs(RoiLinkEvent, select);

            this.RaiseEvent(args);
        }

    }
}
