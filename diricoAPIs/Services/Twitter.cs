using diricoAPIs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public class Twitter : ISocialNetwork
    {
        private readonly IVideoConverter _VideoConverter;
        private readonly IImageConverter _ImageConverter;

        private List<ImageInfo> _imageRequiredScaled = new List<ImageInfo>();
        private List<VideoInfo> _videoRequiredScaled = new List<VideoInfo>();

        private string _imageBaseFolders = "Twitter/Images";
        private string _videoBaseFolders = "Twitter/Videos";


        public Twitter(IVideoConverter VideoConverter, IImageConverter ImageConverter)//,IConfiguration configuration)
        {
            _VideoConverter = VideoConverter;
            _ImageConverter = ImageConverter;
            // /////
            //// we can get this data from API or Database
            _imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Png, Width = 64, Height = 64, FoldersName = "Small/Thumbnail" });
            _imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Png, Width = 256, Height = 256, FoldersName = "Medium/Personal/Folder 1" });
            _imageRequiredScaled.Add(new ImageInfo { Extention = ImageFormat.Png, Width = 128, Height = 128, FoldersName = "Medium/Personal/Folder 2" });

            _videoRequiredScaled.AddRange(new List<VideoInfo>() {
                new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q320 },
                new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.QFullHD },
                new VideoInfo { Extention = VideoFormat.mp4, Quality = VideoQuality.Q4K },
            });
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
                    FoldersName = img.FoldersName
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
                    stream = await _VideoConverter.ConvertAsync(remoteUrl, vd.Quality, vd.Extention)
                });

            return videos;
        }

        public List<ImageInfo> GetImageRequired()
        {
            return _imageRequiredScaled;
        }

        public List<VideoInfo> GetVideoRequired()
        {
            return _videoRequiredScaled;
        }

        public string GetBaseImageFolders()
        {
            return _imageBaseFolders;
        }

        public string GetBaseVideoFolders()
        {
            return _videoBaseFolders;
        }
    }



}
