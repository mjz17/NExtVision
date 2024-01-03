using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StyleBase
{
    public class ImageTool
    {
        public static System.Drawing.Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            try
            {
                if (imageSource != null)
                {
                    BitmapSource m = (BitmapSource)imageSource;
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);// 坑点：选Format32bppRgb将不带透明度
                    System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                        new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size), 
                        System.Drawing.Imaging.ImageLockMode.WriteOnly, 
                        System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                    m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                    bmp.UnlockBits(data);
                    return bmp;
                }
                else
                {
                    return new System.Drawing.Bitmap(1, 1);
                }
            }
            catch (Exception)
            {
                return new System.Drawing.Bitmap(1, 1);
            }
        }
    }
}
