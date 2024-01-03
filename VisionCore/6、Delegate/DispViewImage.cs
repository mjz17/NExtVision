using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 显示窗体
    /// </summary>
    public class DispViewImage
    {
        /// <summary>
        /// 创建的委托
        /// </summary>
        /// <param name="name">流程名</param>
        /// <param name="layouts">窗体变量</param>
        public delegate void DgeDispViewImage(string name,List<LayoutInfo>layouts);

        /// <summary>
        /// 委托变量
        /// </summary>
        public DgeDispViewImage dgeDispView;

    }
}
