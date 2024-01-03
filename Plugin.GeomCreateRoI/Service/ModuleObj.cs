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

namespace Plugin.GeomCreateRoI
{

    [Category("几何组合")]
    [DisplayName("创建ROI")]
    [Serializable]

    public class ModuleObj : ModuleObjBase
    {

        #region 读取图像

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Image_Data;

        #endregion

        #region ROI类型

        public RoIType ModelModel { get; set; }

        #endregion

        //矩形
        #region Length1

        /// <summary>
        /// Length1
        /// </summary>
        public string m_Length1 = string.Empty;

        /// <summary>
        /// Length1是否外部控制
        /// </summary>
        public bool m_ExternalLength1 = false;

        /// <summary>
        /// 链接Length1
        /// </summary>
        public DataVar Link_Length1;

        #endregion

        #region Length2

        /// <summary>
        /// Length1
        /// </summary>
        public string m_Length2 = string.Empty;

        /// <summary>
        /// Length2是否外部控制
        /// </summary>
        public bool m_ExternalLength2 = false;

        /// <summary>
        /// 链接Length1
        /// </summary>
        public DataVar Link_Length2;

        #endregion

        #region CenterRow

        /// <summary>
        /// CenterRow
        /// </summary>
        public string m_CenterRow = string.Empty;

        /// <summary>
        /// CenterRow是否外部控制
        /// </summary>
        public bool m_ExternalCenterRow = false;

        /// <summary>
        /// CenterRow
        /// </summary>
        public DataVar Link_CenterRow;

        #endregion

        #region CenterCol

        /// <summary>
        /// CenterRow
        /// </summary>
        public string m_CenterCol = string.Empty;

        /// <summary>
        /// CenterCol是否外部控制
        /// </summary>
        public bool m_ExternalCenterCol = false;

        /// <summary>
        /// CenterRow
        /// </summary>
        public DataVar Link_CenterCol;

        #endregion

        #region Phi

        /// <summary>
        /// Phi
        /// </summary>
        public string m_Phi = string.Empty;

        /// <summary>
        /// Phi是否外部控制
        /// </summary>
        public bool m_ExternalPhi = false;

        /// <summary>
        /// Phi
        /// </summary>
        public DataVar Link_Phi;

        #endregion

        //圆形
        #region Row

        /// <summary>
        /// Row
        /// </summary>
        public string m_Row = string.Empty;

        /// <summary>
        /// Row是否外部控制
        /// </summary>
        public bool m_ExternalRow = false;

        /// <summary>
        /// 链接Row
        /// </summary>
        public DataVar Link_Row;

        #endregion

        #region Col

        /// <summary>
        /// Length1
        /// </summary>
        public string m_Col = string.Empty;

        /// <summary>
        /// Col是否外部控制
        /// </summary>
        public bool m_ExternalCol = false;

        /// <summary>
        /// 链接Length1
        /// </summary>
        public DataVar Link_Col;

        #endregion

        #region Radius

        /// <summary>
        /// Radius
        /// </summary>
        public string m_Radius = string.Empty;

        /// <summary>
        /// Radius是否外部控制
        /// </summary>
        public bool m_ExternalRadius = false;

        /// <summary>
        /// Radius
        /// </summary>
        public DataVar Link_Radius;

        #endregion

        //是否跟随模式
        public bool IsFollow = false;

        #region 位置补正

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

        #endregion

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 绘制ROI的信息
        /// </summary>
        public ROI m_DrawROI;

        [NonSerialized]
        public HXLDCont m_ResultXLD = new HXLDCont();//ROI轮廓

        [NonSerialized]
        public HXLDCont m_ResultRegion = new HXLDCont();//ROI区域

        [NonSerialized]
        public HRegion m_OutRegion = new HRegion();//输出Region

        /// <summary>
        /// 第一次加载图像时参考坐标 存储图像坐标
        /// </summary>
        public Coordinate_INFO RegCoor = new Coordinate_INFO();

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //加载链接图像
                    DataVar data = ModuleProject.GetLocalVarValue(Link_Image_Data);
                    m_Image = ((List<HImageExt>)(data).m_DataValue)[0];

