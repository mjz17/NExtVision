using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common;
using CommunaCation;

namespace StyleBase
{
    /// <summary>
    /// FrmCommunaCation.xaml 的交互逻辑
    /// </summary>
    public partial class FrmCommunaCation : Window
    {
        public FrmCommunaCation()
        {
            InitializeComponent();
            this.DataContext = new CommunaCationViewModel(Page_Change);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)Commun_Lst.ContainerFromElement((System.Windows.Controls.Primitives.ToggleButton)sender)).Content;

            ComunaInfo info = (ComunaInfo)curItem;

            SysHelper.DeviceInfo device = new SysHelper.DeviceInfo();
            device.DeviceName = info.Name;

            if (info.isSelect)
            {
                //监听成功 返回true,false表示被占用,监听失败
                if (EComManageer.Connect(info.Name))
                {
                    Log.Info("监听" + info.Name + "成功");
                    //更新通讯界面
                    device.IsConnected = true;
                    SysHelper.DataEventChange.DeviceChange = device;
                }
                else
                {
                    Log.Info("监听" + info.Name + "失败");
                    //更新通讯界面
                    device.IsConnected = true;
                    SysHelper.DataEventChange.DeviceChange = device;
                }
            }
            else
            {
                EComManageer.DisConnect(info.Name);
                //更新通讯界面
                device.IsConnected = false;
                SysHelper.DataEventChange.DeviceChange = device;
                Log.Info("断开" + info.Name);
            }
        }



    }
}
