using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace PublicDefine
{
    /// <summary>
    /// 用于展示效果的Hobject
    /// </summary>
    [Serializable]
    public class ModuleROI
    {
        /// <summary>
        /// 模块ID
        /// </summary>
        public string ModuleID { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleCatagory { get; set; }

        /// <summary>
        /// 单元描述
        /// </summary>
        public string ModuleDescribe { get; set; }

        /// <summary>
        /// 轮廓分类
        /// </summary>
        public string RoiType { get; set; }

        /// <summary>
        /// 画笔颜色
        /// </summary>
        public string DrawColor { get; set; }

        /// <summary>
        /// ROI
        /// </summary>
        public HObject hObject { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDispImage { get; set; }

        /// <summary>
        /// 构造函数1
        /// </summary>
        public ModuleROI()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="_moduleID"></param>
        /// <param name="_moduleCatagory"></param>
        /// <param name="_moduleDesCribe"></param>
        /// <param name="_roiType"></param>
        /// <param name="_drawColor"></param>
        /// <param name="_hobject"></param>
        public ModuleROI(string _moduleID, string _moduleCatagory, string _moduleDesCribe,
            string _roiType, string _drawColor, HObject _hobject)
        {
            ModuleID = _moduleID;
            ModuleCatagory = _moduleCatagory;
            ModuleDescribe = _moduleDesCribe;
            ModuleDescribe = _moduleDesCribe;
            RoiType = _roiType;
            DrawColor = _drawColor;
            hObject = _hobject;
        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="_moduleID"></param>
        /// <param name="_moduleCatagory"></param>
        /// <param name="_moduleDesCribe"></param>
        /// <param name="_roiType"></param>
        /// <param name="_drawColor"></param>
        /// <param name="_hobject"></param>
        public ModuleROI(string _moduleID, string _moduleCatagory, string _moduleDesCribe,
            string _roiType, string _drawColor, HObject _hobject, bool isCheck)
        {
            ModuleID = _moduleID;
            ModuleCatagory = _moduleCatagory;
            ModuleDescribe = _moduleDesCribe;
            ModuleDescribe = _moduleDesCribe;
            RoiType = _roiType;
            DrawColor = _drawColor;
            hObject = _hobject;
            IsDispImage = isCheck;
        }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            if (hObject != null && !hObject.IsInitialized())//修复为null 错误 magical 20171103
            {
                hObject = null;
            }
        }

    }

    /// <summary>
    /// 用于展示文字的Hobject
    /// </summary>
    [Serializable]
    public class ModuleROIText : ModuleROI
    {
        /// <summary>
        /// 文字内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 字体大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_moduleID"></param>
        /// <param name="_moduleCatagory"></param>
        /// <param name="_moduleDesCribe"></param>
        /// <param name="_roiType"></param>
        /// <param name="_drawColor"></param>
        /// <param name="_text"></param>
        /// <param name="_font"></param>
        /// <param name="_row"></param>
        /// <param name="_col"></param>
        /// <param name="_size"></param>
        public ModuleROIText(string _moduleID, string _moduleCatagory, string _moduleDesCribe, string _roiType, string _drawColor,
            string _text, int _size)
        {
            ModuleID = _moduleID;
            ModuleCatagory = _moduleCatagory;
            ModuleDescribe = _moduleDesCribe;
            RoiType = _roiType;
            DrawColor = _drawColor;
            Text = _text;
            Size = _size;
        }

    }

    /// <summary>
    /// 轮廓分类
    /// </summary>
    public enum ModuleRoiType
    {
        搜索范围,
        搜索方向,
        屏蔽范围,
        检测点,
        检测范围,
        检测结果,
        参考坐标系,
        文字显示,
    }

}
