using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace diricoAPIs.Domain.Models
{
    public class ImageInfo
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageFormat Extention { get; set; }
        public string FoldersName { get; set; }
        public string GetFolder
        {
            get
            {
                
                return FoldersName;
            }
        }
    }

    public class VideoInfo
    {
        public VideoQuality Quality { get; set; }
        public int Lenght { get; set; }
        public VideoFormat Extention { get; set; }
        public int MaxLenght { get; set; }
        public string FoldersName { get; set; }
    }

    public class ImageScaled : ImageInfo
    {
        public string ImageBase64 { get; set; }
    }

    public class VideoScaled : VideoInfo
    {
        public Stream stream { get; set; }
    }
}
