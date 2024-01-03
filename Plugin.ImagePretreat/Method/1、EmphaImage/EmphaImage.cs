using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using VisionCore;

namespace Plugin.ImagePretreat
{
    /// <summary>
    /// 对比度
    /// </summary>
    [Serializable]
    public class EmphaImage : BaseMethod
    {

        public EmphaImage(PluginFrmBase module) : base(module)
        {
            frm_Obj = module;
            m_Control = new ImageEmpha(this);
            m_Enhan = EnhanType.对比度;
        }

        /// <summary>
        /// 输入的掩膜宽度
        /// </summary>
        public int m_MaskWidth { get; set; } = 3;

        /// <summary>
        /// 输入的掩膜高度
        /// </summary>
        public int m_MaskHeight { get; set; } = 3;

        /// <summary>
        /// 对比英子
        /// </summary>
        public double m_Factor { get; set; } = 0.3;

        /// <summary>
        /// 写入参数
        /// </summary>
        public override void WritepRram()
        {
            HImage out_Image;
            VisionCore.VBA_Function.SharpImage(m_ImageExt, m_MaskWidth, m_MaskHeight, m_Factor, out out_Image);
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
