using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

namespace StyleBase
{
    /// <summary>
    /// ModuleProject.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleProject : UserControl
    {
        public ModuleProject()
        {
            InitializeComponent();
            CreateExp();
            this.DataContext = this;
        }

        List<Expander> ModuleExpander = new List<Expander>();

        /// <summary>
        /// 根据仿射获得模块的属性
        /// </summary>
        private void CreateExp()
        {
            Expander expander;

            Dictionary<string, List<string>> TreeInfo = new Dictionary<string, List<string>>();

            if (PluginService.s_PluginDic.Count == 0)
                return;

            foreach (var item in PluginService.s_PluginDic)
            {
                if (!item.Key.Contains("结束"))//不把结束模块放入工具栏
                {
                    if (TreeInfo.ContainsKey(item.Value.PluginCategory))
                    {
                        TreeInfo[item.Value.PluginCategory].Add(item.Value.PluginName);
                    }
                    else
                    {
                        TreeInfo.Add(item.Value.PluginCategory, new List<string> { item.Value.PluginName });
                    }
                }
            }

            if (TreeInfo.Count == 0)
                return;

            //将字典的信息转为UI信息
            foreach (var item in TreeInfo)
            {
                expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                expander.IsExpanded = false;

                expander.Header = item.Key;
                expander.FontSize = 13;
                expander.Foreground = new SolidColorBrush(Colors.White);
                expander.Content = CreateModuel(item.Value);
                struckInfo.Children.Add(expander);
                ModuleExpander.Add(expander);
                expander.Expanded += Expander_Expanded;
            }
        }

        /// <summary>
        /// Expander选中保护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            foreach (Expander exp in ModuleExpander)
            {
                if (exp == sender)
                {
                    exp.IsExpanded = true;
                }
                else
                {
                    exp.IsExpanded = false;
                }
            }
        }

        /// <summary>
        /// 拖拽时候的光标
        /// </summary>
        private Cursor m_DragCursor;

        /// <summary>
        /// Expander内部的模块信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private ListBox CreateModuel(List<string> info)
        {
            List<ModuleToolNode> toolTreeNodeLst = new List<ModuleToolNode>();

            foreach (var item in info)
            {
                toolTreeNodeLst.Add(new ModuleToolNode()
                {
                    Name = item,
                    IconImage = GetImageByName(item)
                });
            }

            ListBox list = new ListBox();
            list.PreviewMouseDown += List_PreviewMouseDown;//鼠标按下事件
            list.GiveFeedback += List_GiveFeedback;
            list.Background = new SolidColorBrush(Colors.Gray);
            list.ItemsSource = toolTreeNodeLst;
            return list;
        }

        /// <summary>
        /// ListBox鼠标按下事件/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //需要添加程序运行中的保护
            if (SysProcessPro.Cur_Project == null)
            {
                return;
            }

            if (SysProcessPro.Cur_Project.m_ThreadStatus)
            {
                return;
            }

            ListBox listbox = (ListBox)sender;
            Point pt = e.GetPosition(listbox);//获取鼠标位置的TreeViewItem
            HitTestResult result = VisualTreeHelper.HitTest(listbox, pt);
            if (result == null)
            {
                return;
            }

