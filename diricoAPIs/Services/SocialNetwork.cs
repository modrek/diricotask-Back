
using diricoAPIs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{

    #region Interface
    public interface ISocialNetwork
    {
        Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl);
        Task<List<VideoScaled>> CreateVideosAsync(string remoteUrl);
    }

    #endregion Interface

    #region concrete class
    public class FaceBook :ISocialNetwork
    {
        private readonly IVideoConverter _azureVideoConverter;
        private readonly IImageConverter _azureImageConverter;
        private List<ImageInfo> imageRequiredScaled = new List<ImageInfo>();
        private List<VideoInfo> videoRequiredScaled = new List<VideoInfo>();
        

        public FaceBook(IVideoConverter azureVideoConverter, IImageConverter azureImageConverter)
        {
            _azureVideoConverter = azureVideoConverter;
            _azureImageConverter = azureImageConverter;
            /////
            //// we can get this data from API or Database
            imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Bmp, Width = 100, Height = 200 });
            imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Gif, Width = 100, Height = 200 });
            imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Jpeg, Width = 64, Height = 64});
          
            //    {
            //        new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q320 },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.QFullHD },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q4K },
            //};
        }

        public async Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl)
        {
            List<ImageScaled> Images = new List<ImageScaled>();
            foreach (var img in imageRequiredScaled)
                Images.Add(new ImageScaled
                {
                    Extention=img.Extention,
                    Width=img.Width,
                    Height=img.Height,
                    imageString= await _azureImageConverter.ConvertAsync(remoteUrl,img.Width,img.Height,img.Extention)
                });

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
                    stream =await _azureVideoConverter.ConvertAsync(remoteUrl,vd.Quality,vd.Extention)
                });

            return videos;
        }
    }

    public class Twitter : ISocialNetwork
    {
        private readonly IVideoConverter _azureVideoConverter;
        private readonly IImageConverter _azureImageConverter;
        private List<ImageInfo> imageRequiredScaled = new List<ImageInfo>();
        private List<VideoInfo> videoRequiredScaled = new List<VideoInfo>();
        public Twitter(IVideoConverter azureVideoConverter, IImageConverter azureImageConverter)
        {

            _azureVideoConverter = azureVideoConverter;
            _azureImageConverter = azureImageConverter;
            //requiredScaled.AddRange({
            //    new ImageInfo { Extention = ImageFormat.jpg, Width = 100, Height = 200 },
            //    new ImageInfo { Extention = ImageFormat.png, Width = 100, Height = 200 },
            //    new ImageInfo { Extention = ImageFormat.bmp, Width = 256, Height = 256 }}
            //   );
            //    {
            //        new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q320 },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.QFullHD },
            //    new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q4K },
            //};
        }
        public Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl)
        {
            return null;
        }

        public Task<List<VideoScaled>> CreateVideosAsync(string remoteUrl)
        {
            throw new NotImplementedException();
        }
    }

    #endregion concrete class
}
