using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.ImagePretreat
{
    /// <summary>
    /// 灰度腐蚀
    /// </summary>
    [Serializable]
    public class GrayErosionr : BaseMethod
    {
        public GrayErosionr(PluginFrmBase module) : base(module)
        {
            frm_Obj = module;
            m_Control = new ImageGrayErosionr(this);
            m_Enhan = EnhanType.灰度腐蚀;
        }

        /// <summary>
        /// 输入的掩膜宽度
        /// </summary>
        public int m_MaskWidth { get; set; } = 5;

        /// <summary>
        /// 输入的掩膜高度
        /// </summary>
        public int m_MaskHeight { get; set; } = 5;

        /// <summary>
        /// 写入参数
        /// </summary>
        public override void WritepRram()
        {
            HImage out_Image;
            VisionCore.VBA_Function.GrayErosionr(m_ImageExt, m_MaskWidth, m_MaskHeight, out out_Image);
            m_OutObj = new PublicDefine.HImageExt(out_Image);
        }

        /// <summary>
        /// 执行
        /// </summary>
        public override void Execute()
        {
            frm_Obj.ExModule();
        }

    }
}
