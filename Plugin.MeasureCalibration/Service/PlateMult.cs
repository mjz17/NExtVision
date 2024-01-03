using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.MeasureCalibration
{
    /// <summary>
    /// 多相机标定/多相机孔板模式/带Hom
    /// </summary>
    public class PlateMult
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
        /// 标定板起始坐标X
        /// </summary>
        private int _OriginX;
        public int m_OriginX
        {
            get { return _OriginX; }
            set { _OriginX = value; }
        }

        /// <summary>
        /// 标定板起始坐标Y
        /// </summary>
        private int _OriginY;
        public int m_OriginY
        {
            get { return _OriginY; }
            set { _OriginY = value; }
        }

        /// <summary>
        /// Rms误差
        /// </summary>
        private double _Rms;
        public double m_Rms
        {
            get { return _Rms; }
            set { _Rms = value; }
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
        /// 构造函数
        /// </summary>
        /// <param name="_Image">输入图像</param>
        /// <param name="_region">截取区域</param>
        /// <param name="_distance">物理间距</param>
        /// <param name="_threshold">阈值</param>
        /// <param name="_originX">标定板起始点坐标X</param>
        /// <param name="_originY">标定板起始点坐标Y</param>
        public PlateMult(HImage _Image, HRegion _region, double _distance, int _threshold, int _originX, int _originY)
        {
            _Himage = _Image;
            _SearchRegion = _region;
            _Distance = _distance;
            _Threshold = _threshold;
            _OriginX = _originX;
            _OriginY = _originY;
        }

        /// <summary>
        /// 多相机标定
        /// </summary>
        public void CalibrationImage(ref ImageParam m_ImageParam)
        {
            HTuple Row;
            HTuple Col;
            HTuple PositionX = new HTuple();
            HTuple PositionY = new HTuple();

            HTuple tmpRow;
            HTuple tmpCol;
            HTuple tmpX;
            HTuple tmpY;

            double Scale = 1f;
            double ScaleX = 0f;
            double ScaleY = 0f;

            int seed = 0;

            try
            {
                //截取图像
                HImage tmpIMG = m_Himage.ReduceDomain(m_SearchRegion);
                //剔除阈值外
                HXLDCont xld = tmpIMG.ThresholdSubPix(new HTuple(m_Threshold));
                HTuple Length = xld.LengthXld();

                //排序
                int errCount = (int)(Length.Length * 0.8);
                List<double> length = new List<double>(Length.ToDArr());
                length.Sort();
                length.RemoveRange(0, errCount);

                HTuple Radius = new HTuple(), StartPhi = new HTuple(), EndPhi = new HTuple(), order = new HTuple();
                double avgLength = length.Average();

                //拟合圆
                xld = xld.SelectShapeXld("contlength", "and", 0.8 * avgLength, 1.5 * avgLength);
                xld = xld.SelectShapeXld("circularity", "and", 0.8, 1);
                xld.FitCircleContourXld("algebraic", -1, 0, 0, 3, 2, out Row, out Col, out Radius, out StartPhi, out EndPhi, out order);

                if (Row == null || Row.Length < 1)
                {
                    System.Windows.MessageBox.Show("标志点查找失败,请检查设置");
                    return;
                }

                //显示轮廓
                m_xldMark.GenCircleContourXld(Row, Col, Radius, StartPhi, EndPhi, order, 1.0);
                //生成十字坐标系
                m_xldCross.GenCrossContourXld(Row, Col, 10, 0);
                //圆的轮廓和十字坐标系组合
                m_xldMark = m_xldMark.ConcatObj(m_xldCross);

                //ROW,COL点排序
                //VBA_Function.SortPairs(ref Row, ref Col);

                //计算像素比  随机取一个点 到其他点的距离，取最小的作为点间距 ，不允许孤立的点
                Random rd = new Random();
                seed = rd.Next(Row.Length);
                seed = 0;
                HTuple chooseRow = HTuple.TupleGenConst(Row.Length - 1, Row[seed]);
                HTuple chooseCol = HTuple.TupleGenConst(Col.Length - 1, Col[seed]);
                tmpRow = Row.TupleRemove(seed);
                tmpCol = Col.TupleRemove(seed);

                //点到点的距离
                double distance = HMisc.DistancePp(chooseRow, chooseCol, tmpRow, tmpCol).TupleMin().D;

                //像素当量
                Scale = m_Distance / distance;
                ScaleX = Scale;
                ScaleY = Scale;

                //标定板ROW,COL坐标
                for (int i = 0; i < Row.Length; i++)
                {
                    if (i == seed)
                    {
                        PositionX = PositionX.TupleConcat(m_OriginX);
                        PositionY = PositionY.TupleConcat(m_OriginY);
                        continue;
                    }
                    int sRow = (int)((Row[i].D - Row[seed].D) / distance + ((Row[i].D - Row[seed].D) > 0 ? 1 : -1) * 0.5);//四舍五入
                    int sCol = (int)((Col[i].D - Col[seed].D) / distance + ((Col[i].D - Col[seed].D) > 0 ? 1 : -1) * 0.5);//四舍五入

                    PositionX = PositionX.TupleConcat(m_OriginX + sCol * m_Distance);
                    PositionY = PositionY.TupleConcat(m_OriginY + sRow * m_Distance);
                }

                HTuple chooseX = HTuple.TupleGenConst(PositionX.Length - 1, PositionX[seed]);
                HTuple chooseY = HTuple.TupleGenConst(PositionY.Length - 1, PositionY[seed]);
                tmpX = PositionX.TupleRemove(seed);
                tmpY = PositionY.TupleRemove(seed);

                //计算标定板角度偏差
                HHomMat2D hom = new HHomMat2D();
                hom.VectorToHomMat2d(Col, Row, PositionX, PositionY);

                double sx, sy, phi, theta, tx, ty;
                sx = hom.HomMat2dToAffinePar(out sy, out phi, out theta, out tx, out ty);

                m_ImageParam.ScaleX = ScaleX;
                m_ImageParam.ScaleY = ScaleY;
                m_ImageParam.BoardRow = Row;
                m_ImageParam.BoardCol = Col;
                m_ImageParam.BoardX = PositionX;
                m_ImageParam.BoardY = PositionY;
                m_ImageParam.m_homMat2D = hom;

                m_Rms = CalculateRMS(hom, Col, Row, PositionX, PositionY);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double CalculateRMS(HHomMat2D hom2d, HTuple x_Image, HTuple y_Image, HTuple x_Robot, HTuple y_Robot)
        {
            try
            {
                double count = 0;
                for (int i = 0; i < x_Image.Length; i++)
                {
                    double tempX, tempY;
                    tempX = hom2d.HomMat2dInvert().AffineTransPoint2d(x_Robot[i].D, y_Robot[i].D, out tempY);

                    double dis = HMisc.DistancePp(tempY, tempX, y_Image[i], x_Image[i]);
                    count = count + dis * dis;
                }

                double RMS = Math.Sqrt(count / x_Image.Length);

                return RMS;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

    }
}
