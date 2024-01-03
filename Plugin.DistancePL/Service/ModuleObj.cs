using Common;
using DefineImgRoI;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.DistancePL
{

    [Category("几何测量")]
    [DisplayName("点线构建")]
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

        /// <summary>
        /// 输入点1.x图像数据信息
        /// </summary>
        [NonSerialized]
        public double Pont1_ImageX_Data = 0;

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

        /// <summary>
        /// 输入点1.x图像数据信息
        /// </summary>
        [NonSerialized]
        public double Pont1_ImageY_Data = 0;

        #endregion

        #region 输入直线.x

        /// <summary>
        /// 输入直线.x名称
        /// </summary>
        public string m_LinePoint_x = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_LinePont_x_Data;

        /// <summary>
        /// 输入直线.x数据信息
        /// </summary>
        [NonSerialized]
        public double LinePont_x_Data = 0;

        #endregion

        #region 输入直线.y

        /// <summary>
        /// 输入直线.x名称
        /// </summary>
        public string m_LinePoint_y = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_LinePont_y_Data;

        /// <summary>
        /// 输入直线.x数据信息
        /// </summary>
        [NonSerialized]
        public double LinePont_y_Data = 0;

        #endregion

        #region 输入直线.角度

        /// <summary>
        /// 输入点1名称
        /// </summary>
        public string m_LinePhi = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_m_LinePhi_Data;

        /// <summary>
        /// 直线1数据
        /// </summary>
        [NonSerialized]
        public double LinePhi_Data = 0;

        #endregion

        /// <summary>
        /// 是否使用转换
        /// </summary>
        public bool ConverValue = false;

        [NonSerialized]
        public double Droopfoot_dis_x;//垂足X

        [NonSerialized]
        public double Droopfoot_dis_y;//垂足Y

        [NonSerialized]
        public double Droopfoot_dis;//点到直线垂线距离

        [NonSerialized]
        public double Droopfoot_dis_x_w;//垂足X/世界

        [NonSerialized]
        public double Droopfoot_dis_y_w;//垂足Y/世界

        [NonSerialized]
        public double Droopfoot_dis_w;//点到直线垂线距离//世界

        [NonSerialized]
        public Line_INFO Line = new Line_INFO();

        //执行模块
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {
                //初始化
                Droopfoot_dis_x = 0.0;
                Droopfoot_dis_y = 0.0;
                Droopfoot_dis = 0.0;

                Droopfoot_dis_x_w = 0.0;
                Droopfoot_dis_y_w = 0.0;
                Droopfoot_dis_w = 0.0;

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
                            //加载点信息
                            {
                                //加载点1.x信息
                                if (m_Point1_x != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont1_x_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont1_x_Data.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_Pont1_x_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        Pont1_ImageX_Data = Pont1_x_Data = (double)Link_Pont1_x_Data.m_DataValue;
                                    }
                                }
                                //加载点1.y信息
                                if (m_Point1_y != null)
                                {
                                    int Info2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_Pont1_y_Data.m_DataName &&
                                    c.m_DataModuleID == Link_Pont1_y_Data.m_DataModuleID);
                                    if (Info2 > -1)
                                    {
                                        //加载数据
                                        Link_Pont1_y_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info2];
                                        Pont1_ImageY_Data = Pont1_y_Data = (double)Link_Pont1_y_Data.m_DataValue;
                                    }
                                }
                            }

                            //加载直线点信息
                            {
                                //加载直线.x信息
                                if (m_Point1_x != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_LinePont_x_Data.m_DataName &&
                                    c.m_DataModuleID == Link_LinePont_x_Data.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_LinePont_x_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        LinePont_x_Data = (double)Link_LinePont_x_Data.m_DataValue;
                                    }
                                }
                                //加载直线.y信息
                                if (m_Point1_y != null)
                                {
                                    int Info2 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_LinePont_y_Data.m_DataName &&
                                    c.m_DataModuleID == Link_LinePont_y_Data.m_DataModuleID);
                                    if (Info2 > -1)
                                    {
                                        //加载数据
                                        Link_LinePont_y_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info2];
                                        LinePont_y_Data = (double)Link_LinePont_y_Data.m_DataValue;
                                    }
                                }
                                //加载直线.Phi信息
                                if (m_LinePhi != null)
                                {
                                    int Info3 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_m_LinePhi_Data.m_DataName &&
                                    c.m_DataModuleID == Link_m_LinePhi_Data.m_DataModuleID);
                                    if (Info3 > -1)
                                    {
                                        //加载数据
                                        Link_m_LinePhi_Data = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info3];
                                        LinePhi_Data = (double)Link_m_LinePhi_Data.m_DataValue;
                                    }
                                }
                            }

                            //直线垂足，图像坐标
                            {
                                //根据输入中心点及角度，获取一个直线
                                VBA_Function.CreateLine(LinePont_x_Data, LinePont_y_Data, LinePhi_Data, out Line);

                                //垂线坐标
                                VBA_Function.DroopFootLine(Pont1_x_Data, Pont1_y_Data, Line, out Droopfoot_dis_x, out Droopfoot_dis_y);

                                //点点之间距离
                                Droopfoot_dis = VBA_Function.DistancePP(Pont1_x_Data, Pont1_y_Data, Droopfoot_dis_x, Droopfoot_dis_y);

                                DataVar dp_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
                                  DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_x);
                                ModuleProject.UpdateLocalVarValue(dp_x);

                                DataVar dp_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
                                   DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_y);
                                ModuleProject.UpdateLocalVarValue(dp_y);

                                DataVar dis = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离图像.ToString(),
                                 DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis);
                                ModuleProject.UpdateLocalVarValue(dis);
                            }

                            //直线垂足,世界坐标
                            {
                                Line_INFO line1 = new Line_INFO();
                                //根据输入中心点及角度，获取一个直线
                                VBA_Function.CreateLine(LinePont_x_Data, LinePont_y_Data, LinePhi_Data, out line1);

                                //垂线坐标
                                VBA_Function.DroopFootLine(Pont1_x_Data, Pont1_y_Data, line1, out Droopfoot_dis_x_w, out Droopfoot_dis_y_w);

                                //是否使用转换
                                if (!ConverValue)
                                {
                                    SysVisionCore.Pixel2WorldPlane(m_Image, Pont1_x_Data, Pont1_y_Data, out Pont1_x_Data, out Pont1_y_Data);
                                    SysVisionCore.Pixel2WorldPlane(m_Image, Droopfoot_dis_x_w, Droopfoot_dis_y_w, out Droopfoot_dis_x_w, out Droopfoot_dis_y_w);
                                }

                                //点点之间距离
                                Droopfoot_dis_w = VBA_Function.DistancePP(Pont1_x_Data, Pont1_y_Data, Droopfoot_dis_x_w, Droopfoot_dis_y_w);

                                DataVar dp_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X世界.ToString(),
                                  DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_x_w);
                                ModuleProject.UpdateLocalVarValue(dp_x);

                                DataVar dp_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y世界.ToString(),
                                   DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_y_w);
                                ModuleProject.UpdateLocalVarValue(dp_y);

                                DataVar dis = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离世界.ToString(),
                                 DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_w);
                                ModuleProject.UpdateLocalVarValue(dis);

                            }

                            double Middis = 0;

                            if (!ConverValue)
                            {
                                Middis = Droopfoot_dis_w;
                            }
                            else
                            {
                                Middis = Droopfoot_dis;
                            }


                            ////ROI搜索范围的轮廓
                            //ModuleROIText 结果 = new ModuleROIText(
                            //    ModuleParam.ModuleID.ToString(),
                            //    ModuleParam.ModuleName.ToString(),
                            //    ModuleParam.ModuleDesCribe.ToString(),
                            //    ModuleRoiType.文字显示.ToString(),
                            //    "red",
                            //    ModuleParam.ModuleName + "：" + Middis.ToString("f3"),
                            //    24);

                            //m_Image.UpdateTxtList(结果);

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
                DataVar dp_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X图像.ToString(),
                             DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_x);
                ModuleProject.UpdateLocalVarValue(dp_x);

                DataVar dp_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y图像.ToString(),
                   DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_y);
                ModuleProject.UpdateLocalVarValue(dp_y);

                DataVar dis = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离图像.ToString(),
                 DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis);
                ModuleProject.UpdateLocalVarValue(dis);

                DataVar dp_w_x = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标X世界.ToString(),
                            DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_x_w);
                ModuleProject.UpdateLocalVarValue(dp_w_x);

                DataVar dp_w_y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.坐标Y世界.ToString(),
                   DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_y_w);
                ModuleProject.UpdateLocalVarValue(dp_w_y);

                DataVar dis_w = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.距离世界.ToString(),
                 DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Droopfoot_dis_w);
                ModuleProject.UpdateLocalVarValue(dis_w);

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
    }
}
