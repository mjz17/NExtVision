using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DefineImgRoI;
using HalconDotNet;
using PublicDefine;

namespace VisionCore
{
    public class VBA_Function
    {
        /// <summary>
        /// 设置原点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="Phi"></param>
        /// <returns></returns>
        public static HHomMat2D setOrig(double x, double y, double Phi)
        {
            HHomMat2D hom = new HHomMat2D();

            try
            {
                hom = hom.HomMat2dRotateLocal(-Phi);
                hom = hom.HomMat2dTranslateLocal(-x, -y);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hom;
        }

        /// <summary>
        /// 最小二乘法拟合圆
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="circle"></param>
        /// <returns></returns>
        public static bool fitCircle(List<double> rows, List<double> cols, out Circle_INFO circle)
        {
            circle = new Circle_INFO();
            if (cols.Count < 3)
            {
                return false;
            }

            double sum_x = 0.0f, sum_y = 0.0f;
            double sum_x2 = 0.0f, sum_y2 = 0.0f;
            double sum_x3 = 0.0f, sum_y3 = 0.0f;
            double sum_xy = 0.0f, sum_x1y2 = 0.0f, sum_x2y1 = 0.0f;

            int N = cols.Count;
            for (int i = 0; i < N; i++)
            {
                double x = rows[i];
                double y = cols[i];
                double x2 = x * x;
                double y2 = y * y;
                sum_x += x;
                sum_y += y;
                sum_x2 += x2;
                sum_y2 += y2;
                sum_x3 += x2 * x;
                sum_y3 += y2 * y;
                sum_xy += x * y;
                sum_x1y2 += x * y2;
                sum_x2y1 += x2 * y;
            }

            double C, D, E, G, H;
            double a, b, c;

            C = N * sum_x2 - sum_x * sum_x;
            D = N * sum_xy - sum_x * sum_y;
            E = N * sum_x3 + N * sum_x1y2 - (sum_x2 + sum_y2) * sum_x;
            G = N * sum_y2 - sum_y * sum_y;
            H = N * sum_x2y1 + N * sum_y3 - (sum_x2 + sum_y2) * sum_y;
            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * sum_x + b * sum_y + sum_x2 + sum_y2) / N;

            circle.CenterY = a / (-2);
            circle.CenterX = b / (-2);
            circle.Radius = Math.Sqrt(a * a + b * b - 4 * c) / 2;
            return true;

        }

