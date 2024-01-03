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
    /// 空洞填充
    /// </summary>
    [Serializable]
    public class Fill_upImage : BaseMethod
    {
        public Fill_upImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageFill_up(this);
            m_blobMethod = BlobMethod.空洞填充;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        public override void SetRram()
        {
            try
            {
                HObject hObject;
                VisionCore.VBA_Function.Fill_UpRegion(m_InHRegion, out hObject);
                m_OutObj = hObject.Clone();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行文件
        /// </summary>
        public override void Execute()
        {
            frm_Obj.ExModule();
        }

    }
}
