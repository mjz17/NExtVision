using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;
using Common;
using System.Threading;
using ModuleDataVar;
using System.Collections;

namespace StyleBase
{
    /// <summary>
    /// Processbar.xaml 的交互逻辑
    /// </summary>
    public partial class Processbar : UserControl
    {

        private static readonly object objlock = new object();

        /// <summary>
        /// UI显示对应的后端模块信息
        /// </summary>
        private List<ModuleInfo> ModuleInfoList = new List<ModuleInfo>();//

        /// <summary>
        /// ModuleTree的状态
        /// 用于保存是否展开的状态 用key作为容器, 刷新前清除容器, 需要保证键值唯一
        /// </summary>
        private Dictionary<string, bool> m_NodesStatusDic = new Dictionary<string, bool>();

        /// <summary>
        /// TreeView所有的ModuleToolNode
        /// </summary>
        public List<ModuleNode> m_ModuleNodeList = new List<ModuleNode>();

        /// <summary>
        /// treeview绑定的源数据,UI刷新就刷新该数据
        /// </summary>
        public List<ModuleNode> m_TreeSoureList { get; set; } = new List<ModuleNode>();

        private Cursor m_DragCursor;//拖拽时的光标
        private string m_DragModuleName;//移动位置的时候,模块名称
        private bool m_DragMoveFlag;//移动标志
        private double m_MousePressY;//鼠标点下时的y坐标
        private double m_MousePressX;//鼠标点下时的x坐标

        private string MultiSelectedStart { get; set; }//多选下开始的模块名称
        private string MultiSelectedEnd { get; set; }//多选下结束的模块名称
        private int MultiSelectedCount { get; set; }//多选模块总数

        /// <summary>
        /// 连续选择的模式下，选择的Moudel
        /// </summary>
        public List<string> SelectedModuleNameList { get; set; } = new List<string>();

        /// <summary>
        /// 之前选中的ModuleNode
        /// </summary>
        public ModuleNode SelectedNode { get; set; }

