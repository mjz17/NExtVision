using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ModuleCamera;
using ModuleDataVar;
using Common;
using PublicDefine;
using CommunaCation;
using HalconDotNet;

namespace VisionCore
{

    public delegate void DelegateTrigger();

    /// <summary>
    /// 项目解决方案
    /// </summary>
    public class SysProcessPro : IDisposable
    {
        /// <summary>
        /// 项目名称 
        /// </summary>
        public static string SolutionName = string.Empty;

        /// <summary>
        /// 项目路径地址
        /// </summary>
        public static string SolutionPath = string.Empty;

        /// <summary>
        /// 文件路径
        /// </summary>
        public static string SysFliePath = string.Empty;

        public static string ConfigPath = @"NExtVision.nv";

        /// <summary>
        /// 急速模式
        /// </summary>
        public static bool RushMode { get; set; } = false;

        /// <summary>
        /// 项目工程列表
        /// </summary>
        public static List<Project> g_ProjectList = new List<Project>();

        /// <summary>
        /// 当前项目
        /// </summary>
        public static Project Cur_Project = null;

        /// <summary>
        /// 采集设备列表
        /// </summary>
        public static List<CameraBase> g_CameraList = new List<CameraBase>();

        /// <summary>
        /// 全局变量
        /// </summary>
        public static List<DataVar> g_VarList = new List<DataVar>();

        /// <summary>
        /// 是否保存全局变量值
        /// </summary>
        public static bool SaveUpValue { get; set; } = false;

        /// <summary>
        /// 显示ROI集合
        /// </summary>
        public static List<ModuleROI> g_moduleRoiList = new List<ModuleROI>();

        /// <summary>
        /// 通讯集合
        /// </summary>
        public static List<ECommunacation> g_CommunCation = new List<ECommunacation>();

        /// <summary>
        /// 全局唯一引擎 所以使用静态
        /// </summary>
        public static HDevEngine s_HDevEngine = new HDevEngine();

        /// <summary>
        /// 系统运行状态
        /// </summary>
        public static SysStatus g_SysStatus = new SysStatus();

        /// <summary>
        /// 系统流程间隔时间
        /// </summary>
        public static int SysInterval;

        /// <summary>
        /// 获取所有模块名称
        /// </summary>
        /// <returns></returns>
        public static ModuleObjBase GetModuleByName(Project prj, string moduelName, int moudleID)
        {
            return prj.m_ModuleObjList.FirstOrDefault(c => c.ModuleParam.ModuleName == moduelName);
        }

