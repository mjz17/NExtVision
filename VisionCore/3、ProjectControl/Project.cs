using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModuleDataVar;
using PublicDefine;
using Common;
using System.Text.RegularExpressions;
using HalconControl;
using HalconDotNet;

namespace VisionCore
{
    /// <summary>
    /// 项目核心流程控制
    /// </summary>
    [Serializable]
    public class Project : ProjectThread, ICloneable
    {

        /// <summary>
        /// 流程ID（模块）
        /// </summary>
        public int m_LastModuleID = 0;

        /// <summary>
        /// 项目ID累计值
        /// </summary>
        public static int m_LastProjectID = 0;

        /// <summary>
        /// 流程对应窗体(对应显示用的)
        /// </summary>
        [NonSerialized]
        public ViewHwindow m_Viewhwindow = new ViewHwindow();

        /// <summary>
        /// 流程对应窗体（存储图像对象用的）(运用在保存图片上面)
        /// </summary>
        [NonSerialized]
        public ViewHwindow m_Hwindow = new ViewHwindow();

        /// <summary>
        /// 运行模式
        /// </summary>
        [NonSerialized]
        public RunMode run;

        /// <summary>
        /// 项目参数
        /// </summary>
        public ProjectInfo ProjectInfo { get; set; } = new ProjectInfo();

        /// <summary>
        /// 正在执行的模块名称
        /// </summary>
        [NonSerialized]
        public string m_ExeModuleName;

        /// <summary>
        /// 构造函数1（用于首次创建）
        /// </summary>
        public Project()
        {
            m_LastProjectID++;
            ProjectInfo.m_ProjectID = m_LastProjectID;
            ProjectInfo.m_ProjectName = ProjectInfo.m_ProjectName + ProjectInfo.m_ProjectID.ToString();//项目名称+ID组合
        }

        /// <summary>
        /// 当前执行流程模块ID
        /// </summary>
        [NonSerialized]
        public int CurModuleID = -1;

        /// <summary>
        /// 流程运行结果
        /// </summary>
        [NonSerialized]
        public string ModuelResult = string.Empty;

        /// <summary>
        /// 模块单元列表
        /// </summary>
        public List<ModuleObjBase> m_ModuleObjList = new List<ModuleObjBase>();

        /// <summary>
        /// 模块使用变量列表
        /// </summary>
        public List<DataVar> m_Var_List = new List<DataVar>();

        /// <summary>
        /// 树形容器 临时组装 不需要序列化 每次增加和删除后 都要重新组装
        /// </summary>
        [NonSerialized]
        public ModuleNameTreeNode m_BaseTreeNode = new ModuleNameTreeNode("");

        /// <summary>
        /// 树形节点容器
        /// </summary>
        [NonSerialized]
        public Dictionary<string, ModuleNameTreeNode> m_ModuleTreeNodeMap = new Dictionary<string, ModuleNameTreeNode>();

        /// <summary>
        /// 注册图像列表
        /// </summary>
        public List<RegisterIMG_Info> m_RegistImg = new List<RegisterIMG_Info>();

        [NonSerialized]
        List<ModuleInfo> Info;

        [NonSerialized]
        int EverCostTime;//流程运行总时间

        #region 变量列表操作方法

        /// <summary>
        /// 获得局部变量的值
        /// </summary>
        /// <param name="Link_data_Var"></param>
        /// <returns></returns>
        public DataVar GetLocalVarValue(DataVar Link_data_Var)
        {
            //查询流程
            int index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ProjectInfo.m_ProjectID);

            //根据流程获取值
            return SysProcessPro.g_ProjectList[index].m_Var_List.Find(c => c.m_DataName == Link_data_Var.m_DataName &&
            c.m_DataTip == Link_data_Var.m_DataTip);

            // return SysProcessPro.g_ProjectList[ProjectInfo.m_ProjectID].m_Var_List.Find(c => c.m_DataName == Link_data_Var.m_DataName
            //&& c.m_DataTip == Link_data_Var.m_DataTip);
        }

