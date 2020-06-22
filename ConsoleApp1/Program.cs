using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Image imagorg = Image.FromFile(@"https://assetdricotask.blob.core.windows.net/imagecontainer/Original/1637284425373314928_cbfbd2d0-cff5-4f64-9c63-395a34cb6324.jpg");
            Image newImage = ImageLibrary.ImageServices.ResizeImage(imagorg, 256, 256,ImageFormat.Png);
            newImage.Save(@"D:\new.png", ImageFormat.Png);

        }
    }
}
