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
    /// 孔板模式
    /// </summary>
    public class Plate
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
        /// 构造函数
        /// </summary>
        /// <param name="_Image"></param>
        /// <param name="_region"></param>
        public Plate(HImage _Image, HRegion _region, double _distance, int _threshold)
        {
            _Himage = _Image;
            _SearchRegion = _region;
            _Distance = _distance;
            _Threshold = _threshold;
        }

        /// <summary>
        /// 测量标定
        /// </summary>
        public void CalibrationMeasure(ref ImageParam m_ImageParam)
        {
            HTuple Row = new HTuple();
            HTuple Col = new HTuple();
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
                //移除XLD长度小于xld*0.8的参数
                length.RemoveRange(0, errCount);

                //拟合圆参数
                HTuple Radius = new HTuple();
                HTuple StartPhi = new HTuple();
                HTuple EndPhi = new HTuple();
                HTuple order = new HTuple();
                double avgLength = length.Average();
                //根据长度筛选
                xld = xld.SelectShapeXld("contlength", "and", 0.8 * avgLength, 1.5 * avgLength);
                //根据圆度筛选
                xld = xld.SelectShapeXld("circularity", "and", 0.8, 1);
                //拟合圆
                xld.FitCircleContourXld("algebraic", -1, 0, 0, 3, 2, out Row, out Col, out Radius, out StartPhi, out EndPhi, out order);
                //保护判断
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
                VBA_Function.SortPairs(ref Row, ref Col);
                //Length（in）：要生成特定元组的长度,Const（in）：初始化元组元素的常量
                HTuple chooseRow = HTuple.TupleGenConst(Row.Length - 1, Row[seed]);
                HTuple chooseCol = HTuple.TupleGenConst(Col.Length - 1, Col[seed]);

                tmpRow = Row.TupleRemove(seed);
                tmpCol = Col.TupleRemove(seed);

                //点到点的距离，取最小的
                double distance = HMisc.DistancePp(chooseRow, chooseCol, tmpRow, tmpCol).TupleMin().D;

                //像素当量
                Scale = m_Distance / distance;
                ScaleX = Scale;//X方向
                ScaleY = Scale;//Y方向

                for (int i = 0; i < Row.Length; i++)
                {
                    if (i == seed)
                    {
                        //连接HTuple
                        PositionX = PositionX.TupleConcat(0f);
                        PositionY = PositionY.TupleConcat(0f);
                        continue;
                    }
                    int sRow = (int)((Row[i].D - Row[seed].D) / distance + ((Row[i].D - Row[seed].D) > 0 ? 1 : -1) * 0.5);//四舍五入
                    int sCol = (int)((Col[i].D - Col[seed].D) / distance + ((Col[i].D - Col[seed].D) > 0 ? 1 : -1) * 0.5);//四舍五入
                    PositionX = PositionX.TupleConcat(sCol * m_Distance);
                    PositionY = PositionY.TupleConcat(sRow * m_Distance);
                }

                m_ImageParam.ScaleX = ScaleX;
                m_ImageParam.ScaleY = ScaleY;
                m_ImageParam.BoardRow = Row;
                m_ImageParam.BoardCol = Col;
                m_ImageParam.BoardX = PositionX;
                m_ImageParam.BoardY = PositionY;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
