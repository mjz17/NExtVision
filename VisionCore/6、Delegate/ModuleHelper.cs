using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    public class ModuleHelper
    {
        /// <summary>
        /// 更新视觉主窗体列表信息
        /// </summary>
        /// <param name="P"></param>
        public delegate void DlgUpdateProjectList(List<ModuleInfo> P,int time);

        public static DlgUpdateProjectList UpdateProject;

    }
}
