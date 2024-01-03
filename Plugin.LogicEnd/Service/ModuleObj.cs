using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.LogicEnd
{
    [Category("逻辑工具")]
    [DisplayName("结束")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
        }
    }
}
