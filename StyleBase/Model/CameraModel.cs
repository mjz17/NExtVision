using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleBase
{
    public class CameraModel : NoitifyBase
    {
        /// <summary>
        /// 相机名称
        /// </summary>
        private string _cameraName;

        public string CameraName
        {
            get { return _cameraName; }
            set { _cameraName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 设备型号
        /// </summary>
        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; this.DoNitify(); }
        }

        /// <summary>
        /// 设备地址
        /// </summary>
        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; this.DoNitify(); }
        }
    }
}
