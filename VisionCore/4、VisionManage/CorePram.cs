using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{

    /// <summary>
    /// 旋转图像
    /// </summary>
    [Serializable]
    public enum IMG_ADJUST
    {
        None,
        垂直镜像,
        水平镜像,
        顺时针90度,
        逆时针90度,
        旋转180度
    }
}
