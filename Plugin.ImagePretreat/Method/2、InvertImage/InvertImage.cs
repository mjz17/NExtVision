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
    /// 反色
    /// </summary>
    [Serializable]
    internal class InvertImage : BaseMethod
    {
        public InvertImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            //m_Control = new ImageEmpha(this);
            m_Enhan = EnhanType.反色;
        }

        /// <summary>
        /// 写入参数
        /// </summary>
        public override void WritepRram()
        {
            HImage out_Image;
            VisionCore.VBA_Function.InvertImage(m_ImageExt, out out_Image);
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
