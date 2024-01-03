using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Plugin.NpointsCalibration
{
    public class NpointCommon
    {

        /// <summary>
        /// 普通9点标定
        /// </summary>
        /// <param name="PointInfo"></param>
        public HHomMat2D NpointTran(List<PointCoord> PointInfo)
        {
            try
            {
                //9点标定
                List<PointCoord> PointTran = PointInfo.FindAll(c => c.m_Index < 10);

                HHomMat2D m_homMat2D = new HHomMat2D();

                double[] row = PointTran.AsEnumerable().Select(r => r.m_ImageRow).ToArray();
                double[] col = PointTran.AsEnumerable().Select(r => r.m_ImageCol).ToArray();
                double[] x = PointTran.AsEnumerable().Select(r => r.m_Mach_x).ToArray();
                double[] y = PointTran.AsEnumerable().Select(r => r.m_Mach_y).ToArray();

                m_homMat2D.VectorToHomMat2d(row, col, x, y);
                return m_homMat2D;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 9点标定加旋转中心
        /// </summary>
        /// <param name="PointInfo"></param>
        public HHomMat2D NpointTranRotation(List<PointCoord> PointInfo, out double R_Row, out double R_Col, out double R_W_Row, out double R_W_Col)
        {
            try
            {
                HHomMat2D m_homMat2D = new HHomMat2D();
                R_Row = new double();
                R_Col = new double();

                R_W_Row = new double();
                R_W_Col = new double();

                HTuple outRow;
                HTuple outCol;
                HTuple outRadius;
                HTuple outStartPhi;
                HTuple outEndPhi;
                HTuple outPointOrder;

                //9点标定
                List<PointCoord> PointTran = PointInfo.FindAll(c => c.m_Index < 10);

                double[] PointTranrow = PointTran.AsEnumerable().Select(r => r.m_ImageRow).ToArray();
                double[] PointTrancol = PointTran.AsEnumerable().Select(r => r.m_ImageCol).ToArray();
                double[] PointTranx = PointTran.AsEnumerable().Select(r => r.m_Mach_x).ToArray();
                double[] PointTrany = PointTran.AsEnumerable().Select(r => r.m_Mach_y).ToArray();
                m_homMat2D.VectorToHomMat2d(PointTranrow, PointTrancol, PointTranx, PointTrany);

                //旋转中心
                List<PointCoord> PointRotation = PointInfo.FindAll(c => c.m_Index > 9);
                double[] RotationRow = PointRotation.AsEnumerable().Select(r => r.m_ImageRow).ToArray();
                double[] RotationCol = PointRotation.AsEnumerable().Select(r => r.m_ImageCol).ToArray();

                HXLDCont xLDCont = new HXLDCont();
                xLDCont.GenContourPolygonXld(RotationRow, RotationCol);
                xLDCont.FitCircleContourXld("algebraic", -1, 0, 0, 3, 2, out outRow, out outCol, out outRadius, out outStartPhi, out outEndPhi, out outPointOrder);

                //旋转中心图像坐标
                R_Row = outRow;
                R_Col = outCol;

                //求取旋转中心机械坐标
                //旋转中心
                List<PointCoord> PointRotation_W = PointInfo.FindAll(c => c.m_Index > 9);
                double[] RotationRow_W = PointRotation.AsEnumerable().Select(r => r.m_Mach_x).ToArray();
                double[] RotationCol_W = PointRotation.AsEnumerable().Select(r => r.m_Mach_y).ToArray();

                //旋转中心机械坐标
                R_W_Row = RotationRow_W[0];
                R_W_Col = RotationCol_W[0];

                return m_homMat2D;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 两点法求取旋转中心
        /// </summary>
        /// <param name="Imagex1">图像坐标1X</param>
        /// <param name="Imagex2">图像坐标1Y</param>
        /// <param name="Imagey1">图像坐标2X</param>
        /// <param name="Imagey2">图像坐标2Y</param>
        /// <param name="Rad">输入弧度</param>
        /// <param name="x0">输出旋转中心X</param>
        /// <param name="y0">输出旋转中心Y</param>
        public void TwoPointMethod(double Imagex1, double Imagex2, double Imagey1, double Imagey2, double Rad, out double x0, out double y0)
        {
            double d = Math.Sqrt((Imagex2 - Imagex1) * (Imagex2 - Imagex1) + (Imagey2 - Imagey1) * (Imagey2 - Imagey1));
            double r = d / 2 / Math.Sin(Rad / 2);//Rad为弧度
            double xx = (1 - r / d) * Imagex1 + (r / d) * Imagex2;
            double yy = (1 - r / d) * Imagey1 + (r / d) * Imagey2;
            //弧度转角度
            //double rr = (180 / Math.PI) * Rad;
            x0 = Math.Cos(Math.PI / 2 - Rad / 2) * (xx - Imagex1) - Math.Sin(Math.PI / 2 - Rad / 2) * (yy - Imagey1) + Imagex1;
            y0 = Math.Cos(Math.PI / 2 - Rad / 2) * (yy - Imagey1) + Math.Sin(Math.PI / 2 - Rad / 2) * (xx - Imagex1) + Imagey1;
        }

        public void GetCenterPos(double x1, double y1, double x2, double y2, double x3, double y3, ref double X, ref double Y, ref double R)
        {
            double a = 0;
            double b = 0;
            double c = 0;
            double g = 0;
            double e = 0;
            double f = 0;
            e = 2 * (x2 - x1);
            f = 2 * (y2 - y1);
            g = x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1;
            a = 2 * (x3 - x2);
            b = 2 * (y3 - y2);
            c = x3 * x3 - x2 * x2 + y3 * y3 - y2 * y2;
            X = (g * b - c * f) / (e * b - a * f);
            Y = (a * g - c * e) / (a * f - b * e);
            R = Math.Sqrt((X - x1) * (X - x1) + (Y - y1) * (Y - y1));
        }


    }

    [Serializable]
    public class PointCoord
    {

        public int m_Index;

        /// <summary>
        /// 图像Row
        /// </summary>
        private double _ImageRow;

        public double m_ImageRow
        {
            get { return _ImageRow; }
            set { _ImageRow = value; }
        }

        /// <summary>
        /// 图像Col
        /// </summary>
        private double _ImageCol;

        public double m_ImageCol
        {
            get { return _ImageCol; }
            set { _ImageCol = value; }
        }

        /// <summary>
        /// 机械X
        /// </summary>
        private double _Mach_x;

        public double m_Mach_x
        {
            get { return _Mach_x; }
            set { _Mach_x = value; }
        }

        /// <summary>
        /// 机械Y
        /// </summary>
        private double _Mach_y;

        public double m_Mach_y
        {
            get { return _Mach_y; }
            set { _Mach_y = value; }
        }

        /// <summary>
        /// 世界坐标X
        /// </summary>
        [NonSerialized]
        private double _Word_X;

        public double m_Word_X
        {
            get { return _Word_X; }
            set { _Word_X = value; }
        }

        /// <summary>
        /// 世界坐标Y
        /// </summary>
        [NonSerialized]
        private double _Word_Y;

        public double m_Word_Y
        {
            get { return _Word_Y; }
            set { _Word_Y = value; }
        }

        /// <summary>
        /// 世界坐标X-机械坐标
        /// </summary>
        [NonSerialized]
        private double _WordMach_X;

        public double m_WordMach_X
        {
            get { return _WordMach_X; }
            set { _WordMach_X = value; }
        }

        /// <summary>
        /// 世界坐标Y-机械坐标
        /// </summary>
        [NonSerialized]
        private double _WordMach_Y;

        public double m_WordMach_Y
        {
            get { return _WordMach_Y; }
            set { _WordMach_Y = value; }
        }

    }

}
