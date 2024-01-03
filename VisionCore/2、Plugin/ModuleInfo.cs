using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VisionCore
{

    /// <summary>
    /// 模块信息
    /// </summary>
    [Serializable]
    public class ModuleInfo : DependencyObject
    {
        public Type ModuleObjType { get; set; }     //处理类

        public Type ModuleFormType { get; set; }    //UI类

        public int ModuleNO { get; set; }           //编号

        public string ModuleName { get; set; }      //模块名称  PluginName+ Encode

        public string ModuleRemarks { get; set; }   //模块注释

        public string PluginName { get; set; }      //插件名称  每一个插件都是唯一

        public string PluginCategory { get; set; }  //分类

        public bool IsUse = true;                   //是否启用--!!!!!!!!!放在前面会造成代码块乱码

        [NonSerialized]
        public bool IsSuccess = false;         //是否运行成功

        [NonSerialized]
        public int CostTime = 0;         //模块运行时间ms

        [NonSerialized]
        public ImageSource StateImage; //运行状态

        [NonSerialized]
        public bool IsRunning;       //是否正在运行

        public bool IsUseSuperTool { get; set; }    //使用超级模式

    }
}
