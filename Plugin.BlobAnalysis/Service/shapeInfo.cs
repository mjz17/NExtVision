using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// 特征筛选
    /// </summary>
    [Serializable]
    public class SelectedShapeInfo
    {
        /// <summary>
        /// 特征
        /// </summary>
        private SelectedshapeType _shapeType;
        public SelectedshapeType m_shapeType
        {
            get { return _shapeType; }
            set { _shapeType = value; }
        }

        /// <summary>
        /// 最小
        /// </summary>
        private int _min = 0;
        public int m_min
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// 最大
        /// </summary>
        private int _max = 999999;
        public int m_max
        {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool _EnableOrnot;
        public bool m_EnableOrnot
        {
            get { return _EnableOrnot; }
            set { _EnableOrnot = value; }
        }

        /// <summary>
        /// 结果
        /// </summary>
        private bool _result = false;
        public bool m_result
        {
            get { return _result; }
            set { _result = value; }
        }

    }

    [Serializable]
    public enum SelectedshapeType
    {
        总面积,
        各个面积,
        个数,

    }

}
