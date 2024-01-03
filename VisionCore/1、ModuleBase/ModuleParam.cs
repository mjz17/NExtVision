using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 模块参数
    /// </summary>
    [Serializable]
    public class ModuleParam
    {
        public int ProjectID { get; set; } = -1;                    //流程ID

        public int ModuleID { get; set; } = 0;                      //模块ID

        public string ModuleName { get; set; } = "";                //模块名称+模块编号

        public string ModuleDesCribe { get; set; } = string.Empty;  //单元描述

        public string Remarks { get; set; } = "";                   //模块注释

        public bool IsUse { get; set; } = true;                    //是否屏蔽//false不屏蔽

        [NonSerialized]
        public bool BlnSuccessed = false;                           //是否执行成功

        public int ModuleEncode { get; set; } = 0;                  //模块编号

        [NonSerialized]
        public int ModuleCostTime = 0;                              //模块运行时间

        [NonSerialized]
        public int pIndex = -1;                                     //当前模块所在的循环的index地址,不需要序列化

        [NonSerialized]
        public int CyclicCount = 0;                                 //针对循环工具 用来模拟循环多少次的,实际中,当循环次数到达CyclicCount的时候

        public string PluginName { get; set; } = "";                //插件名称，每一个插件都是唯一的

        public string ImageName { get; set; } = "";                //使用VAR中图片名称  

    }
}