        public Processbar()
        {
            InitializeComponent();
            //点击项目名称,切换操作栏模块集合
            DataChange.propertyChanged += (val) =>
            {
                ModuleInfoList = val;
                if (!Dispatcher.CheckAccess())
                {
                    ModuleTree.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateTree();//变更操作栏显示
                    }));
                }
                else
                {
                    UpdateTree();//变更操作栏显示
                }
            };

            //项目刷新
            ModuleHelper.UpdateProject = new ModuleHelper.DlgUpdateProjectList(ProjectUpdateTree);
        }

        #region 后台委托刷新UI界面

        private void ProjectUpdateTree(List<ModuleInfo> info, int EverCostTime)
        {
            if (!Dispatcher.CheckAccess())
            {
                ModuleTree.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lock (objlock)
                    {
                        UpdateTreeview(info);
                        Process_time.Text = "流程总耗时：" + EverCostTime + "ms";
                    }
                }));
            }
            else
            {
                UpdateTreeview(info);
                Process_time.Text = "流程总耗时：" + EverCostTime + "ms";
            }
        }

        List<ModuleInfo> Moduleinfos = null;

        /// <summary>
        /// 刷新Tree
        /// </summary>
        private void UpdateTreeview(List<ModuleInfo> infos)
        {
            Moduleinfos = infos;

            if (m_TreeSoureList.Count > 0)
            {
                UpdateNode(m_TreeSoureList);
            }
        }

        private void UpdateNode(List<ModuleNode> Info)
        {
            foreach (ModuleNode item in Info)
            {
                ModuleInfo module = Moduleinfos.Find(c => c.ModuleName.Contains(item.Name));

                if (module != null)
                {
                    item.IsRunning = module.IsRunning;
                    if (module.IsUse)
                    {
                        item.CostTime = module.CostTime.ToString() + "ms";
                        item.StateImage = module.IsSuccess ? ModuleProject.GetImageByName("确定") : ModuleProject.GetImageByName("取消");
                    }
                    else
                    {
                        item.CostTime = "";
                        item.StateImage = ModuleProject.GetImageByName("禁用");
                    }
                }
                UpdateNode(item.Children);
            }
        }

        #endregion

        /// <summary>
        /// 拖拽丢下数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_Drop(object sender, DragEventArgs e)
        {

            if (SelectedNode != null)// 恢复之前的下划线
            {
                SelectedNode.DragOverHeight = 1;
            }

            if (e.AllowedEffects == DragDropEffects.Copy)//标识从工具栏拖动，需要创建新的模块
            {
                if (SysProcessPro.Cur_Project == null)
                {
                    System.Windows.MessageBox.Show("当前无项目流程！");
                    Log.Info("未创建解决方案！");
                    return;
                }

                string plugName = e.Data.GetData("Text").ToString();

                if (SelectedNode != null && ModuleInfoList.Count != 0)
                {
                    AddModule(SelectedNode.Name, plugName, true);
                }
                else if (ModuleInfoList.Count == 0)
                {
                    AddModule("", plugName, true);//第一个创建
                }
                else//没有选择默认选择最后一个
                {
                    AddModule(ModuleInfoList.Last().ModuleName, plugName, true);
                }
            }
            else if (e.AllowedEffects == DragDropEffects.Move)//表示移动位置
            {
                if (e.Data.GetData("Text") != null && SelectedNode != null)
                {
                    string moduleName = e.Data.GetData("Text").ToString();
                    if (moduleName != SelectedNode.Name)
                    {
                        if (IsMultiSelectedModel() == true)
                        {
                            ChangeModulePos(MultiSelectedStart, MultiSelectedEnd, SelectedNode.Name, true);//多选模式下移动列表
                        }
                        else
                        {
                            ChangeModulePos(moduleName, moduleName, SelectedNode.Name, true);//单选模式下移动列表
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个模块
        /// </summary>
        /// <param name="relativeModuleName">要追加的模块目标位置模块名称</param>
        /// <param name="plugName">模块信息</param>
        /// <param name="isNext">是否在后方追加</param>
        private void AddModule(string relativeModuleName, string plugName, bool isNext)
        {
            //获取模块编号
            int no = 0;
            List<string> nameList = ModuleInfoList.Select(c => c.ModuleName).ToList();
            while (true)
            {
                if (!nameList.Contains(plugName + no))
                {
                    break;
                }
                no++;
            }

            //模块信息
            ModuleInfo moduleInfo = new ModuleInfo();
            moduleInfo.PluginName = plugName;
            moduleInfo.ModuleName = plugName + no;//模块名称
            moduleInfo.ModuleNO = no + 1;

            if (string.IsNullOrEmpty(relativeModuleName))
            {
                ModuleInfoList.Add(moduleInfo);
            }
            else
            {
                ModuleInfoList.Insert(ModuleInfoList.FindIndex(c => c.ModuleName == relativeModuleName) + 1, moduleInfo);
            }

            //模块添加至Project集合
            moduleInfo = PluginService.s_PluginDic[plugName];
            moduleInfo.ModuleName = plugName + no;//模块名称
            ModuleObjBase moduleObj = (ModuleObjBase)moduleInfo.ModuleObjType.Assembly.CreateInstance(moduleInfo.ModuleObjType.FullName);//根据选中的插件 New一个模块
            if (plugName.Contains("如果"))
            {
                moduleObj.ModuleParam.PluginName = "条件分支"; //设置插件名称
            }
            else if (plugName.Contains("循环开始"))
            {
                moduleObj.ModuleParam.PluginName = "循环工具"; //设置插件名称
            }
            else if (plugName.Contains("停止循环"))
            {
                moduleObj.ModuleParam.PluginName = "循环工具"; //设置插件名称
            }
            else
            {
                moduleObj.ModuleParam.PluginName = plugName; //设置插件名称
            }
            moduleObj.m_ModuleProject = SysProcessPro.Cur_Project;
            moduleObj.ModuleParam.ModuleID = SysProcessPro.Cur_Project.m_LastModuleID;
            moduleObj.ModuleParam.ModuleName = plugName + no;

            //如果模块是条件分支如果
            if (plugName.Contains("如果"))
            {
                ModuleInfo moduleInfo1 = new ModuleInfo();
                moduleInfo1.PluginName = "结束";
                moduleInfo1.ModuleName = "结束" + no;
                moduleInfo1.ModuleNO = no + 1;
                ModuleInfoList.Insert(ModuleInfoList.FindIndex(c => c.ModuleName == moduleInfo.ModuleName) + 1, moduleInfo1);//插在如果模块后面

                //结束模块，添加至Project集合
                moduleInfo1 = PluginService.s_PluginDic[moduleInfo1.PluginName];
                moduleInfo1.ModuleName = "结束" + no;
                ModuleObjBase moduleObj1 = (ModuleObjBase)moduleInfo1.ModuleObjType.Assembly.CreateInstance(moduleInfo1.ModuleObjType.FullName);//根据选中的插件 New一个模块
                moduleObj1.ModuleParam.PluginName = "条件分支"; //设置插件名称
                moduleObj1.m_ModuleProject = SysProcessPro.Cur_Project;
                moduleObj1.ModuleParam.ModuleID = SysProcessPro.Cur_Project.m_LastModuleID;
                moduleObj1.ModuleParam.ModuleName = moduleInfo1.ModuleName;

            }
            else if (plugName.Contains("循环开始"))
            {
                ModuleInfo moduleInfo1 = new ModuleInfo();
                moduleInfo1.PluginName = "循环结束";
                moduleInfo1.ModuleName = "循环结束" + no;
                moduleInfo1.ModuleNO = no + 1;
                ModuleInfoList.Insert(ModuleInfoList.FindIndex(c => c.ModuleName == moduleInfo.ModuleName) + 1, moduleInfo1);//插在如果模块后面

                //结束模块，添加至Project集合
                moduleInfo1 = PluginService.s_PluginDic[moduleInfo1.PluginName];
                moduleInfo1.ModuleName = "循环结束" + no;
                ModuleObjBase moduleObj1 = (ModuleObjBase)moduleInfo1.ModuleObjType.Assembly.CreateInstance(moduleInfo1.ModuleObjType.FullName);//根据选中的插件 New一个模块
                moduleObj1.ModuleParam.PluginName = "循环工具"; //设置插件名称
                moduleObj1.m_ModuleProject = SysProcessPro.Cur_Project;
                moduleObj1.ModuleParam.ModuleID = SysProcessPro.Cur_Project.m_LastModuleID;
                moduleObj1.ModuleParam.ModuleName = moduleInfo1.ModuleName;
            }

            //将UI树形结构改成Project对应的结构
            Project project = new Project();
            foreach (ModuleInfo item in ModuleInfoList)
            {
                project.m_ModuleObjList.Add(SysProcessPro.Cur_Project.m_ModuleObjList.FirstOrDefault(c => c.ModuleParam.ModuleName == item.ModuleName));
            }
            SysProcessPro.Cur_Project.m_ModuleObjList = project.m_ModuleObjList;

            //转为Project的树形结构
            SysProcessPro.Cur_Project.ConvertModuleNameTreeNode();

            UpdateTree();//添加一个模块
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="moduleStartName"></param>
        /// <param name="moduleEndName"></param>
        /// <param name="relativeModuleName"></param>
        /// <param name="isNext"></param>
        private void ChangeModulePos(string moduleStartName, string moduleEndName, string relativeModuleName, bool isNext)
        {
            //名称相同则不修改
            if (moduleStartName == relativeModuleName)
            {
                return;
            }
            List<string> modulenameList = ModuleInfoList.Select(c => c.ModuleName).ToList();
            if (moduleStartName != moduleEndName)
            {
                List<string> tempList = ModuleInfoList.Select(c => c.ModuleName).ToList();

                int startIndex = modulenameList.IndexOf(moduleStartName);
                int endIndex = modulenameList.IndexOf(moduleEndName);
                for (int i = startIndex; i < endIndex + 1; i++)
                {
                    modulenameList.Remove(tempList[i]);//先删除
                    int index = modulenameList.IndexOf(relativeModuleName);
                    modulenameList.Insert(index + 1, tempList[i]);
                    relativeModuleName = tempList[i];
                }
            }
            else
            {
                if (
                    (!moduleStartName.StartsWith("如果") &&
                    !moduleStartName.StartsWith("执行片段") &&
                    !moduleStartName.StartsWith("文件夹") &&
                    !moduleStartName.StartsWith("坐标补正") &&
                    !moduleStartName.StartsWith("点云补正") &&
                    !moduleStartName.StartsWith("循环开始")))
                {
                    //先删除
                    modulenameList.Remove(moduleStartName);
                    //获取定位模块的位置
                    int index = modulenameList.IndexOf(relativeModuleName);

                    if (index == -1 && isNext == true)//添加在首位
                    {
                        modulenameList.Insert(0, moduleStartName);
                    }
                    else if (index == -1 && isNext == false)//添加在末尾
                    {
                        modulenameList.Add(moduleStartName);
                    }
                    else if (index != -1 && isNext == true)//插在后面
                    {
                        modulenameList.Insert(index + 1, moduleStartName);
                    }
                    else if (index != -1 && isNext == false)//插在前面
                    {
                        modulenameList.Insert(index, moduleStartName);
                    }
                }
                else if (Regex.IsMatch(moduleStartName, "文件夹[0-9]*$") ||
                    moduleStartName.StartsWith("如果") ||
                    moduleStartName.StartsWith("坐标补正") ||
                    moduleStartName.StartsWith("点云补正") ||
                    moduleStartName.StartsWith("循环开始"))
                {
                    {
                        List<string> brotherList;
                        if (m_ModuleNodeList.FirstOrDefault(c => c.Name == moduleStartName).ParentModuleNode != null)
                        {
                            //获取同级别的下一个结束
                            brotherList = m_ModuleNodeList.FirstOrDefault(c => c.Name == moduleStartName)
                                 .ParentModuleNode.Children.Select(c => c.Name).ToList();
                        }
                        else
                        {
                            brotherList = m_TreeSoureList.Select(c => c.Name).ToList();
                        }

                        int curIndex = brotherList.IndexOf(moduleStartName);//当前模块的位置

                        string endModuleName = "";
                        // 在同级模块查找结束模块
                        for (int i = curIndex + 1; i < brotherList.Count(); i++)
                        {
                            string endModuleStartName = "";
                            if (moduleStartName.StartsWith("如果"))
                            {
                                //endModuleStartName = GetEndModuleByPluginName("条件分支");
                            }
                            else if (moduleStartName.StartsWith("坐标补正"))
                            {
                                //   endModuleStartName = GetEndModuleByPluginName("坐标补正");
                            }
                            else if (moduleStartName.StartsWith("点云补正"))
                            {
                                //  endModuleStartName = GetEndModuleByPluginName("点云补正");
                            }
                            else if (moduleStartName.StartsWith("循环开始"))
                            {
                                // endModuleStartName = GetEndModuleByPluginName("循环工具");
                            }
                            else if (Regex.IsMatch(moduleStartName, "文件夹[0-9]*$"))
                            {
                                endModuleStartName = "文件夹结束";
                            }

                            if (brotherList[i].StartsWith(endModuleStartName))
                            {
                                endModuleName = brotherList[i];
                                break;
                            }
                        }

                        curIndex = modulenameList.IndexOf(moduleStartName);//当前模块的位置
                        int endIndex = modulenameList.IndexOf(endModuleName);//结束的位置

                        List<string> tempList = CloneObject.DeepCopy<List<string>>(modulenameList);//必须先准备一个副本 不能在foreach里删除自己的元素,会导致跌倒器更新错位

                        //获取定位模块的位置
                        for (int i = curIndex; i < endIndex + 1; i++)
                        {
                            modulenameList.Remove(tempList[i]);  //先删除
                            int index = modulenameList.IndexOf(relativeModuleName);
                            modulenameList.Insert(index + 1, tempList[i]);//插入
                            relativeModuleName = tempList[i];
                        }
                    }
                }
            }

            //根据新的modulenameList 重新调整ModuleInfoList
            List<ModuleInfo> tempModuleInfoList = new List<ModuleInfo>();

            foreach (string moduleName in modulenameList)
            {
                tempModuleInfoList.Add(ModuleInfoList.FirstOrDefault(c => c.ModuleName == moduleName));
            }

            ModuleInfoList = tempModuleInfoList;

            Project project = new Project();
            foreach (string moduleName in modulenameList)
            {
                project.m_ModuleObjList.Add(SysProcessPro.Cur_Project.m_ModuleObjList.FirstOrDefault(c => c.ModuleParam.ModuleName == moduleName));
            }
            SysProcessPro.Cur_Project.m_ModuleObjList = project.m_ModuleObjList;
            //转为Project的树形结构
            SysProcessPro.Cur_Project.ConvertModuleNameTreeNode();
            UpdateTree();//拖拽修改模块
        }

        /// <summary>
        /// 是否是在多选模式下
        /// </summary>
        /// <returns></returns>
        private bool IsMultiSelectedModel()
        {
            foreach (ModuleNode moduleNode in m_ModuleNodeList)
            {
                if (moduleNode.IsMultiSelected == true)
                {
                    return moduleNode.IsMultiSelected;
                }
            }
            return false;
        }

        /// <summary>
        /// 拖拽的时候,鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_DragOver(object sender, DragEventArgs e)
        {
            Point pt = e.GetPosition(ModuleTree);
            HitTestResult result = VisualTreeHelper.HitTest(ModuleTree, pt);

            if (result == null)
                return;
            TreeViewItem selectedItem = ElementTool.FindVisualParent<TreeViewItem>(result.VisualHit);

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
                ModuleNode node = selectedItem.DataContext as ModuleNode;
                if (SelectedNode != null)
                {
                    if (SelectedNode.Name != node.Name)
                    {
                        SelectedNode.DragOverHeight = 1;
                    }
                }
                SelectedNode = node;
                SelectedNode.DragOverHeight = 3;//划过的时候高度变为2
            }

            //获取treeview本身的
            TreeViewAutomationPeer lvap = new TreeViewAutomationPeer(ModuleTree);
            ScrollViewerAutomationPeer svap = lvap.GetPattern(PatternInterface.Scroll) as ScrollViewerAutomationPeer;
            ScrollViewer scroll = svap.Owner as ScrollViewer;
            pt = e.GetPosition(ModuleTree);
            if (ModuleTree.ActualHeight - pt.Y <= 50)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset + 10);
            }
            if (Math.Abs(pt.Y) <= 50)
            {
                scroll.ScrollToVerticalOffset(scroll.VerticalOffset - 10);
            }
        }

        /// <summary>
        /// /拖拽的时候，离开区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_DragLeave(object sender, DragEventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.DragOverHeight = 1;//恢复之前的下滑线
            }
        }

        /// <summary>
        /// 拖拽的时候鼠标样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_DragMoveFlag == true)
            {
                Point pt = e.GetPosition(ModuleTree);
                if (Math.Abs(pt.Y - m_MousePressY) > 10 || Math.Abs(pt.X - m_MousePressX) > 10)//在y方向差异10像素 才开始拖动
                {
                    string showText = "";
                    int width = 0;
                    if (IsMultiSelectedModel() == true)
                    {
                        showText = $"[{MultiSelectedStart}] ~ [{MultiSelectedEnd}]";
                        width = 400;
                    }
                    else
                    {
                        width = 200;
                        showText = SelectedNode.Name;
                    }
                    m_DragCursor = CursorTool.CreateCursor(width, 26, 12, ImageTool.ImageSourceToBitmap(SelectedNode.IconImage), 26, showText);
                    m_DragMoveFlag = false;
                    DragDrop.DoDragDrop(ModuleTree, $"{m_DragModuleName}", DragDropEffects.Move);
                }
            }
        }

        /// <summary>
        /// 拖拽的时候鼠标样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Mouse.SetCursor(m_DragCursor);
            e.Handled = true;
        }

        /// <summary>
        /// shift按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                //只按下了shift 则开始记录是从那里开始连续选中
                if (SelectedNode != null && !SelectedModuleNameList.Contains(SelectedNode.ModuleInfo.ModuleName))
                {
                    SelectedModuleNameList.Add(SelectedNode.ModuleInfo.ModuleName);
                }
                //e.Handled = true;//2023.3.23 赵一添加，为了消除UI显示混乱
            }
        }

        /// <summary>
        /// 鼠标左键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ModuleTree.Items.Count == 0)
            {
                ModuleTree.Focus();//在没有任何元素的时候 需要这几句来获得焦点
                return;
            }

            //获取鼠标位置的TreeViewItem 然后选中
            Point pt = e.GetPosition(ModuleTree);
            HitTestResult result = VisualTreeHelper.HitTest(ModuleTree, pt);
            if (result == null)
                return;

            TreeViewItem selectedItem = ElementTool.FindVisualParent<TreeViewItem>(result.VisualHit);

            if (selectedItem != null)
            {
                SelectedNode = selectedItem.DataContext as ModuleNode;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)//按住shift 多选
            {
                MultiSelect();
                e.Handled = true;
                return;
            }

            if (selectedItem != null)
            {
                SelectedNode = selectedItem.DataContext as ModuleNode;
                selectedItem.IsSelected = true;
            }

            //靠近滚轮则不执行拖动
            if (ModuleTree.ActualWidth - pt.X > 80)
            {
                if (SelectedNode != null && SelectedNode.IsCategory == false)
                {
                    m_MousePressY = pt.Y;
                    m_MousePressX = pt.X;
                    m_DragModuleName = SelectedNode.Name;
                    m_DragMoveFlag = true;
                }
            }
        }

        /// <summary>
        /// 鼠标弹起,多选模式取消显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleTree_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMultiSelectedModel() == true && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                CancelMultiSelect();
            }
        }

        /// <summary>
        /// 多选
        /// </summary>
        private void MultiSelect()
        {
            if (SelectedNode == null) return;

            SelectedModuleNameList.Add(SelectedNode.ModuleInfo.ModuleName);

            //获取多选的module的index
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (string moduleName in SelectedModuleNameList)
            {
                dic[ModuleInfoList.FindIndex(c => c.ModuleName == moduleName)] = moduleName;
            }

            //从小到大全部选中
            foreach (ModuleNode moduleNode in m_ModuleNodeList)
            {
                int index = ModuleInfoList.FindIndex(c => c.ModuleName == moduleNode.ModuleInfo.ModuleName);
                if (index >= dic.Keys.Min() && index <= dic.Keys.Max())
                {
                    if (moduleNode.ModuleInfo.ModuleName.Contains("否则"))
                    {
                        string startName = "";//查找否则 否则如果的 起始模块名称
                        string endName = "";
                        // NativeFun.GetStartEndModuleNameByElse(this. ProjectInfo.ProjectID, moduleNode.ModuleInfo.ModuleName, out startName, out endName);
                        SelectedModuleNameList.Add(startName);
                        SelectedModuleNameList.Add(endName);
                    }
                    else
                    {
                        string endModuleName = "";   //获得其结束模块
                        //  NativeFun.GetEndModuleNameByStartName(moduleNode.ModuleInfo.ModuleName, out endModuleName);
                        //这里将其修改为判断
                        if (Regex.IsMatch(moduleNode.ModuleInfo.ModuleName, "文件夹[0-9]*$"))
                        {
                            endModuleName = moduleNode.ModuleInfo.ModuleName.Replace("文件夹", "文件夹结束");
                        }//还可以自己添加其他的

                        if (endModuleName != "")
                        {
                            SelectedModuleNameList.Add(endModuleName);
                        }

                        string startModuleName = "";//获得开始模块
                        //   NativeFun.GetStartModuleNameByEndName(moduleNode.ModuleInfo.ModuleName, out startModuleName);
                        if (Regex.IsMatch(moduleNode.ModuleInfo.ModuleName, "文件夹结束[0-9]*$"))
                        {
                            startModuleName = moduleNode.ModuleInfo.ModuleName.Replace("文件夹结束", "文件夹");
                        }//还可以自己添加其他的

                        if (startModuleName != "")
                        {
                            SelectedModuleNameList.Add(startModuleName);
                        }
                    }
                }
            }

            //重新计算选择的范围
            foreach (string moduleName in SelectedModuleNameList)
            {
                dic[ModuleInfoList.FindIndex(c => c.ModuleName == moduleName)] = moduleName;
            }

            MultiSelectedStart = dic[dic.Keys.Min()];
            MultiSelectedEnd = dic[dic.Keys.Max()];
            MultiSelectedCount = dic.Keys.Max() - dic.Keys.Min() + 1;

            //将结束模块也加入
            foreach (ModuleNode moduleNode in m_ModuleNodeList)
            {
                int index = ModuleInfoList.FindIndex(c => c.ModuleName == moduleNode.ModuleInfo.ModuleName);
                if (index >= dic.Keys.Min() && index <= dic.Keys.Max())
                {
                    moduleNode.IsMultiSelected = true;

                    if (moduleNode.Children.Count > 0)
                    {
                        //如果当前模块含有子类,则选中所有子类
                        MultiSelectModuleNode(moduleNode);
                    }
                }
            }
        }

        /// <summary>
        /// 取消多选模式
        /// </summary>
        private void CancelMultiSelect()
        {
            //点击的时候取消 多重选择效果
            foreach (ModuleNode item in m_ModuleNodeList)
            {
                item.IsMultiSelected = false;
            }
            SelectedModuleNameList.Clear();
            MultiSelectedCount = 0;
        }

        /// <summary>
        /// 遍历 获取当前ModuleNode下所有的子类,并设为multiselected=true
        /// </summary>
        /// <param name="moduleNode"></param>
        private void MultiSelectModuleNode(ModuleNode moduleNode)
        {
            if (moduleNode != null)
            {
                foreach (ModuleNode item in moduleNode.Children)
                {
                    item.IsMultiSelected = true;

                    if (item.Children.Count > 0)
                    {
                        MultiSelectModuleNode(item);
                    }
                }
            }
        }

        private void ModuleTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //获取鼠标位置的TreeViewItem 然后选中
            Point pt = e.GetPosition(ModuleTree);
            HitTestResult result = VisualTreeHelper.HitTest(ModuleTree, pt);
            if (result == null)
                return;
            TreeViewItem selectedItem = ElementTool.FindVisualParent<TreeViewItem>(result.VisualHit);

            if (selectedItem != null)
            {
                SelectedNode = selectedItem.DataContext as ModuleNode;//2023.2.4 赵一添加，为了处理鼠标右键点击后，未刷新数据源
                selectedItem.Focus();
            }
        }

        private void ModuleTree_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_DragMoveFlag = false;
        }

        /// <summary>
        /// 刷新TreeView
        /// </summary>
        private void UpdateTree()
        {
            m_ModuleNodeList.Clear();
            m_NodesStatusDic.Clear();
            GetTreeNodesStatus(ModuleTree);//保存展开节点信息

            List<ModuleInfo> moduleDic = ModuleInfoList;//模块信息
            Stack<ModuleNode> s_ParentItemStack = new Stack<ModuleNode>();//将父节点放入栈容器 

            m_TreeSoureList.Clear();

            for (int i = 0; i < moduleDic.Count; i++)
            {
                ModuleInfo info = moduleDic[i];
                ModuleNode nodeItem = new ModuleNode(info);
                nodeItem.IsExpanded = m_NodesStatusDic.ContainsKey(info.ModuleName) ? m_NodesStatusDic[info.ModuleName] : true;//还原展开状态
                m_ModuleNodeList.Add(nodeItem);

                if (i == 0)
                    nodeItem.IsFirstNode = true;

                if (info.ModuleName.StartsWith("结束") ||
                    info.ModuleName.StartsWith("否则") ||// 是结束则 取消栈里对应的if
                    info.ModuleName.StartsWith("坐标补正结束") ||
                     info.ModuleName.StartsWith("文件夹结束") ||
                    info.ModuleName.StartsWith("点云补正结束") ||
                    info.ModuleName.StartsWith("循环结束"))
                {
                    s_ParentItemStack.Pop();
                }

                //~~~~~~~~~~~~~~~
                if (s_ParentItemStack.Count > 0)
                {
                    nodeItem.Hierarchy = s_ParentItemStack.Count;//层级
                    ModuleNode parentItem = s_ParentItemStack.Peek();
                    nodeItem.ParentModuleNode = parentItem;
                    parentItem.Children.Add(nodeItem);
                }
                else
                {
                    nodeItem.Hierarchy = 0;
                    nodeItem.ParentModuleNode = null;
                    m_TreeSoureList.Add(nodeItem);    //根目录
                }
                //~~~~~~~~~~~~~~~
                //判断当前节点是否是父节点开始
                if (info.ModuleName.StartsWith("如果") ||
                    info.ModuleName.StartsWith("否则") ||
                    Regex.IsMatch(info.ModuleName, "坐标补正[0-9]*$") ||
                    Regex.IsMatch(info.ModuleName, "点云补正[0-9]*$") ||
                    Regex.IsMatch(info.ModuleName, "文件夹[0-9]*$") ||
                    Regex.IsMatch(info.ModuleName, "执行片段[0-9]*$") ||
                    Regex.IsMatch(info.ModuleName, "循环开始[0-9]*$")
                    )
                {
                    s_ParentItemStack.Push(nodeItem);
                }

                //最后一个node如果层级大于0 则需要补划最后一条横线
                if (i == moduleDic.Count - 1 && nodeItem.Hierarchy > 0)
                {
                    nodeItem.LastNodeMargin = $"{nodeItem.Hierarchy * -14},0,0,0";
                }
            }
            ModuleTree.ItemsSource = m_TreeSoureList.ToList();
            SelectPreNode(ModuleTree);//选中之前选中的节点
        }

        /// <summary>
        /// 选中之前选中的节点
        /// </summary>
        /// <param name="control"></param>
        private void SelectPreNode(ItemsControl control)
        {

            if (control != null && SelectedNode != null)
            {
                foreach (object item in control.Items)
                {
                    TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                    ModuleNode node = null;

                    if (treeItem != null)
                    {
                        node = treeItem.DataContext as ModuleNode;
                    }

                    if (node != null && node.Name == SelectedNode.Name)
                    {
                        treeItem.IsSelected = true;
                        return;
                    }

                    if (treeItem != null && treeItem.HasItems)
                    {
                        ModuleNode moduleNode = treeItem.DataContext as ModuleNode;

                        m_NodesStatusDic[moduleNode.Name] = treeItem.IsExpanded;

                        SelectPreNode(treeItem as ItemsControl);
                    }
                }
            }
        }

        /// <summary>
        /// 获取结构树的展开状态
        /// </summary>
        /// <param name="control"></param>
        private void GetTreeNodesStatus(ItemsControl control)
        {
            if (control != null)
            {
                foreach (object item in control.Items)
                {
                    TreeViewItem treeItem = control.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (treeItem != null && treeItem.HasItems)
                    {
                        ModuleNode moduleNode = treeItem.DataContext as ModuleNode;
                        m_NodesStatusDic[moduleNode.Name] = treeItem.IsExpanded;
                        GetTreeNodesStatus(treeItem as ItemsControl);
                    }
                }
            }
        }

        private void ModuleTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //鼠标左键
                if (e.ChangedButton != MouseButton.Left)
                    return;

                if (SysProcessPro.Cur_Project == null)
                {
                    System.Windows.Forms.MessageBox.Show("当前流程未空！");
                    return;
                }

                //当前流程正在运行无法打开模块参数
                if (SysProcessPro.Cur_Project.m_ThreadStatus)
                {
                    //消息提示窗体
                    NotifyBox notify = new NotifyBox() { NotifyMessage = "程序正在运行，无法打开模块参数！" };
                    notify.ShowInTaskbar = false;
                    notify.Show();
                    return;
                }


                if (ModuleInfoList.Count == 0)
                    return;

                if (SelectedNode == null)
                    return;

                //模块信息
                ModuleInfo module = ModuleInfoList.Find(c => c.ModuleName == SelectedNode.Name);
                ModuleInfo moduleInfo;
                //条件分支模块
                if (module.ModuleName.Contains("结束"))
                {
                    return;
                }

                if (module.ModuleName.Contains("循环结束"))
                {
                    return;
                }

                if (module.ModuleName.Contains("停止循环"))
                {
                    return;
                }

                if (module.PluginName.Contains("条件分支"))
                {
                    moduleInfo = PluginService.s_PluginDic["如果"];
                }
                else if (module.PluginName.Contains("循环工具"))
                {
                    moduleInfo = PluginService.s_PluginDic["循环开始"];
                }
                else
                {
                    moduleInfo = PluginService.s_PluginDic[module.PluginName];
                }

                ModuleObjBase moduleObj = SysProcessPro.GetModuleByName(SysProcessPro.Cur_Project, SelectedNode.Name, module.ModuleNO);

                if (moduleObj == null)
                {
                    return;
                }

                //查询模块对应数据
                PluginFrmBase m_frm_UnitBase = (PluginFrmBase)moduleInfo.ModuleObjType.Assembly.CreateInstance(moduleInfo.ModuleFormType.FullName);

                m_frm_UnitBase.m_ModuleObjBase = moduleObj;

                m_frm_UnitBase.Owner = Window.GetWindow(this);

                m_frm_UnitBase.Show();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 模块菜单栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StackPanel_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //鼠标左键
            if (e.ChangedButton == MouseButton.Right && SelectedNode != null)
            {
                if (!SelectedNode.Name.Contains("结束"))
                {
                    ContextMenu cm = CreateContextMenu();
                    cm.PlacementTarget = sender as StackPanel;
                    cm.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    cm.IsOpen = true;
                }
                //e.Handled = true; 2023.3.24 赵一注释
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 禁用模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableModule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ModuleInfoList.Count == 0)
                    return;
                if (SelectedNode == null)
                    return;

                //模块信息
                ModuleInfo module = ModuleInfoList.Find(c => c.ModuleName == SelectedNode.Name);
                if (module != null)
                {
                    ModuleObjBase obj = SysProcessPro.Cur_Project.m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == module.ModuleName);
                    if (obj != null)
                    {
                        if (obj.ModuleParam.IsUse)
                        {
                            obj.ModuleParam.IsUse = false;
                            module.IsUse = false;
                        }
                        else
                        {
                            obj.ModuleParam.IsUse = true;
                            module.IsUse = true;
                        }
                    }
                    UpdateTree();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteModule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SysProcessPro.Cur_Project == null)
                    return;
                if (ModuleInfoList.Count == 0)
                    return;
                if (SelectedNode == null)
                    return;
                if (SelectedNode.Name.Contains("结束"))
                    return;

                //模块信息
                if (SelectedNode.Name.Contains("如果"))
                {
                    int StartModule = ModuleInfoList.FindIndex(c => c.ModuleName == SelectedNode.Name);
                    if (StartModule > -1)
                    {
                        string str = SelectedNode.Name;
                        string newStr = "结束" + str.Replace("如果", "");
                        //模块信息
                        int EndModule = ModuleInfoList.FindIndex(c => c.ModuleName == newStr);

                        if (EndModule > -1)
                        {
                            for (int i = -1; i < EndModule - StartModule; i++)
                            {
                                ModuleInfoList.RemoveAt(StartModule);
                            }
                        }
                    }
                }
                else if (SelectedNode.Name.Contains("循环开始"))
                {
                    int StartModule = ModuleInfoList.FindIndex(c => c.ModuleName == SelectedNode.Name);
                    if (StartModule > -1)
                    {
                        string str = SelectedNode.Name;
                        string newStr = "循环结束" + str.Replace("循环开始", "");
                        //模块信息
                        int EndModule = ModuleInfoList.FindIndex(c => c.ModuleName == newStr);

                        if (EndModule > -1)
                        {
                            for (int i = -1; i < EndModule - StartModule; i++)
                            {
                                ModuleInfoList.RemoveAt(StartModule);
                            }
                        }
                    }
                }
                else
                {
                    int StartModule = ModuleInfoList.FindIndex(c => c.ModuleName == SelectedNode.Name);

                    if (StartModule > -1)
                    {
                        ModuleInfoList.RemoveAt(StartModule);
                    }
                }

                //获取moduleName的索引名称
                List<string> modulenameList = ModuleInfoList.Select(c => c.ModuleName).ToList();

                //根据新的modulenameList 重新调整ModuleInfoList
                List<ModuleInfo> tempModuleInfoList = new List<ModuleInfo>();

                foreach (string moduleName in modulenameList)
                {
                    tempModuleInfoList.Add(ModuleInfoList.FirstOrDefault(c => c.ModuleName == moduleName));
                }

                ModuleInfoList = tempModuleInfoList;

                Project project = new Project();
                foreach (string moduleName in modulenameList)
                {
                    project.m_ModuleObjList.Add(SysProcessPro.Cur_Project.m_ModuleObjList.FirstOrDefault(c => c.ModuleParam.ModuleName == moduleName));
                }

                SysProcessPro.Cur_Project.m_ModuleObjList = project.m_ModuleObjList;

                //SysProcessPro.Cur_Project.m_Var_List.Clear();2027.9.27,Nick全部清空所有的变量数据
                List<DataVar> FindVar = SysProcessPro.Cur_Project.m_Var_List.FindAll(c => c.m_DataTip == SelectedNode.Name);
                SysProcessPro.Cur_Project.m_Var_List.RemoveAll(data => FindVar.Contains(data));

                //转为Project的树形结构
                SysProcessPro.Cur_Project.ConvertModuleNameTreeNode();
                UpdateTree();//拖拽修改模块

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private ContextMenu CreateContextMenu()
        {
            ContextMenu aMenu = new ContextMenu();

            aMenu.Style = (Style)this.FindResource("ContextPro");
            Style Line = (Style)this.FindResource("SeperatorTemplate");
            Style MenuItem1 = (Style)this.FindResource("MenuItemTemplate");
            Style MenuItem2 = (Style)this.FindResource("MenuItemTemplate1");

            MenuItem null1 = new MenuItem();
            null1.Style = MenuItem2;
            aMenu.Items.Add(null1);

            //复制
            MenuItem CopyMenu = new MenuItem();
            CopyMenu.Style = MenuItem1;
            CopyMenu.Header = "复制";
            CopyMenu.Icon = "pack://application:,,,/StyleBase;component/03LayoutFrm/ImgConMenu/复制.png";
            aMenu.Items.Add(CopyMenu);

            Separator sep1 = new Separator();
            sep1.Style = Line;
            aMenu.Items.Add(sep1);

            //粘贴
            MenuItem PasteMenu = new MenuItem();
            PasteMenu.Style = MenuItem1;
            PasteMenu.Header = "粘贴";
            PasteMenu.Icon = "pack://application:,,,/StyleBase;component/03LayoutFrm/ImgConMenu/粘贴.png";
            aMenu.Items.Add(PasteMenu);

            //禁用模块
            MenuItem DisableMenu = new MenuItem();
            DisableMenu.Style = MenuItem1;

            //模块信息
            ModuleInfo module = ModuleInfoList.Find(c => c.ModuleName == SelectedNode.Name);

            if (module != null)
            {
                DisableMenu.Header = module.IsUse ? "禁用模块" : "启用模块";
                DisableMenu.Icon = module.IsUse ? "pack://application:,,,/StyleBase;component/03LayoutFrm/ImgConMenu/禁用.png" :
                    "pack://application:,,,/StyleBase;component/03LayoutFrm/ImgConMenu/启用.png";
            }
            DisableMenu.Click += DisableModule_Click;
            aMenu.Items.Add(DisableMenu);

            Separator sep2 = new Separator();
            sep2.Style = Line;
            aMenu.Items.Add(sep2);

            //删除模块
            MenuItem DeleteMenu = new MenuItem();
            DeleteMenu.Style = MenuItem1;
            DeleteMenu.Header = "删除模块";
            DeleteMenu.Click += DeleteModule_Click;
            DeleteMenu.Icon = "pack://application:,,,/StyleBase;component/03LayoutFrm/ImgConMenu/删除.png";
            aMenu.Items.Add(DeleteMenu);

            MenuItem null2 = new MenuItem();
            null2.Style = MenuItem2;
            aMenu.Items.Add(null2);

            return aMenu;

        }

    }
}
