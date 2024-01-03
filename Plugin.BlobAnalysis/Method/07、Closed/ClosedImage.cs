using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using static VisionCore.VBA_Function;

namespace Plugin.BlobAnalysis
{

    /// <summary>
    /// 闭运算
    /// </summary>
    [Serializable]
    public class ClosedImage : BaseMethod
    {
        public ClosedImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageClosed(this);
            m_blobMethod = BlobMethod.闭运算;
        }

        public VisionCore.VBA_Function.StructElement element { get; set; } = StructElement.圆行;

        public int m_Radius { get; set; } = 5;

        public int m_Width { get; set; } = 5;

        public int m_Height { get; set; } = 5;

        public override void SetRram()
        {
            try
            {
                HObject hObject = new HObject();

                switch (element)
                {
                    case StructElement.圆行:
                        VisionCore.VBA_Function.ClosingCircleRegion(m_InHRegion, m_Radius, out hObject);
                        break;
                    case StructElement.矩形:
                        VisionCore.VBA_Function.ClosingRectRegion(m_InHRegion, m_Width, m_Height, out hObject);
                        break;
                    default:
                        break;
                }

                m_OutObj = hObject.Clone();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Execute()
        {
            frm_Obj.ExModule();
        }

    }
}
