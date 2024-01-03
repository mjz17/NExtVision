using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace VisionCore
{
    /// <summary>
    /// 创建模板参数
    /// </summary>
    [Serializable]
    public class CreateMatchPram
    {
        public int numLevels { get; set; } = 5;                 //金字塔层数*[定义数组][开始层和结束层][可以达到多层金字塔搜索]
        public double angleStart { get; set; } = -30;           //开始角度//最小角度*
        public double angleExtent { get; set; } = 30;           //角度范围//最大角度*
        public HTuple angleStep { get; set; } = "auto";          //角度步长//设定值越小，程序耗时越长//auto
        public double scaleMin { get; set; } = 0.9;             //最小缩放//最小比例*
        public double scaleMax { get; set; } = 1.1;             //最大缩放//最大比例*
        public HTuple scaleStep { get; set; } = "auto";         //缩放步长
        public HTuple optimization { get; set; } = "auto";      //优化方式
        public string metric { get; set; } = "use_polarity";    //极性控制*
        //黑白对比度一致//使用极性use_polarity
        //黑白对比度不一致//忽略极性ignore_global_polarity
        //黑白对比局部不一致//忽略局部极性ignore_local_polarity
        public int contrast { get; set; } = 30;                 //对比度*
        public HTuple minContrast { get; set; } = "auto";       //最小灰度值*//auto

    }

    /// <summary>
    /// 查询模板参数
    /// </summary>
    [Serializable]
    public class FindMatchPram
    {
        public double angleStart { get; set; } = -30;           //开始角度//最小角度*
        public double angleExtent { get; set; } = 30;           //角度范围//最大角度*
        public double scaleMin { get; set; } = 0.9;             //最小缩放//最小比例*
        public double scaleMax { get; set; } = 1.1;             //最大缩放//最大比例*
        public double minScore { get; set; } = 0.5;             //得分
        public int numMatches { get; set; } = 1;                //匹配个数
        public double maxOverlap { get; set; } = 0.5;           //最大重叠
        public HTuple subPixel { get; set; } = "least_squares"; //是否亚像素//一般不选择//least_squares
        public HTuple numLevels { get; set; } = 5;              //金字塔层数*
        public double greediness { get; set; } = 0.5;           //贪婪度//值越小越容易匹配
        public HTuple row { get; set; } = 0;
        public HTuple column { get; set; } = 0;
        public HTuple angle { get; set; } = 0;
        public HTuple scale { get; set; } = 0;
        public HTuple score { get; set; } = 0;

    }
}
