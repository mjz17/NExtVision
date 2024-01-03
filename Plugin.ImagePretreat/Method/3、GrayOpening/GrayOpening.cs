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
    /// 灰度开运算
    /// </summary>
    [Serializable]
    public class GrayOpening : BaseMethod
    {
        public GrayOpening(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageGrayOpening(this);
            m_Enhan = EnhanType.灰度开运算;
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
            VisionCore.VBA_Function.GrayoOening(m_ImageExt, m_MaskWidth, m_MaskHeight, out out_Image);
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
