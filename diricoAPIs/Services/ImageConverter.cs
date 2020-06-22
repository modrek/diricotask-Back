
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace diricoAPIs.Services
{
    public interface IImageConverter
    {
         Task<string> ConvertAsync(string remoteUrl, int width ,int height , ImageFormat extention);
    }

    public class AzureImageConverter : IImageConverter
    {
        private readonly IConfiguration _configuration;

        public AzureImageConverter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> ConvertAsync(string remoteUrl, int width, int height, ImageFormat extention)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            var subscriptionKey = _configuration.GetValue<string>("SubscriptionKey");
            var GenerateThumbnailEndPoint = _configuration.GetValue<string>("GenerateThumbnailEndPoint");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",  subscriptionKey );
           

            // Request parameters
            queryString["width"] = $"{width}";
            queryString["height"] = $"{height}";
            queryString["smartCropping"] = "true";
            var uri = GenerateThumbnailEndPoint + queryString;
       
            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(@"{'url':'"+ remoteUrl + "'}");
            try
            {
               
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(uri, content);
                   
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsByteArrayAsync();
                        return bytesToSrcString(responseContent);
                                               
                    }
                }

            }
            catch (Exception ee)
            {

                return null;
            }
            return null;
        }

        private string bytesToSrcString(byte[] bytes) => Convert.ToBase64String(bytes);

    }

    public class ModrekImageConverter : IImageConverter
    {
        public async Task<string> ConvertAsync(string remoteUrl, int width, int height, ImageFormat extention)
        {
            try
            {

                Image image = GetImageFromURL(remoteUrl);
                Image newimg = ImageLibrary.ImageServices.ResizeImage(image, width, height);

                using (MemoryStream m = new MemoryStream())
                {
                    newimg.Save(m, extention);
                    byte[] imageBytes = m.ToArray();

                    string base64String = Convert.ToBase64String(imageBytes);
                    
                    return base64String;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            
        }
        private Image GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }

    }


}
