using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VisionCore;

namespace StyleBase
{
    /// <summary>
    /// 模块，树形类
    /// </summary>
    public class ModuleNode : DependencyObject
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="moduleInfo"></param>
        public ModuleNode(ModuleInfo info)
        {
            Children = new List<ModuleNode>();
            Name = info.ModuleName;
            IsUseSuperTool = info.IsUseSuperTool;
            ModuleNo = info.ModuleNO;
            ModuleInfo = info;

            DispName = Name;
            Remarks = info.ModuleRemarks;

            if (info.IsUse)
            {
                CostTime = info.CostTime.ToString() + "ms";//运行时间
            }
            else
            {
                CostTime = info.CostTime.ToString() + "ms";//运行时间
                StateImage = ModuleProject.GetImageByName("禁用");
            }

            IconImage = ModuleProject.GetImageByName(info.ModuleName.Substring(0, info.ModuleName.Length - 1));
        }

        public int ModuleNo { get; set; }
        public ImageSource IconImage { get; set; }

        private ModuleInfo m_ModuleInfo;

        public ModuleInfo ModuleInfo
        {
            get { return m_ModuleInfo; }
            set
            {
                m_ModuleInfo = value;
                if (m_ModuleInfo != null)
                {
                    IsUse = true;
                }
            }
        }

        public bool IsUse { get; set; }

        public bool IsCategory { get; set; } = false;

        public int Hierarchy { get; set; } = 0;//层级


        private string m_Name;

        public string Name
        {
            get { return m_Name.Trim(); }
            set { m_Name = value; }
        }

        public string DispName { get; set; }//显示的名称

        public string Remarks { get; set; }

        public ModuleNode ParentModuleNode { get; set; }//父类

        public List<ModuleNode> Children { get; set; }

        public bool IsExpanded { get; set; }//是否展开

        public string ModuleForeground { get; set; } = "#FFFFFF";

        public bool IsHideExpanded { get; set; } = false;

        //DragOver的时候 下划线加粗
        public static readonly DependencyProperty DragOverHeightProperty =
            DependencyProperty.Register("DragOverHeight", typeof(int), typeof(ModuleNode), new PropertyMetadata((int)1));

        public int DragOverHeight
        {
            get { return (int)GetValue(DragOverHeightProperty); }
            set
            {
                if (value != DragOverHeight)
                {
                    SetValue(DragOverHeightProperty, value);
                }
            }
        }

        //当最后一个元素 是 子集的时候,下划线要往左移动
        public static readonly DependencyProperty LastNodeMarginProperty =
            DependencyProperty.Register("LastNodeMargin", typeof(string), typeof(ModuleNode), new PropertyMetadata((string)"0,0,0,0"));

        public string LastNodeMargin
        {
            get { return (string)GetValue(LastNodeMarginProperty); }
            set
            {
                SetValue(LastNodeMarginProperty, value);
            }
        }

        //模块运行状态
        public static readonly DependencyProperty StateImageProperty =
            DependencyProperty.Register("StateImage", typeof(ImageSource), typeof(ModuleNode), new PropertyMetadata(null));

        public ImageSource StateImage
        {
            get { return (ImageSource)GetValue(StateImageProperty); }
            set
            {
                SetValue(StateImageProperty, value);
            }
        }

        //模块运行时间
        public static readonly DependencyProperty CostTimeProperty =
            DependencyProperty.Register("CostTime", typeof(string), typeof(ModuleNode), new PropertyMetadata(""));
        private int IntCostTime { get; set; }

        public string CostTime
        {
            get { return (string)GetValue(CostTimeProperty); }
            set
            {
                SetValue(CostTimeProperty, value);
            }
        }

        //是否正在运行
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(ModuleNode), new PropertyMetadata(false));

        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set
            {
                SetValue(IsRunningProperty, value);
            }
        }

        //是否第一个元素,要补画上划线
        public static readonly DependencyProperty IsFirstNodeProperty =
            DependencyProperty.Register("IsFirstNode", typeof(bool), typeof(ModuleNode), new PropertyMetadata(false));

        public bool IsFirstNode
        {
            get { return (bool)GetValue(IsFirstNodeProperty); }
            set
            {
                SetValue(IsFirstNodeProperty, value);
            }
        }

        //
        public static readonly DependencyProperty IsUseSuperToolProperty =
            DependencyProperty.Register("IsUseSuperTool", typeof(bool), typeof(ModuleNode), new PropertyMetadata(false));

        public bool IsUseSuperTool
        {
            get { return (bool)GetValue(IsUseSuperToolProperty); }
            set
            {
                SetValue(IsUseSuperToolProperty, value);
            }
        }

        //
        public static readonly DependencyProperty IsMultiSelectedProperty =
            DependencyProperty.Register("IsMultiSelected", typeof(bool), typeof(ModuleNode), new PropertyMetadata(false));

        public bool IsMultiSelected
        {
            get { return (bool)GetValue(IsMultiSelectedProperty); }
            set
            {
                SetValue(IsMultiSelectedProperty, value);
            }
        }


    }

}
