using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using HalconDotNet;
using DefineImgRoI;
using PublicDefine;
using Common;
using ModuleDataVar;
using System.Runtime.Serialization;

namespace Plugin.TemplateMatch
{
    [Category("检测识别")]
    [DisplayName("模板匹配")]
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

        /// <summary>
        /// 使用模板类型
        /// </summary>
        public ModelType m_ModelType;

        /// <summary>
        /// 模板基类
        /// </summary>
        public HHandle m_Model;

        /// <summary>
        /// 创建模板参数
        /// </summary>
        public CreateMatchPram CreateMatch = new CreateMatchPram();

        /// <summary>
        /// 查询模板参数
        /// </summary>
        public FindMatchPram FindMatch = new FindMatchPram();

        /// <summary>
        /// 搜索区域
        /// </summary>
        public ROI m_SearchRegion;

        /// <summary>
        /// 搜索区域方式
        /// </summary>
        public RoILink SearchModel;

        /// <summary>
        /// 模板区域方式
        /// </summary>
        public RoIType ModelModel;

        /// <summary>
        /// 模板区域
        /// </summary>
        public ROI m_ModelRegion;

        /// <summary>
        /// 模板图片
        /// </summary>
        [NonSerialized]
        public HImageExt m_ModelImage = new HImageExt();

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 查询模板参数
        /// </summary>
        [NonSerialized]
        public FindMatchPram out_FindMathch = new FindMatchPram();

        /// <summary>
        /// 注册模板位置信息，像素坐标
        /// </summary>
        public Coordinate_INFO m_PositionInfo = new Coordinate_INFO();

        /// <summary>
        /// 屏蔽区域
        /// </summary>
        public ROI MatchdisableRegion;

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

                    if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                    {
                        m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                        if (m_Image != null || m_Image.IsInitialized())
                        {
                            if (m_Model == null)
                            {
                                //运行成功
                                ModuleParam.BlnSuccessed = false;
                                return;
                            }

                            //根据搜索区域切割图像
                            HImage m_SearchImage = m_Image.ReduceDomain(m_SearchRegion.GenRegion());

                            //模板基类
                            if (!m_Model.IsInitialized())
                            {
                                HImage m_ModuelImage = m_Image.ReduceDomain(m_ModelRegion.GenRegion()).CropDomain();
                                m_ModelImage = new HImageExt(m_ModuelImage);

                                SysVisionCore.CreateMatchModel(m_ModelType, m_ModelImage, ref m_Model, CreateMatch);//创建模板
                                SysVisionCore.FindMatchModel(m_ModelType, m_SearchImage, m_Model, FindMatch, out out_FindMathch);//查询模板
                                m_PositionInfo.Y = out_FindMathch.row;
                                m_PositionInfo.X = out_FindMathch.column;
                                m_PositionInfo.Phi = out_FindMathch.angle;
                            }

                            SysVisionCore.FindMatchModel(m_ModelType, m_SearchImage, m_Model, FindMatch, out out_FindMathch);//查询模板

                            //位置补正
                            //HHomMat2D hHomMat2D = new HHomMat2D();
                            //hHomMat2D.VectorAngleToRigid(m_PositionInfo.Y, m_PositionInfo.X, m_PositionInfo.Phi,
                            //    out_FindMathch.row[0].D, out_FindMathch.column[0].D, out_FindMathch.angle[0].D);

                            //ROI搜索范围的轮廓
                            ModuleROI roi搜索范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                                ModuleParam.ModuleDesCribe.ToString(), ModuleRoiType.搜索范围.ToString(), "blue", new HObject(m_SearchRegion.GenXld()), m_IsDispSearch);
                            m_Image.UpdateRoiList(roi搜索范围);

                            ////ROI模板范围的轮廓
                            //HXLDCont modelRegionXLD = m_ModelRegion.GenXld().AffineTransContourXld(hHomMat2D);
                            //ModuleROI roi检测范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                            //    ModuleParam.ModuleDesCribe.ToString(), ModuleRoiType.检测范围.ToString(), "yellow", new HObject(modelRegionXLD), m_IsDispDirect);
                            //m_Image.UpdateRoiList(roi检测范围);

                            //匹配结果//十字中心
                            HXLDCont cross = new HXLDCont();
                            cross.GenCrossContourXld(out_FindMathch.row, out_FindMathch.column, 36, out_FindMathch.angle);
                            ModuleROI roi检测点 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                                ModuleParam.ModuleDesCribe.ToString(), ModuleRoiType.检测点.ToString(), "red", new HObject(cross), m_IsDispResult);
                            m_Image.UpdateRoiList(roi检测点);

                            //模板轮廓显示
                            HRegion region = new HRegion();

                            HObject out_obj = new HObject();
                            out_obj.GenEmptyObj();

                            HXLDCont unit_xld = new HXLDCont();

                            for (int i = 0; i < out_FindMathch.row.Length; i++)
                            {
                                //模板轮廓显示
                                HHomMat2D tempMat2D = new HHomMat2D();
                                tempMat2D.VectorAngleToRigid(0, 0, 0, out_FindMathch.row[i].D, out_FindMathch.column[i].D, out_FindMathch.angle[i].D);

                                HXLDCont contour_xld = ((HShapeModel)this.m_Model).GetShapeModelContours(1);//2023.7.2赵一修改
                                contour_xld = contour_xld.AffineTransContourXld(tempMat2D);

                                out_obj = contour_xld.ConcatObj(out_obj);

                            }

                            ModuleROI roi检测结果 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                                  ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "cyan", out_obj, m_IsDispResult);
                            m_Image.UpdateRoiList(roi检测结果);


                            //模块对应数据(实际的图像坐标)
                            {
                                //坐标X
                                DataVar data_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, out_FindMathch.column[0].D);
                                ModuleProject.UpdateLocalVarValue(data_x);

                                //坐标X
                                DataVar data_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, out_FindMathch.row[0].D);
                                ModuleProject.UpdateLocalVarValue(data_y);

                                //坐标Deg//新的角度-注册的角度
                                DataVar data_p = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Deg图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, out_FindMathch.angle[0].D - m_PositionInfo.Phi);
                                ModuleProject.UpdateLocalVarValue(data_p);

                                //坐标X
                                List<double> x_array = new List<double>();
                                //坐标X
                                List<double> y_array = new List<double>();
                                //坐标Phi
                                List<double> phi_array = new List<double>();
                                //坐标Deg//新的角度-注册的角度
                                HTuple out_phi = out_FindMathch.angle - m_PositionInfo.Phi;

                                for (int i = 0; i < out_phi.Length; i++)
                                {
                                    x_array.Add(out_FindMathch.column[i]);
                                    y_array.Add(out_FindMathch.row[i]);
                                    phi_array.Add(out_phi[i]);
                                }

                                DataVar data_x_array = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像数组.ToString(),
                           DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, x_array);
                                ModuleProject.UpdateLocalVarValue(data_x_array);

                                DataVar data_y_array = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像数组.ToString(),
                           DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, y_array);
                                ModuleProject.UpdateLocalVarValue(data_y_array);

                                DataVar data_p_array = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Deg图像数组.ToString(),
                           DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, phi_array);
                                ModuleProject.UpdateLocalVarValue(data_p_array);
                            }
                            //运行成功
                            ModuleParam.BlnSuccessed = true;
                        }
                        else
                        {
                            //运行成功
                            ModuleParam.BlnSuccessed = false;
                        }
                    }
                    else
                    {
                        ModuleParam.BlnSuccessed = false;
                    }
                }
                else
                {
                    //运行成功
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

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {

        }

    }
}
