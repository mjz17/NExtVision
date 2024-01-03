using HalconControl;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 主窗体Hwindow显示
    /// </summary>
    public class DispHwImg
    {
        #region 视觉窗体显示图片或者检测结果

        /// <summary>
        /// 显示图片或者检测结果
        /// </summary>
        /// <param name="moduleObjBase">模块基类</param>
        /// <param name="hWindow_Final">基类</param>
        public static void UpdateWindow(ModuleObjBase module, HWindow_Final hWindow)
        {
            if (module.m_Image != null && module.m_Image.IsInitialized())
            {
                List<ModuleROI> roiList = module.m_Image.moduleROIlist.Where(c => c.ModuleID == module.ModuleParam.ModuleID.ToString()).ToList();
                hWindow.HobjectToHimage(module.m_Image);
                foreach (ModuleROI roi in roiList)
                {
                    if (roi != null && roi.RoiType == ModuleRoiType.文字显示.ToString())
                    {
                        ModuleROIText roiText = (ModuleROIText)roi;
                    }
                    else
                    {
                        if (roi != null && roi.hObject.IsInitialized())
                        {
                            hWindow.DispObj(roi.hObject, roi.DrawColor);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
