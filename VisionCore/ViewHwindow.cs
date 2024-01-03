using Common;
using HalconDotNet;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VisionCore
{
    [Serializable]
    public class ViewHwindow
    {

        //HWindow窗体的显示
        public delegate void RepaintHwindow();

        //委托变量
        public RepaintHwindow m_RepaintHwindow;

        //窗体显示的图像
        public HImageExt m_HImageExt = new HImageExt();

        //窗体中显示的ROI
        private List<ModuleROI> m_RoiList = new List<ModuleROI>();

        //窗体中显示的ROI
        private List<ModuleROIText> m_TxtList = new List<ModuleROIText>();

        //窗体显示模式设置
        private ViewModel showModel = ViewModel.显示效果图;

        //显示的窗体
        public HalconControl.HWindow_Final hWindow;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ViewHwindow()
        {
            m_RepaintHwindow = ShwoModel;
        }

        public void ShwoModel()
        {
            hWindow.HobjectToHimage(m_HImageExt);//显示图片

            m_RoiList = m_HImageExt.moduleROIlist.ToList();
            m_TxtList = m_HImageExt.moduleTxtlist.ToList();

            foreach (ModuleROI item in m_RoiList)
            {
                if (item.IsDispImage)
                {
                    hWindow.DispObj(item.hObject, item.DrawColor);
                }
            }

            foreach (ModuleROIText item in m_TxtList)
            {
                if (item.Text != null)
                {
                    hWindow.DispTxt(item.Text, item.DrawColor);
                }
            }
        }
    }

    /// <summary>
    /// ROI显示模式
    /// </summary>
    public enum ViewModel
    {
        显示原图,
        显示效果图,
        显示检测范围,
        显示检测点,
        显示检测结果,
        显示搜索范围
    }
}
