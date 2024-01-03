using DefineImgRoI;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using Common;
using System.Runtime.Serialization;
using System.Windows;

namespace Plugin.NpointsCalibration
{
    [Category("坐标变换")]
    [DisplayName("N点标定")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        #region 图像信息

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接图像信息
        /// </summary>
        public DataVar Link_Image_Data;

        #endregion
        #region 标定方式


        /// <summary>
        /// 标定方式
        /// </summary>
        private CalibraMethod _CalibraMethod;

        public CalibraMethod m_CalibraMethod
        {
            get { return _CalibraMethod; }
            set { _CalibraMethod = value; }
        }

        #endregion
        #region 图像坐标X

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public string m_ImgX = string.Empty;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public DataVar Link_ImgX;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        [NonSerialized]
        public double ImgX_Data = 0;

        #endregion
        #region 图像坐标Y

        /// <summary>
        /// 图像坐标Y
        /// </summary>
        public string m_ImgY = string.Empty;

        /// <summary>
        /// 图像坐标Y
        /// </summary>
        public DataVar Link_ImgY;

        /// <summary>
        /// 图像坐标Y
        /// </summary>
        [NonSerialized]
        public double ImgY_Data = 0;

        #endregion
        #region 相机安装方式

        /// <summary>
        /// 相机安装方式
        /// </summary>
        private CameraInstall _CameraInstall;

        public CameraInstall m_CameraInstall
        {
            get { return _CameraInstall; }
            set { _CameraInstall = value; }
        }

        #endregion
        #region 是否启用旋转中心

        /// <summary>
        /// 是否启用旋转中心
        /// </summary>
        private bool _EnableorNot;

        public bool m_EnableorNot
        {
            get { return _EnableorNot; }
            set { _EnableorNot = value; }
        }

        #endregion
        #region X移动距离

        /// <summary>
        /// X移动距离
        /// </summary>
        private double _XDis;

        public double m_XDis
        {
            get { return _XDis; }
            set { _XDis = value; }
        }

        #endregion
        #region Y移动距离

        /// <summary>
        /// Y移动距离
        /// </summary>
        private double _YDis;

        public double m_YDis
        {
            get { return _YDis; }
            set { _YDis = value; }
        }

        #endregion
        #region X基准点

        /// <summary>
        /// X基准点
        /// </summary>
        private double _XCriterion;

        public double m_XCriterion
        {
            get { return _XCriterion; }
            set { _XCriterion = value; }
        }

        #endregion
        #region Y基准点

        /// <summary>
        /// Y基准点
        /// </summary>
        private double _YCriterion;

        public double m_YCriterion
        {
            get { return _YCriterion; }
            set { _YCriterion = value; }
        }

        #endregion
        #region 角度基准点

        /// <summary>
        /// 角度基准点
        /// </summary>
        private double _PhiCriterion;

        public double m_PhiCriterion
        {
            get { return _PhiCriterion; }
            set { _PhiCriterion = value; }
        }

        #endregion
        #region 角度取反

        /// <summary>
        /// 角度取反
        /// </summary>
        private bool _PhiNegation;

        public bool m_PhiNegation
        {
            get { return _PhiNegation; }
            set { _PhiNegation = value; }
        }

        #endregion

        //两点法标定旋转中心
        #region 是否启用旋转中心

        /// <summary>
        /// 是否启用旋转中心
        /// </summary>
        private bool _EnableTwoPoint;

        public bool m_EnableTwoPoint
        {
            get { return _EnableTwoPoint; }
            set { _EnableTwoPoint = value; }
        }

        #endregion
        #region 输入坐标X1

        public double InputX1 = 0;

        #endregion
        #region 输入坐标Y1

        public double InputY1 = 0;

        #endregion
        #region 输入坐标X2

        public double InputX2 = 0;

        #endregion
        #region 输入坐标Y2

        public double InputY2 = 0;

        #endregion
        #region 输入角度

        public double InputPhi = 0;

        #endregion

        //机械和图像坐标
        public List<PointCoord> PonitList = new List<PointCoord>();

        //映射矩阵
        public HHomMat2D m_Mat2D = new HHomMat2D();
        //旋转中心X
        public double RotationCenter_X;
        //旋转中心Y
        public double RotationCenter_Y;
        //旋转中心X世界坐标
        public double RotationCenter_X_W;
        //旋转中心Y世界坐标
        public double RotationCenter_Y_W;

        /// <summary>
        /// 运行模块
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
                    //判断标定方式
                    if (m_CalibraMethod == CalibraMethod.自动)
                    {
                        //查询图像坐标X
                        if (m_ImgX != null)
                        {
                            int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_ImgX.m_DataName &&
                            c.m_DataModuleID == Link_ImgX.m_DataModuleID);
                            if (Info1 > -1)
                            {
                                //加载数据
                                Link_ImgX = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                ImgX_Data = (double)Link_ImgX.m_DataValue;
                            }
                        }

                        //查询图像坐标Y
                        if (m_ImgY != null)
                        {
                            int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_ImgY.m_DataName &&
                            c.m_DataModuleID == Link_ImgY.m_DataModuleID);
                            if (Info1 > -1)
                            {
                                //加载数据
                                Link_ImgY = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                ImgY_Data = (double)Link_ImgY.m_DataValue;
                            }
                        }