        /// <summary>
        /// /使用halcon的拟合直线算法,比fitLine更准确,因为有其自己的剔除异常点算法
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="line"></param>
        /// <returns>结果直线</returns>
        public static bool fitLineByH(List<Double> rows, List<Double> cols, out Line_INFO line)
        {
            line = new Line_INFO();
            try
            {
                SortPairs(ref rows, ref cols);
                double rowBegin, colBegin, rowEnd, colEnd, nr, nc, dist;
                HXLDCont lineXLD = new HXLDCont(new HTuple(rows.ToArray()), new HTuple(cols.ToArray()));
                lineXLD.FitLineContourXld("tukey", -1, 0, 5, 2, out rowBegin, out colBegin, out rowEnd, out colEnd, out nr, out nc, out dist);//tukey剔除算法为halcon推荐算法
                line = new Line_INFO(rowBegin, colBegin, rowEnd, colEnd);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 点排序
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public static void SortPairs(ref List<double> rows, ref List<double> cols)
        {
            HTuple hv_T1 = new HTuple(rows.ToArray());
            HTuple hv_T2 = new HTuple(cols.ToArray());
            //相同的方法 直接使用htuple返回结果
            SortPairs(ref hv_T1, ref hv_T2);
            rows = hv_T1.ToDArr().ToList();
            cols = hv_T2.ToDArr().ToList();
            return;

            //HTuple hv_Sorted1 = new HTuple();
            //HTuple hv_Sorted2 = new HTuple();
            //HTuple hv_SortMode = new HTuple();
            //HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            //if ((rows.Max() - rows.Min()) > (cols.Max() - cols.Min()))
            //    hv_SortMode = new HTuple("1");
            //else
            //    hv_SortMode = new HTuple("2");
            //if ((int)((new HTuple(hv_SortMode.TupleEqual("1"))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
            //    1)))) != 0)
            //{
            //    HOperatorSet.TupleSortIndex(hv_T1, out hv_Indices1);
            //    hv_Sorted1 = hv_T1.TupleSelect(hv_Indices1);
            //    hv_Sorted2 = hv_T2.TupleSelect(hv_Indices1);
            //}
            //else if ((int)((new HTuple((new HTuple(hv_SortMode.TupleEqual("column"))).TupleOr(
            //    new HTuple(hv_SortMode.TupleEqual("2"))))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
            //    2)))) != 0)
            //{
            //    HOperatorSet.TupleSortIndex(hv_T2, out hv_Indices2);
            //    hv_Sorted1 = hv_T1.TupleSelect(hv_Indices2);
            //    hv_Sorted2 = hv_T2.TupleSelect(hv_Indices2);
            //}
            //rows = hv_Sorted1.ToDArr().ToList();
            //cols = hv_Sorted2.ToDArr().ToList();
        }

        /// <summary>
        /// 点排序
        /// </summary>
        /// <param name="hv_T1"></param>
        /// <param name="hv_T2"></param>
        public static void SortPairs(ref HTuple hv_T1, ref HTuple hv_T2)
        {
            HTuple hv_Sorted1 = new HTuple();
            HTuple hv_Sorted2 = new HTuple();
            HTuple hv_SortMode = new HTuple();
            HTuple hv_Indices1 = new HTuple(), hv_Indices2 = new HTuple();
            if ((hv_T1.TupleMax().D - hv_T1.TupleMin().D) > (hv_T2.TupleMax().D - hv_T2.TupleMin().D))
                hv_SortMode = new HTuple("1");
            else
                hv_SortMode = new HTuple("2");
            if ((int)((new HTuple(hv_SortMode.TupleEqual("1"))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                1)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T1, out hv_Indices1);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices1);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices1);
            }
            else if ((int)((new HTuple((new HTuple(hv_SortMode.TupleEqual("column"))).TupleOr(
                new HTuple(hv_SortMode.TupleEqual("2"))))).TupleOr(new HTuple(hv_SortMode.TupleEqual(
                2)))) != 0)
            {
                HOperatorSet.TupleSortIndex(hv_T2, out hv_Indices2);
                hv_Sorted1 = hv_T1.TupleSelect(hv_Indices2);
                hv_Sorted2 = hv_T2.TupleSelect(hv_Indices2);
            }
            hv_T1 = hv_Sorted1;
            hv_T2 = hv_Sorted2;
        }


        /// <summary>
        /// 点到点的距离
        /// </summary>
        /// <param name="rows1">点1的行序列</param>
        /// <param name="cols1">点1的列序列</param>
        /// <param name="rows2">点2的行序列</param>
        /// <param name="cols2">点2的列序列</param>
        /// <returns>距离结果</returns>
        public static double DistancePP(double rows1, double cols1, double rows2, double cols2)
        {
            double dis = 0.0;
            try
            {
                dis = HMisc.DistancePp(rows1, cols1, rows2, cols2);
            }
            catch (System.Exception ex)
            {

            }
            return dis;
        }

