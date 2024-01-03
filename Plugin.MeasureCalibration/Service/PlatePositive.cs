using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.MeasureCalibration
{
    /// <summary>
    /// 孔板效正图像模式/单图像标定
    /// </summary>
    public class PlatePositive
    {
        /// <summary>
        /// 输入图像
        /// </summary>
        private HImage _Himage;
        public HImage m_Himage
        {
            get { return _Himage; }
            set { _Himage = value; }
        }

        /// <summary>
        /// 搜索区域
        /// </summary>
        private HRegion _SearchRegion;
        public HRegion m_SearchRegion
        {
            get { return _SearchRegion; }
            set { _SearchRegion = value; }
        }

        /// <summary>
        /// 物理间距
        /// </summary>
        private double _Distance;
        public double m_Distance
        {
            get { return _Distance; }
            set { _Distance = value; }
        }

        /// <summary>
        /// 梯度阈值
        /// </summary>
        private int _Threshold;
        public int m_Threshold
        {
            get { return _Threshold; }
            set { _Threshold = value; }
        }

        /// <summary>
        /// 标定查询的Mark点生成的XLD
        /// </summary>
        public HXLDCont m_xldMark = new HXLDCont();

        /// <summary>
        /// 生成十字坐标系列
        /// </summary>
        public HXLDCont m_xldCross = new HXLDCont();

        /// <summary>
        /// 构造函数1
        /// </summary>
        public PlatePositive(HImage _Image, HRegion _region, double _distance, int _threshold)
        {
            _Himage = _Image;
            _SearchRegion = _region;
            _Distance = _distance;
            _Threshold = _threshold;
        }

        /// <summary>
        /// 孔板效正图像
        /// </summary>
        public void CalibrationStand()
        {
            //获得图像大小
            int hv_Width, hv_Height;
            m_Himage.GetImageSize(out hv_Width, out hv_Height);
            //截取图像
            HImage tmpIMG = m_Himage.ReduceDomain(m_SearchRegion);
            //剔除阈值外
            HXLDCont xld = tmpIMG.ThresholdSubPix(new HTuple(m_Threshold));
            HTuple Length = xld.LengthXld();



        }

    }
}