        /// <summary>
        /// 获得局部变量的值
        /// </summary>
        /// <param name="Link_data_Var"></param>
        /// <returns></returns>
        public DataVar GetCurLocalVarValue(DataVar Link_data_Var)
        {

            return SysProcessPro.Cur_Project.m_Var_List.Find(c => c.m_DataName == Link_data_Var.m_DataName &&
            c.m_DataTip == Link_data_Var.m_DataTip);

            // return SysProcessPro.g_ProjectList[ProjectInfo.m_ProjectID].m_Var_List.Find(c => c.m_DataName == Link_data_Var.m_DataName
            //&& c.m_DataTip == Link_data_Var.m_DataTip);
        }

        /// <summary>
        /// 添加变量列表中的数据(局部变量不加锁)
        /// </summary>
        /// <param name="data_Var"></param>
        public void UpdateLocalVarValue(DataVar data_Var)
        {
            UpdateLocalVarValue(ref m_Var_List, data_Var);
        }

        /// <summary>
        /// 添加变量列表中的值
        /// </summary>
        /// <param name="inVariableList"></param>
        /// <param name="data"></param>
        public void UpdateLocalVarValue(ref List<DataVar> inVariableList, DataVar data)
        {
            int index = inVariableList.FindIndex(c => c.m_DataModuleID == data.m_DataModuleID && c.m_DataName == data.m_DataName && c.m_DataTip == data.m_DataTip);
            if (index > -1)
            {
                inVariableList[index] = data;
            }
            else
            {
                inVariableList.Add(data);
            }
        }

        #endregion

        #region 模块运行

        /// <summary>
        /// 流程启动运行
        /// </summary>
        public override void Thread_Start()
        {
            if (_thread == null)
            {
                _threadStatus = true;
                _thread = new System.Threading.Thread(UnitProcess);
                _thread.Name = ProjectInfo.m_ProjectName;
                _thread.IsBackground = true;
                _thread.Start();
            }
            else
            {
                _threadStatus = true;
                _thread = new System.Threading.Thread(UnitProcess);
                _thread.Name = ProjectInfo.m_ProjectName;
                _thread.IsBackground = true;
                _thread.Start();
            }
        }

