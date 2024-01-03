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
    /// 连通区域
    /// </summary>
    [Serializable]
    public class ConnectionImage : BaseMethod
    {
        public ConnectionImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageConnection(this);
            m_blobMethod = BlobMethod.连通;
        }

        public override void SetRram()
        {
            try
            {
                HObject hObject;
                VisionCore.VBA_Function.Connection(m_InHRegion, out hObject);
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
