using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using Common;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using DefineImgRoI;

namespace Plugin.AffineTrans
{
    [Category("检测识别")]
    [DisplayName("位置补正")]
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

        #region 数据类型

        public bool m_DataType = false;

        #endregion

        #region 位置补正Row

        /// <summary>
        /// 位置补正Row
        /// </summary>
        public string m_Row = string.Empty;

        public DataVar Link_Row_data;

        #endregion

        #region 位置补正Colum

        /// <summary>
        /// 位置补正Colum
        /// </summary>
        public string m_Colum = string.Empty;

        public DataVar Link_Colum_data;

        #endregion

        #region 位置补正Phi

        /// <summary>
        /// 位置补正Phi
        /// </summary>
        public string m_Phi = string.Empty;

        public DataVar Link_Phi_data;

        #endregion

        #region 数组索引值

        /// <summary>
        /// 数组索引值
        /// </summary>
        public string m_Index = string.Empty;

        public DataVar Link_Index_data;

        #endregion

        /// <summary>
        /// 数组的索引
        /// </summary>
        [NonSerialized]
        public int m_ArrayNum = 0;

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
                    if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                    {
                        m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                        if (m_Image != null || m_Image.IsInitialized())
                        {
                            Coordinate_INFO coord = new Coordinate_INFO();

                            //位置补正Row
                            Link_Row_data = ModuleProject.GetLocalVarValue(Link_Row_data);
                            //位置补正Colum
                            Link_Colum_data = ModuleProject.GetLocalVarValue(Link_Colum_data);
                            //位置补正Phi
                            Link_Phi_data = ModuleProject.GetLocalVarValue(Link_Phi_data);

                            if (m_DataType)//数据类型为数组
                            {
                                //查询Index索引
                                Link_Index_data = ModuleProject.GetLocalVarValue(Link_Index_data);
                                if (Link_Index_data.m_DataValue is int)
                                {
                                    m_ArrayNum = (int)Link_Index_data.m_DataValue;
                                }

                                if (Link_Row_data.m_DataValue is List<double>)
                                {
                                    coord.X = ((List<double>)Link_Row_data.m_DataValue)[m_ArrayNum + 1];
                                }
                                if (Link_Colum_data.m_DataValue is List<double>)
                                {
                                    coord.Y = ((List<double>)Link_Colum_data.m_DataValue)[m_ArrayNum + 1];
                                }
                                if (Link_Phi_data.m_DataValue is List<double>)
                                {
                                    coord.Phi = ((List<double>)Link_Phi_data.m_DataValue)[m_ArrayNum + 1];
                                }

                            }
                            else//数据类型为单量
                            {
                                if (Link_Row_data.m_DataValue is double)
                                {
                                    coord.X = (double)Link_Row_data.m_DataValue;
                                }

                                if (Link_Colum_data.m_DataValue is double)
                                {
                                    coord.Y = (double)Link_Colum_data.m_DataValue;
                                }

                                if (Link_Phi_data.m_DataValue is double)
                                {
                                    coord.Phi = (double)Link_Phi_data.m_DataValue;
                                }

                            }

                            //窗体显示的坐标
                            HXLDCont CoorXLD = SysVisionCore.GetCoord_Image(m_Image, coord);
                            ModuleROI roi坐标系 = new ModuleROI(ModuleParam.ModuleID.ToString(), ModuleParam.ModuleName.ToString(), ModuleParam.ModuleDesCribe,
                                ModuleRoiType.参考坐标系.ToString(), "red", new HObject(CoorXLD), m_IsDispResult);
                            m_Image.UpdateRoiList(roi坐标系);

                            //存储数据
                            DataVar data_region = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Coord轮廓.ToString(),
                       DataVarType.DataType.轮廓, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, CoorXLD);
                            ModuleProject.UpdateLocalVarValue(data_region);

                            //模块对应数据（实际的图像/圆坐标）
                            {
                                object Coord_I = new List<Coordinate_INFO>() { new Coordinate_INFO(coord.Y, coord.X, coord.Phi) };
                                //存储数据
                                DataVar data_Var_CoordI = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Coord图像.ToString(),
                           DataVarType.DataType.坐标系, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Coord_I);
                                ModuleProject.UpdateLocalVarValue(data_Var_CoordI);
                            }

                            //模块对应数据（实际的世界/圆坐标）
                            {
                                //坐标转换为世界坐标
                                SysVisionCore.Points2WorldPlane(m_Image, coord.Y, coord.X, out coord.X, out coord.Y);
                                object Coord_W = new List<Coordinate_INFO>() { new Coordinate_INFO(coord.Y, coord.X, coord.Phi) };
                                //存储数据

                                DataVar data_Var_CoordW = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Coord世界.ToString(),
                          DataVarType.DataType.坐标系, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, Coord_W);
                                ModuleProject.UpdateLocalVarValue(data_Var_CoordW);

                            }

                            //索引增加
                            m_ArrayNum += 1;



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