            ListBoxItem selectedItem = ElementTool.FindVisualParent<ListBoxItem>(result.VisualHit);
            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
                ModuleToolNode toolTreeNode = listbox.SelectedItem as ModuleToolNode;
                if (toolTreeNode != null && toolTreeNode.IsCategory == false)
                {
                    m_DragCursor = CursorTool.CreateCursor(100, 26, 12, ImageTool.ImageSourceToBitmap(toolTreeNode.IconImage), 26, toolTreeNode.Name);
                    DragDrop.DoDragDrop(listbox, toolTreeNode.Name, DragDropEffects.Copy);//增加模块是 copy 
                    //e.Handled = true;
                }
                else if (toolTreeNode != null && toolTreeNode.IsCategory == true)
                {
                    e.Handled = true;
                }
            }

            //((UIElement)e.Source).CaptureMouse();

            //((UIElement)e.Source).ReleaseMouseCapture();
        }

        /// <summary>
        /// 拖拽过程中，改变鼠标的样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Mouse.SetCursor(m_DragCursor);
            e.Handled = true;
        }

        /// <summary>
        /// 根据名称获得图片的路径
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public static BitmapImage GetImageByName(string pluginName)
        {

            if (pluginName == "工具箱")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/工具箱.png", UriKind.Relative));
            }
            else if (pluginName == "确定")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/确定.png", UriKind.Relative));
            }
            else if (pluginName == "取消")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/取消.png", UriKind.Relative));
            }
            else if (pluginName == "禁用")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/禁用.png", UriKind.Relative));
            }
            else if (pluginName == "采集图像")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/采集图像.png", UriKind.Relative));
            }
            else if (pluginName == "图像合并")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/图像合并.png", UriKind.Relative));
            }
            else if (pluginName == "极坐标图像")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/极坐标图像.png", UriKind.Relative));
            }
            else if (pluginName == "显示图像")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/图片.png", UriKind.Relative));
            }
            else if (pluginName == "存储图像")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/存储图像.png", UriKind.Relative));
            }
            else if (pluginName == "位置补正")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/位置补正.png", UriKind.Relative));
            }
            else if (pluginName == "二维码")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/二维码.png", UriKind.Relative));
            }
            else if (pluginName == "一维码")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/一维码.png", UriKind.Relative));
            }
            else if (pluginName == "模板匹配")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/模板匹配.png", UriKind.Relative));
            }
            else if (pluginName == "矩形")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/矩形.png", UriKind.Relative));
            }
            else if (pluginName == "圆形测量")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/圆形测量.png", UriKind.Relative));
            }
            else if (pluginName == "直线测量")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/直线测量.png", UriKind.Relative));
            }
            else if (pluginName == "文本发送")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/文本发送.png", UriKind.Relative));
            }
            else if (pluginName == "文本接收")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/文本接收.png", UriKind.Relative));
            }
            else if (pluginName == "时间")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/时间.png", UriKind.Relative));
            }
            else if (pluginName == "斑点分析")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/斑点分析.png", UriKind.Relative));
            }
            else if (pluginName == "平滑去噪")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/平滑去噪.png", UriKind.Relative));
            }
            else if (pluginName == "颜色增强")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/颜色增强.png", UriKind.Relative));
            }
            else if (pluginName == "N点标定")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/N点标定.png", UriKind.Relative));
            }
            else if (pluginName == "N点映射")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/N点标定.png", UriKind.Relative));
            }
            else if (pluginName == "机械手控制")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/机械手控制.png", UriKind.Relative));
            }
            else if (pluginName == "仿射变换")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/仿射变换.png", UriKind.Relative));
            }
            else if (pluginName == "畸变校正")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/畸变校正.png", UriKind.Relative));
            }
            else if (pluginName == "Halcon畸变标定")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/Halcon畸变标定.png", UriKind.Relative));
            }
            else if (pluginName == "VB脚本")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/VB脚本.png", UriKind.Relative));
            }
            else if (pluginName == "测量标定")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/测量标定.png", UriKind.Relative));
            }
            else if (pluginName == "点点构建")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/点点构建.png", UriKind.Relative));
            }
            else if (pluginName == "点线构建")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/点线构建.png", UriKind.Relative));
            }
            else if (pluginName == "线线距离")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/线线距离.png", UriKind.Relative));
            }
            else if (pluginName == "线线交点")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/线线交点.png", UriKind.Relative));
            }
            else if (pluginName == "创建ROI")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/创建ROI.png", UriKind.Relative));
            }
            else if (pluginName == "预先处理")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/预先处理.png", UriKind.Relative));
            }
            else if (pluginName == "PLC通讯")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/PLC通讯.png", UriKind.Relative));
            }
            else if (pluginName == "PLC写入")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/PLC写入.png", UriKind.Relative));
            }
            else if (pluginName == "PLC读取")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/PLC读取.png", UriKind.Relative));
            }
            else if (pluginName == "数据出队")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/数据出队.png", UriKind.Relative));
            }
            else if (pluginName == "数据入队")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/数据入队.png", UriKind.Relative));
            }
            else if (pluginName == "清空队列")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/清空队列.png", UriKind.Relative));
            }
            else if (pluginName == "如果")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/如果.png", UriKind.Relative));
            }
            else if (pluginName == "否则")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/否则.png", UriKind.Relative));
            }
            else if (pluginName == "否则如果")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/否则如果.png", UriKind.Relative));
            }
            else if (pluginName == "结束")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/结束.png", UriKind.Relative));
            }
            else if (pluginName == "执行流程")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/执行流程.png", UriKind.Relative));
            }
            else if (pluginName == "变量定义")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/变量定义.png", UriKind.Relative));
            }
            else if (pluginName == "变量设置")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/变量定义.png", UriKind.Relative));
            }
            else if (pluginName == "图像脚本")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/图像脚本.png", UriKind.Relative));
            }
            else if (pluginName == "CSV存储")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/CSV存储.png", UriKind.Relative));
            }
            else if (pluginName == "切换方案")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/切换方案.png", UriKind.Relative));
            }
            else if (pluginName == "循环开始")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/循环开始.png", UriKind.Relative));
            }
            else if (pluginName == "循环结束")
            {
                return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/循环结束.png", UriKind.Relative));
            }

            return new BitmapImage(new Uri("/StyleBase;component/03LayoutFrm/ImgRes/图片.png", UriKind.Relative));
        }

    }
}
