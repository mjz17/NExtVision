using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.ExecutionProcess
{
    [Category("逻辑工具")]
    [DisplayName("执行流程")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        [NonSerialized]
        public AutoResetEvent EventWait = new AutoResetEvent(false);

        /// <summary>
        /// 执行模式
        /// </summary>
        public ExecutionMode m_execution { get; set; } = 0;

        /// <summary>
        /// 流程执行类型
        /// </summary>
        public List<ExecutionType> Project_Module = new List<ExecutionType>();

        /// <summary>
        /// 执行流程
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                switch (m_execution)
                {
                    case ExecutionMode.顺序单次执行:
                        SingleExecut();
                        break;
                    case ExecutionMode.并行单次执行:
                        ParaExecut();
                        break;
                    default:
                        break;
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

        //顺序单次执行
        void SingleExecut()
        {
            //判断流程是否在执行中
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        if (project.Thread_Status() != ThreadState.Stopped)
                        {
                            project.ProjectEventWait.WaitOne();
                        }
                    }
                }
            }

            //执行流程
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        //确定线程完成运行
                        if (!project.m_ThreadStatus)
                        {
                            project.run = RunMode.执行一次;
                            project.Thread_Start();
                        }
                    }

                    //判断是否需要阻塞等待
                    if (item.WaitType)
                    {
                        //执行流程
                        if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                        {
                            if (project.m_ThreadStatus)
                            {
                                project.ProjectEventWait.WaitOne();
                            }
                        }
                    }
                }
            }

            //等待流程执行完成
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        //判断是否需要阻塞等待
                        if (item.WaitType)
                        {
                            if (project.m_ThreadStatus)
                            {
                                project.ProjectEventWait.WaitOne();
                            }
                        }
                    }
                }
            }

        }

        //并行单次运行
        void ParaExecut()
        {
            //判断流程是否在执行中
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        if (project.Thread_Status() != ThreadState.Stopped)
                        {
                            project.ProjectEventWait.WaitOne();
                        }
                    }
                }
            }

            //执行流程
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        //确定线程完成运行
                        if (!project.m_ThreadStatus)
                        {
                            project.run = RunMode.执行一次;
                            project.Thread_Start();
                        }
                    }
                    //判断是否需要阻塞等待
                    if (item.WaitType)
                    {
                        //执行流程
                        if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                        {
                            if (project.Thread_Status() != ThreadState.Stopped)
                            {
                                project.ProjectEventWait.WaitOne();
                            }
                        }
                    }
                }
            }

            //等待流程执行完成
            foreach (var item in Project_Module)
            {
                //判断流程是否需要执行
                if (item.SingleType)
                {
                    //根据流程名称查询流程
                    Project project = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName.Contains(item.ProjectName));
                    //执行流程
                    if (project != null && project.ProjectInfo.m_Execution == Execution.调用时执行)
                    {
                        //判断是否需要阻塞等待
                        if (item.WaitType)
                        {
                            if (project.Thread_Status() != ThreadState.Stopped)
                            {
                                project.ProjectEventWait.WaitOne();
                            }
                        }
                    }
                }
            }

        }

    }

    public enum ExecutionMode
    {
        顺序单次执行,
        并行单次执行,
    }

    [Serializable]
    public class ExecutionType
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 单次运行
        /// </summary>
        public bool SingleType { get; set; } = false;

        /// <summary>
        /// 等待执行完成
        /// </summary>
        public bool WaitType { get; set; } = false;

    }

}



