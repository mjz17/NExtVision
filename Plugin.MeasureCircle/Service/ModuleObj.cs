using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using ModuleDataVar;
using DefineImgRoI;
using HalconDotNet;
using PublicDefine;
using System.Runtime.Serialization;
using Common;

namespace Plugin.MeasureCircle
{
    [Category("几何测量")]
    [DisplayName("圆形测量")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 链接图像名称
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
        /// 链接位置补正信息
        /// </summary>
        public DataVar Link_Affine_data;

        /// <summary>
        /// 输入链接坐标/圆心坐标
        /// </summary>
        public Coordinate_INFO LinkAffineCorre = new Coordinate_INFO();

        /// <summary>
        /// 绘制圆信息
        /// </summary>
        public Circle_INFO m_DrawCircle = new Circle_INFO();

        /// <summary>
        /// 测量信息
        /// </summary>
        public Metrology_INFO m_MetrologyInfo = new Metrology_INFO();

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 是否有屏蔽区域
        /// </summary>
        public bool isCorrect = false;

        public HHomMat2D tempMat2D;

        [NonSerialized]
        public HXLDCont m_LinkAffineCross = new HXLDCont();

        [NonSerialized]
        public HXLDCont m_MeasureXLD = new HXLDCont();//检测形态轮廓

        [NonSerialized]
        public HXLDCont m_MeasureCross = new HXLDCont();//检测点十字

        [NonSerialized]
        public HXLDCont m_ResultXLD = new HXLDCont();//检测结果轮廓

        [NonSerialized]
        public HXLDCont m_ArrowXLD = new HXLDCont();//检测方向

        [NonSerialized]
        protected HTuple m_Row = new HTuple();
        [NonSerialized]
        protected HTuple m_Col = new HTuple();

        /// <summary>
        /// 输出的圆
        /// </summary>
        private Circle_INFO _outCircleInfo = new Circle_INFO();

        public Circle_INFO m_OutCircle
        {
            get { return _outCircleInfo; }
        }

        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                //仿射变换后的屏蔽区域
                HRegion tempDisableRegion = new HRegion();
                MessureCircle(m_DrawCircle);
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

        public void MessureCircle(Circle_INFO circle)
        {
            Circle_INFO in_Circle = circle;
            Circle_INFO tempCircle = new Circle_INFO();
            HRegion tempDisableRegion = new HRegion();//仿射变换后的屏蔽区域

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

                //开始运行没有图像数据直接返回
                if (m_Image == null || m_Image.IsInitialized() == false)
                {
                    return;
                }

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
                        tempCircle = (Circle_INFO)m_DrawCircle.Clone();
                        tempCircle.CenterX = homMat2D.AffineTransPoint2d(m_DrawCircle.CenterX, m_DrawCircle.CenterY, out tempCircle.CenterY);
                        //tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, tempCircle);//世界坐标转图像坐标
                    }
                    else
                    {
                        coord = new Coordinate_INFO();
                        tempCircle = (Circle_INFO)m_DrawCircle.Clone();
                        //tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, m_DrawCircle);//世界坐标转图像坐标
                    }
                }
                else
                {
                    coord = new Coordinate_INFO();
                    tempCircle = (Circle_INFO)m_DrawCircle.Clone();
                    //tempCircle = SysVisionCore.Circle2PixelPlane(m_Image, m_DrawCircle);//世界坐标转图像坐标
                }

