using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using ModuleDataVar;
using HalconDotNet;
using System.Xml.Linq;
using System.Windows.Markup;
using PublicDefine;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Plugin.ImageScript
{

    [Category("图像处理")]
    [DisplayName("图像脚本")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        /// <summary>
        /// 图像脚本对象
        /// </summary>
        public List<EProcedure> m_EProcedureList;

        /// <summary>
        /// 输入链接
        /// </summary>
        public List<Inputlink> m_Inputlink = new List<Inputlink>();

        /// <summary>
        /// 输出变量
        /// </summary>
        public List<OutputVar> m_OutputVar = new List<OutputVar>();

        /// <summary>
        /// 调用算子方法
        /// </summary>
        [NonSerialized]
        public HDevProcedureCall m_HDevProcedureCall;

        /// <summary>
        /// 执行算子名称
        /// </summary>
        public string RunMethod = string.Empty;

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            try
            {
                base.ExeModule(blnByHand);
                sw = new System.Diagnostics.Stopwatch();//模块运行时间
                sw.Start();

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //输入链接
                    if (m_Inputlink != null && m_Inputlink.Count > 0)
                    {
                        //输入图像，输入参数
                        foreach (Inputlink item in m_Inputlink)
                        {
                            switch (item.m_ImageScriptType)
                            {
                                case ImageScriptType.输入图像参数:
                                    DataVar data1 = ModuleProject.GetLocalVarValue(item.m_Link_Var);
                                    HImage m_Image = ((List<HImageExt>)(data1).m_DataValue)[0];
                                    m_HDevProcedureCall.SetInputIconicParamObject(item.m_LinkName, m_Image);
                                    break;
                                case ImageScriptType.输入变量参数:
                                    DataVar data2 = ModuleProject.GetLocalVarValue(item.m_Link_Var);
                                    m_HDevProcedureCall.SetInputCtrlParamTuple(item.m_LinkName, new HTuple(Convert.ToInt32(data2.m_DataValue)));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //模块运行失败
                        ModuleParam.BlnSuccessed = false;
                        InitOut();
                        return;
                    }

                    //执行图像脚本
                    m_HDevProcedureCall.Execute();

                    //输出链接
                    if (m_OutputVar != null && m_OutputVar.Count > 0)
                    {
                        DataVar objStatus;
                        foreach (OutputVar item in m_OutputVar)
                        {
                            switch (item.m_DataType)
                            {
                                case OutDataVarType.None:
                                    break;
                                case OutDataVarType.Bool:
                                    item.m_object = (bool)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.Int:
                                    item.m_object = (int)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.Double:
                                    item.m_object = (double)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.Double数组:
                                    item.m_object = (double[])m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.String:
                                    item.m_object = (string)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.String, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.图像:
                                    item.m_object = (HImage)m_HDevProcedureCall.GetOutputIconicParamImage(item.m_LinkName);
                                    {
                                        List<HImageExt> M_ImageList = new List<HImageExt>()
                                        {
                                            new HImageExt((HImage)item.m_object),
                                        };

                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.Image, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, M_ImageList);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.区域:
                                    item.m_object = (HRegion)m_HDevProcedureCall.GetOutputIconicParamRegion(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                case OutDataVarType.轮廓:
                                    item.m_object = (HXLD)m_HDevProcedureCall.GetOutputIconicParamXld(item.m_LinkName);
                                    {
                                        objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                            DataVarType.DataType.轮廓, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, item.m_object);
                                    }
                                    ModuleProject.UpdateLocalVarValue(objStatus);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //模块运行失败
                        ModuleParam.BlnSuccessed = false;
                        return;
                    }

                    //模块运行失败
                    ModuleParam.BlnSuccessed = true;

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
                InitOut();
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

        private void InitOut()
        {
            //清空数据
            DataVar objStatus;
            foreach (OutputVar item in m_OutputVar)
            {
                switch (item.m_DataType)
                {
                    case OutDataVarType.None:
                        break;
                    case OutDataVarType.Bool:
                        item.m_object = (bool)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, false);
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.Int:
                        item.m_object = (int)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, 0);
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.Double:
                        item.m_object = (double)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.Double, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, 0);
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.Double数组:
                        item.m_object = (double[])m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.Double_Array, DataVarType.DataGroup.数组, 1, "0", ModuleParam.ModuleName, 0);
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.String:
                        item.m_object = (string)m_HDevProcedureCall.GetOutputCtrlParamTuple(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.String, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, "0");
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.图像:
                        item.m_object = (HImage)m_HDevProcedureCall.GetOutputIconicParamImage(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.Image, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, new HImage());
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.区域:
                        item.m_object = (HRegion)m_HDevProcedureCall.GetOutputIconicParamRegion(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.区域, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, new HRegion());
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    case OutDataVarType.轮廓:
                        item.m_object = (HXLD)m_HDevProcedureCall.GetOutputIconicParamXld(item.m_LinkName);
                        {
                            objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, item.m_LinkName,
                                DataVarType.DataType.轮廓, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, new HXLD());
                        }
                        ModuleProject.UpdateLocalVarValue(objStatus);
                        break;
                    default:
                        break;
                }
            }
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {

            string tempName = Environment.GetEnvironmentVariable("TEMP") + $"/temp.hdev";

            EProcedure.SaveToFile(tempName, m_EProcedureList);

            HDevProgram program = new HDevProgram(tempName);

            //加载的方法名称 halcon导出后和文件名称一致 使用哪个算子
            HDevProcedure procedure = new HDevProcedure(program, RunMethod);

            m_HDevProcedureCall = new HDevProcedureCall(procedure);

            //用完就删 嘿嘿
            //System.IO.File.Delete(tempName);

        }

    }

    /// <summary>
    /// 输入链接
    /// </summary>
    [Serializable]
    public class Inputlink
    {

        /// <summary>
        /// 链接名称
        /// </summary>
        private string _LinkName;
        public string m_LinkName
        {
            get { return _LinkName; }
            set { _LinkName = value; }
        }

        /// <summary>
        /// 变量链接,只做显示
        /// </summary>
        private string _LinkData;
        public string m_LinkData
        {
            get { return _LinkData; }
            set { _LinkData = value; }
        }

        /// <summary>
        /// 变量类型
        /// </summary>
        private ImageScriptType _ImageScriptType;
        public ImageScriptType m_ImageScriptType
        {
            get { return _ImageScriptType; }
            set { _ImageScriptType = value; }
        }

        /// <summary>
        /// 变量值
        /// </summary>
        private DataVar _Link_Var;
        public DataVar m_Link_Var
        {
            get { return _Link_Var; }
            set { _Link_Var = value; }
        }

    }

    /// <summary>
    /// 输出变量
    /// </summary>
    [Serializable]
    public class OutputVar
    {
        /// <summary>
        /// 链接名称
        /// </summary>
        private string _LinkName;
        public string m_LinkName
        {
            get { return _LinkName; }
            set { _LinkName = value; }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        private OutDataVarType _DataType;
        public OutDataVarType m_DataType
        {
            get { return _DataType; }
            set { _DataType = value; }
        }

        /// <summary>
        /// 变量类型
        /// </summary>
        private ImageScriptType _ImageScriptType;
        public ImageScriptType m_ImageScriptType
        {
            get { return _ImageScriptType; }
            set { _ImageScriptType = value; }
        }

        /// <summary>
        /// 数据结果
        /// </summary>
        [NonSerialized]
        private object _object;
        public object m_object
        {
            get { return _object; }
            set { _object = value; }
        }

    }

    /// <summary>
    /// 图像脚本类型
    /// </summary>
    [Serializable]
    public enum ImageScriptType
    {
        输入图像参数,
        输入变量参数,
        输出图像参数,
        输出变量参数
    }

    /// <summary>
    /// 变量类型
    /// </summary>
    [Serializable]
    public enum OutDataVarType
    {
        None,
        Bool,
        Int,
        Double,
        Double数组,
        String,
        图像,
        区域,
        轮廓
    }

    /// <summary>
    /// 输出图像参数
    /// </summary>
    [Serializable]
    public enum OutImageVar
    {

    }

}
