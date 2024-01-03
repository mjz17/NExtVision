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
using Common;
using System.Runtime.Serialization;
using static System.Windows.Forms.AxHost;

namespace Plugin.QrCord
{
    [Category("检测识别")]
    [DisplayName("二维码")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        #region 链接图片

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接图像信息
        /// </summary>
        public DataVar Link_Image_Data;

        #endregion

        #region 链接位置补正

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
        /// 模板基类
        /// </summary>
        public HHandle m_Model;

        /// <summary>
        /// 结果输出
        /// </summary>
        [NonSerialized]
        public string m_DataCode = string.Empty;

        /// <summary>
        /// 绘制矩形信息
        /// </summary>
        public Rectangle_INFO m_DrawInRect = new Rectangle_INFO();

        /// <summary>
        /// RoI集合
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 检查结果
        /// </summary>
        [NonSerialized]
        public HXLDCont m_ResultXld = new HXLDCont();

        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                //仿射变换后的屏蔽区域
                HRegion tempDisableRegion = new HRegion();
                Rectangle_INFO tempLine = AffineRect(m_DrawInRect);

                //创建二维码句柄
                if (!m_Model.IsInitialized())
                {
                    SysVisionCore.CreateCode2D(m_Image, ref m_Model, tempLine, "default_parameters", "maximum_recognition");
                    SysVisionCore.FindCode2D(m_Image, m_Model, tempLine, ref m_ResultXld, ref m_DataCode);
                }

                //查询二维码信息
                SysVisionCore.FindCode2D(m_Image, m_Model, tempLine, ref m_ResultXld, ref m_DataCode);

                //ROI搜索范围
                ModuleROI roi搜索范围 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                    ModuleParam.ModuleDesCribe.ToString(), ModuleRoiType.搜索范围.ToString(), "red", new HObject(tempLine.GenXld()));

                //二维码结果
                ModuleROI roi检测结果 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(),
                     ModuleParam.ModuleDesCribe, ModuleRoiType.检测结果.ToString(), "red", new HObject(m_ResultXld));

                m_Image.UpdateRoiList(roi搜索范围);
                m_Image.UpdateRoiList(roi检测结果);

                //模块结果输出变量地址
                {
                    DataVar data_Var = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.QrCord.ToString(),
               DataVarType.DataType.String, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_DataCode);
                    ModuleProject.UpdateLocalVarValue(data_Var);
                }

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

        public Rectangle_INFO AffineRect(Rectangle_INFO _INFO)
        {
            Rectangle_INFO m_InLine = _INFO;

            Rectangle_INFO tempLine = new Rectangle_INFO();

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

                    if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                    {
                        m_Image = ((List<HImageExt>)(data).m_DataValue)[0];

                        if (m_Image != null || m_Image.IsInitialized())
                        {
                            Coordinate_INFO coord = new Coordinate_INFO();

                            //获得读取二维码的矩形区域
                            if (m_LinkDataName != null)
                            {
                                if (m_LinkDataName.Length > 0)
                                {
                                    //查询坐标系
                                    DataVar data1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == Link_Affine_data.m_DataName &&
                                    c.m_DataModuleID == Link_Affine_data.m_DataModuleID);
                                    coord = ((List<Coordinate_INFO>)data1.m_DataValue)[0];

                                    HHomMat2D homMat2D = new HHomMat2D();
                                    homMat2D = homMat2D.HomMat2dRotateLocal(-coord.Phi);
                                    homMat2D = homMat2D.HomMat2dTranslate(coord.X, coord.Y);

                                    startX = homMat2D.AffineTransPoint2d(m_DrawInRect.StartX, m_DrawInRect.StartY, out startY);
                                    endX = homMat2D.AffineTransPoint2d(m_DrawInRect.EndX, m_DrawInRect.EndY, out endY);

                                    tempLine = new Rectangle_INFO(startY, startX, endY, endX);

                                }
                                else
                                {
                                    coord = new Coordinate_INFO();
                                    tempLine = new Rectangle_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);
                                    //tempLine = SysVisionCore.Line2PixelPlane(m_Image, m_InLine);//世界坐标转图像坐标
                                }

                            }
                            else
                            {
                                coord = new Coordinate_INFO();
                                tempLine = new Rectangle_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);
                                //tempLine = SysVisionCore.Line2PixelPlane(m_Image, m_InLine);//世界坐标转图像坐标
                            }

                            ModuleParam.BlnSuccessed = true;

                            return tempLine;

                        }
                        else
                        {
                            ModuleParam.BlnSuccessed = false;
                            return tempLine;
                        }
                    }
                    else
                    {
                        ModuleParam.BlnSuccessed = false;
                        return tempLine;
                    }
                }
                else
                {
                    ModuleParam.BlnSuccessed = false;
                    return tempLine;
                }
            }
            catch (Exception ex)
            {
                ModuleParam.BlnSuccessed = false;
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
                return tempLine;
            }
        }

        /// <summary>
        /// 坐标补正区域
        /// </summary>
        /// <param name="_INFO"></param>
        public void VerifyParam(Rectangle_INFO _INFO)
        {
            HRegion tempDisableRegion = new HRegion();//仿射变换后的屏蔽区域

            Rectangle_INFO m_DrawRect = new Rectangle_INFO(_INFO.StartY, _INFO.StartX, _INFO.EndY, _INFO.EndX);

            if (m_LinkDataName != null)//是否位置补正
            {
                if (m_LinkDataName.Length > 0)
                {
                    LinkAffineCorre = ((List<Coordinate_INFO>)Link_Affine_data.m_DataValue)[0];
                    //世界坐标转当前相对坐标，链接了位置补正
                    HHomMat2D hom1 = VBA_Function.setOrig(LinkAffineCorre.X, LinkAffineCorre.Y, -LinkAffineCorre.Phi);
                    m_DrawRect.StartX = hom1.AffineTransPoint2d(m_DrawRect.StartX, m_DrawRect.StartY, out m_DrawRect.StartY);
                    m_DrawRect.EndX = hom1.AffineTransPoint2d(m_DrawRect.EndX, m_DrawRect.EndY, out m_DrawRect.EndY);
                    m_DrawInRect = m_DrawRect;
                }
                else
                {
                    //世界坐标转当前相对坐标,不带位置补正
                    HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                    m_DrawRect.StartX = hom1.AffineTransPoint2d(m_DrawRect.StartX, m_DrawRect.StartY, out m_DrawRect.StartY);
                    m_DrawRect.EndX = hom1.AffineTransPoint2d(m_DrawRect.EndX, m_DrawRect.EndY, out m_DrawRect.EndY);
                    m_DrawInRect = m_DrawRect;
                }
            }
            else
            {
                //世界坐标转当前相对坐标,不带位置补正
                HHomMat2D hom1 = VBA_Function.setOrig(0, 0, 0);
                m_DrawRect.StartX = hom1.AffineTransPoint2d(m_DrawRect.StartX, m_DrawRect.StartY, out m_DrawRect.StartY);
                m_DrawRect.EndX = hom1.AffineTransPoint2d(m_DrawRect.EndX, m_DrawRect.EndY, out m_DrawRect.EndY);
                m_DrawInRect = m_DrawRect;
            }

            AffineRect(m_DrawInRect);
        }

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            m_ResultXld = new HXLDCont();//检查结果轮廓
        }
    }
}
