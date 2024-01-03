using Common;
using HalconDotNet;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using VisionCore;

namespace Plugin.RobotCotrol
{
    [Category("坐标变换")]
    [DisplayName("机械手控制")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        /// <summary>
        /// 链接流程
        /// </summary>
        public string ProcessName { get; set; } = string.Empty;

        /// <summary>
        /// 拍照方式
        /// </summary>
        private CameraType _CameraType;

        public CameraType m_CameraType
        {
            get { return _CameraType; }
            set { _CameraType = value; }
        }

        #region 输入机械坐标X

        /// <summary>
        /// 输入机械坐标X
        /// </summary>
        public string m_InputMachX = string.Empty;

        /// <summary>
        /// 输入机械坐标X
        /// </summary>
        public DataVar Link_InputMachX;

        /// <summary>
        /// 输入机械坐标X
        /// </summary>
        [NonSerialized]
        public double InputMachX_Data = 0;

        #endregion
        #region 输入机械坐标Y

        /// <summary>
        /// 输入机械坐标Y
        /// </summary>
        public string m_InputMachY = string.Empty;

        /// <summary>
        /// 输入机械坐标Y
        /// </summary>
        public DataVar Link_InputMachY;

        /// <summary>
        /// 输入机械坐标Y
        /// </summary>
        [NonSerialized]
        public double InputMachY_Data = 0;

        #endregion
        #region 输入机械坐标角度

        /// <summary>
        /// 输入机械坐标角度
        /// </summary>
        public string m_InputMachPhi = string.Empty;

        /// <summary>
        /// 输入机械坐标角度
        /// </summary>
        public DataVar Link_InputMachPhi;

        /// <summary>
        /// 输入机械坐标角度
        /// </summary>
        [NonSerialized]
        public double InputMachPhi_Data = 0;

        #endregion

        #region 输入图像坐标X

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public string m_InputImgX = string.Empty;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public DataVar Link_InputImgX;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        [NonSerialized]
        public double InputImgX_Data = 0;

        #endregion
        #region 输入图像坐标Y

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public string m_InputImgY = string.Empty;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        public DataVar Link_InputImgY;

        /// <summary>
        /// 图像坐标X
        /// </summary>
        [NonSerialized]
        public double InputImgY_Data = 0;

        #endregion
        #region 输入角度

        /// <summary>
        /// 输入角度
        /// </summary>
        public string m_InputPhi = string.Empty;

        /// <summary>
        /// 输入角度
        /// </summary>
        public DataVar Link_InputPhi;

        /// <summary>
        /// 输入角度
        /// </summary>
        [NonSerialized]
        public double InputPhi_Data = 0;

        #endregion

        #region 平移X（mm）

        /// <summary>
        /// 平移X
        /// </summary>
        public string m_InputTranX = string.Empty;

        /// <summary>
        /// 平移X
        /// </summary>
        public DataVar Link_InputTranX;

        /// <summary>
        /// 平移X
        /// </summary>
        [NonSerialized]
        public double InputTranX_Data = 0;

        #endregion
        #region 平移Y（mm）

        /// <summary>
        /// 平移Y
        /// </summary>
        public string m_InputTranY = string.Empty;

        /// <summary>
        /// 平移Y
        /// </summary>
        public DataVar Link_InputTranY;

        /// <summary>
        /// 平移Y
        /// </summary>
        [NonSerialized]
        public double InputTranY_Data = 0;

        #endregion
        #region 补偿角度（mm）

        /// <summary>
        /// 平移Y
        /// </summary>
        public string m_InputSupple_Angle = string.Empty;

        /// <summary>
        /// 平移Y
        /// </summary>
        public DataVar Link_InputSupple_Angle;

        /// <summary>
        /// 平移Y
        /// </summary>
        [NonSerialized]
        public double InputSupple_AngleData = 0;

        #endregion

        #region 参考坐标X

        /// <summary>
        /// 参考坐标X
        /// </summary>
        public string m_InputReferenceX = string.Empty;

        /// <summary>
        /// 参考坐标X
        /// </summary>
        public DataVar Link_InputReferenceX;

        /// <summary>
        /// 参考坐标X
        /// </summary>
        [NonSerialized]
        public double InputReferenceX_Data = 0;

        #endregion
        #region 参考坐标Y

        /// <summary>
        /// 参考坐标X
        /// </summary>
        public string m_InputReferenceY = string.Empty;

        /// <summary>
        /// 参考坐标X
        /// </summary>
        public DataVar Link_InputReferenceY;

        /// <summary>
        /// 参考坐标X
        /// </summary>
        [NonSerialized]
        public double InputReferenceY_Data = 0;

        #endregion
        #region 参考坐标Phi

        /// <summary>
        /// 参考坐标Phi
        /// </summary>
        public string m_InputReferencePhi = string.Empty;

        /// <summary>
        /// 参考坐标Phi
        /// </summary>
        public DataVar Link_InputReferencePhi;

        /// <summary>
        /// 参考坐标Phi
        /// </summary>
        [NonSerialized]
        public double InputReferencePhi_Data = 0;

        #endregion
        #region 接受角度差

        /// <summary>
        /// 接受角度差
        /// </summary>
        public string m_InputAcceptPhi = string.Empty;

        /// <summary>
        /// 接受角度差
        /// </summary>
        public DataVar Link_InputAcceptPhi;

        /// <summary>
        /// 接受角度差
        /// </summary>
        [NonSerialized]
        public double InputAccept_Data = 0;

        #endregion

        [NonSerialized]
        private RobotCommon robot = new RobotCommon();

        //机械坐标X
        [NonSerialized]
        public double m_OutX = 0;

        //机械坐标Y
        [NonSerialized]
        public double m_OutY = 0;

        //机械坐标Phi
        [NonSerialized]
        public double m_OutPhi = 0;

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
                    //链接N点标定数据
                    string[] Info = ProcessName.Split('.');
                    //获去流程
                    int ProcessNum = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == Info[0]);
                    if (ProcessNum > -1)
                    {
                        //模块ID
                        int ModuleName = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList.FindIndex(c => c.ModuleParam.PluginName.Contains(Info[1]));
                        if (ModuleName > -1)
                        {
                            //获取模块ID
                            int ModuleID = SysProcessPro.g_ProjectList[ProcessNum].m_ModuleObjList[ModuleName].ModuleParam.ModuleID;
                            if (ModuleID > -1)
                            {
                                #region 查询N点标定模块的数据

                                //获取矩阵变换
                                DataVar HomMat2D = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.HomMat2D.ToString()));
                                HHomMat2D hHom = ((List<HHomMat2D>)(HomMat2D).m_DataValue)[0];
                                //HHomMat2D hHom = (HHomMat2D)HomMat2D.m_DataValue;

                                //使用旋转中心
                                DataVar objEnOrNot = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.使用旋转中心.ToString()));
                                bool EnOrNot = (bool)objEnOrNot.m_DataValue;

                                //旋转中心X(图像坐标)
                                DataVar CircleX = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.Rotation_I_X.ToString()));
                                double Center_X = (double)CircleX.m_DataValue;

                                //旋转中心Y(图像坐标)
                                DataVar CircleY = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.Rotation_I_Y.ToString()));
                                double Center_Y = (double)CircleY.m_DataValue;

                                //旋转中心X（机械坐标）
                                DataVar CircleX_W = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.Rotation_M_X.ToString()));
                                double Center_X_W = (double)CircleX_W.m_DataValue;

                                //旋转中心Y（机械坐标）
                                DataVar CircleY_W = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.Rotation_M_Y.ToString()));
                                double Center_Y_W = (double)CircleY_W.m_DataValue;

                                //基准角度
                                DataVar Criterion_Phi = SysProcessPro.g_ProjectList[ProcessNum].m_Var_List.Find(C => C.m_DataModuleID == ModuleID &&
                                C.m_DataName.Contains(ConstantVar.Criterion_Phi.ToString()));
                                double InputPhi = (double)Criterion_Phi.m_DataValue;

                                #endregion

                                //获取输入的图像X坐标
                                if (m_InputImgX != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputImgX.m_DataName &&
                                    c.m_DataModuleID == Link_InputImgX.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_InputImgX = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        InputImgX_Data = (double)Link_InputImgX.m_DataValue;
                                    }
                                }
                                //获取输入的图像Y坐标
                                if (m_InputImgY != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputImgY.m_DataName &&
                                    c.m_DataModuleID == Link_InputImgY.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_InputImgY = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        InputImgY_Data = (double)Link_InputImgY.m_DataValue;
                                    }
                                }
                                //输入角度
                                if (m_InputPhi != null)
                                {
                                    int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputPhi.m_DataName &&
                                    c.m_DataModuleID == Link_InputPhi.m_DataModuleID);
                                    if (Info1 > -1)
                                    {
                                        //加载数据
                                        Link_InputPhi = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                        InputPhi_Data = (double)Link_InputPhi.m_DataValue;
                                    }
                                }

                                //拍照模式
                                if (m_CameraType == CameraType.固定相机先拍再取或放)
                                {
                                    //带旋转中心的坐标
                                    if (EnOrNot)
                                    {
                                        //输出角度等于 基准角度+输入角度
                                        m_OutPhi = InputPhi + InputPhi_Data;
                                        robot.AffineTransRotate(hHom, Center_X, Center_Y, Center_X_W, Center_Y_W, InputImgX_Data, InputImgY_Data, out m_OutX, out m_OutY);
                                    }
                                    else //不带旋转中心的坐标
                                    {
                                        //输出角度等于 基准角度+输入角度
                                        m_OutPhi = InputPhi + InputPhi_Data;
                                        robot.AffineTrans(hHom, InputImgX_Data, InputImgY_Data, out m_OutX, out m_OutY);
                                    }
                                }
                                else if (m_CameraType == CameraType.固定相机抓取后拍照)
                                {
                                    //输入机械坐标X
                                    if (m_InputMachX != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputMachX.m_DataName &&
                                        c.m_DataModuleID == Link_InputMachX.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputMachX = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputMachX_Data = (double)Link_InputMachX.m_DataValue;
                                        }
                                    }
                                    //输入机械坐标Y
                                    if (m_InputMachY != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputMachY.m_DataName &&
                                        c.m_DataModuleID == Link_InputMachY.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputMachY = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputMachY_Data = (double)Link_InputMachY.m_DataValue;
                                        }
                                    }
                                    //输入机械坐标角度
                                    if (m_InputMachPhi != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputMachPhi.m_DataName &&
                                        c.m_DataModuleID == Link_InputMachPhi.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputMachPhi = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputMachPhi_Data = (double)Link_InputMachPhi.m_DataValue;
                                        }
                                    }
                                    //参考坐标X
                                    if (m_InputReferenceX != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputReferenceX.m_DataName &&
                                        c.m_DataModuleID == Link_InputReferenceX.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputReferenceX = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputReferenceX_Data = (double)Link_InputReferenceX.m_DataValue;
                                        }
                                    }
                                    //参考坐标Y
                                    if (m_InputReferenceY != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputReferenceY.m_DataName &&
                                        c.m_DataModuleID == Link_InputReferenceY.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputReferenceY = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputReferenceY_Data = (double)Link_InputReferenceY.m_DataValue;
                                        }
                                    }
                                    //参考坐标Phi
                                    if (m_InputReferencePhi != null)
                                    {
                                        int Info1 = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataName == Link_InputReferencePhi.m_DataName &&
                                        c.m_DataModuleID == Link_InputReferencePhi.m_DataModuleID);
                                        if (Info1 > -1)
                                        {
                                            //加载数据
                                            Link_InputReferencePhi = SysProcessPro.g_ProjectList[proIndex].m_Var_List[Info1];
                                            InputReferencePhi_Data = (double)Link_InputReferencePhi.m_DataValue;
                                        }
                                    }

                                    //输出角度等于 基准角度+输入角度
                                    m_OutPhi = InputPhi + InputPhi_Data;
                                    robot.AffineTransRotate(hHom, Center_X, Center_Y, InputImgX_Data, InputImgY_Data, InputPhi_Data,
                                        InputReferenceX_Data, InputReferenceY_Data, InputMachX_Data, InputMachY_Data, out m_OutX, out m_OutY);
                                }

                                //输出坐标X
                                {
                                    DataVar data_X = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.输出坐标X.ToString(),
                                       DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutX);
                                    ModuleProject.UpdateLocalVarValue(data_X);
                                }

                                //输出坐标Y
                                {
                                    DataVar data_Y = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.输出坐标Y.ToString(),
                                       DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, m_OutY);
                                    ModuleProject.UpdateLocalVarValue(data_Y);
                                }

                                //输出角度
                                {
                                    DataVar data_Phi = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.输出Deg.ToString(),
                               DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, m_OutPhi);
                                    ModuleProject.UpdateLocalVarValue(data_Phi);
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

        [OnDeserializing()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            robot = new RobotCommon();
        }
    }

    public enum CameraType
    {
        固定相机先拍再取或放,
        固定相机抓取后拍照,
        运动相机先拍再取或放,
    }

}
