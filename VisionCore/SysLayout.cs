using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace VisionCore
{
    /// <summary>
    /// 窗体布局
    /// </summary>
    [Serializable]
    public class SysLayout
    {
        /// <summary>
        /// 窗体的数量
        /// </summary>
        public static int LayoutFrmNum { get; set; } = 1;

        /// <summary>
        /// 窗体的数量
        /// </summary>
        [NonSerialized]
        public static List<LayoutInfo> Frm_Info = new List<LayoutInfo>();

        /// <summary>
        /// 创建窗体
        /// </summary>
        /// <param name="grid"></param>
        public static void CreateLayoutFrm(Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            grid.Children.Clear();
            Frm_Info.Clear();

            //布局窗体为1或者2
            if (LayoutFrmNum == 1 || LayoutFrmNum == 2)
            {
                //创建1个或者2个窗体
                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    ColumnDefinition column = new ColumnDefinition();//创建列

                    grid.ColumnDefinitions.Add(column);

                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器

                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体

                    HWindow.Name = "Disp_Window" + (i + 1).ToString();

                    host.Child = HWindow;

                    Grid.SetColumn(host, i);

                    grid.Children.Add(host);

                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }
            //布局窗体为3
            else if (LayoutFrmNum == 3)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器
                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体
                    HWindow.Name = "Disp_Window" + (i + 1).ToString();
                    host.Child = HWindow;

                    if (i == 0)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRowSpan(host, 2);//行列并列的位置                       
                    }
                    else if (i == 1)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 2)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }

                    grid.Children.Add(host);
                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }
            //布局窗体为4
            else if (LayoutFrmNum == 4)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器
                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体
                    HWindow.Name = "Disp_Window" + (i + 1).ToString();
                    host.Child = HWindow;

                    if (i == 0)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 0);//行列并列的位置                       
                    }
                    else if (i == 1)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 2)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 3)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }

                    grid.Children.Add(host);
                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }
            //布局窗体为5
            else if (LayoutFrmNum == 5)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器
                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体
                    HWindow.Name = "Disp_Window" + (i + 1).ToString();
                    host.Child = HWindow;

                    if (i == 0)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRowSpan(host, 2);//行列并列的位置                       
                    }
                    else if (i == 1)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 2)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 3)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 4)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }

                    grid.Children.Add(host);
                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }
            //布局窗体为6
            else if (LayoutFrmNum == 6)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器
                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体
                    HWindow.Name = "Disp_Window" + (i + 1).ToString();
                    host.Child = HWindow;

                    if (i == 0)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 0);//行列并列的位置                       
                    }
                    else if (i == 1)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 2)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 3)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 4)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 5)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }

                    grid.Children.Add(host);
                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }
            //布局窗体为8
            else if (LayoutFrmNum == 8)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int i = 0; i < LayoutFrmNum; i++)
                {
                    WindowsFormsHost host = new WindowsFormsHost();//图像窗体容器
                    HalconControl.HWindow_Final HWindow = new HalconControl.HWindow_Final();//图像窗体
                    HWindow.Name = "Disp_Window" + (i + 1).ToString();
                    host.Child = HWindow;

                    if (i == 0)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 0);//行列并列的位置                       
                    }
                    else if (i == 1)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 2)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 3)
                    {
                        Grid.SetColumn(host, 3);
                        Grid.SetRow(host, 0);//设定列的位置         
                    }
                    else if (i == 4)
                    {
                        Grid.SetColumn(host, 0);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 5)
                    {
                        Grid.SetColumn(host, 1);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 6)
                    {
                        Grid.SetColumn(host, 2);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }
                    else if (i == 7)
                    {
                        Grid.SetColumn(host, 3);
                        Grid.SetRow(host, 1);//设定列的位置         
                    }

                    grid.Children.Add(host);
                    Frm_Info.Add(new LayoutInfo { Name = HWindow.Name, HWindow = HWindow });//数据添加进泛型集合

                }
            }


        }

    }

    public class LayoutInfo
    {
        public string Name { get; set; }

        public HalconControl.HWindow_Final HWindow;
    }
}
