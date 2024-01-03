using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 图像参数
    /// </summary>
    [Serializable]
    public class ImageParam
    {
        /// <summary>
        /// X方向像素比率
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// Y方向像素比率
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// 标定板行坐标
        /// </summary>
        public HTuple BoardRow { get; set; }

        /// <summary>
        /// 标定板列坐标
        /// </summary>
        public HTuple BoardCol { get; set; }

        /// <summary>
        /// 标定板X坐标
        /// </summary>
        public HTuple BoardX { get; set; }

        /// <summary>
        /// 标定板Y坐标
        /// </summary>
        public HTuple BoardY { get; set; }

        /// <summary>
        /// 标定板矩阵
        /// </summary>
        public HHomMat2D m_homMat2D { get; set; }

    }
}
