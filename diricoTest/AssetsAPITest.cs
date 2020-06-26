using diricoAPIs.Controllers;
using diricoAPIs.Domain.Models;
using diricoAPIs.Domain.Repositories;
using diricoAPIs.Logger;
using diricoAPIs.Services;
using LoggerService;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace diricoTest
{
    public class AssetsAPITest : DiricoDBContextTest
    {
        AssetController _controller;

        private readonly ILoggerManager _logger;
        private readonly IBlobService _azureBlobService;
        private readonly IImageAnalyzer _azureImageAnalyzer;
        private readonly IImageConverter _azureImageConverter;
        private readonly IVideoConverter _azureVideoConverter;
        private readonly IAssetRepository _assetRepository;
        


        public AssetsAPITest()
        {
            _logger = new LoggerManager();
            _assetRepository = new AssetRepository(_context);
            //_azureBlobService = new AzureBlobService();
            //_azureImageAnalyzer = new AzureImageAnalyzer();
            //_azureImageConverter = new AzureImageConverter();
            //_azureVideoConverter = new AzureVideoConverter();
            
            

            //_controller = new AssetController();
            
        }

        [Fact]
        public void GetFoldersTest()
        {
            var okResult= _controller.GetFolders(null);

        }
    }
}