                //显示一个直接坐标系
                HXLDCont CoorXLD = SysVisionCore.GetCoord_Image(m_Image, coord);
                ModuleROI roi坐标系 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(), ModuleParam.ModuleDesCribe,
                    ModuleRoiType.参考坐标系.ToString(), "green", new HObject(CoorXLD));
                m_Image.UpdateRoiList(roi坐标系);

                SysVisionCore.MeasureCircle(m_Image, tempCircle, m_MetrologyInfo, tempDisableRegion,
                   out _outCircleInfo, out m_Row, out m_Col, out m_MeasureXLD);

                m_MeasureCross.GenCrossContourXld(m_Row, m_Col, (HTuple)m_MetrologyInfo.Length2, new HTuple(45).TupleRad());
                m_ResultXLD.GenCircleContourXld(_outCircleInfo.CenterY, _outCircleInfo.CenterX, _outCircleInfo.Radius, 0, 6.28318, "positive", 1);


                //模块对应数据(实际的图像坐标)
                {
                    //坐标X
                    DataVar data_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.CircleX图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, _outCircleInfo.CenterY);//为了对应输入2023.4.11
                    ModuleProject.UpdateLocalVarValue(data_x);

                    //坐标Y
                    DataVar data_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.CircleY图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, _outCircleInfo.CenterX);//为了对应输入2023.4.11
                    ModuleProject.UpdateLocalVarValue(data_y);

                    //坐标Radius
                    DataVar data_r = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.圆半径Radius.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, _outCircleInfo.Radius);
                    ModuleProject.UpdateLocalVarValue(data_r);
                }

                //模块对应数据（实际的图像/圆坐标）
                {
                    List<Circle_INFO> Circle_Image = new List<Circle_INFO>() { _outCircleInfo };
                    DataVar dataCircleI = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Circle图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Circle_Image);
                    ModuleProject.UpdateLocalVarValue(dataCircleI);
                }

                //模块对应数据（实际的世界/圆坐标）
                {
                    //图像坐标转世界坐标
                    Circle_INFO circleW = SysVisionCore.Circle2WorldPlane(m_Image, _outCircleInfo);
                    List<Circle_INFO> Circle_W = new List<Circle_INFO>() { circleW };

                    DataVar dataCircleW = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Circle世界.ToString(),
              DataVarType.DataType.Circle, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Circle_W);
                    ModuleProject.UpdateLocalVarValue(dataCircleW);

                }


                ModuleROI roi检测范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                       ModuleParam.ModuleDesCribe, ModuleRoiType.检测范围.ToString(), "blue", new HObject(m_MeasureXLD), m_IsDispOutRang);

                ModuleROI roi检测点 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe, ModuleRoiType.检测点.ToString(), "yellow", new HObject(m_MeasureCross), m_IsDispOutPoint);

                ModuleROI roi检测结果 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red", new HObject(m_ResultXLD), m_IsDispResult);

                ModuleROI roi屏蔽范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe, ModuleRoiType.屏蔽范围.ToString(), "red", new HObject(tempDisableRegion));

                m_Image.UpdateRoiList(roi检测范围);
                m_Image.UpdateRoiList(roi检测点);
                m_Image.UpdateRoiList(roi检测结果);
                m_Image.UpdateRoiList(roi屏蔽范围);
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
        public void VerifyParam(Circle_INFO in_Circle)
        {
            HRegion tempDisableRegion = new HRegion();//仿射变换后的屏蔽区域

            Circle_INFO DrawCircle = new Circle_INFO();

            //像素坐标转世界坐标
            //DrawCircle = SysVisionCore.Circle2WorldPlane(m_Image, in_Circle);
            DrawCircle = (Circle_INFO)in_Circle.Clone();

            if (m_LinkDataName != null)
            {
                if (m_LinkDataName.Length > 0)
                {
                    LinkAffineCorre = ((List<Coordinate_INFO>)Link_Affine_data.m_DataValue)[0];

                    ////世界坐标转当前相对坐标
                    HHomMat2D hom1 = VBA_Function.setOrig(LinkAffineCorre.X, LinkAffineCorre.Y, -LinkAffineCorre.Phi);
                    DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                    m_DrawCircle = DrawCircle;
                }
                else
                {
                    ////世界坐标转当前相对坐标
                    HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                    DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                    m_DrawCircle = DrawCircle;
                }
            }
            else
            {
                ////世界坐标转当前相对坐标
                HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                DrawCircle.CenterX = hom1.AffineTransPoint2d(DrawCircle.CenterX, DrawCircle.CenterY, out DrawCircle.CenterY);
                m_DrawCircle = DrawCircle;
            }
            MessureCircle(m_DrawCircle);
        }

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            m_LinkDataName = string.Empty;
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