        /// <summary>
        /// 实际流程运行方式
        /// </summary>
        public void UnitProcess()
        {
            //停止时执行，进行线程阻塞操作
            if (ProjectInfo.m_Execution == Execution.停止时执行)
            {
                foreach (var item in SysProcessPro.g_ProjectList)
                {
                    //执行流程
                    if (item != null && item._thread != null && item.ProjectInfo.m_Execution != Execution.停止时执行)
                    {
                        if (item.m_ThreadStatus != false)
                        {
                            item.ProjectEventWait.WaitOne();
                        }
                    }
                }
            }

            //调用时执行并且是当前流程，委托刷新UI
            if (ProjectInfo.m_Execution == Execution.调用时执行 && SysProcessPro.Cur_Project == this)
            {
                SysHelper.DataEventChange.SingleProjectChangedEnabelData = false;
            }

            //执行流程
            while (_threadStatus)
            {
                if (_threadStatus)
                {
                    try
                    {
                        //获取第一个ModuleName
                        m_ExeModuleName = m_ModuleObjList.First().ModuleParam.ModuleName;

                        //ID写入
                        this.CurModuleID = ProjectInfo.m_ProjectID;

                        #region 急速模式

                        if (!SysProcessPro.RushMode)
                        {
                            //刷新选中的流程列表,清除流程的显示
                            if (SysProcessPro.Cur_Project != null && SysProcessPro.Cur_Project.CurModuleID == this.CurModuleID)
                            {
                                UptateClearTree(this, out Info, out EverCostTime);
                                ModuleHelper.UpdateProject(Info, EverCostTime);//更新流程列表
                            }
                        }

                        #endregion

                        //窗体是否显示图片
                        bool is_DispImage = false;

                        //运行流程模块
                        while (m_ExeModuleName != "")
                        {
                            //查询模块信息
                            ModuleParam moduleParam = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == (m_ExeModuleName)).ModuleParam;
                            //模块执行结果
                            bool flag = false;
                            //下一个执行的模块是否被逻辑工具修改
                            bool IsNextModuleUpdate = false;

                            //执行模块 判断模块是否执行成功 
                            if (m_ExeModuleName.Contains("采集图像"))
                            {
                                ModuleObjBase obj = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == (m_ExeModuleName));
                                is_DispImage = obj.m_IsDispImage;//是否显示图像
                                if (obj.ModuleParam.IsUse)//如果流程被屏蔽了
                                {
                                    obj.ExeModule(true);
                                    flag = obj.ModuleParam.BlnSuccessed;
                                }
                            }
                            else if (m_ExeModuleName.Contains("显示图像"))
                            {
                                ModuleObjBase obj = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == (m_ExeModuleName));
                                if (obj.ModuleParam.IsUse)//如果流程被屏蔽了
                                {
                                    obj.ExeModule();
                                    flag = obj.ModuleParam.BlnSuccessed;
                                    is_DispImage = false;//屏蔽默认显示
                                }
                            }
                            else
                            {
                                ModuleObjBase obj = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == (m_ExeModuleName));
                                if (obj.ModuleParam.IsUse)//如果流程被屏蔽了
                                {
                                    obj.ExeModule();
                                    flag = obj.ModuleParam.BlnSuccessed;
                                }
                            }

                            //是否有循环运行
                            if (m_ExeModuleName.StartsWith("循环开始"))
                            {
                                moduleParam.pIndex++;
                                if (moduleParam.CyclicCount > moduleParam.pIndex)
                                {
                                    flag = true;//继续循环
                                }
                                else
                                {
                                    moduleParam.pIndex = -1;//这里要重置为-1,嵌套后,
                                    flag = false;//循环完成 停止循环
                                }
                            }
                            else
                            {
                                flag = moduleParam.BlnSuccessed;
                            }

                            //执行逻辑工具处理
                            LogicMethod(ref moduleParam, flag, ref IsNextModuleUpdate);

                            //将搜索到的模块名称放入m_ExeModuleName
                            if (IsNextModuleUpdate == false)
                            {
                                int index = m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleName == (m_ExeModuleName));
                                if (index < m_ModuleObjList.Count() - 1)
                                {
                                    m_ExeModuleName = m_ModuleObjList[index + 1].ModuleParam.ModuleName;
                                }
                                else
                                {
                                    m_ExeModuleName = "";
                                }
                            }

                            #region 急速模式

                            if (!SysProcessPro.RushMode)
                            {
                                //刷新选中的流程列表
                                if (SysProcessPro.Cur_Project != null && SysProcessPro.Cur_Project.CurModuleID == this.CurModuleID)
                                {
                                    UptateTree(this, moduleParam.ModuleName, out Info, out EverCostTime);
                                    Thread.Sleep(1);
                                    ModuleHelper.UpdateProject(Info, EverCostTime);//更新流程列表
                                }
                            }

                            #endregion

