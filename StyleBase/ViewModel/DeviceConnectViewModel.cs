using ClassLibBase;
using CommunaCation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace StyleBase
{
    /// <summary>
    /// 设备状态栏
    /// </summary>
    public class DeviceConnectViewModel : NoitifyBase
    {
        /// <summary>
        /// 通讯连接状态
        /// </summary>
        public ObservableCollection<SysHelper.DeviceInfo> CommunConnect { get; set; } = new ObservableCollection<SysHelper.DeviceInfo>();

        private Dispatcher m_Dispatcher = Dispatcher.CurrentDispatcher;

        public DeviceConnectViewModel()
        {
            //传入集合重新显示界面
            SysHelper.DataEventChange.DeviceFrmStatusChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    InitCommunaCation();
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        InitCommunaCation();
                    }));
                }
            };

            //后台状态来变更通讯窗体UI显示
            SysHelper.DataEventChange.DeviceFrmChangeHandler += (e) =>
            {
                if (m_Dispatcher.CheckAccess())
                {
                    UpdateDeviceFrm(e);
                }
                else
                {
                    m_Dispatcher.BeginInvoke((Action)(() =>
                    {
                        UpdateDeviceFrm(e);
                    }));
                }
            };
        }

        /// <summary>
        /// 显示通讯控件
        /// </summary>
        public void InitCommunaCation()
        {
            CommunConnect.Clear();
            foreach (var item in EComManageer.s_ECommunacationDic)
            {
                CommunConnect.Add(new SysHelper.DeviceInfo
                {
                    DeviceName = item.Value.Key,
                    IsConnected = item.Value.IsConnected,
                    IconImage = "\xe62c",
                    ImageColor = item.Value.IsConnected ? "#00CB9A" : "#C21B05"
                });
            }
        }

        //通讯控件状态变更
        public void UpdateDeviceFrm(SysHelper.DeviceInfo Info)
        {

            List<SysHelper.DeviceInfo> Info1 = new List<SysHelper.DeviceInfo>();
            foreach (SysHelper.DeviceInfo item in CommunConnect)
            {
                Info1.Add(item);
            }

            SysHelper.DeviceInfo DevInfo = Info1.FirstOrDefault(c => c.DeviceName == Info.DeviceName);
            if (DevInfo != null)
            {
                DevInfo.IsConnected = Info.IsConnected;
                DevInfo.ImageColor = Info.IsConnected ? "#00CB9A" : "#C21B05";
            }

            CommunConnect.Clear();
            foreach (SysHelper.DeviceInfo item in Info1)
            {
                CommunConnect.Add(item);
            }
        }

    }
}
