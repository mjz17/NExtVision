using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using ModuleDataVar;

namespace StyleBase
{
    public static class DataChange
    {

        //流程栏事件刷新
        public delegate void PropertyChanged(List<ModuleInfo> e);

        static public event PropertyChanged propertyChanged;

        //定义数据类型
        static private List<ModuleInfo> Infos;

        static public List<ModuleInfo> m_Infos
        {
            get
            {
                return Infos;
            }
            set
            {
                Infos = value;
                propertyChanged(Infos);
            }
        }
     
    }
}
