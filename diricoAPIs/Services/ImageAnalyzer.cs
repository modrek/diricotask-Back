using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public interface IImageAnalyzer
    {
        Task<ImageAnalysis> AnalyzImageAsync(Stream imageStream);
    }

    
    public class AzureImageAnalyzer : IImageAnalyzer
    {
        private readonly IConfiguration _configuration;

        public AzureImageAnalyzer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ImageAnalysis> AnalyzImageAsync(Stream imageStream)
        {
            var subscriptionKey = _configuration.GetValue<string>("SubscriptionKey");
            var endpoint = _configuration.GetValue<string>("CognitiveServiceEndPoint");

            try
            {
                List<string> result = new List<string>();                
                return await runAsync(endpoint, subscriptionKey, imageStream);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }


        }


        private async Task<ImageAnalysis> runAsync(string endpoint, string key,Stream imageStream )
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };

            //string localImagePath = @"D:\pic.jpg"; // See this repo's readme.md for info on how to get these images. Alternatively, you can just set the path to any appropriate image on your machine.
            // string remoteImageUrl = "https://github.com/Azure-Samples/cognitive-services-sample-data-files/raw/master/ComputerVision/Images/landmark.jpg";

            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };

            
            //await AnalyzeFromUrlAsync(computerVision, remoteImageUrl, features);
            //AnalyzeLocalAsync(computerVision, localImagePath, features);
            return await analyzeStreamAsync(computerVision, imageStream, features);
            
        }


        // stream
        private async Task<ImageAnalysis> analyzeStreamAsync(ComputerVisionClient computerVision, Stream imageStream, List<VisualFeatureTypes> features)
        {
            List<string> AnalyzResult = new List<string>();
            ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
           
            return analysis;
            //foreach (var caption in analysis.Categories)
            //{                
            //    AnalyzResult.Add(caption.Name);
            //}
            //return AnalyzResult;


        }
        // Analyze a remote image
        private  async Task analyzeFromUrlAsync(ComputerVisionClient computerVision, string imageUrl, List<VisualFeatureTypes> features)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                 throw new Exception($"Invalid remote image url:{imageUrl}");                
            }

            ImageAnalysis analysis = await computerVision.AnalyzeImageAsync(imageUrl, features);
            
        }

        // Analyze a local image
        private  async Task analyzeLocalAsync(ComputerVisionClient computerVision, string imagePath, List<VisualFeatureTypes> features)
        {
            List<string> AnalyzResult = new List<string>();
            if (!File.Exists(imagePath))
            {
                throw new Exception($"Unable to open or read local image path:{imagePath} ");
                
            }

            using (Stream imageStream = File.OpenRead(imagePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
                //DisplayResults(analysis, imagePath);
                foreach (var caption in analysis.Categories)
                {                    
                    AnalyzResult.Add(caption.Name);
                }

                //return result;
            }
        }

        
    }

}
