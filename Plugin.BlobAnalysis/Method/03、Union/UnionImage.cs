using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// 合并
    /// </summary>
    [Serializable]
    public class UnionImage : BaseMethod
    {
        public UnionImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageUnion(this);
            m_blobMethod = BlobMethod.合并;
        }

        public override void SetRram()
        {
            try
            {
                HObject hObject;
                VisionCore.VBA_Function.Union(m_InHRegion, out hObject);
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