        /// <summary>
        /// 两条直线交点
        /// </summary>
        /// <param name="line1">直线1</param>
        /// <param name="line2">直线2</param>
        /// <param name="row">交点row</param>
        /// <param name="col">交点col</param>
        /// <param name="isParallel">平行1，不平行0</param>
        public static void IntersectionLl(Line_INFO line1, Line_INFO line2, out Double row, out Double col, out int isParallel)
        {
            row = 0.0;
            col = 0.0;
            isParallel = 0;
            try
            {
                HMisc.IntersectionLl(line1.StartY, line1.StartX, line1.EndY, line1.EndX,
                    line2.StartY, line2.StartX, line2.EndY, line2.EndX, out row, out col, out isParallel);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 求点到线的垂足
        /// </summary>
        /// <param name="inRow">输入点row</param>
        /// <param name="inCol">输入点col</param>
        /// <param name="srcLine"></param>
        /// <param name="outY"></param>
        /// <param name="outX"></param>
        public static void DroopFootLine(Double inRow, Double inCol, Line_INFO srcLine, out Double outY, out Double outX)
        {
            HMisc.ProjectionPl(inRow, inCol, srcLine.StartX, srcLine.StartY, srcLine.EndX, srcLine.EndY, out outY, out outX);
        }

        /// <summary>
        /// 计算点到线的距离
        /// </summary>
        /// <param name="rows">点行序列</param>
        /// <param name="cols">点列序列</param>
        /// <param name="Line1">线</param>
        /// <param name="dis">返回距离</param>
        public static void DistancePL(List<Double> rows, List<Double> cols, Line_INFO Line1, out List<Double> dis)
        {
            dis = new List<Double>() { -999.999 };
            try
            {
                HTuple disT = HMisc.DistancePl(new HTuple(rows.ToArray()), new HTuple(cols.ToArray()), new HTuple(Line1.StartY), new HTuple(Line1.StartX), new HTuple(Line1.EndY), new HTuple(Line1.EndX));
                dis = disT.ToDArr().ToList();
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// 计算两条线的距离
        /// </summary>
        /// <param name="line1">输入线1</param>
        /// <param name="line2">输入线2</param>
        /// <param name="MinDis">最小距离</param>
        /// <param name="MaxDis">最大距离</param>
        /// <param name="Phi">线线角度</param>
        public static void DistanceLL(Line_INFO line1, Line_INFO line2, out int MinDis, out int MaxDis, out double Phi)
        {
            MinDis = 0;
            MaxDis = 0;
            Phi = 0;

            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 计算两直线夹角
        /// </summary>
        /// <param name="Line1"></param>
        /// <param name="Line2"></param>
        /// <returns>返回弧度值</returns>
        public static double CalAngleL2L(Line_INFO Line1, Line_INFO Line2)
        {
            HTuple angle = new HTuple();
            HOperatorSet.AngleLl(
                new HTuple(Line1.StartY),
                new HTuple(Line1.StartX),
                new HTuple(Line1.EndY),
                new HTuple(Line1.EndX),
                new HTuple(Line2.StartY),
                new HTuple(Line2.StartX),
                new HTuple(Line2.EndY),
                new HTuple(Line2.EndX),
                out angle);


            return angle[0].D;
        }

        /// <summary>
        /// 根据终点X,Y,Phi,获取直线
        /// </summary>
        /// <param name="Row">中点X</param>
        /// <param name="Col">中点Y</param>
        /// <param name="Phi">角度</param>
        /// <param name="line">输出直线</param>
        /// <returns></returns>
        public static void CreateLine(double Row, double Col, double Phi, out Line_INFO line)
        {
            double RowStart;
            double ColStart;
            double RowEnd;
            double ColEnd;

            double Angle = Phi;//角度转浮点

            try
            {

                //直线起点
                RowStart = Row - Math.Cos(Angle + 1.5708) * 100;
                ColStart = Col - Math.Sin(Angle + 1.5708) * 100;

                //直线终点
                RowEnd = Row - Math.Cos(Angle - 1.5708) * 100;
                ColEnd = Col - Math.Sin(Angle - 1.5708) * 100;

                line = new Line_INFO(ColStart, RowStart, ColEnd, RowEnd);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图像增加（对比度）
        /// </summary>
        /// <param name="maskWidth"></param>
        /// <param name="maskHeight"></param>
        /// <param name="factor"></param>
        /// <param name="In_Image"></param>
        public static void SharpImage(HImageExt In_Image, int maskWidth, int maskHeight, double factor, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.Emphasize(maskWidth, maskHeight, factor);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 灰度开运算
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="maskWidth">宽度</param>
        /// <param name="maskHeight">高度</param>
        /// <param name="Out_Image">输出图像</param>
        public static void GrayoOening(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.GrayOpeningRect(maskWidth, maskHeight);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 灰度闭运算
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="maskWidth">宽度</param>
        /// <param name="maskHeight">高度</param>
        /// <param name="Out_Image">输出图像</param>
        public static void GrayClosing(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.GrayClosingRect(maskWidth, maskHeight);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 图像反色
        /// </summary>
        /// <param name="maskWidth"></param>
        /// <param name="maskHeight"></param>
        /// <param name="factor"></param>
        /// <param name="In_Image"></param>
        public static void InvertImage(HImageExt In_Image, out HImage Out_Image)
        {
            try
            {
                HObject hObject;

                HOperatorSet.InvertImage(In_Image, out hObject);

                Out_Image = new HImage(hObject);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 二值化
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="minValue">下限</param>
        /// <param name="maxValue">上限</param>
        /// <param name="region">区域</param>
        public static void ThresholdImage(HImage In_Image, double minValue, double maxValue, out HRegion region)
        {
            try
            {
                region = In_Image.Threshold(minValue, maxValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自动二值化
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="lightDark">属性</param>
        /// <param name="region">输出Region</param>
        /// <param name="number">输出阈值</param>
        public static void BinaryThresholdImage(HImage In_Image, string lightDark, out HRegion region, out int number)
        {
            try
            {
                region = In_Image.BinaryThreshold("max_separability", lightDark, out number);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 均值滤波
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="maskWidth"></param>
        /// <param name="maskHeight"></param>
        /// <param name="Out_Image"></param>
        public static void MeanImage(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.MeanImage(maskWidth, maskHeight);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///// <summary>
        ///// 中值滤波
        ///// </summary>
        ///// <param name="In_Image"></param>
        ///// <param name="maskWidth"></param>
        ///// <param name="maskHeight"></param>
        ///// <param name="Out_Image"></param>
        //public static void MedianImage(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        //{
        //    try
        //    {
        //        Out_Image = In_Image.MedianImage(maskWidth, maskHeight);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 高斯滤波
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="Size">尺寸</param>
        /// <param name="Out_Image">输出图像</param>
        public static void GaussFilter(HImageExt In_Image, int Size, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.GaussFilter(Size);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 灰度腐蚀
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="maskWidth">宽度</param>
        /// <param name="maskHeight">高度</param>
        /// <param name="Out_Image">输出图像</param>
        public static void GrayErosionr(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.GrayErosionRect(maskWidth, maskHeight);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 灰度膨胀
        /// </summary>
        /// <param name="In_Image">输入图像</param>
        /// <param name="maskWidth">宽度</param>
        /// <param name="maskHeight">高度</param>
        /// <param name="Out_Image">输出图像</param>
        public static void GrayDilation(HImageExt In_Image, int maskWidth, int maskHeight, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.GrayDilationRect(maskWidth, maskHeight);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 连通
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void Connection(HRegion In_hRegion, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.Connection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void Union(HRegion In_hRegion, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.Union1();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 空洞填充
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void Fill_UpRegion(HRegion In_hRegion, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.FillUp();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 闭运算(结构元素矩形)
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void ClosingRectRegion(HRegion In_hRegion, int width, int heigth, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.ClosingRectangle1(width, heigth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 闭运算(结构元素圆形)
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void ClosingCircleRegion(HRegion In_hRegion, double radius, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.ClosingCircle(radius);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 开运算(结构元素矩形)
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void OpeningRectRegion(HRegion In_hRegion, int width, int heigth, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.OpeningRectangle1(width, heigth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 开运算(结构元素圆形)
        /// </summary>
        /// <param name="In_Image"></param>
        /// <param name="Out_hRegion"></param>
        public static void OpeningCircleRegion(HRegion In_hRegion, double radius, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.OpeningCircle(radius);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 膨胀（圆形结构元素）
        /// </summary>
        /// <param name="In_hRegion"></param>
        /// <param name="radius"></param>
        /// <param name="Out_hRegion"></param>
        public static void DilationCircleRegion(HRegion In_hRegion, double radius, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.DilationCircle(radius);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 膨胀（矩形形结构元素）
        /// </summary>
        /// <param name="In_hRegion"></param>
        /// <param name="radius"></param>
        /// <param name="Out_hRegion"></param>
        public static void DilationRectRegion(HRegion In_hRegion, int width, int heigth, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.DilationRectangle1(width, heigth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 腐蚀（圆形结构元素）
        /// </summary>
        /// <param name="In_hRegion"></param>
        /// <param name="radius"></param>
        /// <param name="Out_hRegion"></param>
        public static void ErosionCircleRegion(HRegion In_hRegion, double radius, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.ErosionCircle(radius);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 腐蚀（矩形形结构元素）
        /// </summary>
        /// <param name="In_hRegion"></param>
        /// <param name="radius"></param>
        /// <param name="Out_hRegion"></param>
        public static void ErosionRectRegion(HRegion In_hRegion, int width, int heigth, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.ErosionRectangle1(width, heigth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 特征筛选
        /// </summary>
        /// <param name="In_hRegion">输入Region</param>
        /// <param name="ShapeType"></param>
        /// <param name="shapeModel"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="Out_hRegion"></param>
        public static void SelectShapeModel(HRegion In_hRegion, HTuple ShapeType, string shapeModel, HTuple min, HTuple max, out HObject Out_hRegion)
        {
            try
            {
                Out_hRegion = In_hRegion.SelectShape(ShapeType, shapeModel, min, max);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取Region的参数
        /// </summary>
        /// <param name="In_hRegion"></param>
        /// <param name="Features"></param>
        /// <param name="hRegionInfo"></param>
        public static void GenRegionFeatures(HRegion In_hRegion, string Features, out double hRegionInfo)
        {
            try
            {
                hRegionInfo = In_hRegion.RegionFeatures(Features);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void RegionRedumain(HImage In_Image, HRegion In_hRegion, out HImage Out_Image)
        {
            try
            {
                Out_Image = In_Image.ReduceDomain(In_hRegion);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public enum StructElement
        {
            圆行,
            矩形,
        }


    }
}
