using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Windows.Interop;

namespace StyleBase
{

    /// <summary>
    /// 光标工具
    /// </summary>
    public class CursorTool
    {
        /// <summary>
        /// 自定义光标
        /// </summary>
        /// <param name="width">光标宽度</param>
        /// <param name="height">光标高度</param>
        /// <param name="fontSize">光标字体的大小</param>
        /// <param name="ico">显示的图片</param>
        /// <param name="imageSize">图片尺寸</param>
        /// <param name="text">显示的内容</param>
        /// <returns></returns>
        public static Cursor CreateCursor(int width, int height, float fontSize, Bitmap ico, int imageSize, string text)
        {
            Bitmap m_Buff = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(m_Buff))
            {
                graphics.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, width, height);
                using (System.Drawing.Font font = new System.Drawing.Font("宋体", fontSize, System.Drawing.FontStyle.Regular))
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        graphics.DrawString(text, font, brush, imageSize + 10, (height - fontSize) / 2);
                    }
                }
                graphics.DrawImage(ico, 0, (height - imageSize) / 2, imageSize, imageSize);
            }
            return CreateCursor(m_Buff, 0, 0);
        }

        private static Cursor CreateCursor(Bitmap bm, uint xHotSpot = 0, uint yHotSpot = 0)
        {
            Cursor ret = null;
            if (bm == null)
            {
                return ret;
            }
            try
            {
                ret = InternalCreateCursor(bm, xHotSpot, yHotSpot);
            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;
        }

        private static Cursor InternalCreateCursor(Bitmap bitmap, uint xHotSpot, uint yHotSpot)
        {
            var iconInfo = new NativeMethods.IconInfo();
            NativeMethods.GetIconInfo(bitmap.GetHicon(), ref iconInfo);
            iconInfo.xHotspot = xHotSpot;
            iconInfo.yHotspot = yHotSpot;
            iconInfo.ficon = false;
            SafeIconHandle cursorHandle = NativeMethods.CreateIconIndirect(ref iconInfo);
            return CursorInteropHelper.Create(cursorHandle);
        }

    }

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeIconHandle() : base(true)
        {

        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            return NativeMethods.DestroyIcon(handle);
        }

    }

    public static class NativeMethods
    {
        public struct IconInfo
        {
            public bool ficon;
            public uint xHotspot;
            public uint yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
    }

}
