using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using HalconDotNet;

namespace Plugin.TemplateMatch
{
    /// <summary>
    /// 模板匹配工具
    /// </summary>
    public class MatchTool
    {

        HalconControl.HWindow_Final hWindow_Final;

        HImage image;

        HTuple hv_WindowHandle;

        BrushType brush;

        private HObject brush_region = new HObject();//笔刷

        public HObject final_region;//需要获得的区域

        /// <summary>
        ///构造函数 1
        /// </summary>
        public MatchTool()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="in_hWindow_Final">输入窗体</param>
        /// <param name="hv_InImage">输入图像</param>
        /// <param name="type">笔画类型</param>
        public MatchTool(HalconControl.HWindow_Final in_hWindow_Final, HImage hv_InImage, BrushType type, HObject painRegion)
        {
            hWindow_Final = in_hWindow_Final;
            image = hv_InImage;
            hv_WindowHandle = in_hWindow_Final.getHWindowControl().HalconWindow; ;
            brush = type;
            final_region = painRegion;
        }

        //设置画笔
        public void set_hwindow_brush()
        {
            HTuple hv_Row1 = null, hv_Column1 = null, hv_Row2 = null, hv_Column2 = null;
            HObject ho_temp_brush = new HObject();

            try
            {
                //画图模式 开
                hWindow_Final.DrawModel = true;
                hWindow_Final.Focus();
                //显示提示
                hWindow_Final.ClearWindow();
                hWindow_Final.HobjectToHimage(image);

                ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                ViewROI.HalconTool.disp_message(hv_WindowHandle, "在窗口画出" + brush.ToString() + ",点击右键结束", "window", 20, 20, "red", "false");

                //显示为黄色
                HOperatorSet.SetColor(hv_WindowHandle, "yellow");

                switch (brush)
                {
                    case BrushType.矩形:
                        HOperatorSet.DrawRectangle1(hv_WindowHandle, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                        ho_temp_brush.Dispose();
                        HOperatorSet.GenRectangle1(out ho_temp_brush, hv_Row1, hv_Column1, hv_Row2, hv_Column2);
                        if (hv_Row1.D != 0)
                        {
                            brush_region.Dispose();
                            brush_region = ho_temp_brush;
                        }
                        else
                        {
                            hWindow_Final.HobjectToHimage(image);
                            ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                            ViewROI.HalconTool.disp_message(hv_WindowHandle, "未画出有效区域", "window", 20, 20, "red", "false");
                            return;
                        }
                        break;
                    case BrushType.圆形:
                        HTuple radius;
                        HOperatorSet.DrawCircle(hv_WindowHandle, out hv_Row1, out hv_Column1, out radius);

                        ho_temp_brush.Dispose();
                        HOperatorSet.GenCircle(out ho_temp_brush, hv_Row1, hv_Column1, radius);
                        //
                        if (hv_Row1.D != 0)
                        {
                            brush_region.Dispose();
                            brush_region = ho_temp_brush;
                        }
                        else
                        {
                            hWindow_Final.HobjectToHimage(image);
                            ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                            ViewROI.HalconTool.disp_message(hv_WindowHandle, "未画出有效区域", "window", 20, 20, "red", "false");
                            return;
                        }
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("错误指令");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                hWindow_Final.HobjectToHimage(image);
                ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                ViewROI.HalconTool.disp_message(hv_WindowHandle, brush.ToString() + " 笔刷创建成果", "window", 20, 20, "red", "false");
                hWindow_Final.DispObj(ho_temp_brush, "yellow");
                hWindow_Final.DrawModel = false;
            }
        }

        //涂抹
        public void set_hwindow_paint(BrushAction brush, HObject hXLD)
        {
            if (hWindow_Final == null)
            {
                System.Windows.Forms.MessageBox.Show("输入窗体为Null！");
                return;
            }

            hWindow_Final.DrawModel = true;
            hWindow_Final.Focus();

            HTuple hv_Button = null;
            HTuple hv_Row = null, hv_Column = null;
            HTuple areaBrush, rowBrush, columnBrush, homMat2D;

            HObject brush_region_affine = new HObject();
            HObject ho_Image = new HObject(image);

            try
            {
                if (!brush_region.IsInitialized())
                {
                    System.Windows.Forms.MessageBox.Show("请先设置笔刷！");
                    return;
                }
                else
                {
                    HOperatorSet.AreaCenter(brush_region, out areaBrush, out rowBrush, out columnBrush);
                }

                //显示
                hWindow_Final.HobjectToHimage(image);
                hWindow_Final.DispObj(final_region);
                if (hXLD != null)
                    hWindow_Final.DispObj(hXLD, "red");

                switch (brush)
                {
                    case BrushAction.涂抹:
                        HOperatorSet.SetColor(hv_WindowHandle, "blue");
                        break;
                    case BrushAction.擦除:
                        HOperatorSet.SetColor(hv_WindowHandle, "red");
                        //检查final_region是否有效
                        if (!final_region.IsInitialized())
                        {
                            System.Windows.Forms.MessageBox.Show("请先使用画出合适区域,在使用擦除功能");
                            return;
                        }
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("设置错误！");
                        break;
                }

                //鼠标状态
                hv_Button = 0;
                while (hv_Button != 4)
                {
                    //一直在循环,需要让halcon控件也响应事件,不然到时候跳出循环,之前的事件会一起爆发触发,
                    System.Windows.Forms.Application.DoEvents();

                    hv_Row = -1;
                    hv_Column = -1;

                    //获取鼠标坐标
                    try
                    {
                        HOperatorSet.GetMposition(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Button);
                    }
                    catch (HalconException ex)
                    {
                        hv_Button = 0;
                    }

                    HOperatorSet.SetSystem("flush_graphic", "false");
                    HOperatorSet.DispObj(ho_Image, hv_WindowHandle);
                    if (hXLD != null)
                        HOperatorSet.DispObj(hXLD, hv_WindowHandle);

                    if (final_region.IsInitialized())
                    {
                        HOperatorSet.DispObj(final_region, hv_WindowHandle);
                    }

                    if (hv_Row >= 0 && hv_Column >= 0)
                    {
                        //放射变换
                        HOperatorSet.VectorAngleToRigid(rowBrush, columnBrush, 0, hv_Row, hv_Column, 0, out homMat2D);
                        brush_region_affine.Dispose();
                        HOperatorSet.AffineTransRegion(brush_region, out brush_region_affine, homMat2D, "nearest_neighbor");
                        HOperatorSet.DispObj(brush_region_affine, hv_WindowHandle);

                        HOperatorSet.SetSystem("flush_graphic", "true");
                        ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                        ViewROI.HalconTool.disp_message(hv_WindowHandle, "按下鼠标左键涂画,右键结束", "window", 20, 20, "red", "false");

                        if (hv_Button == 1)
                        {
                            switch (brush)
                            {
                                case BrushAction.涂抹:
                                    {
                                        if (final_region.IsInitialized())
                                        {
                                            HObject ExpTmpOutVar_0;
                                            HOperatorSet.Union2(final_region, brush_region_affine, out ExpTmpOutVar_0);
                                            final_region.Dispose();
                                            final_region = ExpTmpOutVar_0;
                                        }
                                        else
                                        {
                                            final_region = new HObject(brush_region_affine);
                                        }
                                    }
                                    break;
                                case BrushAction.擦除:
                                    {
                                        HObject ExpTmpOutVar_0;
                                        HOperatorSet.Difference(final_region, brush_region_affine, out ExpTmpOutVar_0);
                                        final_region.Dispose();
                                        final_region = ExpTmpOutVar_0;
                                    }
                                    break;
                                default:
                                    System.Windows.Forms.MessageBox.Show("设置错误！");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        ViewROI.HalconTool.set_display_font(hv_WindowHandle, 20, "mono", new HTuple("true"), new HTuple("false"));
                        ViewROI.HalconTool.disp_message(hv_WindowHandle, "请将鼠标移动到窗口内部", "window", 20, 20, "red", "false");
                    }
                }
            }
            catch (HalconException HDevExpDefaultException)
            {
                throw HDevExpDefaultException;
            }
            finally
            {
                hWindow_Final.HobjectToHimage(image);
                hWindow_Final.DispObj(final_region, "blue");
                hWindow_Final.DrawModel = false;
            }
        }
    }

    public enum BrushType
    {
        矩形,
        圆形,
    }
    public enum BrushAction
    {
        涂抹,
        擦除,
    }

}
