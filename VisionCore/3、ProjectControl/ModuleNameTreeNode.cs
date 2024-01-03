using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 项目执行顺序控制
    /// </summary>
    public class ModuleNameTreeNode
    {
        public ModuleNameTreeNode Parent = null;

        public string Name;

        public List<string> ChildList = new List<string>();

        public ModuleNameTreeNode(string name)
        {
            Name = name;
        }

    }
}
