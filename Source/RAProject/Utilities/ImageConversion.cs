using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RAProject.Utilities
{
    public static class ImageConversion
    {
        /// <summary>
        /// Converts a System.Drawing.Image to a System.Windows.Controls.Image.
        /// </summary>
        /// <param name="gdiImg">System.Drawing.Image</param>
        /// <returns>System.Windows.Controls.Image</returns>
        public static System.Windows.Controls.Image ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
        {
            System.Windows.Controls.Image img = null; 
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Bitmap bmp = new Bitmap(gdiImg);
                IntPtr hBitmap = bmp.GetHbitmap();
                ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                img = new System.Windows.Controls.Image();
                img.Source = WpfBitmap;
                img.Width = 500;
                img.Height = 500;
                img.Stretch = Stretch.Fill;

                
            });
            return img;
        }

        /// <summary>
        /// Converts an image to an imageSource.
        /// </summary>
        /// <param name="img">Image to convert</param>
        /// <returns>An imageSource of the image parsed</returns>
        public static ImageSource ImageToSource (Image img)
        {
            Bitmap bmp = new Bitmap(img);
            IntPtr hBitmap = bmp.GetHbitmap();
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
