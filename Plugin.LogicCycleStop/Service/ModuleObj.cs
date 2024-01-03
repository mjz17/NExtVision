using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.LogicCycleStop
{
    [Category("逻辑工具")]
    [DisplayName("停止循环")]
    [Serializable]
    internal class ModuleObj: ModuleObjBase
    {
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
        }
    }

}