                            if (_threadStatus == false)//线程终止
                                break;
                        }

                        //窗体是否显示图像
                        if (is_DispImage)
                        {
                            //更新窗体的显示
                            int index1 = SysLayout.Frm_Info.FindIndex(c => c.Name == ProjectInfo.m_DispHwindowName);
                            if (index1 > -1)
                            {
                                m_Viewhwindow.hWindow = SysLayout.Frm_Info[index1].HWindow;//设置显示窗体
                            }

                            //查询所有的图像HImageExt
                            List<DataVar> datalist = m_Var_List.FindAll(c => c.m_DataType == DataVarType.DataType.Image);

                            //根据图片喷绘显示效果
                            foreach (DataVar item in datalist)
                            {
                                if (item.m_DataTip.Contains("采集图像"))
                                {
                                    if (((List<HImageExt>)item.m_DataValue)[0].IsInitialized())
                                    {
                                        //m_Viewhwindow.m_HImageExt = null;
                                        m_Viewhwindow.m_HImageExt = ((List<HImageExt>)item.m_DataValue)[0];//2023.7.20,赵一取消Clone方法
                                        m_Viewhwindow.m_RepaintHwindow();
                                    }
                                }
                            }
                        }

                        if (run == RunMode.执行一次)
                        {
                            _threadStatus = false;
                        }

                        if (_threadStatus == false)//线程终止 
                            break;

                    }
                    catch (Exception ex)
                    {
                        run = RunMode.None;
                        _threadStatus = false;
                        Log.Error(ex.ToString());
                    }
                }

                //延时时间
                if (SysProcessPro.g_ProjectList != null)
                {
                    int index = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ProjectInfo.m_ProjectID);
                    Thread.Sleep(SysProcessPro.SysInterval + index * 5);
                }
                else
                {
                    Thread.Sleep(SysProcessPro.SysInterval);
                }

                System.GC.Collect();
            }

            _threadStatus = false;

            //委托刷新UI.2023.7.25赵一修改
            Contrast();

            if (run != RunMode.None && SysProcessPro.Cur_Project == this)//2023.6.26添加
            {
                SysHelper.DataEventChange.SingleProjectChangedEnabelData = true;
            }

            //清空当前运行模式
            run = RunMode.None;

            //调用时执行并且是当前流程，委托刷新UI
            if (ProjectInfo.m_Execution == Execution.调用时执行 && SysProcessPro.Cur_Project == this)
            {
                SysHelper.DataEventChange.SingleProjectChangedEnabelData = true;
            }

            //线程执行完成
            ProjectEventWait.Set();

        }

        /// <summary>
        /// 委托刷新UI
        /// </summary>
        private void Contrast()
        {
            bool status = false;
            foreach (var item in SysProcessPro.g_ProjectList)
            {
                if (item.m_ThreadStatus)
                {
                    status = true;
                    break;
                }
            }

            if (!status)
            {
                SysHelper.DataEventChange.ProjectChangedEnabelData = true;
                SysProcessPro.g_SysStatus.m_RunMode = RunMode.None;
            }

        }

        /// <summary>
        /// 用于外部调用
        /// </summary>
        public void UnitProcess(HWindow_Final hWindow)
        {

        }

        #endregion

        #region 委托刷新UI界面

        /// <summary>
        /// 委托刷新UI界面
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="ModuleName"></param>
        /// <param name="m_TreeSoureList"></param>
        /// <param name="EverCostTime"></param>
        private void UptateTree(Project prj, string ModuleName, out List<ModuleInfo> m_TreeSoureList, out int EverCostTime)
        {
            m_TreeSoureList = new List<ModuleInfo>();
            EverCostTime = 0;
            int SingleTime = 0;
            if (prj != null && prj.m_ModuleTreeNodeMap != null)
            {
                foreach (ModuleObjBase item in prj.m_ModuleObjList)
                {
                    if (!item.ModuleParam.ModuleName.Contains("结束"))
                    {
                        m_TreeSoureList.Add(new ModuleInfo()
                        {
                            PluginName = item.ModuleParam.PluginName,
                            ModuleNO = item.ModuleParam.ModuleID,
                            ModuleName = item.ModuleParam.ModuleName,
                            CostTime = item.ModuleParam.IsUse == true ? item.ModuleParam.ModuleCostTime : 0,
                            IsSuccess = item.ModuleParam.BlnSuccessed,
                            IsUse = item.ModuleParam.IsUse,
                            IsRunning = item.ModuleParam.ModuleName == ModuleName ? true : false,//是否是当前执行模块
                            ModuleRemarks = item.ModuleParam.Remarks,
                        }); ;
                    }
                    else
                    {
                        m_TreeSoureList.Add(new ModuleInfo()
                        {
                            PluginName = item.ModuleParam.PluginName,
                            ModuleNO = item.ModuleParam.ModuleID,
                            ModuleName = item.ModuleParam.ModuleName,
                            CostTime = 0,
                            IsSuccess = true,
                            IsRunning = item.ModuleParam.ModuleName == ModuleName ? true : false,//是否是当前执行模块
                            ModuleRemarks = item.ModuleParam.Remarks,
                        });
                    }
                    SingleTime = item.ModuleParam.IsUse == true ? item.ModuleParam.ModuleCostTime : 0;
                    EverCostTime += SingleTime;
                }
            }
        }

        /// <summary>
        /// 委托刷新UI界面
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="ModuleName"></param>
        /// <param name="m_TreeSoureList"></param>
        /// <param name="EverCostTime"></param>
        private void UptateClearTree(Project prj, out List<ModuleInfo> m_TreeSoureList, out int EverCostTime)
        {
            m_TreeSoureList = new List<ModuleInfo>();
            EverCostTime = 0;

            if (prj != null && prj.m_ModuleTreeNodeMap != null)
            {
                foreach (ModuleObjBase item in prj.m_ModuleObjList)
                {
                    item.ModuleParam.BlnSuccessed = false;
                    m_TreeSoureList.Add(new ModuleInfo()
                    {
                        PluginName = item.ModuleParam.PluginName,
                        ModuleNO = item.ModuleParam.ModuleID,
                        ModuleName = item.ModuleParam.ModuleName,
                        CostTime = 0,
                        IsUse = item.ModuleParam.IsUse,//是否是当前执行模块
                        IsSuccess = false,
                    });
                }
                EverCostTime = 0;
            }
        }

        #endregion

        #region 转换为树形结构

        /// <summary>
        /// 转换为树形结构
        /// 拖拽，显示新项目重新生成一次
        /// </summary>
        /// <returns></returns>
        public bool ConvertModuleNameTreeNode()
        {
            m_ModuleTreeNodeMap.Clear();//树形节点容器
            m_BaseTreeNode.ChildList.Clear();//树形容器

            Stack<ModuleNameTreeNode> stack = new Stack<ModuleNameTreeNode>();//流程列表
            Stack<int> eIndexStack = new Stack<int>();//循环索引
            int index = 0;

            foreach (ModuleObjBase item in m_ModuleObjList)
            {
                ModuleNameTreeNode node = new ModuleNameTreeNode(item.ModuleParam.ModuleName);

                if (item.ModuleParam.ModuleName.StartsWith("结束") ||
                   item.ModuleParam.ModuleName.StartsWith("否则") ||
                   item.ModuleParam.ModuleName.StartsWith("坐标补正结束") ||
                   item.ModuleParam.ModuleName.StartsWith("点云补正结束") ||
                   item.ModuleParam.ModuleName.StartsWith("循环结束"))
                {
                    stack.Pop();//堆栈删除数据    
                }

                //判断堆栈是否有数据
                if (stack.Count() > 0)
                {
                    ModuleNameTreeNode parentNameNode = stack.Peek();//堆栈提取数据
                    node.Parent = parentNameNode;
                    parentNameNode.ChildList.Add(item.ModuleParam.ModuleName);
                }
                else
                {
                    node.Parent = m_BaseTreeNode;
                    m_BaseTreeNode.ChildList.Add(item.ModuleParam.ModuleName);//根目录
                }

                m_ModuleTreeNodeMap.Add(item.ModuleParam.ModuleName, node);//放入到容器中 便于查找//判断当前节点是否是父节点开始

                if (item.ModuleParam.ModuleName.StartsWith("如果") ||
                  item.ModuleParam.ModuleName.StartsWith("否则") ||
                  Regex.IsMatch(item.ModuleParam.ModuleName, "坐标补正[0-9]*$") ||
                  Regex.IsMatch(item.ModuleParam.ModuleName, "点云补正[0-9]*$") ||
                  Regex.IsMatch(item.ModuleParam.ModuleName, "文件夹[0-9]*$") ||
                  Regex.IsMatch(item.ModuleParam.ModuleName, "执行片段[0-9]*$") ||
                  Regex.IsMatch(item.ModuleParam.ModuleName, "循环开始[0-9]*$"))
                {
                    stack.Push(node);//堆栈放入数据
                }
                //文件夹结束也放入到文件夹中
                else if (item.ModuleParam.ModuleName.StartsWith("文件夹结束"))
                {
                    stack.Pop();
                }
                //执行片段结束也放入到执行片段中
                else if (item.ModuleParam.ModuleName.StartsWith("执行片段结束"))
                {
                    stack.Pop();
                }
                ModuleParam moduleParam = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == item.ModuleParam.ModuleName).ModuleParam;

                if (!moduleParam.ModuleName.Contains("执行片段结束"))
                {
                    index++;
                }

                #region 循环索引专用

                if (item.ModuleParam.ModuleName.StartsWith("循环结束"))
                {
                    eIndexStack.Pop();
                }
                else if (item.ModuleParam.ModuleName.StartsWith("循环开始"))
                {
                    eIndexStack.Push(moduleParam.pIndex);
                }
                else
                {
                    if (eIndexStack.Count() > 0)
                    {
                        moduleParam.pIndex = eIndexStack.Peek();
                    }
                    else
                    {
                        moduleParam.pIndex = -1;
                    }
                }

                #endregion

            }
            return true;
        }

        #endregion

        #region 逻辑工具


        /// <summary>
        /// 逻辑工具
        /// </summary>
        /// <param name="moduleParam">模块参数</param>
        /// <param name="flag">模块执行状态</param>
        /// <param name="IsNextModuleUpdate">是否循环</param>
        public void LogicMethod(ref ModuleParam moduleParam, bool flag, ref bool IsNextModuleUpdate)
        {
            if (moduleParam.PluginName == "条件分支")
            {
                //m_ExeModuleName的同级模块名称
                List<string> logicList = m_ModuleTreeNodeMap[m_ExeModuleName].Parent.ChildList;
                if (flag == false && (m_ExeModuleName.StartsWith("如果") || m_ExeModuleName.StartsWith("否则如果")))
                {
                    // 跳转到  下一个否则如果 否则 结束
                    int curIndex = logicList.IndexOf(m_ExeModuleName);//当前执行模块的位置// 查找一个执行的模块
                    for (int i = curIndex + 1; i < logicList.Count(); i++)
                    {
                        if (logicList[i].StartsWith("否则如果") || logicList[i].StartsWith("否则") || logicList[i].StartsWith("结束"))
                        {
                            m_ExeModuleName = logicList[i];
                            IsNextModuleUpdate = true;
                            break;
                        }
                    }
                }
                else if (m_ExeModuleName.StartsWith("结束"))
                {
                    //do nothing
                }
            }
            else if (moduleParam.PluginName == "循环工具")
            {
                //m_ExeModuleName的同级模块名称
                List<string> childlist = m_ModuleTreeNodeMap[m_ExeModuleName].Parent.ChildList;
                //增加禁用是 跳出循环
                if ((flag == false && m_ExeModuleName.StartsWith("循环开始")) || moduleParam.IsUse == false)
                {
                    //从 开始循环 跳转到 结束循环的下一个
                    int curIndex = childlist.IndexOf(m_ExeModuleName);//当前循环开始的位置//查找循环结束
                    for (int i = curIndex + 1; i < childlist.Count(); i++)
                    {
                        if (childlist[i].StartsWith("循环结束"))
                        {
                            m_ExeModuleName = childlist[i];
                            flag = true;
                            IsNextModuleUpdate = false;//故意写出false 这样后面的代码执行 就会执行 循环结束的下一个模块
                            break;
                        }
                    }
                }
                else if (m_ExeModuleName.StartsWith("循环结束"))
                {
                    int curIndex = childlist.IndexOf(m_ExeModuleName);//当前循环结束的位置
                    // 查找 循环开始
                    for (int i = curIndex - 1; i >= 0; i--)
                    {
                        if (childlist[i].StartsWith("循环开始"))
                        {
                            m_ExeModuleName = childlist[i];
                            break;
                        }
                    }

                    IsNextModuleUpdate = true;//从 结束循环  跳转到 开始循环
                }
               
            }
            else if (moduleParam.PluginName == "停止循环" && moduleParam.IsUse == true)
            {
                //判断是是否有父类 没有父类 则是不和要求的停止循环,
                if (m_ModuleTreeNodeMap[m_ExeModuleName].Parent != null && m_ModuleTreeNodeMap[m_ExeModuleName].Parent.Parent != null)
                {
                    List<string> parentlist = new List<string>();
                    for (int i = 0; i < m_ModuleObjList.Count; i++)//赵一添加
                    {
                        parentlist.Add(m_ModuleObjList[i].ModuleParam.ModuleName);
                    }

                    //从 停止循环 跳转到 结束循环的下一个
                    int curIndex = parentlist.IndexOf(m_ModuleTreeNodeMap[m_ExeModuleName].Parent.Name);//当前模块的父节点位置
                                                                                                        // 查找 循环结束
                    for (int i = curIndex + 1; i < parentlist.Count(); i++)
                    {
                        if (parentlist[i].StartsWith("循环开始"))//如果先找到循环开始 则该停止循环模块是非法的 默认不处理
                        {
                            break;
                        }
                        if (parentlist[i].StartsWith("循环结束"))//没有找到则默认执行下一个
                        {
                            //先将循环开始停止,索引停止后变为-1
                            ModuleParam tempModuleParam = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName.Contains(m_ModuleTreeNodeMap[m_ExeModuleName].Parent.Name)).ModuleParam;
                            //PluginManager::Instance().StopModule(tempModuleParam); 停止模块 抽demo 故意注释
                            //
                            m_ExeModuleName = parentlist[i];
                            flag = true;
                            IsNextModuleUpdate = false;//故意写出false 这样后面的代码执行 就会执行 循环结束的下一个模块
                            break;
                        }
                    }

                    //还需要将当前的循环开始停止, 解决深度嵌套循环的问题 magical 2019-7-22 09:33:27
                    for (int i = curIndex - 1; i >= 0; i--)
                    {
                        if (parentlist[i].StartsWith("循环开始"))//
                        {
                            ModuleParam tempModuleParam = m_ModuleObjList.Find(c => c.ModuleParam.ModuleName.Contains(parentlist[i])).ModuleParam;
                            // PluginManager::Instance().StopModule(tempModuleParam);//将当前循环开始停止 抽demo 故意注释
                            break;
                        }
                    }
                }
            }
            //如果下一个模块是 "否则如果" "否则" , 则直接跳转到结束
            if (moduleParam.PluginName != "条件分支" ||//正常模块 
                flag == true)//条件分支模块返回为true
            {

                int curIndex = m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleName.Contains(m_ExeModuleName));

                if (curIndex < m_ModuleObjList.Count() - 1)//判断是否是最后一个
                {
                    string nextModuleName = m_ModuleObjList[curIndex + 1].ModuleParam.ModuleName;
                    if (nextModuleName.StartsWith("否则如果") || nextModuleName.StartsWith("否则"))
                    {
                        List<string> logicList;
                        if (moduleParam.PluginName != "条件分支" || m_ExeModuleName.StartsWith("结束"))
                        {
                            logicList = m_ModuleTreeNodeMap[m_ExeModuleName].Parent.Parent.ChildList;//获取父一级的上一级名称 if else 
                            curIndex = logicList.IndexOf(m_ModuleTreeNodeMap[m_ExeModuleName].Parent.Name);
                        }
                        else
                        {
                            logicList = m_ModuleTreeNodeMap[m_ExeModuleName].Parent.ChildList;//获取同级的上一级名称 if else 
                            curIndex = logicList.IndexOf(m_ExeModuleName);
                        }

                        // 查找结束模块
                        for (int i = curIndex + 1; i < logicList.Count(); i++)
                        {
                            if (logicList[i].StartsWith("结束"))
                            {
                                m_ExeModuleName = logicList[i];
                                IsNextModuleUpdate = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            Project temp = (Project)formatter.Deserialize(stream);
            return temp;
        }

        [OnDeserialized()]
        internal void OnDeSerializedMethod(StreamingContext context)
        {
            CurModuleID = -1;
            m_Viewhwindow = new ViewHwindow();

            m_Hwindow = new ViewHwindow();

            m_ModuleTreeNodeMap = new Dictionary<string, ModuleNameTreeNode>();
            m_BaseTreeNode = new ModuleNameTreeNode("");
            ConvertModuleNameTreeNode();//转为树形结构
        }

    }
}
