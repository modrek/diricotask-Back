using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageLibrary
{
    public static class ImageServices
    {
        public static Image ResizeImage(Image img, int maxWidth, int maxHeight,ImageFormat ext)
        {            
            using (img)
            {
                Double xRatio = (double)img.Width / maxWidth;
                Double yRatio = (double)img.Height / maxHeight;
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int)Math.Floor(img.Width / ratio);
                int nny = (int)Math.Floor(img.Height / ratio);
                Bitmap cpy = new Bitmap(nnx, nny, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    gr.Clear(Color.Transparent);

                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    gr.DrawImage(img,
                        new Rectangle(0, 0, nnx, nny),
                        new Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                //return cpy;

                using (MemoryStream m = new MemoryStream())
                {
                    cpy.Save(m, ext);
                    byte[] imageBytes = m.ToArray();

                    using (var streamimage = new MemoryStream(imageBytes))
                    {
                        Image newimg = System.Drawing.Image.FromStream(streamimage);
        //                newimg.Save(@"D:\1.jpg", ext);
                        return newimg;
                    }
                }
            }

        }

    }
}
