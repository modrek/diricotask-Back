using diricoAPIs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public class VariantAssetConfig
    {
        private List<ISocialNetwork> _socialNetworks= new List<ISocialNetwork>();
        private static VariantAssetConfig instance = null;

        public VariantAssetConfig AddSocialNetwork(ISocialNetwork socialNetwork)
        {
            _socialNetworks.Add(socialNetwork);
            return this;
        }

        public static VariantAssetConfig Instance
        {
            get
            {
                if (VariantAssetConfig.instance == null)
                    VariantAssetConfig.instance = new VariantAssetConfig();

                return VariantAssetConfig.instance;
            }
        }


        public VariantAssetConfig GetInstance()
        {
            if (instance == null)
            {
                instance = new VariantAssetConfig();
                return instance;
            }
            else
                return instance;

        }

        public List<ImageScaled> ApplyImage(string ImageremoteURL)
        {
            List<ImageScaled> result = new List<ImageScaled>();
            foreach (var item in _socialNetworks)
            {
               result.AddRange( item.CreateImagesAsync(ImageremoteURL).Result);                
            }

            return result;
        }

        public List<VideoScaled> ApplyVideo(string VideoremoteURL)
        {
            List<VideoScaled> result = new List<VideoScaled>();
            foreach (var item in _socialNetworks)
            {
                result.AddRange(item.CreateVideosAsync(VideoremoteURL).Result);
            }

            return result;
        }
    }
}
