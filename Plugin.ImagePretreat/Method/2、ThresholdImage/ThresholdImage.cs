using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionCore;

namespace Plugin.ImagePretreat
{
    public class ThresholdImage : BaseMethod
    {
        public ThresholdImage(PluginFrmBase module):base(module)     
        {
            frm_Obj= module;
            m_Control = new ImageThreshold(this);
            m_Enhan = EnhanType.二值化;
        }

        /// <summary>
        /// 下限
        /// </summary>
        public int m_MinValue { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        public int m_MaxValue { get; set; }

        /// <summary>
        /// 执行
        /// </summary>
        public override void Execute()
        {
            HRegion out_region;
            VisionCore.VBA_Function.ThresholdImage(m_ImageExt, m_MinValue, m_MaxValue, out out_region);
            m_OutObj = out_region;
        }

    }
}
