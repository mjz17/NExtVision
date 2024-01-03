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
    /// 高斯滤波
    /// </summary>
    [Serializable]
    public class GaussImage : BaseMethod
    {
        public GaussImage(PluginFrmBase module) : base(module)
        {
            frm_Obj = module;
            m_Control = new ImageGauss(this);
            m_Enhan = EnhanType.高斯滤波;
        }

        /// <summary>
        /// 
        /// </summary>
        public int m_Size { get; set; } = 3;

        /// <summary>
        /// 写入参数
        /// </summary>
        public override void WritepRram()
        {
            HImage out_Image;
            VisionCore.VBA_Function.GaussFilter(m_ImageExt, m_Size, out out_Image);
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
