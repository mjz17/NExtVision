using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.MeasureLine
{
    [Category("几何测量")]
    [DisplayName("直线测量")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接图像信息
        /// </summary>
        public DataVar Link_Image_Data;

        /// <summary>
        /// 链接位置补正名称
        /// </summary>
        public string m_LinkDataName = string.Empty;

        /// <summary>
        /// 位置补正
        /// </summary>
        public DataVar Link_Affine_data;

        /// <summary>
        /// 输入链接坐标/圆心坐标
        /// </summary>
        public Coordinate_INFO LinkAffineCorre = new Coordinate_INFO();

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 测量信息
        /// </summary>
        public Metrology_INFO m_MetrologyInfo = new Metrology_INFO();

        /// <summary>
        /// 补正
        /// </summary>
        public HHomMat2D tempMat2D1;

        /// <summary>
        /// 补正
        /// </summary>
        public HHomMat2D tempMat2D2;

        /// <summary>
        /// 绘制直线信息
        /// </summary>
        public Line_INFO m_DrawInLine = new Line_INFO();

        /// <summary>
        /// 绘制直线信息
        /// </summary>
        public Line_INFO m_OutLine = new Line_INFO();

        [NonSerialized]
        public HXLDCont m_ArrowXLD = new HXLDCont();//检测方向
        [NonSerialized]
        public HXLDCont m_MeasureXLD = new HXLDCont();//检测形态轮廓
        [NonSerialized]
        public HXLDCont m_MeasureCross = new HXLDCont();//检测点十字
        [NonSerialized]
        public HXLDCont m_ResultXLD = new HXLDCont();//检测结果轮廓

        [NonSerialized]
        protected HTuple m_Row = new HTuple();
        [NonSerialized]
        protected HTuple m_Col = new HTuple();

        /// <summary>
        /// 执行一次
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                //仿射变换后的屏蔽区域
                HRegion tempDisableRegion = new HRegion();
                MessureLine(m_DrawInLine);
                ModuleParam.BlnSuccessed = true;
            }
            catch (Exception ex)
            {
                //运行失败
                ModuleParam.BlnSuccessed = false;
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
            }
            finally
            {
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                //模块运行状态
                {
                    DataVar objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.状态.ToString(),
                       DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, ModuleParam.BlnSuccessed);
                    ModuleProject.UpdateLocalVarValue(objStatus);
                }
                //模块运行状态
                {
                    DataVar objTime = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.时间.ToString(),
                       DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ModuleParam.ModuleCostTime);
                    ModuleProject.UpdateLocalVarValue(objTime);
                }
                sw.Reset();
            }
        }

        public void MessureLine(Line_INFO _INFO)
        {
            Line_INFO m_InLine = _INFO;

            Line_INFO tempLine = new Line_INFO();

            HRegion tempDisableRegion = new HRegion();//仿射变换后的屏蔽区域

            double startX, startY, endX, endY;

            try
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //加载链接图像
                    DataVar data = ModuleProject.GetLocalVarValue(Link_Image_Data);
                    m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                }

                if (m_Image == null || !m_Image.IsInitialized())
                {
                    return;
                }

                Coordinate_INFO coord = new Coordinate_INFO();
                if (m_LinkDataName != null)
                {
                    if (m_LinkDataName.Length > 0)
                    {
                        //查询坐标系
                        DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Affine_data.m_DataName &&
                        c.m_DataModuleID == Link_Affine_data.m_DataModuleID);
                        coord = ((List<Coordinate_INFO>)data.m_DataValue)[0];

                        HHomMat2D homMat2D = new HHomMat2D();
                        homMat2D = homMat2D.HomMat2dRotateLocal(-coord.Phi);
                        homMat2D = homMat2D.HomMat2dTranslate(coord.X, coord.Y);

                        startX = homMat2D.AffineTransPoint2d(m_DrawInLine.StartX, m_DrawInLine.StartY, out startY);
                        endX = homMat2D.AffineTransPoint2d(m_DrawInLine.EndX, m_DrawInLine.EndY, out endY);
                        tempLine = new Line_INFO(startY, startX, endY, endX);
                        //tempLine = SysVisionCore.Line2PixelPlane(m_Image, tempLine);//世界坐标转图像坐标
                    }
                    else
                    {
                        coord = new Coordinate_INFO();
                        tempLine = new Line_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);
                        //tempLine = SysVisionCore.Line2PixelPlane(m_Image, m_InLine);//世界坐标转图像坐标
                    }
                }
                else
                {
                    coord = new Coordinate_INFO();
                    tempLine = new Line_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);
                    //tempLine = SysVisionCore.Line2PixelPlane(m_Image, m_InLine);//世界坐标转图像坐标
                }


                HXLDCont CoorXLD = SysVisionCore.GetCoord_Image(m_Image, coord);
                ModuleROI roi坐标系 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(), ModuleParam.ModuleDesCribe,
                    ModuleRoiType.参考坐标系.ToString(), "green", new HObject(CoorXLD));
                m_Image.UpdateRoiList(roi坐标系);

                //显示搜索方向 magical 20180404
                double lineCenterRow, lineCenterCol;
                lineCenterRow = (tempLine.StartY + tempLine.EndY) / 2;
                lineCenterCol = (tempLine.StartX + tempLine.EndX) / 2;
                double lineAngle = HMisc.AngleLx(tempLine.StartY, tempLine.StartX, tempLine.EndY, tempLine.EndX) - Math.PI / 2;
                HHomMat2D hom = new HHomMat2D();
                hom.VectorAngleToRigid(0, 0, 0, lineCenterRow, lineCenterCol, lineAngle);
                double arrowRow, arrowCol;
                arrowRow = hom.AffineTransPoint2d(0, m_MetrologyInfo.Length1, out arrowCol);
                SysVisionCore.GenArrowContourXld(out m_ArrowXLD, lineCenterRow, lineCenterCol, arrowRow, arrowCol, m_MetrologyInfo.Length2, m_MetrologyInfo.Length2);

                //直线测量
                SysVisionCore.MeasureLine(m_Image, tempLine, m_MetrologyInfo, out m_OutLine, out m_Row, out m_Col, out m_MeasureXLD, tempDisableRegion);

                m_MeasureCross.GenCrossContourXld(m_Row, m_Col, (HTuple)m_MetrologyInfo.Length2, new HTuple(m_OutLine.Nx + new HTuple(30).TupleRad()));
                m_ResultXLD.GenContourPolygonXld(new HTuple(m_OutLine.StartY, m_OutLine.EndY), new HTuple(m_OutLine.StartX, m_OutLine.EndX));

                //直线中心
                double m_row, m_col;
                HRegion region = new HRegion();
                region.GenRegionLine(m_OutLine.StartY, m_OutLine.StartX, m_OutLine.EndY, m_OutLine.EndX);
                region.AreaCenter(out m_row, out m_col);

                //模块对应数据(实际的图像坐标)
                {
                    DataVar data_LineI = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line图像.ToString(),
               DataVarType.DataType.Line, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine);
                    ModuleProject.UpdateLocalVarValue(data_LineI);

                    DataVar data_Line_startX = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line_StartX图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine.StartX);
                    ModuleProject.UpdateLocalVarValue(data_Line_startX);

                    DataVar data_Line_startY = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line_StartY图像.ToString(),
             DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine.StartX);
                    ModuleProject.UpdateLocalVarValue(data_Line_startY);

                    DataVar data_Line_endX = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line_EndX图像.ToString(),
             DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine.StartX);
                    ModuleProject.UpdateLocalVarValue(data_Line_endX);

                    DataVar data_Line_endY = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line_EndY图像.ToString(),
             DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine.EndX);
                    ModuleProject.UpdateLocalVarValue(data_Line_endY);

                    DataVar data_Line_midX = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.直线中心X图像.ToString(),
            DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_row);
                    ModuleProject.UpdateLocalVarValue(data_Line_midX);

                    DataVar data_Line_midY = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.直线中心Y图像.ToString(),
             DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_col);
                    ModuleProject.UpdateLocalVarValue(data_Line_midY);

                    //弧度
                    DataVar data_Line_rad = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.直线角度Rad.ToString(),
            DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutLine.Phi);
                    ModuleProject.UpdateLocalVarValue(data_Line_rad);

                    //角度
                    DataVar data_Line_phi = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.直线角度Phi.ToString(),
            DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, (180 / Math.PI) * m_OutLine.Phi);
                    ModuleProject.UpdateLocalVarValue(data_Line_phi);

                    //doubleradians=(Math.PI/180)*degrees;角度转弧度
                    //degrees = (180 / Math.PI) *radians ;弧度转角度

                }

                //模块对应数据(实际的世界坐标)
                {
                    //图像坐标系转世界坐标系
                    Line_INFO lineW = SysVisionCore.Line2WorldPlane(m_Image, m_OutLine);

                    DataVar dataLineW = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line世界.ToString(),
            DataVarType.DataType.Line, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, lineW);
                    ModuleProject.UpdateLocalVarValue(dataLineW);

                }



                ModuleROI roi检测范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                      ModuleParam.ModuleDesCribe, ModuleRoiType.检测范围.ToString(), "blue", new HObject(m_MeasureXLD), m_IsDispOutRang);

                ModuleROI roi检测点 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                  ModuleParam.ModuleDesCribe, ModuleRoiType.检测点.ToString(), "yellow", new HObject(m_MeasureCross), m_IsDispOutPoint);

                ModuleROI roi检测结果 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red", new HObject(m_ResultXLD), m_IsDispResult);

                ModuleROI roi搜索方向 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                             ModuleParam.ModuleDesCribe, ModuleRoiType.搜索方向.ToString(), "cyan", new HObject(m_ArrowXLD), m_IsDispDirect);

                ModuleROI roi屏蔽范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe, ModuleRoiType.屏蔽范围.ToString(), "red", new HObject(tempDisableRegion));

                m_Image.UpdateRoiList(roi检测范围);
                m_Image.UpdateRoiList(roi检测点);
                m_Image.UpdateRoiList(roi检测结果);
                m_Image.UpdateRoiList(roi屏蔽范围);
                m_Image.UpdateRoiList(roi搜索方向);
            }
            catch (Exception ex)
            {
                ModuleParam.BlnSuccessed = false;
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
            }
        }

        /// <summary>
        /// 模块测量的方法
        /// </summary>
        public void VerifyParam(Line_INFO _INFO)
        {
            HRegion tempDisableRegion = new HRegion();//仿射变换后的屏蔽区域
            Line_INFO m_DrawLine = new Line_INFO();

            //像素坐标转世界坐标
            //m_DrawLine = SysVisionCore.Line2WorldPlane(m_Image, _INFO);

            m_DrawLine = new Line_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);

            if (m_LinkDataName != null)
            {
                if (m_LinkDataName.Length > 0)
                {
                    LinkAffineCorre = ((List<Coordinate_INFO>)Link_Affine_data.m_DataValue)[0];
                    //世界坐标转当前相对坐标
                    HHomMat2D hom1 = VBA_Function.setOrig(LinkAffineCorre.X, LinkAffineCorre.Y, -LinkAffineCorre.Phi);
                    m_DrawLine.StartX = hom1.AffineTransPoint2d(m_DrawLine.StartX, m_DrawLine.StartY, out m_DrawLine.StartY);
                    m_DrawLine.EndX = hom1.AffineTransPoint2d(m_DrawLine.EndX, m_DrawLine.EndY, out m_DrawLine.EndY);
                    m_DrawInLine = m_DrawLine;
                }
                else
                {
                    //世界坐标转当前相对坐标
                    HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                    m_DrawLine.StartX = hom1.AffineTransPoint2d(m_DrawLine.StartX, m_DrawLine.StartY, out m_DrawLine.StartY);
                    m_DrawLine.EndX = hom1.AffineTransPoint2d(m_DrawLine.EndX, m_DrawLine.EndY, out m_DrawLine.EndY);
                    m_DrawInLine = m_DrawLine;
                }
            }
            else
            {
                //世界坐标转当前相对坐标
                HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                m_DrawLine.StartX = hom1.AffineTransPoint2d(m_DrawLine.StartX, m_DrawLine.StartY, out m_DrawLine.StartY);
                m_DrawLine.EndX = hom1.AffineTransPoint2d(m_DrawLine.EndX, m_DrawLine.EndY, out m_DrawLine.EndY);
                m_DrawInLine = m_DrawLine;
            }
            MessureLine(m_DrawInLine);
        }

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            m_Row = new HTuple();
            m_Col = new HTuple();
            m_MeasureXLD = new HXLDCont();      ///检测形态轮廓
            m_MeasureCross = new HXLDCont();    ///检测点十字
            m_ResultXLD = new HXLDCont();       ///检测结果轮廓
            //disableRegion = new HRegion();
        }

        [OnSerializing()]
        internal void OnSerializingMethod(StreamingContext context)
        {
            //if (disableRegion == null || !disableRegion.IsInitialized())
            //{
            //    disableRegion = new HRegion((double)0, 0, 0);
            //}
        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //if (!disableRegion == null)
            //{
            //    disableRegion = new HRegion();
            //}
        }

    }
}