                        if (m_CalibraMethod == CalibraMethod.自动)
                        {
                            if (m_Index > 14)
                            {
                                m_Index = 0;
                            }

                            m_Index += 1;

                            //启用旋转中心
                            if (m_EnableorNot)
                            {
                                //添加数据
                                if (m_Index < 15)
                                {
                                    UpdatePointPram(ImgX_Data, ImgY_Data, m_Index);
                                }
                            }
                            else
                            {
                                //添加数据
                                if (m_Index < 10)
                                {
                                    UpdatePointPram(ImgX_Data, ImgY_Data, m_Index);
                                }
                            }               
                        }

                        //模块运行状态
                        {
                            DataVar Index = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.次数.ToString(),
                               DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_Index);
                            ModuleProject.UpdateLocalVarValue(Index);
                        }

                        ModuleParam.BlnSuccessed = true;

                    }
                    else//手动标定
                    {

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

        public void SavePram()
        {
            //HHomMat2D
            {
                DataVar HHomMat2D_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.HomMat2D.ToString(),
                    DataVarType.DataType.位置转换2D, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, new List<HHomMat2D>() { m_Mat2D });
                ModuleProject.UpdateLocalVarValue(HHomMat2D_Data);
            }

            //使用旋转中心
            {
                DataVar EnOrNot_Data = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.使用旋转中心.ToString(),
                   DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, m_EnableorNot);
                ModuleProject.UpdateLocalVarValue(EnOrNot_Data);
            }

            //旋转中心X(图像坐标)
            {
                DataVar Rotation_X = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Rotation_I_X.ToString(),
                    DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, RotationCenter_Y);
                ModuleProject.UpdateLocalVarValue(Rotation_X);
            }

            //旋转中心Y(图像坐标)
            {
                DataVar Rotation_Y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Rotation_I_Y.ToString(),
                    DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, RotationCenter_X);
                ModuleProject.UpdateLocalVarValue(Rotation_Y);
            }

            //旋转中心X(机械坐标)
            {
                DataVar Rotation_X_W = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Rotation_M_X.ToString(),
                    DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, RotationCenter_Y_W);
                ModuleProject.UpdateLocalVarValue(Rotation_X_W);
            }

            //旋转中心Y(机械坐标)
            {
                DataVar Rotation_Y_W = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Rotation_M_Y.ToString(),
                    DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, RotationCenter_X_W);
                ModuleProject.UpdateLocalVarValue(Rotation_Y_W);
            }

            //基准角度
            {
                DataVar data_Phi = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.Criterion_Phi.ToString(),
                    DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_PhiCriterion);
                ModuleProject.UpdateLocalVarValue(data_Phi);
            }

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
        }

        [NonSerialized]
        public int m_Index = 0;

        public void AddNpointsPram()
        {
            switch (PonitList.Count)
            {
                case 0:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 1,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 1:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 2,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion + m_XDis,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 2:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 3,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion + m_XDis,
                        m_Mach_y = m_YCriterion + m_YDis,
                    });
                    break;
                case 3:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 4,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion + m_YDis,
                    });
                    break;
                case 4:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 5,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion - m_XDis,
                        m_Mach_y = m_YCriterion + m_YDis,
                    });
                    break;
                case 5:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 6,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion - m_XDis,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 6:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 7,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion - m_XDis,
                        m_Mach_y = m_YCriterion - m_YDis,
                    });
                    break;
                case 7:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 8,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion - m_YDis,
                    });
                    break;
                case 8:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 9,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion + m_XDis,
                        m_Mach_y = m_YCriterion - m_YDis,
                    });
                    break;
                case 9:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 10,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 10:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 11,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 11:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 12,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 12:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 13,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 13:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 14,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                case 14:
                    PonitList.Add(new PointCoord
                    {
                        m_Index = 15,
                        m_ImageRow = 0,
                        m_ImageCol = 0,
                        m_Mach_x = m_XCriterion,
                        m_Mach_y = m_YCriterion,
                    });
                    break;
                default:
                    break;
            }
        }

        public void UpdatePointPram(double Img_x, double Img_y, int Index)
        {
            PointCoord point = PonitList.Find(c => c.m_Index == Index);
            if (point != null)
            {
                point.m_ImageRow = Img_x;
                point.m_ImageCol = Img_y;
            }
        }


        [OnDeserializing()]
        internal void OnDeserializingMethod(StreamingContext context)
        {

        }

        [OnDeserialized()]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            NpointCommon npoint = new NpointCommon();
            //启用旋转中心
            if (m_EnableorNot)
            {
                //14点标定
                PonitList = PonitList.FindAll(c => c.m_Index < 15);
                m_Mat2D = npoint.NpointTranRotation(PonitList, out RotationCenter_X, out RotationCenter_Y, out RotationCenter_X_W, out RotationCenter_Y_W);
            }
            else
            {
                //9点标定
                PonitList = PonitList.FindAll(c => c.m_Index < 10);
                m_Mat2D = npoint.NpointTran(PonitList);
            }
            SavePram();
        }

    }

    /// <summary>
    /// 标定方法
    /// </summary>
    public enum CalibraMethod
    {
        自动,
        手动,
    }

    /// <summary>
    /// 相机安装
    /// </summary>
    public enum CameraInstall
    {
        相机固定,
        相机移动,
    }

}
