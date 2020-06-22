
using diricoAPIs.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{

    #region Interface
    public interface ISocialNetwork
    {
        Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl);
        Task<List<VideoScaled>> CreateVideosAsync(string remoteUrl);

        List<ImageInfo> GetImageRequired();
        List<VideoInfo> GetVideoRequired();
        //string GetImageFolders();
        //string GetVideoFolders();
    }

    #endregion Interface

    #region concrete class
    public class FaceBook :ISocialNetwork
    {
        private readonly IVideoConverter _VideoConverter;
        private readonly IImageConverter _ImageConverter;
       // private readonly IConfiguration _configuration;
        private List<ImageInfo> _imageRequiredScaled = new List<ImageInfo>();
        private List<VideoInfo> _videoRequiredScaled = new List<VideoInfo>();
        private string ImageBaseFolders = "Facebook/Images";
        private string VideoBaseFolders = "Facebook/Videos";


        public FaceBook(IVideoConverter VideoConverter, IImageConverter ImageConverter)//,IConfiguration configuration)
        {
            _VideoConverter = VideoConverter;
            _ImageConverter = ImageConverter;
           // _configuration = configuration;
           // /////
            //// we can get this data from API or Database
            _imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Png, Width = 64, Height = 64,FoldersName="Small/Thumbnail"});
            _imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Png, Width = 256, Height = 256 , FoldersName = "Medium/Personal/Folder 1" });

            //    {
            //        new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q320 },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.QFullHD },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q4K },
            //};
        }

        public async Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl)
        {
            List<ImageScaled> Images = new List<ImageScaled>();
            foreach (var img in _imageRequiredScaled)
            {

                string newimage = await _ImageConverter.ConvertAsync(remoteUrl, img.Width, img.Height, img.Extention);
                Images.Add(new ImageScaled
                {
                    Extention = img.Extention,
                    Width = img.Width,
                    Height = img.Height,
                    ImageBase64 = newimage,
                    FoldersName=img.FoldersName
                });
                
                
            }

            return Images;
        }

        public async Task<List<VideoScaled>> CreateVideosAsync(string remoteUrl)
        {
            List<VideoScaled> videos = new List<VideoScaled>();
            foreach (var vd in videos)
                videos.Add(new VideoScaled
                {
                    Extention = vd.Extention,
                    Quality = vd.Quality,                    
                    stream =await _VideoConverter.ConvertAsync(remoteUrl,vd.Quality,vd.Extention)
                });

            return videos;
        }

        public string GetImagesFolders()
        {
            //var folderstructure= _configuration.GetValue<string>("FolderStructure");
            //if (string.IsNullOrWhiteSpace(folderstructure))
            //{
            //    throw new ArgumentException("Configuration must contain FolderStructure");
            //}

            //if (folderstructure == "Field")
            //    return ImageBaseFolders;
            //else
            //    return "";
            return "";
        }

        public List<ImageInfo> GetImageRequired()
        {
            return _imageRequiredScaled;
        }

        public List<VideoInfo> GetVideoRequired()
        {
            return _videoRequiredScaled;
        }

        public string GetImageFolders()
        {
            throw new NotImplementedException();
        }

        public string GetVideoFolders()
        {
            throw new NotImplementedException();
        }
    }

   

    #endregion concrete class
}
