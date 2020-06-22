
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Services
{
    public interface IBlobConnectionFactory
    {
        Task<CloudBlobContainer> GetBlobContainer();
        Task<CloudBlobContainer> CreateContainer(string containerName);
    }

    public class AzureBlobConnectionFactory : IBlobConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        

        public AzureBlobConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public async Task<CloudBlobContainer> CreateContainer(string containerName)
        {
            if (_blobContainer != null)
                return _blobContainer;

            
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Configuration must contain ContainerName");
            }

            var blobClient = getBlobClient();

            _blobContainer = blobClient.GetContainerReference(containerName);
            if (await _blobContainer.CreateIfNotExistsAsync())
            {
                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return _blobContainer;
        }

        public async Task<CloudBlobContainer> GetBlobContainer()
        {
            if (_blobContainer!=null)            
                return _blobContainer;

            var containerName = _configuration.GetValue<string>("ContainerName");
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Configuration must contain ContainerName");
            }

            var blobClient = getBlobClient();           

            _blobContainer = blobClient.GetContainerReference(containerName);
            if (await _blobContainer.CreateIfNotExistsAsync())
            {
                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            return _blobContainer;





        }

        private CloudBlobClient getBlobClient()
        {
            if (_blobClient != null)
                return _blobClient;

            var storageConnectionString = _configuration.GetValue<string>("StorageConnectionString");
            if(string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("Configuration must contain StorageConnectionString");      
            }

            if(!CloudStorageAccount.TryParse(storageConnectionString,out CloudStorageAccount storageAccount ))
            {
                throw new Exception("Could not create storage account whith StorageConnectionString configuration");
            }

            _blobClient = storageAccount.CreateCloudBlobClient();
            return _blobClient;
        }

    }
}
