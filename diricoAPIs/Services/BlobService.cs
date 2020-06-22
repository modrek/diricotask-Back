
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public interface IBlobService
    {
        Task<IEnumerable<Uri>> ListAsync(string path);

        Task<string> UploadAcync(IFormFileCollection files, string filename, string directories);

        Task<string> UploadStreamAcync(Stream stream,string filename,string directories);

        Task DeleteAsync(string fileUri);
        Task DeleteAllAcync();

    }

    public class AzureBlobService : IBlobService
    {
        private readonly IBlobConnectionFactory _azureBlobConnectionFactory;        
        private readonly IConfiguration _configuration;

        public AzureBlobService(
            IBlobConnectionFactory azureBlobConnectionFactory, 
            IConfiguration configuration)
        {
            _azureBlobConnectionFactory = azureBlobConnectionFactory;            
            _configuration = configuration;
            
        }

       
        public async Task DeleteAllAcync()
        {
            var blobCotainer = await _azureBlobConnectionFactory.GetBlobContainer();
            
            BlobContinuationToken blobContinuationToken = null;
            do
            {

                var response = await blobCotainer.ListBlobsSegmentedAsync(blobContinuationToken);
                foreach (IListBlobItem blob in response.Results)
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        await((CloudBlockBlob)blob).DeleteIfExistsAsync();
                }
                blobContinuationToken = response.ContinuationToken;

            } while (blobContinuationToken != null);
        }

        public async Task DeleteAsync(string fileUri)
        {
            var blobCotainer = await _azureBlobConnectionFactory.GetBlobContainer();
            Uri uri = new Uri(fileUri);
            string filename = Path.GetFileName(uri.LocalPath);

            var blob = blobCotainer.GetBlockBlobReference(filename);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<IEnumerable<Uri>> ListAsync(string path)
        {
            try
            {
                var blobCotainer = await _azureBlobConnectionFactory.GetBlobContainer();
                var Dic = blobCotainer.GetDirectoryReference(path);
                var allBlobs = new List<Uri>();
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var response = await Dic.ListBlobsSegmentedAsync(blobContinuationToken);
                    foreach (IListBlobItem blob in response.Results)
                    {
                        if (blob.GetType() == typeof(CloudBlockBlob))
                            allBlobs.Add(blob.Uri);
                    }
                    blobContinuationToken = response.ContinuationToken;

                } while (blobContinuationToken != null);

                return allBlobs;
            }
            catch
            {
                return null;
            }
        }


        
        public async Task<string> UploadAcync(IFormFileCollection files, string filename,string directories)
        {            
            var blobCotainer = await _azureBlobConnectionFactory.GetBlobContainer();
            //for (int i = 0; i < 1; i++)
            //{                
                using (var stream = files[0].OpenReadStream())
                {
                    var Dic = blobCotainer.GetDirectoryReference(directories);
                    var blob = Dic.GetBlockBlobReference(filename);
                    await blob.UploadFromStreamAsync(stream);
                    return blob.Uri.AbsoluteUri;
                }
            //}
            
        }

      


        public async Task<string> UploadStreamAcync(Stream stream,string filename ,string directories)
        {
            var blobCotainer = await _azureBlobConnectionFactory.GetBlobContainer();            
            var Dic = blobCotainer.GetDirectoryReference(directories);
            var blob = Dic.GetBlockBlobReference(filename);
            await blob.UploadFromStreamAsync(stream);
            return blob.Uri.AbsoluteUri;

        }

       
    }
}
