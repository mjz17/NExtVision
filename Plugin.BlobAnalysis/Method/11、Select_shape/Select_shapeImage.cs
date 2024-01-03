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
    /// 特征筛选
    /// </summary>
    [Serializable]
    public class Select_shapeImage : BaseMethod
    {
        public Select_shapeImage(PluginFrmBase Obj) : base(Obj)
        {
            frm_Obj = Obj;
            m_Control = new ImageSelect_shape(this);
            m_blobMethod = BlobMethod.特征筛选;
        }

        public shapeModel m_shapeModel { get; set; }

        public List<shapeInfo> m_shapes = new List<shapeInfo>();

        /// <summary>
        /// 设置参数
        /// </summary>
        public override void SetRram()
        {
            HObject hObject = new HObject();
            try
            {
                //解析参数
                HTuple model = new HTuple();
                HTuple min = new HTuple();
                HTuple max = new HTuple();

                //添加数据
                for (int i = 0; i < m_shapes.Count; i++)
                {
                    model = new HTuple(m_shapes[i].m_shapeType.ToString());
                    min = new HTuple(m_shapes[i].m_min);
                    max = new HTuple(m_shapes[i].m_max);
                }

                VisionCore.VBA_Function.SelectShapeModel(m_InHRegion, model, m_shapeModel.ToString(), min, max, out hObject);
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

    [Serializable]
    public class shapeInfo
    {
        //特征
        public shapeType m_shapeType { get; set; }

        //最小
        public int m_min { get; set; } = 0;

        //最大
        public int m_max { get; set; } = 999999;
    }

    [Serializable]
    public enum shapeType
    {
        area,
    }

    [Serializable]
    public enum shapeModel
    {
        and,
        or,
    }

}
