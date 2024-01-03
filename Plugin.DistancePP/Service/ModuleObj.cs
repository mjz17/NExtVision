using Common;
using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DistancePP
{

    [Category("几何测量")]
    [DisplayName("点点构建")]
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

        #region 输入点1.x

        /// <summary>
        /// 输入点1.x名称
        /// </summary>
        public string m_Point1_x = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Pont1_x_Data;

        /// <summary>
        /// 输入点1.x数据信息
        /// </summary>
        [NonSerialized]
        public double Pont1_x_Data = 0;

        #endregion

        #region 输入点1.y

        /// <summary>
        /// 输入点1.y名称
        /// </summary>
        public string m_Point1_y = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Pont1_y_Data;

        /// <summary>
        /// 输入点1.y数据信息
        /// </summary>
        [NonSerialized]
        public double Pont1_y_Data = 0;

        #endregion

        #region 输入点2.x

        /// <summary>
        /// 输入点2.x名称
        /// </summary>
        public string m_Point2_x = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Pont2_x_Data;

        /// <summary>
        /// 输入点2.x数据信息
        /// </summary>
        [NonSerialized]
        public double Pont2_x_Data = 0;

        #endregion

        #region 输入点2.y

        /// <summary>
        /// 输入点2.y名称
        /// </summary>
        public string m_Point2_y = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Pont2_y_Data;

        /// <summary>
        /// 输入点2.x数据信息
        /// </summary>
        [NonSerialized]
        public double Pont2_y_Data = 0;

        #endregion

        /// <summary>
        /// 是否使用转换
        /// </summary>
        public bool ConverValue = false;

        //中心
        [NonSerialized]
        public string centerResult = string.Empty;

        //交点row，col
        [NonSerialized]
        double m_row = 0;
        double m_col = 0;
        //角度
        [NonSerialized]
        public double phiResult = 0.0;
        //距离
        [NonSerialized]
        public double disResult = 0.0;
        [NonSerialized]
        public double pointX1, pointY1, pointX2, pointY2;
        [NonSerialized]
        Line_INFO out_Line;

        //执行模块
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                //初始化输出数值
                m_row = 0;
                m_col = 0;
                phiResult = 0;
                disResult = 0;

                centerResult = string.Empty;

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
                            //加载点1.x信息
                            {
                                if (m_Point1_x != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont1_x_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont1_x_Data.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_Pont1_x_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        pointX1 = Pont1_x_Data = (double)Link_Pont1_x_Data.m_DataValue;
                                    }
                                }
                            }
                            //加载点1.y信息
                            {
                                if (m_Point1_y != null)
                                {
                                    int Info2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont1_y_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont1_y_Data.m_DataModuleID);
                                    if (Info2 > -1)
                                    {
                                        //加载数据
                                        Link_Pont1_y_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info2];
                                        pointY1 = Pont1_y_Data = (double)Link_Pont1_y_Data.m_DataValue;
                                    }
                                }
                            }
                            //加载点2.x信息
                            {
                                if (m_Point2_x != null)
                                {
                                    int Info3 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont2_x_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont2_x_Data.m_DataModuleID);
                                    if (Info3 > -1)
                                    {
                                        //加载数据
                                        Link_Pont2_x_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info3];
                                        pointX2 = Pont2_x_Data = (double)Link_Pont2_x_Data.m_DataValue;
                                    }
                                }
                            }
                            //加载点2.y信息
                            {
                                if (m_Point2_y != null)
                                {
                                    int Info4 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont2_y_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont2_y_Data.m_DataModuleID);
                                    if (Info4 > -1)
                                    {
                                        //加载数据
                                        Link_Pont2_y_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info4];
                                        pointY2 = Pont2_y_Data = (double)Link_Pont2_y_Data.m_DataValue;
                                    }
                                }
                            }

                            //点点中心
                            HRegion region = new HRegion();
                            region.GenRegionLine(Pont1_x_Data, Pont1_y_Data, Pont2_x_Data, Pont2_y_Data);
                            region.AreaCenter(out m_row, out m_col);
                            centerResult = m_col.ToString("0.00") + " / " + m_row.ToString("0.00");

                            //region.GenRegionLine(Pont1_y_Data, Pont1_x_Data, Pont2_y_Data, Pont2_x_Data);

                            //点点角度
                            phiResult = HMisc.AngleLx(Pont1_x_Data, Pont1_y_Data, Pont2_x_Data, Pont2_y_Data);//不是特别准确//后期改成反向正切

                            //点点构成一个直线
                            out_Line = new Line_INFO(Pont1_x_Data, Pont1_y_Data, Pont2_x_Data, Pont2_y_Data);

                            DataVar data_LineI = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Line图像.ToString(),
                  DataVarType.DataType.Line, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, out_Line);
                            ModuleProject.UpdateLocalVarValue(data_LineI);

                            //点点角度
                            //phiResult = HMisc.AngleLx(Pont1_y_Data, Pont1_x_Data, Pont2_y_Data, Pont2_x_Data);//不是特别准确//后期改成反向正切

                            //图像坐标转世界坐标
                            //点点距离
                            //是否使用转换
                            if (!ConverValue)
                            {
                                SysVisionCore.Pixel2WorldPlane(m_Image, Pont1_y_Data, Pont1_x_Data, out Pont1_y_Data, out Pont1_x_Data);
                                SysVisionCore.Pixel2WorldPlane(m_Image, Pont2_y_Data, Pont2_x_Data, out Pont2_y_Data, out Pont2_x_Data);
                            }

                            //disResult = VBA_Function.DistancePP(Pont1_y_Data, Pont1_x_Data, Pont2_y_Data, Pont2_x_Data);

                            disResult = VBA_Function.DistancePP(Pont1_x_Data, Pont1_y_Data, Pont2_x_Data, Pont2_y_Data);


                            ////ROI搜索范围的轮廓
                            //ModuleROIText 结果 = new ModuleROIText(
                            //    ModuleParam.ModuleID.ToString(),
                            //    ModuleParam.ModuleName.ToString(),
                            //    ModuleParam.ModuleDesCribe.ToString(),
                            //    ModuleRoiType.文字显示.ToString(),
                            //    "blue",
                            //    ModuleParam.ModuleName + "：" + disResult.ToString("f3"),
                            //    24);

                            //m_Image.UpdateTxtList(结果);


                            //点点构建中心
                            {
                                //坐标X
                                DataVar m_row_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_row);
                                ModuleProject.UpdateLocalVarValue(m_row_Data);

                                //坐标Y
                                DataVar m_col_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_col);
                                ModuleProject.UpdateLocalVarValue(m_col_Data);
                            }

                            //点点构建角度
                            {
                                //坐标角度
                                DataVar m_phi_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Deg图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, phiResult);
                                ModuleProject.UpdateLocalVarValue(m_phi_Data);
                            }
                            //点点构建距离
                            {
                                //坐标距离
                                DataVar m_dis_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离图像.ToString(),
                           DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, disResult);
                                ModuleProject.UpdateLocalVarValue(m_dis_Data);
                            }

                            ModuleParam.BlnSuccessed = true;
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
                else
                {
                    ModuleParam.BlnSuccessed = false;
                }
            }
            catch (Exception ex)
            {
                //报错之后将存储的数值初始化掉/2023.3.29 赵一添加
                //点点构建中心
                {
                    //坐标X
                    DataVar m_row_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_row);
                    ModuleProject.UpdateLocalVarValue(m_row_Data);

                    //坐标Y
                    DataVar m_col_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_col);
                    ModuleProject.UpdateLocalVarValue(m_col_Data);
                }

                //点点构建角度
                {
                    //坐标角度
                    DataVar m_phi_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Deg图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, phiResult);
                    ModuleProject.UpdateLocalVarValue(m_phi_Data);
                }
                //点点构建距离
                {
                    //坐标距离
                    DataVar m_dis_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离图像.ToString(),
               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, disResult);
                    ModuleProject.UpdateLocalVarValue(m_dis_Data);
                }

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

    }
}
