using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefineImgRoI;
using HalconDotNet;
using PublicDefine;
using Common;

namespace VisionCore
{
    /// <summary>
    /// 封装算子
    /// </summary>
    public class SysVisionCore
    {
        /// <summary>
        /// 图像显示方式
        /// </summary>
        /// <returns></returns>
        public static HImage AffineImage(HImage img, IMG_ADJUST iMG)
        {
            HImage tempImg = new HImage();

            switch (iMG)
            {
                case IMG_ADJUST.None:
                    tempImg = img.Clone();
                    break;
                case IMG_ADJUST.垂直镜像:
                    tempImg = img.MirrorImage("row");
                    break;
                case IMG_ADJUST.水平镜像:
                    tempImg = img.MirrorImage("column");
                    break;
                case IMG_ADJUST.顺时针90度:
                    tempImg = img.RotateImage(270.0, "nearest_neighbor");
                    break;
                case IMG_ADJUST.逆时针90度:
                    tempImg = img.RotateImage(90.0, "nearest_neighbor");
                    break;
                case IMG_ADJUST.旋转180度:
                    tempImg = img.RotateImage(180.0, "nearest_neighbor");
                    break;
                default:
                    break;
            }
            return tempImg;
        }

        /// <summary>
        /// 创建模板匹配模板
        /// </summary>
        /// <param name="_Type">模板类型</param>
        /// <param name="_Templateimg">输入图像</param>
        /// <param name="_Model">句柄</param>
        /// <param name="matchPram">模板参数</param>
        public static void CreateMatchModel(ModelType _Type, HImage _Templateimg, ref HHandle _Model, CreateMatchPram matchPram)
        {
            try
            {
                if (_Templateimg.IsInitialized())
                {
                    if (_Type == ModelType.形状模板)
                    {
                        ((HShapeModel)_Model).CreateScaledShapeModel(
                            _Templateimg,
                            (HTuple)matchPram.numLevels,
                            Math.Round(matchPram.angleStart * Math.PI / 180, 3),
                            Math.Round((matchPram.angleExtent - matchPram.angleStart) * Math.PI / 180, 3),
                            (HTuple)matchPram.angleStep,
                            matchPram.scaleMin,
                            matchPram.scaleMax,
                            (HTuple)matchPram.scaleStep,
                            matchPram.optimization,
                            matchPram.metric,
                            (HTuple)matchPram.contrast,
                            matchPram.minContrast);
                    }
                    else if (_Type == ModelType.灰度模板)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 模板查询
        /// </summary>
        /// <param name="_Type"></param>
        /// <param name="_image"></param>
        /// <param name="_Model"></param>
        /// <param name="findMatch"></param>
        /// <param name="outParm"></param>
        public static void FindMatchModel(ModelType _Type, HImage _image, HHandle _Model, FindMatchPram findMatch, out FindMatchPram outParm)
        {
            outParm = new FindMatchPram();

            try
            {
                HTuple out_row = 0;
                HTuple out_col = 0;
                HTuple out_Phi = 0;
                HTuple out_scale = 0;//比例
                HTuple out_score = 0;//得分

                if (_image.IsInitialized())
                {
                    if (_Type == ModelType.形状模板)
                    {
                        ((HShapeModel)_Model).FindScaledShapeModel(
                           _image,
                         Math.Round(findMatch.angleStart * Math.PI / 180, 3),
                            Math.Round((findMatch.angleExtent - findMatch.angleStart) * Math.PI / 180, 3),
                           findMatch.scaleMin,
                           findMatch.scaleMax,
                           (HTuple)findMatch.minScore,
                           findMatch.numMatches,
                           findMatch.maxOverlap,
                           findMatch.subPixel,
                           findMatch.numLevels,
                           findMatch.greediness,
                           out out_row, out out_col, out out_Phi, out out_scale, out out_score);

                        if (out_score.Length > 0)
                        {
                            outParm.row = out_row;
                            outParm.column = out_col;
                            outParm.angle = out_Phi;
                            outParm.scale = out_scale;
                            outParm.score = out_score;
                        }

                    }
                    else if (_Type == ModelType.灰度模板)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 创建二维码模型
        /// </summary>
        /// <param name="_image">输入图像</param>
        /// <param name="_Model">模型</param>
        /// <param name="_Roi">区域</param>
        /// <param name="genParamName">参数名称</param>
        /// <param name="genParamValue">参数值</param>
        public static void CreateCode2D(HImage _image, ref HHandle _Model, ROI _Roi, string genParamName,
            string genParamValue)
        {
            try
            {
                if (_image.IsInitialized())
                {
                    HRegion _region = _Roi.GenRegion();
                    if (_region.IsInitialized())
                    {
                        _image = _image.ReduceDomain(_region);
                    }

                    if (!_Model.IsInitialized())
                    {
                        ((HDataCode2D)_Model).CreateDataCode2dModel("Data Matrix ECC 200", genParamName, genParamValue);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 查询二维码
        /// </summary>
        /// <param name="_image">输入图像</param>
        /// <param name="_Model">模板</param>
        /// <param name="rOI">搜索区域</param>
        /// <param name="rectangle_INFO"></param>
        /// <param name="DataStrings"></param>
        public static void FindCode2D(HImage _image, HHandle _Model, ROI rOI, ref HXLDCont xld, ref string DataStrings)
        {

            int resultHandles = 0; string decodedDataStrings = string.Empty;

            if (_image.IsInitialized())
            {
                HRegion _region = rOI.GenRegion();

                if (_region.IsInitialized())
                {
                    _image = _image.ReduceDomain(_region);

                    HXLDCont Cordxld = ((HDataCode2D)_Model).FindDataCode2d(_image, "stop_after_result_num", 1, out resultHandles, out decodedDataStrings);

                    if (resultHandles != 0)
                    {
                        xld = Cordxld;
                        DataStrings = decodedDataStrings;
                    }
                    else
                    {
                        DataStrings = decodedDataStrings;
                    }
                }
            }
        }

        /// <summary>
        /// 直角坐标系显示
        /// </summary>
        /// <param name="img"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static HXLDCont GetCoord(HImageExt img, Coordinate_INFO coord)
        {
            int Width, Height;
            double row, col;
            HXLDCont CoordXLD;
            img.GetImageSize(out Width, out Height);
            HTuple row1 = new HTuple(new double[] { 0, 0 });
            HTuple col1 = new HTuple(new double[] { 0, 0 });
            HTuple row2 = new HTuple(new double[] { 0, Height / 15 });
            HTuple col2 = new HTuple(new double[] { Width / 15, 0 });
            GenArrowContourXld(out CoordXLD, row1, col1, row2, col2, 10, 10);
            WorldPlane2Point(img, coord.X, coord.Y, out row, out col);
            HHomMat2D hom = new HHomMat2D();
            hom.VectorAngleToRigid(0, 0, 0, row, col, coord.Phi);
            CoordXLD = CoordXLD.AffineTransContourXld(hom);
            return CoordXLD;
        }

        /// <summary>
        /// 直角坐标系显示
        /// </summary>
        /// <param name="img"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static HXLDCont GetCoord_Image(HImageExt img, Coordinate_INFO coord)
        {
            int Width, Height;
            double row, col;
            HXLDCont CoordXLD;
            img.GetImageSize(out Width, out Height);
            HTuple row1 = new HTuple(new double[] { 0, 0 });
            HTuple col1 = new HTuple(new double[] { 0, 0 });
            HTuple row2 = new HTuple(new double[] { 0, Height / 10 });
            HTuple col2 = new HTuple(new double[] { Width / 10, 0 });
            GenArrowContourXld(out CoordXLD, row1, col1, row2, col2, 10, 10);
            //WorldPlane2Point(img, coord.X, coord.Y, out row, out col);
            HHomMat2D hom = new HHomMat2D();
            hom.VectorAngleToRigid(0, 0, 0, coord.Y, coord.X, coord.Phi);
            CoordXLD = CoordXLD.AffineTransContourXld(hom);
            return CoordXLD;
        }

        /// <summary>
        /// 世界坐标转换为当前图像的像素坐标
        /// </summary>
        /// <param name="img"></param>
        /// <param name="wX"></param>
        /// <param name="wY"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public static void WorldPlane2Point(HImageExt img, double wX, double wY, out double row, out double col)
        {
            row = 0f; col = 0f;
            double xImg, yImg;
            try
            {
                double xAxis, yAxis;
                xAxis = img.getHomAxis().AffineTransPoint2d(img.X, img.Y, out yAxis);
                xImg = wX - xAxis;
                yImg = wY - yAxis;
                ImagePlane2Pixel(img, xImg, yImg, out row, out col);

            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 圆转换成世界坐标系
        /// </summary>
        /// <param name="img">图像信息</param>
        /// <param name="inCircle">输入圆</param>
        /// <returns>返回世界坐标系圆</returns>
        public static Circle_INFO Circle2WorldPlane(HImageExt img, Circle_INFO inCircle)
        {
            Circle_INFO outCircle = new Circle_INFO();
            try
            {
                Points2WorldPlane(img, inCircle.CenterY, inCircle.CenterX, out outCircle.CenterX, out outCircle.CenterY);
                outCircle.Radius = inCircle.Radius * (img.ScaleY + img.ScaleX) / 2;
                return outCircle;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outCircle;
            }
        }

        /// <summary>
        /// 图像坐标点转换为世界坐标点
        /// </summary>
        /// <param name="img">坐标信息图像</param>
        /// <param name="row">输入坐标行</param>
        /// <param name="col">输入坐标列</param>
        /// <param name="wX">输出世界坐标行</param>
        /// <param name="wY">输出世界坐标列</param>
        public static void Points2WorldPlane(HImageExt img, double row, double col, out double wX, out double wY)
        {
            wX = 0f;
            wY = 0f;
            try
            {
                double xImg, yImg;
                double xAxis, yAxis;
                //相机缩放比率校正
                //xImg = img.getHomImg().AffineTransPoint2d(new HTuple(cols.ToArray()), new HTuple(rows.ToArray()), out yImg);
                Pixel2WorldPlane(img, row, col, out xImg, out yImg);
                xAxis = img.getHomAxis().AffineTransPoint2d(img.X, img.Y, out yAxis);

                wX = xImg + xAxis;
                wY = yImg + yAxis;
            }
            catch (System.Exception ex)
            {
            }
        }

        /// <summary>
        /// 图像坐标点转换为世界坐标点
        /// </summary>
        /// <param name="img">坐标信息图像</param>
        /// <param name="rows">输入坐标行</param>
        /// <param name="cols">输入坐标列</param>
        /// <param name="wX">输出世界坐标行</param>
        /// <param name="wY">输出世界坐标列</param>
        public static void Points2WorldPlane(HImageExt img, List<double> rows, List<double> cols, out List<double> wX, out List<double> wY)
        {
            wX = new List<double>();
            wY = new List<double>();
            try
            {
                HTuple xImg, yImg;
                double xAxis, yAxis;
                //相机缩放比率校正
                //xImg = img.getHomImg().AffineTransPoint2d(new HTuple(cols.ToArray()), new HTuple(rows.ToArray()), out yImg);
                Pixel2WorldPlane(img, rows, cols, out xImg, out yImg);
                xAxis = img.getHomAxis().AffineTransPoint2d(img.X, img.Y, out yAxis);

                wX = xImg.TupleAdd(xAxis).ToDArr().ToList();
                wY = yImg.TupleAdd(yAxis).ToDArr().ToList();
            }
            catch (System.Exception ex)
            {
            }
        }

        /// <summary>
        /// 图像坐标点转换为mm坐标点，使用区域标定的方法
        /// </summary>
        /// <param name="img">坐标信息图像</param>
        /// <param name="rows">输入坐标行</param>
        /// <param name="cols">输入坐标列</param>
        /// <param name="X">输出mm坐标行</param>
        /// <param name="Y">输出mm坐标列</param>
        public static void Pixel2WorldPlane(HImageExt img, List<double> rows, List<double> cols, out HTuple X, out HTuple Y)
        {
            X = new HTuple();
            Y = new HTuple();
            try
            {
                double xImg, yImg;
                //缩放校正
                for (int i = 0; i < rows.Count; i++)
                {
                    if (img.blnCalibrated)
                    {
                        HTuple row = HTuple.TupleGenConst(img.BoardRow.Length, rows[i]);
                        HTuple col = HTuple.TupleGenConst(img.BoardRow.Length, cols[i]);
                        HTuple distance = HMisc.DistancePp(row, col, img.BoardRow, img.BoardCol);
                        int index = distance.TupleFindFirst(distance.TupleMin()).I;
                        xImg = img.BoardX[index].D + (cols[i] - img.BoardCol[index].D) * img.ScaleX;
                        yImg = img.BoardY[index].D + (rows[i] - img.BoardRow[index].D) * img.ScaleY;
                    }
                    else
                    {
                        xImg = cols[i] * img.ScaleX;
                        yImg = rows[i] * img.ScaleY;
                    }
                    X = X.TupleConcat(xImg);
                    Y = Y.TupleConcat(yImg);
                }
            }
            catch (System.Exception ex)
            {
            }
        }

        /// <summary>
        /// 直线转换世界坐标系
        /// </summary>
        /// <param name="img">图片信息</param>
        /// <param name="inLine">输入世界坐标直线</param>
        /// <returns>返回图像坐标系直线</returns>
        public static Line_INFO Line2PixelPlane(HImageExt img, Line_INFO inLine)
        {
            Line_INFO outLine = new Line_INFO();
            try
            {
                WorldPlane2Point(img, inLine.StartX, inLine.StartY, out outLine.StartY, out outLine.StartX);
                WorldPlane2Point(img, inLine.EndX, inLine.EndY, out outLine.EndY, out outLine.EndX);
                outLine = new Line_INFO(outLine.StartY, outLine.StartX, outLine.EndY, outLine.EndX);
                return outLine;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outLine;
            }
        }

        /// <summary>
        /// 图像坐标点转换为mm坐标
        /// </summary>
        /// <param name="img">坐标信息图像</param>
        /// <param name="row">输入坐标行</param>
        /// <param name="col">输入坐标列</param>
        /// <param name="wX">输出mm坐标行</param>
        /// <param name="wY">输出mm坐标列</param>
        public static void Pixel2WorldPlane(HImageExt img, double row, double col, out double X, out double Y)
        {
            X = 0f; Y = 0f;
            try
            {
                if (img.blnCalibrated)
                {
                    //缩放校正
                    HTuple rows = HTuple.TupleGenConst(img.BoardRow.Length, row);
                    HTuple cols = HTuple.TupleGenConst(img.BoardRow.Length, col);
                    HTuple distance = HMisc.DistancePp(rows, cols, img.BoardRow, img.BoardCol);
                    int index = distance.TupleFindFirst(distance.TupleMin()).I;
                    X = img.BoardX[index].D + (col - img.BoardCol[index].D) * img.ScaleX;
                    Y = img.BoardY[index].D + (row - img.BoardRow[index].D) * img.ScaleY;
                }
                else
                {
                    X = col * img.ScaleX;
                    Y = row * img.ScaleY;
                }
            }
            catch (System.Exception ex)
            {
            }
        }


        /// <summary>
        /// 世界坐标圆转换成当前图像坐标系
        /// </summary>
        /// <param name="img"></param>
        /// <param name="inCircle"></param>
        /// <returns></returns>
        public static Circle_INFO Circle2PixelPlane(HImageExt img, Circle_INFO inCircle)
        {
            Circle_INFO outCircle = new Circle_INFO();
            try
            {
                WorldPlane2Point(img, inCircle.CenterX, inCircle.CenterY, out outCircle.CenterY, out outCircle.CenterX);

                outCircle.Radius = inCircle.Radius * 2 / (img.ScaleY + img.ScaleX);
                return outCircle;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outCircle;
            }
        }

        /// <summary>
        /// 矩形转换成世界坐标系
        /// </summary>
        /// <param name="img">图像信息</param>
        /// <param name="inCircle">输入圆</param>
        /// <returns>返回世界坐标系圆</returns>
        public static Rectangle2_INFO Rect2WorldPlane(HImageExt img, Rectangle2_INFO inRectangle2)
        {
            Rectangle2_INFO outRectangle2 = new Rectangle2_INFO();
            try
            {

                Points2WorldPlane(img, inRectangle2.CenterY, inRectangle2.CenterX, out outRectangle2.CenterX, out outRectangle2.CenterY);

                outRectangle2.Length1 = inRectangle2.Length1 * (img.ScaleY + img.ScaleX) / 2;
                outRectangle2.Length2 = inRectangle2.Length2 * (img.ScaleY + img.ScaleX) / 2;
                outRectangle2.Phi = inRectangle2.Phi;

                return outRectangle2;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outRectangle2;
            }
        }

        /// <summary>
        /// 世界坐标矩形转换成当前图像坐标系
        /// </summary>
        /// <param name="img"></param>
        /// <param name="inCircle"></param>
        /// <returns></returns>
        public static Rectangle2_INFO Rect2PixelPlane(HImageExt img, Rectangle2_INFO inRectangle2)
        {
            Rectangle2_INFO outRectangle2 = new Rectangle2_INFO();
            try
            {
                WorldPlane2Point(img, inRectangle2.CenterX, inRectangle2.CenterY, out outRectangle2.CenterY, out outRectangle2.CenterX);

                outRectangle2.Length1 = inRectangle2.Length1 * 2 / (img.ScaleY + img.ScaleX);
                outRectangle2.Length2 = inRectangle2.Length2 * 2 / (img.ScaleY + img.ScaleX);
                outRectangle2.Phi = inRectangle2.Phi;

                return outRectangle2;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outRectangle2;
            }
        }

        /// <summary>
        /// mm坐标转换为图像坐标
        /// </summary>
        /// <param name="img">坐标信息图像</param>
        /// <param name="X">当前图像mm坐标X</param>
        /// <param name="Y">当前图像mm坐标Y</param>
        /// <param name="row">图像坐标row</param>
        /// <param name="col">图像坐标col</param>
        public static void ImagePlane2Pixel(HImageExt img, double X, double Y, out double row, out double col)
        {
            row = 0f; col = 0f;
            try
            {
                if (img.blnCalibrated)
                {
                    //缩放校正
                    HTuple Xs = HTuple.TupleGenConst(img.BoardRow.Length, X);
                    HTuple Ys = HTuple.TupleGenConst(img.BoardRow.Length, Y);
                    HTuple distance = HMisc.DistancePp(Xs, Ys, img.BoardX, img.BoardY);
                    int index = distance.TupleFindFirst(distance.TupleMin()).I;
                    col = img.BoardCol[index].D + (X - img.BoardX[index].D) / img.ScaleX;
                    row = img.BoardRow[index].D + (Y - img.BoardY[index].D) / img.ScaleY;
                }
                else
                {
                    col = X / img.ScaleX;
                    row = Y / img.ScaleY;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测圆
        /// </summary>
        /// <param name="inImage">输入图像</param>
        /// <param name="inCircle">输入圆信息</param>
        /// <param name="inMetrology">测量信息</param>
        /// <param name="disableRegion">屏蔽区域</param>
        /// <param name="outCircle">输出的圆</param>
        /// <param name="outR"></param>
        /// <param name="outC"></param>
        /// <param name="outMeasureXLD"></param>
        public static void MeasureCircle(HImage inImage, Circle_INFO inCircle, Metrology_INFO inMetrology, HRegion disableRegion,
            out Circle_INFO outCircle, out HTuple outR, out HTuple outC, out HXLDCont outMeasureXLD)
        {
            //2d计量模型
            HMetrologyModel hMetrologyModel = new HMetrologyModel();
            outCircle = new Circle_INFO();
            HTuple CircleResult = new HTuple();
            HTuple CircleInfo = new HTuple();

            try
            {

                CircleInfo.Append(new HTuple(new double[] { inCircle.CenterY, inCircle.CenterX, inCircle.Radius }));

                hMetrologyModel.AddMetrologyObjectGeneric(
                    new HTuple("circle"),
                    CircleInfo,
                    new HTuple(inMetrology.Length1),
                    new HTuple(inMetrology.Length2),
                    new HTuple(1),
                    new HTuple(inMetrology.Threshold),
                    inMetrology.ParamName,
                    inMetrology.ParamValue);

                hMetrologyModel.ApplyMetrologyModel(inImage);
                outMeasureXLD = hMetrologyModel.GetMetrologyObjectMeasures("all", "all", out outR, out outC);

                //判断是否有屏蔽区域
                if (disableRegion != null && disableRegion.IsInitialized() && disableRegion.Area > 0 && outR.Length > 0)
                {
                    List<double> tempOutR = new List<double>(), tempOutC = new List<double>();
                    for (int i = 0; i < outR.DArr.Length - 1; i++)
                    {
                        //0 表示没有包含
                        if (disableRegion.TestRegionPoint(outR[i].D, outC[i].D) == 0)
                        {
                            tempOutR.Add(outR[i].D);
                            tempOutC.Add(outC[i].D);
                        }
                    }
                    outR = new HTuple(tempOutR.ToArray());
                    outC = new HTuple(tempOutC.ToArray());
                    VBA_Function.fitCircle(outR.ToDArr().ToList(), outC.ToDArr().ToList(), out outCircle);

                }
                else
                {
                    CircleResult = hMetrologyModel.GetMetrologyObjectResult(new HTuple("all"), new HTuple("all"), new HTuple("result_type"), new HTuple("all_param"));
                    if (CircleResult.TupleLength() >= 3)
                    {
                        outCircle.CenterY = CircleResult[0].D;
                        outCircle.CenterX = CircleResult[1].D;
                        outCircle.Radius = CircleResult[2].D;
                    }
                    else
                    {
                        VBA_Function.fitCircle(outR.ToDArr().ToList(), outC.ToDArr().ToList(), out outCircle);
                    }
                }

                hMetrologyModel.Dispose();
            }
            catch (Exception ex)
            {
                outCircle = inCircle;
                outR = new HTuple();
                outC = new HTuple();
                outMeasureXLD = new HXLDCont();
                hMetrologyModel.Dispose();
                throw ex;
            }
        }

        /// <summary>
        /// 检测直线 增加屏蔽区域 magical20171028
        /// </summary>
        /// <param name="inImage">检测图像</param>
        /// <param name="inLine">输入检测直线区域</param>
        /// <param name="inMetrology">形态参数</param>
        /// <param name="outLine">输出直线</param>
        /// <param name="outR">输出行点</param>
        /// <param name="outC">输出列点</param>
        /// <param name="outMeasureXLD">输出检测轮廓</param>
        /// <param name="disableRegion">屏蔽区域 可选</param>
        /// <param name="isPaint">对屏蔽区域进行喷绘 可选</param>
        public static void MeasureLine(HImage inImage, Line_INFO inLine, Metrology_INFO inMetrology, out Line_INFO outLine, out HTuple outR, out HTuple outC, out HXLDCont outMeasureXLD, HRegion disableRegion = null)
        {
            HMetrologyModel hMetrologyModel = new HMetrologyModel();
            try
            {
                outLine = new Line_INFO();
                HTuple lineResult = new HTuple();
                HTuple lineInfo = new HTuple();
                lineInfo.Append(new HTuple(new double[] { inLine.StartY, inLine.StartX, inLine.EndY, inLine.EndX }));

                //magical 20180405增加最强边的计算
                if (inMetrology.ParamValue[1] == "strongest")
                {
                    MeasureLine1D(inImage, inLine, inMetrology, out outLine, out outR, out outC, out outMeasureXLD, disableRegion);
                    return;
                }

                hMetrologyModel.AddMetrologyObjectGeneric(new HTuple("line"), lineInfo, new HTuple(inMetrology.Length1),
                    new HTuple(inMetrology.Length2), new HTuple(1), new HTuple(inMetrology.Threshold)
                    , inMetrology.ParamName, inMetrology.ParamValue);
                hMetrologyModel.SetMetrologyObjectParam(0, "min_score", 0.1);//降低直线拟合的最低得分,尽量使用halcon的拟合方法,因为VBA_Function.fitLine方法拟合的直线不准


                if (disableRegion != null && disableRegion.IsInitialized())
                {
                    hMetrologyModel.ApplyMetrologyModel(inImage);

                    //单个测量区域 刚好 有一大半在屏蔽区域,一小部分在有效区域,这时候也会测出一个点,这个点在屏蔽区域内,导致精度损失约为1个像素左右.需要喷绘之后,再进行点是否在屏蔽区域判断
                    outMeasureXLD = hMetrologyModel.GetMetrologyObjectMeasures("all", "all", out outR, out outC);

                    List<double> tempOutR = new List<double>(), tempOutC = new List<double>();

                    for (int i = 0; i < outR.DArr.Length - 1; i++)
                    {
                        //0 表示没有包含
                        if (disableRegion.TestRegionPoint(outR[i].D, outC[i].D) == 0)
                        {
                            tempOutR.Add(outR[i].D);
                            tempOutC.Add(outC[i].D);
                        }
                    }
                    outR = new HTuple(tempOutR.ToArray());
                    outC = new HTuple(tempOutC.ToArray());
                }
                else
                {
                    hMetrologyModel.ApplyMetrologyModel(inImage);
                    outMeasureXLD = hMetrologyModel.GetMetrologyObjectMeasures("all", "all", out outR, out outC);
                }
                lineResult = hMetrologyModel.GetMetrologyObjectResult(new HTuple("all"), new HTuple("all"), new HTuple("result_type"), new HTuple("all_param"));
                if (lineResult.TupleLength() >= 4)
                {
                    outLine = new Line_INFO(lineResult[0].D, lineResult[1].D, lineResult[2].D, lineResult[3].D);
                }
                else
                {
                    if (VBA_Function.fitLineByH(outR.ToDArr().ToList(), outC.ToDArr().ToList(), out outLine))
                        outLine = inLine;
                }

                hMetrologyModel.Dispose();
            }
            catch (Exception ex)
            {
                outLine = inLine;
                outR = new HTuple();
                outC = new HTuple();
                outMeasureXLD = new HXLDCont();
                hMetrologyModel.Dispose();
                //异常写入日志文件
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 利用一维测量算子,检测直线.再利用halcon的拟合直线算法拟合直线 主要用于最强边缘的测量 magical20180405
        /// </summary>
        /// <param name="inImage"></param>
        /// <param name="inLine"></param>
        /// <param name="inMetrology"></param>
        /// <param name="outLine"></param>
        /// <param name="outR"></param>
        /// <param name="outC"></param>
        /// <param name="outMeasureXLD"></param>
        /// <param name="disableRegion"></param>
        /// <param name="isPaint"></param>
        public static void MeasureLine1D(HImage inImage, Line_INFO inLine, Metrology_INFO inMetrology, out Line_INFO outLine, out HTuple outR, out HTuple outC, out HXLDCont outMeasureXLD, HRegion disableRegion = null, bool isPaint = true)
        {

            outLine = inLine;
            outR = new HTuple();
            outC = new HTuple();
            List<double> outRList = new List<double>();
            List<double> outCList = new List<double>();
            HImage tempImage;
            if (disableRegion != null && disableRegion.IsInitialized())
            {
                //将屏蔽区域喷绘为0,这样就无法测量到点 magical 20171028
                tempImage = disableRegion.PaintRegion(inImage, 0d, "fill");
            }
            else
            {
                tempImage = inImage;
            }

            //注意下这里的角度
            double angle = HMisc.AngleLx(inLine.StartY, inLine.StartX, inLine.EndY, inLine.EndX);
            int pointsNum = (int)((HMisc.DistancePp(inLine.StartY, inLine.StartX, inLine.EndY, inLine.EndX) - 2 * inMetrology.Length2) / inMetrology.MeasureDis) + 1;
            double newMeasureDis = (HMisc.DistancePp(inLine.StartY, inLine.StartX, inLine.EndY, inLine.EndX) - 2 * inMetrology.Length2) / pointsNum;
            double rectRowC, rectColC;

            outMeasureXLD = new HXLDCont();
            outMeasureXLD.GenEmptyObj();
            for (int i = 0; i <= pointsNum; i++)
            {
                //rectRowC = inLine.StartY + (((inLine.EndY - inLine.StartY) * (i + 1)) / (pointsNum)) + inMetrology.Length2*Math.Sin(angle);
                //rectColC = inLine.StartX + (((inLine.EndX - inLine.StartX) * (i )) / (pointsNum))+ inMetrology.Length2 * Math.Cos(angle);
                rectRowC = inLine.StartY + (inMetrology.Length2 - i * newMeasureDis) * Math.Sin(angle);
                rectColC = inLine.StartX + (inMetrology.Length2 + i * newMeasureDis) * Math.Cos(angle);



                HXLDCont tempRect = new HXLDCont();
                tempRect.GenRectangle2ContourXld(rectRowC, rectColC, angle - Math.PI / 2, inMetrology.Length1, inMetrology.Length2);
                outMeasureXLD = outMeasureXLD.ConcatObj(tempRect);


                HMeasure mea = new HMeasure();
                int width, height;
                inImage.GetImageSize(out width, out height);
                mea.GenMeasureRectangle2(rectRowC, rectColC, angle - Math.PI / 2, inMetrology.Length1, inMetrology.Length2, width, height, "nearest_neighbor");
                HTuple rowEdge, columnEdge, amplitude, distance;
                mea.MeasurePos(tempImage, 1, inMetrology.Threshold, inMetrology.ParamValue[0], "all", out rowEdge, out columnEdge, out amplitude, out distance);

                if (amplitude != null & amplitude.Length > 0)
                {
                    // amplitude.TupleSort();
                    HTuple HIndex = amplitude.TupleAbs().TupleSortIndex();
                    outRList.Add(rowEdge[HIndex[HIndex.Length - 1].I]);
                    outCList.Add(columnEdge[HIndex[HIndex.Length - 1].I]);
                }

                mea.Dispose();
            }
            outR = new HTuple(outRList.ToArray());
            outC = new HTuple(outCList.ToArray());

            if (disableRegion != null && disableRegion.IsInitialized())
            {
                List<double> tempOutR = new List<double>(), tempOutC = new List<double>();
                for (int i = 0; i < outR.DArr.Length - 1; i++)
                {
                    //0 表示没有包含
                    if (disableRegion.TestRegionPoint(outR[i].D, outC[i].D) == 0)
                    {
                        tempOutR.Add(outR[i].D);
                        tempOutC.Add(outC[i].D);
                    }
                }
                outR = new HTuple(tempOutR.ToArray());
                outC = new HTuple(tempOutC.ToArray());
            }

            if (outR.Length > 0)
            {
                VBA_Function.fitLineByH(outRList, outCList, out outLine);
            }
            else
            {
                outLine = inLine;
            }

        }

        /// <summary>
        /// 直线转换世界坐标系
        /// </summary>
        /// <param name="img">图片信息</param>
        /// <param name="inLine">输入直线</param>
        /// <returns>返回世界坐标系直线</returns>
        public static Line_INFO Line2WorldPlane(HImageExt img, Line_INFO inLine)
        {
            Line_INFO outLine = new Line_INFO();
            try
            {
                Points2WorldPlane(img, inLine.StartY, inLine.StartX, out outLine.StartX, out outLine.StartY);
                Points2WorldPlane(img, inLine.EndY, inLine.EndX, out outLine.EndX, out outLine.EndY);
                outLine = new Line_INFO(outLine.StartY, outLine.StartX, outLine.EndY, outLine.EndX);
                return outLine;
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
                return outLine;
            }
        }

        /// <summary>
        /// Halcon自动畸变矫正
        /// </summary>
        /// <param name="hImage">输入图像</param>
        /// <param name="Length">精度等级</param>
        /// <param name="out_Image">畸变矫正后的图像</param>
        /// <param name="camPar">相机内参</param>
        /// <param name="out_camPar">相机外参</param>
        public static void CreateDistortioCorrection(HImageExt hImage, int Length, out HImage out_Image, out HCamPar camPar, out HCamPar out_camPar)
        {
            HTuple Height, Width;
            try
            {
                HImage hv_Image = new HImage(hImage);
                hv_Image.GetImageSize(out Width, out Height);
                HXLDCont hv_XLDCont = hv_Image.EdgesSubPix("canny", 1, 20, 40);
                hv_XLDCont = hv_XLDCont.SegmentContoursXld("lines_circles", 5, 4, 2);
                hv_XLDCont = hv_XLDCont.SelectShapeXld("contlength", "and", 60, 99999);

                hv_XLDCont = hv_XLDCont.RadialDistortionSelfCalibration(Width, Height, 0.05, 42, "division", "variable", Length, out camPar);

                HRegion hv_Region = hv_Image.GetDomain();

                out_camPar = camPar.ChangeRadialDistortionCamPar("fixed", (HTuple)0);

                out_Image = hImage.ChangeRadialDistortionImage(hv_Region, camPar, out_camPar);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 畸变效正
        /// </summary>
        /// <param name="hImage"></param>
        /// <param name="camPar"></param>
        /// <param name="out_camPar"></param>
        /// <param name="out_Image"></param>
        public static void DistortioCorrection(HImageExt hImage, HCamPar camPar, HCamPar out_camPar, out HImage out_Image)
        {
            HImage hv_Image = new HImage(hImage);

            HRegion hv_Region = hv_Image.GetDomain();

            out_Image = hImage.ChangeRadialDistortionImage(hv_Region, camPar, out_camPar);

        }

        #region ROI方法

        public static void GenArrowContourXld(out HXLDCont ho_Arrow, HTuple hv_Row1, HTuple hv_Column1, HTuple hv_Row2, HTuple hv_Column2,
            HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {

            HTuple hv_Length = null, hv_ZeroLengthIndices = null;
            HTuple hv_DR = null, hv_DC = null, hv_HalfHeadWidth = null;
            HTuple hv_RowP1 = null, hv_ColP1 = null, hv_RowP2 = null;
            HTuple hv_ColP2 = null, hv_Index = null;

            ho_Arrow = new HXLDCont();
            HXLDCont ho_TempArrow = new HXLDCont();

            HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);

            hv_ZeroLengthIndices = hv_Length.TupleFind(0);
            if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
            {
                if (hv_Length == null)
                    hv_Length = new HTuple();
                hv_Length[hv_ZeroLengthIndices] = -1;
            }

            hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
            hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
            hv_HalfHeadWidth = hv_HeadWidth / 2.0;

            hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
            hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
            hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
            hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);

            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
            {
                if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                {
                    ho_TempArrow.Dispose();
                    ho_TempArrow.GenContourPolygonXld(hv_Row1.TupleSelect(hv_Index),
                        hv_Column1.TupleSelect(hv_Index));
                }
                else
                {
                    ho_TempArrow.Dispose();
                    ho_TempArrow.GenContourPolygonXld(((((((((((hv_Row1.TupleSelect(
                        hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                        hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                        ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                        hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                        hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                }
                if (!ho_Arrow.IsInitialized())
                {
                    ho_Arrow = ho_TempArrow;
                }
                ho_Arrow = ho_Arrow.ConcatObj(ho_TempArrow);
            }
            ho_TempArrow.Dispose();

            return;
        }

        #endregion


    }
}
