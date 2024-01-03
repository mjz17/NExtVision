using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCamera
{

    /// <summary>
    /// 设备类型
    /// </summary>
    [Serializable]
    public enum DeviceType
    {
        None,
        海康相机,
        大华相机,
    }

    /// <summary>
    /// 触发模式
    /// </summary>
    [Serializable]
    public enum TRIGGER_MODE
    {
        软件触发,
        连续采集,
        硬件触发,
    }
}
