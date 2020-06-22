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
    public static class Helper
    {
        public static string GetRandomBlobName(string filename, string extention)
        {
            if (!extention.StartsWith('.'))
                extention = "."+extention;
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), extention);
        }

        public static Image GetImageFromURL(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();
            return Image.FromStream(stream);
        }


        public static Stream ImageToStream(this Image image, ImageFormat format)
        {
            var stream = new System.IO.MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        public static Image ByteToImage(byte[] bytes)
        {
            //data:image/gif;base64,
            //this image is a single pixel (black)
            //byte[] bytes = Convert.FromBase64String(Base64);
            string sss = Convert.ToBase64String(bytes);
           
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
                
                return image;
            }

            
        }
    }


}
