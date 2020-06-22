using diricoAPIs.Domain.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public interface IVideoConverter
    {
         Task<Stream> ConvertAsync(string remoteUrl, VideoQuality videoQuality , VideoFormat videoFormat);
    }

    public class AzureVideoConverter : IVideoConverter
    {
        public Task<Stream> ConvertAsync(string remoteUrl, VideoQuality videoQuality, VideoFormat videoFormat)
        {
            throw new NotImplementedException();
        }
    }

}