        /// <summary>
        /// 初始化视觉工程文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool InitialVisionPram(string filepath = @"NE.nv")
        {
            try
            {
                string ThePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

                if (filepath.Contains(@":\") == false)
                {
                    filepath = System.IO.Path.Combine(ThePath, filepath);
                }

                if (filepath.Trim() == "" || System.IO.File.Exists(filepath) == false)
                {
                    System.Windows.Forms.MessageBox.Show("输入文件名错误");
                    throw new Exception("视觉测量模块报错:" + filepath + "不存在！");
                }
                else
                {
                    ConfigPath = filepath;
                }

                //设备也要同步关闭
                DisposeDev();

                //关闭通讯连接
                EComManageer.DisConnectAll();

                //读取保存文件
                ReadConfig(ConfigPath);

                //通讯//反序列化后刷新字典
                EComManageer.setEcomList(g_CommunCation);

                //初始化驱动界面
                InitDevStatus();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());

                return false;
            }
        }

        /// <summary>
        /// 加载项目文件
        /// </summary>
        /// <param name="filePath">加载地址</param>
        public static void ReadConfig(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    fs.Seek(0, SeekOrigin.Begin);

                    BinaryFormatter binaryFmt = new BinaryFormatter();

                    SysProcessPro.g_VarList = (List<DataVar>)binaryFmt.Deserialize(fs);

                    SysProcessPro.g_CameraList = (List<CameraBase>)binaryFmt.Deserialize(fs);

                    CameraBase.m_LastDeviceID = (int)binaryFmt.Deserialize(fs);

                    SysProcessPro.g_ProjectList = (List<Project>)binaryFmt.Deserialize(fs);

                    Project.m_LastProjectID = (int)binaryFmt.Deserialize(fs);

                    SysProcessPro.g_CommunCation = (List<ECommunacation>)binaryFmt.Deserialize(fs);

                    SysLayout.LayoutFrmNum = (int)binaryFmt.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 保存项目文件
        /// </summary>
        /// <param name="filePath">保存地址</param>
        public static void SaveConfig(string filePath)
        {
            string ThePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            string temFile = "NE.nv";
            temFile = System.IO.Path.Combine(ThePath, temFile);
            try
            {
                System.GC.Collect();
                using (FileStream fs = new FileStream(temFile, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    fs.Seek(0, SeekOrigin.Begin);
                    formatter.Serialize(fs, SysProcessPro.g_VarList);//变量

                    formatter.Serialize(fs, SysProcessPro.g_CameraList);//相机
                    formatter.Serialize(fs, CameraBase.m_LastDeviceID);

                    formatter.Serialize(fs, SysProcessPro.g_ProjectList);//模块
                    formatter.Serialize(fs, Project.m_LastProjectID);

                    g_CommunCation = EComManageer.GetEcomList();//通讯
                    formatter.Serialize(fs, SysProcessPro.g_CommunCation);

                    formatter.Serialize(fs, SysLayout.LayoutFrmNum);//窗体布局
                }

                string outPath = filePath;
                if (filePath.Contains(@":\") == false)
                {
                    outPath = System.IO.Path.Combine(ThePath, filePath);
                }

                File.Copy(temFile, outPath, true);

                //System.GC.Collect();//主动回收下系统未使用的资源
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show("保存配置文件失败：" + ex.ToString());
            }
        }

        /// <summary>
        /// 初始化设备连接状态
        /// </summary>
        public static void InitDevStatus()
        {
            foreach (CameraBase dev in SysProcessPro.g_CameraList)
            {
                if (dev.m_bConnected)
                {
                    dev.m_bConnected = false;
                    dev.ConnectDev();
                    dev.SetSetting();
                }
                else
                {
                    dev.DisConnectDev();
                }
            }
        }

        /// <summary>
        /// 释放设备
        /// </summary>
        public static void DisposeDev()
        {
            foreach (CameraBase item in SysProcessPro.g_CameraList)
            {
                if (item.m_bConnected)
                {
                    item.DisConnectDev();
                }
            }
        }

        /// <summary>
        /// 解决方案所有项目执行一次
        /// </summary>
        public static void Sys_Run_Once()
        {
            Sys_Start(SysProcessPro.g_SysStatus.m_RunMode);
        }

        /// <summary>
        /// 解决方案所有项目循环运行
        /// </summary>
        public static void Sys_Run_Cycle()
        {
            SysProcessPro.g_SysStatus.m_RunMode = RunMode.循环运行;
            Sys_Start(SysProcessPro.g_SysStatus.m_RunMode);
        }

        /// <summary>
        /// 系统启动
        /// </summary>
        public static void Sys_Start(RunMode runMode)
        {
            for (int i = 0; i < g_ProjectList.Count; i++)
            {
                //判断流程状态,只有主动执行状态下才能够运行
                if (g_ProjectList[i].ProjectInfo.m_Execution == Execution.主动执行)
                {
                    Sys_Start(i, runMode);
                }
            }
        }

        /// <summary>
        /// 指定项目循环运行
        /// </summary>
        /// <param name="index"></param>
        public static void Sys_Start(int index, RunMode runMode)
        {
            Sys_ThreadStart(index, runMode);
        }

        public static void Sys_ThreadStart(int index, RunMode runMode)
        {
            if (index > SysProcessPro.g_ProjectList.Count - 1)
            {
                return;
            }

            if (SysProcessPro.g_ProjectList[index].m_ModuleObjList.Count == 0)
            {
                Log.Error(SysProcessPro.g_ProjectList[index].ProjectInfo.m_ProjectName + "无任何模块");
                return;
            }

            foreach (ModuleObjBase item in SysProcessPro.g_ProjectList[index].m_ModuleObjList)
            {
                if (item.ModuleParam.ModuleName.Contains("采集图像"))
                {
                    item.Stop();
                }
                else if (item.ModuleParam.ModuleName.Contains("时间"))
                {
                    item.Stop();
                }
                else if (item.ModuleParam.ModuleName.Contains("文本接收"))
                {
                    item.Stop();
                }
                else if (item.ModuleParam.ModuleName.Contains("数据出队"))
                {
                    item.Stop();
                }
            }

            SysProcessPro.g_ProjectList[index].run = runMode;//流程模块写入
            SysProcessPro.g_ProjectList[index].Thread_Start();
        }

        /// <summary>
        ///系统检测停止
        /// </summary>
        public static void Sys_Stop_Run()
        {
            try
            {
                //关闭主动执行流程的线程
                for (int i = 0; i < SysProcessPro.g_ProjectList.Count; i++)
                {
                    if (SysProcessPro.g_ProjectList[i].ProjectInfo.m_Execution != Execution.停止时执行)
                    {
                        Sys_Stop(i);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("系统检测停止:" + ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 系统检测停止
        /// </summary>
        public static void Sys_Stop(int index)
        {
            try
            {
                if (index > SysProcessPro.g_ProjectList.Count - 1)
                {
                    return;
                }

                SysProcessPro.g_ProjectList[index].Thread_Stop();

                foreach (ModuleObjBase item in SysProcessPro.g_ProjectList[index].m_ModuleObjList)
                {
                    if (item.ModuleParam.ModuleName.Contains("采集图像"))
                    {
                        item.Stop();
                    }
                    else if (item.ModuleParam.ModuleName.Contains("时间"))
                    {
                        item.Stop();
                    }
                    else if (item.ModuleParam.ModuleName.Contains("文本接收"))
                    {
                        item.Stop();
                    }
                    else if (item.ModuleParam.ModuleName.Contains("数据出队"))
                    {
                        item.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Measure.HMeasureSYS.Sys_Stop:" + ex.ToString());
            }
        }

        /// <summary>
        /// 关闭软件，释放资源
        /// </summary>
        public static void DisposeVisionProgram()
        {
            //停止解决方案内所有项目运行
            Sys_Stop_Run();
            //断开相机
            SysProcessPro.DisposeDev();
            //关闭通讯连接
            EComManageer.DisConnectAll();
        }

        public void Dispose()
        {

        }

    }
}