                    if (m_Image != null || m_Image.IsInitialized())
                    {
                        //是否开启跟随模式
                        if (IsFollow)
                        {
                            FllowRegion(m_DrawROI);
                            ModuleParam.BlnSuccessed = true;
                        }
                        else
                        {
                            OperateROI();
                            ModuleParam.BlnSuccessed = true;
                        }

                        //输出Region
                        {
                            DataVar objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ModuleParam.ModuleName,
                               DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, new HObject(m_OutRegion));
                            ModuleProject.UpdateLocalVarValue(objStatus);
                        }

                        //ROI显示
                        ModuleROI roi检测结果 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                        ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "cyan", new HObject(m_OutRegion), m_IsDispOutPoint);

                        m_Image.UpdateRoiList(roi检测结果);

                    }
                    else
                    {
                        ModuleParam.BlnSuccessed = false;
                    }
                }
                else
                {
                    ModuleParam.BlnSuccessed = false;
                }
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

        #region 操作ROI

        /// <summary>
        /// 操作ROI
        /// </summary>
        public void OperateROI()
        {
            try
            {
                switch (ModelModel)
                {
                    case RoIType.矩形:
                        m_OutRegion = OperateRect2();
                        break;
                    case RoIType.圆形:
                        m_OutRegion = OperateCircle();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 操作矩形

        /// <summary>
        /// 操作矩形
        /// </summary>
        public HXLDCont OperateRect2()
        {
            //坐标  
            List<double> Mid_data = new List<double>();
            #region 矩形

            //CenterCol判断是否链接了外部数据
            if (m_ExternalCenterCol)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_CenterCol.m_DataName &&
            c.m_DataModuleID == Link_CenterCol.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_CenterCol));
            }

            //CenterRow判断是否链接了外部数据
            if (m_ExternalCenterRow)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_CenterRow.m_DataName &&
           c.m_DataModuleID == Link_CenterRow.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_CenterRow));
            }

            //Link_Phi判断是否链接了外部数据
            if (m_ExternalPhi)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Phi.m_DataName &&
           c.m_DataModuleID == Link_Phi.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(-Convert.ToDouble(m_Phi));
            }

            //Length1判断是否链接了外部数据
            if (m_ExternalLength1)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Length1.m_DataName &&
                c.m_DataModuleID == Link_Length1.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_Length1));
            }

            //Length2判断是否链接了外部数据
            if (m_ExternalLength2)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Length2.m_DataName &&
           c.m_DataModuleID == Link_Length2.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_Length2));
            }

            #endregion
            ROI roi = new Rectangle2_INFO(Mid_data[0], Mid_data[1], Mid_data[2], Mid_data[3], Mid_data[4]);
            return roi.GenRegion();
        }

        #endregion

        #region 操作圆形

        /// <summary>
        /// 操作圆形
        /// </summary>
        public HXLDCont OperateCircle()
        {
            //坐标   
            List<double> Mid_data = new List<double>();
            #region 圆形

            if (m_ExternalCol)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Col.m_DataName &&
              c.m_DataModuleID == Link_Col.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_Col));
            }

            if (m_ExternalRow)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Row.m_DataName &&
              c.m_DataModuleID == Link_Row.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_Row));
            }

            if (m_ExternalRadius)
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Radius.m_DataName &&
          c.m_DataModuleID == Link_Radius.m_DataModuleID);
                    Mid_data.Add(Convert.ToDouble(data.m_DataValue));
                }
            }
            else
            {
                Mid_data.Add(Convert.ToDouble(m_Radius));
            }

            #endregion
            ROI roi = new Circle_INFO(Mid_data[0], Mid_data[1], Mid_data[2]);
            return roi.GenRegion();
        }

        #endregion

        #region 跟随模式

        public void FllowRegion(ROI _roi)
        {
            ROI tempCircle;
            try
            {
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //加载链接图像
                    DataVar Imagedata = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Image_Data.m_DataName &&
                    c.m_DataModuleID == Link_Image_Data.m_DataModuleID);
                    m_Image = ((List<HImageExt>)(Imagedata).m_DataValue)[0];
                }

                //开始运行没有图像数据直接返回
                if (m_Image == null || m_Image.IsInitialized() == false)
                {
                    return;
                }

                if (ModelModel == RoIType.矩形)
                {
                    Coordinate_INFO coord = new Coordinate_INFO();
                    if (m_LinkDataName != null)
                    {
                        if (m_LinkDataName.Length > 0)
                        {

                            double regRow, regCol;

                            //查询坐标系
                            DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Affine_data.m_DataName
                            && c.m_DataModuleID == Link_Affine_data.m_DataModuleID);
                            coord = ((List<Coordinate_INFO>)data.m_DataValue)[0];

                            HHomMat2D homMat2D = new HHomMat2D();
                            homMat2D = homMat2D.HomMat2dRotateLocal(-coord.Phi);
                            homMat2D = homMat2D.HomMat2dTranslate(coord.X, coord.Y);

                            tempCircle = (ROI)((Rectangle2_INFO)m_DrawROI).Clone();

                            regCol = homMat2D.AffineTransPoint2d(RegCoor.X, RegCoor.Y, out regRow);
                            SysVisionCore.WorldPlane2Point(m_Image, regCol, regRow, out regRow, out regCol);

                            HHomMat2D homMat = new HHomMat2D();
                            homMat.VectorAngleToRigid(((Rectangle2_INFO)tempCircle).CenterY, ((Rectangle2_INFO)tempCircle).CenterX,
                                RegCoor.Phi, regRow, regCol, coord.Phi);

                            m_ResultXLD = homMat.AffineTransContourXld(((Rectangle2_INFO)m_DrawROI).GenXld());

                            m_ResultRegion = m_ResultXLD.GenRegionContourXld("filled");
                        }
                        else
                        {
                            coord = new Coordinate_INFO();
                            tempCircle = (Rectangle2_INFO)m_DrawROI;
                            tempCircle = SysVisionCore.Rect2PixelPlane(m_Image, (Rectangle2_INFO)tempCircle);

                            m_ResultXLD.GenRectangle2ContourXld(((Rectangle2_INFO)tempCircle).CenterY, ((Rectangle2_INFO)tempCircle).CenterX,
                                ((Rectangle2_INFO)tempCircle).Phi, ((Rectangle2_INFO)tempCircle).Length1, ((Rectangle2_INFO)tempCircle).Length2);
                        }
                    }
                    else
                    {
                        coord = new Coordinate_INFO();
                        tempCircle = (Rectangle2_INFO)m_DrawROI;
                        tempCircle = SysVisionCore.Rect2PixelPlane(m_Image, (Rectangle2_INFO)tempCircle);

                        m_ResultXLD.GenRectangle2ContourXld(((Rectangle2_INFO)tempCircle).CenterY, ((Rectangle2_INFO)tempCircle).CenterX,
                            ((Rectangle2_INFO)tempCircle).Phi, ((Rectangle2_INFO)tempCircle).Length1, ((Rectangle2_INFO)tempCircle).Length2);
                    }
                }
                else if (ModelModel == RoIType.圆形)
                {
                    Coordinate_INFO coord = new Coordinate_INFO();
                    if (m_LinkDataName != null)
                    {
                        if (m_LinkDataName.Length > 0)
                        {
                            //查询坐标系
                            DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Affine_data.m_DataName
                            && c.m_DataModuleID == Link_Affine_data.m_DataModuleID);
                            coord = ((List<Coordinate_INFO>)data.m_DataValue)[0];

                            HHomMat2D homMat2D = new HHomMat2D();
                            homMat2D = homMat2D.HomMat2dRotateLocal(-coord.Phi);
                            homMat2D = homMat2D.HomMat2dTranslate(coord.X, coord.Y);

                            tempCircle = (ROI)((Circle_INFO)m_DrawROI).Clone();
                            ((Circle_INFO)tempCircle).CenterX = homMat2D.AffineTransPoint2d(((Circle_INFO)m_DrawROI).CenterX,
                                ((Circle_INFO)m_DrawROI).CenterY, out ((Circle_INFO)tempCircle).CenterY);

                            tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, (Circle_INFO)tempCircle);
                        }
                        else
                        {
                            coord = new Coordinate_INFO();
                            tempCircle = (Circle_INFO)m_DrawROI;
                            tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, (Circle_INFO)tempCircle);
                        }
                    }
                    else
                    {
                        coord = new Coordinate_INFO();
                        tempCircle = (Circle_INFO)m_DrawROI;
                        tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, (Circle_INFO)tempCircle);
                    }


                    m_ResultXLD.GenCircleContourXld(((Circle_INFO)tempCircle).CenterY, ((Circle_INFO)tempCircle).CenterX,
                        ((Circle_INFO)tempCircle).Radius, 0, 6.28318, "positive", 1);

                }


                DataVar objRegion = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.区域.ToString(),
                    DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_ResultRegion);

                ModuleProject.UpdateLocalVarValue(objRegion);

                ModuleROI roi检测范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                   ModuleParam.ModuleDesCribe, ModuleRoiType.检测范围.ToString(), "blue", new HObject(m_ResultXLD), true);
                m_Image.UpdateRoiList(roi检测范围);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 仿射变换

        /// <summary>
        /// 模块测量方法
        /// </summary>
        /// <param name="in_Circle"></param>
        public void VerifyParam(ROI _roi)
        {
            try
            {
                //ROI为矩形
                if (ModelModel == RoIType.矩形)
                {
                    Rectangle2_INFO DrawRect2 = new Rectangle2_INFO();

                    //像素坐标转世界坐标
                    DrawRect2 = SysVisionCore.Rect2WorldPlane(m_Image, (Rectangle2_INFO)_roi);

                    if (m_LinkDataName != null)
                    {
                        if (m_LinkDataName.Length > 0)
                        {
                            LinkAffineCorre = ((List<Coordinate_INFO>)Link_Affine_data.m_DataValue)[0];

                            ////世界坐标转当前相对坐标
                            HHomMat2D hom1 = VBA_Function.setOrig(LinkAffineCorre.X, LinkAffineCorre.Y, -LinkAffineCorre.Phi);
                            DrawRect2.CenterX = hom1.AffineTransPoint2d(DrawRect2.CenterX, DrawRect2.CenterY, out DrawRect2.CenterY);

                            RegCoor = new Coordinate_INFO(DrawRect2.CenterY, DrawRect2.CenterX, LinkAffineCorre.Phi);

                            m_DrawROI = DrawRect2;

                        }
                        else
                        {
                            ////世界坐标转当前相对坐标
                            HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                            DrawRect2.CenterX = hom1.AffineTransPoint2d(DrawRect2.CenterX, DrawRect2.CenterY, out DrawRect2.CenterY);
                            m_DrawROI = DrawRect2;
                        }
                    }
                    else
                    {
                        ////世界坐标转当前相对坐标
                        HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                        DrawRect2.CenterX = hom1.AffineTransPoint2d(DrawRect2.CenterX, DrawRect2.CenterY, out DrawRect2.CenterY);
                        m_DrawROI = DrawRect2;
                    }
                }
                //ROI为圆形
                else if (ModelModel == RoIType.圆形)
                {
                    Circle_INFO DrawCircle = new Circle_INFO();

                    //像素坐标转世界坐标
                    DrawCircle = SysVisionCore.Circle2WorldPlane(m_Image, (Circle_INFO)_roi);

                    if (m_LinkDataName != null)
                    {
                        if (m_LinkDataName.Length > 0)
                        {
                            LinkAffineCorre = ((List<Coordinate_INFO>)Link_Affine_data.m_DataValue)[0];

                            ////世界坐标转当前相对坐标
                            HHomMat2D hom1 = VBA_Function.setOrig(LinkAffineCorre.X, LinkAffineCorre.Y, -LinkAffineCorre.Phi);
                            DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                            m_DrawROI = DrawCircle;
                        }
                        else
                        {
                            ////世界坐标转当前相对坐标
                            HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                            DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                            m_DrawROI = DrawCircle;
                        }
                    }
                    else
                    {
                        ////世界坐标转当前相对坐标
                        HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                        DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                        m_DrawROI = DrawCircle;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
            }

        }

        #endregion

    }

    public enum DrawROI
    {
        矩形,
        圆形
    }

}
