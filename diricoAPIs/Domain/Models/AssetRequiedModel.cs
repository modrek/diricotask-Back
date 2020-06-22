using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Models
{
    public class ImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageFormat Extention { get; set; }
    }

    public class VideoInfo
    {
        public VideoQuality Quality { get; set; }
        public int Lenght { get; set; }
        public VideoFormat Extention { get; set; }
    }

    public class ImageScaled : ImageInfo
    {
        public string imageString { get; set; }
    }

    public class VideoScaled : VideoInfo
    {
        public Stream stream { get; set; }
    }
}
