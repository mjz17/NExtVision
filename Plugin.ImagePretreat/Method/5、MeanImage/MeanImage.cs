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
    /// 均值滤波
    /// </summary>
    [Serializable]
    public class MeanImage : BaseMethod
    {
        public MeanImage(PluginFrmBase module) : base(module)
        {
            frm_Obj = module;
            m_Control = new ImageMean(this);
            m_Enhan = EnhanType.均值滤波;
        }

        /// <summary>
        /// 输入的掩膜宽度
        /// </summary>
        public int m_MaskWidth { get; set; } = 1;

        /// <summary>
        /// 输入的掩膜高度
        /// </summary>
        public int m_MaskHeight { get; set; } = 1;

        /// <summary>
        /// 写入参数
        /// </summary>
        public override void WritepRram()
        {
            HImage out_Image;
            VisionCore.VBA_Function.MeanImage(m_ImageExt, m_MaskWidth, m_MaskHeight, out out_Image);
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
