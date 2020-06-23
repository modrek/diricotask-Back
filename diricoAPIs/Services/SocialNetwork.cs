
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

    public interface ISocialNetwork
    {
        Task<List<ImageScaled>> CreateImagesAsync(string remoteUrl);
        Task<List<VideoScaled>> CreateVideosAsync(string remoteUrl);
        
        string GetBaseImageFolders();
        string GetBaseVideoFolders();
    }



   
    
}
