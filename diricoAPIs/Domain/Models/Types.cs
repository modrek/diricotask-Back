using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Models
{
    public enum AssetTypes
    {
        Folder = 0,
        Image = 1,
        Video = 2
    }
    //public enum ImageFormat
    //{
    //    png,
    //    jpg,
    //    bmp,
    //    svg
    //}
    public enum VideoFormat
    {
        avi,
        mp4,
        mpg,
        wvm
    }

    public enum VideoQuality
    {
        QFullHD,
        Q4K,
        Q320,
        Q720
    }
}
