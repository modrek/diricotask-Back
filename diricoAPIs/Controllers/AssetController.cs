using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using diricoAPIs.Domain.Models;
using diricoAPIs.Domain.Repositories;
using diricoAPIs.Logger;
using diricoAPIs.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;



namespace diricoAPIs.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("1")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AssetController : Controller
    {
        private readonly ILoggerManager _logger;

        private readonly IBlobService _azureBlobService;
        private readonly IImageAnalyzer _azureImageAnalyzer;
        private readonly IImageConverter _azureImageConverter;
        private readonly IVideoConverter _azureVideoConverter;
        private readonly IAssetRepository _assetRepository;

        public AssetController(
            ILoggerManager logger,
            IBlobService azureBlobService,
            IImageAnalyzer azureImageAnalyzer,
            IImageConverter azureImageConverter,
            IVideoConverter azureVideoConverter,
            IAssetRepository assetRepository
            )
        {
            _logger = logger;
            _azureBlobService = azureBlobService;
            _azureImageAnalyzer = azureImageAnalyzer;
            _azureImageConverter = azureImageConverter;
            _azureVideoConverter = azureVideoConverter;
            _assetRepository = assetRepository;
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteAsset(Guid AssetID)
        {

            var asset = _assetRepository.Get(AssetID);
            await _azureBlobService.DeleteAsync(asset.AssetFileName);

            _assetRepository.Remove(asset);
            _assetRepository.Complete();

            return Ok("Asset deleted successfully.");

        }


        [HttpDelete]
        public async Task<ActionResult> DeleteAllAssets()
        {

            await _azureBlobService.DeleteAllAcync();
            _assetRepository.RemoveAll();
            _assetRepository.Complete();

            return Ok("Asset deleted successfully.");

        }


        [HttpGet]

        public List<FolderResponse> GetFolders(Guid? CurrentLevelKey)
        {
            var sss = _assetRepository.GetFolders(CurrentLevelKey);
            return sss;
        }

        [HttpGet]
        public List<FolderContentResponse> GetFolderContents(bool showDetail, Guid FolderId)
        {
            return _assetRepository.GetFolderContents(showDetail, FolderId);
        }

        [HttpGet]
        public List<FolderContentResponse> GetRelatedAssets(Guid AssetId)
        {
            return _assetRepository.GetRelatedAssets(AssetId);
        }


        [HttpGet]
        public MetadataResponse GetAssetMetadata(Guid AssetId)
        {

            return _assetRepository.GetAssetMetadata(AssetId);
        }


        [HttpPost]
        public async Task<IActionResult> UploadAsync([FromForm]  Int16 assettype)
        {
            AssetTypes assetType = (AssetTypes)assettype;

            if (assetType==AssetTypes.Folder)
                return StatusCode(500, "Could not uplaod file.");

            var request = await HttpContext.Request.ReadFormAsync();
            if (request.Files == null)
                return StatusCode(500, "Could not uplaod file.");


            var files = request.Files;
            if (files.Count != 1)
                return StatusCode(500, "Could uplaod just only one file.");



            // Analyze Image
            ImageAnalysis metadatalist = new ImageAnalysis();
            if (assetType == AssetTypes.Folder)            
            using (var streamimage = files[0].OpenReadStream())
            {
                metadatalist = await _azureImageAnalyzer.AnalyzImageAsync(streamimage);
            }


            // create folder and upload original file in storage
            creatFolders(new List<string> { "Original" });
            string filename = Helper.GetRandomBlobName(files[0].FileName, Path.GetExtension(files[0].FileName));
            string filepath = await _azureBlobService.UploadAcync(files, filename, "Original");


            // add Original asset in database
            var parent = _assetRepository.GetEntityByPath("/Original");
            Guid newAssetID = new Guid();
            _assetRepository.Add(new AssetModel
            {
                AssetId = newAssetID,
                AssetFileName = filename,
                AssetFilePath = filepath,
                Datetime = DateTime.Now,
                AssetType = assetType,
                MetaData = JsonConvert.SerializeObject(metadatalist).ToString(),
                Parent = parent != null ? parent.AssetId : Guid.Empty
            });
            _assetRepository.Complete();
            
            // Create and upload SocialRequirement
            await uploadSocialRequirementsAsync(newAssetID, assetType, files[0],filepath, metadatalist);

            return Ok("Asset uploded successfully.");


        }

        private async Task uploadSocialRequirementsAsync(Guid newAssetID, AssetTypes AssetType, IFormFile file,string url, ImageAnalysis metadatalist)
        {


            string CreateAssetMethodName = "";
            string GetBaseAssetFoldersMethodName = "";


            #region reflection call all social netwrok requiured
            switch (AssetType)
            {
                case AssetTypes.Folder:
                    CreateAssetMethodName = "CreateImagesAsync";
                    GetBaseAssetFoldersMethodName = "GetBaseImageFolders";
                    break;
                case AssetTypes.Image:
                    CreateAssetMethodName = "CreateImagesAsync";
                    GetBaseAssetFoldersMethodName = "GetBaseImageFolders";
                    break;
                case AssetTypes.Video:
                    CreateAssetMethodName = "CreateVideosAsync";
                    GetBaseAssetFoldersMethodName = "GetBaseVideoFolders";
                    break;
                default:
                    break;
            }
            var type = typeof(ISocialNetwork);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Select(x => new { x.FullName, x.Name }).Where(x => x.Name != "ISocialNetwork").ToList();

            if (AssetType != AssetTypes.Image) // only for image imepeleted
                return;

            foreach (var socialclass in types)
            {                
                List<ImageScaled> imageConverteds = new List<ImageScaled>();

                Type typeName = Type.GetType(socialclass.FullName);
                MethodInfo CreateImagesAsyncMethod = typeName.GetMethod(CreateAssetMethodName);
                object socialclassinstance = Activator.CreateInstance(typeName, null, _azureImageConverter);
                dynamic task = CreateImagesAsyncMethod.Invoke(socialclassinstance, new object[] { url });
                await task;
                imageConverteds = (List<ImageScaled>)task.GetAwaiter().GetResult();

                foreach (var item in imageConverteds)
                {
                    MethodInfo GetBaseImageFoldersMethos = typeName.GetMethod(GetBaseAssetFoldersMethodName);
                    string fullfplderpath = (string)GetBaseImageFoldersMethos.Invoke(socialclassinstance, null) + "/" + item.GetFolder;
                    creatFolders(fullfplderpath.Split('/').ToList());

                    var bytes = Convert.FromBase64String(item.ImageBase64);
                    var stream = new MemoryStream(bytes);

                    string socialfilename = Helper.GetRandomBlobName(file.FileName, item.Extention.ToString());
                    string socialfilepath = await _azureBlobService.UploadStreamAcync(stream, socialfilename, fullfplderpath);

                    var parentsocial = _assetRepository.GetEntityByPath("/" + fullfplderpath);
                    _assetRepository.Add(new AssetModel
                    {
                        AssetId = new Guid(),
                        AssetFileName = socialfilename,
                        AssetFilePath = socialfilepath,
                        Datetime = DateTime.Now,
                        AssetType = AssetTypes.Image,
                        MetaData = JsonConvert.SerializeObject(metadatalist).ToString(),
                        Parent = parentsocial != null ? parentsocial.AssetId : Guid.Empty,
                        OriginalAssetRef = newAssetID
                    });
                    _assetRepository.Complete();
                }



            }

            #endregion


        }


        private void creatFolders(List<string> folders)
        {

            string path = "";

            foreach (var cat in folders)
            {
                var parentrecord = _assetRepository.GetEntityByPath(path);

                path = path + "/" + cat;
                var existingFolder = _assetRepository.GetEntityByPath(path);

                if (existingFolder == null)
                {
                    _assetRepository.Add(new AssetModel
                    {
                        AssetId = new Guid(),
                        AssetFileName = cat,
                        AssetFilePath = path,
                        Datetime = DateTime.Now,
                        AssetType = AssetTypes.Folder,
                        MetaData = "",
                        Parent = parentrecord != null ? parentrecord.AssetId : Guid.Empty
                    });
                    _assetRepository.Complete();
                }
            }


        }


    }
}

