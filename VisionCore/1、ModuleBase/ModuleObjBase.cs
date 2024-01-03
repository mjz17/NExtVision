using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CommunaCationPLC;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;

namespace VisionCore
{
    /// <summary>
    /// 模块单元抽象基类
    /// </summary>
    [Serializable]
    public abstract class ModuleObjBase : ICloneable
    {
        /// <summary>
        /// 模块单元参数
        /// </summary>
        public ModuleParam ModuleParam { get; set; } = new ModuleParam();

        /// <summary>
        /// 图像参数
        /// </summary>
        public ImageParam ModuleImageParam = new ImageParam();

        /// <summary>
        /// 记录当前图片
        /// </summary>
        [NonSerialized]
        public HImageExt m_Image = new HImageExt();

        /// <summary>
        /// 集合
        /// </summary>
        protected Project ModuleProject = null;

        /// <summary>
        /// 是否初次打开
        /// </summary>
        public bool blnNewModule = false;

        /// <summary>
        /// 模块运行时间
        /// </summary>
        [NonSerialized]
        public Stopwatch sw;

        /// <summary>
        /// 是否显示图像
        /// </summary>
        public bool m_IsDispImage { get; set; } = false;

        /// <summary>
        /// 是否显示搜索范围
        /// </summary>
        public bool m_IsDispSearch { get; set; } = false;

        /// <summary>
        /// 是否显示搜索方向
        /// </summary>
        public bool m_IsDispDirect { get; set; } = false;

        /// <summary>
        /// 是否显示搜索范围
        /// </summary>
        public bool m_IsDispDisable { get; set; } = false;

        /// <summary>
        /// 是否检测点
        /// </summary>
        public bool m_IsDispOutPoint { get; set; } = false;

        /// <summary>
        /// 检测范围
        /// </summary>
        public bool m_IsDispOutRang { get; set; } = false;

        /// <summary>
        /// 检测结果
        /// </summary>
        public bool m_IsDispResult { get; set; } = false;

        /// <summary>
        /// PLC通讯类型
        /// </summary>
        public CommunaBase m_Communa;

        /// <summary>
        /// 数据出队
        /// </summary>
        public DataOutQueue m_DataOut = new DataOutQueue();

        /// <summary>
        /// 构造函数1
        /// </summary>
        public ModuleObjBase()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="moduleName">模块单元名称</param>
        /// <param name="m_lastModuleID">ID</param>
        public ModuleObjBase(string moduleName, int m_lastModuleID)
        {
            ModuleParam.ModuleName = moduleName;
            ModuleParam.ModuleID = m_lastModuleID;
            m_ModuleProject = SysProcessPro.Cur_Project;
        }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public virtual void ExeModule(bool blnByHand = false)
        {
            ModuleParam.BlnSuccessed = false;
            ModuleParam.ModuleCostTime = 0;
        }

        /// <summary>
        /// 模块运行中停止一些动作
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// 模块参数设置后执行的方法的,子类可以不用实现 也可以实现
        /// </summary>
        virtual protected void AfterSetModuleParam()
        {

        }

        public bool GenModuleIndex(out string LinkName, out HImageExt Out_hImage, out DataVar dataVar)
        {
            LinkName = string.Empty;
            Out_hImage = new HImageExt();
            dataVar = new DataVar();

            //查询上一个图像变量
            int imageIndex = m_ModuleProject.m_Var_List.FindLastIndex(c => c.m_DataName == ConstantVar.图像.ToString());
            //获得模块ID
            string moduleName = m_ModuleProject.m_Var_List[imageIndex].m_DataTip;
            int modelLastID = m_ModuleProject.m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleName == moduleName);
            //查询当前模块所属ID
            int moduleIndex = m_ModuleProject.m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleID == ModuleParam.ModuleID);
            if (modelLastID > -1 && modelLastID < moduleIndex)
            {
                DataVar var = m_ModuleProject.m_Var_List[imageIndex];
                if (var.m_DataValue is List<HImageExt>)
                {
                    Out_hImage = ((List<HImageExt>)var.m_DataValue)[0];
                    dataVar = var;
                    LinkName = var.m_DataTip + "." + var.m_DataName;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 只要引用就会自动添加至模块集合
        /// </summary>
        public Project m_ModuleProject
        {
            set
            {
                if (value != null)
                {
                    ModuleProject = value;
                    if (ModuleProject.m_ModuleObjList.FindIndex(c => c.ModuleParam.ProjectID == this.ModuleParam.ProjectID) < 0)
                    {
                        ModuleParam.ModuleName = ModuleParam.PluginName;
                        ModuleParam.ProjectID = ModuleProject.ProjectInfo.m_ProjectID;

                        ModuleProject.m_ModuleObjList.Add(this);
                        ModuleProject.m_LastModuleID++;
                    }
                }
            }
            get
            {
                return ModuleProject;
            }
        }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            ModuleObjBase temp = (ModuleObjBase)formatter.Deserialize(stream);
            return temp;
        }

    }
}
