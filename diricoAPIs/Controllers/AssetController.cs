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
using diricoAPIs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;


namespace diricoAPIs.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("0.01")]
    [ApiController]
    public class AssetController : Controller
    {
        private readonly IBlobService _azureBlobService;
        private readonly IImageAnalyzer _azureImageAnalyzer;
        private readonly IImageConverter _azureImageConverter;        
        private readonly IAssetRepository _assetRepository;

        public AssetController(
            IBlobService azureBlobService,
            IImageAnalyzer azureImageAnalyzer,
            IImageConverter azureImageConverter,
            IAssetRepository assetRepository
            )
        {

            _azureBlobService = azureBlobService;
            _azureImageAnalyzer = azureImageAnalyzer;
            _azureImageConverter = azureImageConverter;            
            _assetRepository = assetRepository;
        }

        [HttpPost]
        public async Task<ActionResult> UploadAsync()
        {
            try
            {

                var request = await HttpContext.Request.ReadFormAsync();
                if (request.Files == null)
                    return BadRequest("Could not uplaod files.");

                var files = request.Files;
                if (files.Count == 0)
                    return BadRequest("Could not uplaod empty file.");


                await _azureBlobService.UploadAcync(files, files[0].FileName, "");

                return Ok("Asset uploaded successfully.");
            }
            catch (Exception ex)
            {                
                return BadRequest("Unexpected Error ."+ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsset(Guid AssetID)
        {
            try
            {
                var asset=_assetRepository.Get(AssetID);
                await _azureBlobService.DeleteAsync(asset.AssetFileName);
                
                _assetRepository.Remove(asset);
                _assetRepository.Complete();

                return Ok("Asset deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("Unexpected Error ." + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAllAssets()
        {
            try
            {
                await _azureBlobService.DeleteAllAcync();
                _assetRepository.RemoveAll();
                _assetRepository.Complete();

                return Ok("Asset deleted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("Unexpected Error ." + ex.Message);
            }
        }
           

        [HttpGet]
        public List<FolderResponse> GetFolders(FolderRequest request)
        {
            return _assetRepository.GetFolders(request);
        }

        [HttpGet]
        public List<FolderContentResponse> GetFolderContents(FolderContentRequest request)
        {

            return _assetRepository.GetFolderContents(request);
        }


        [HttpGet]
        public MetadataResponse GetAssetMetadata(MetadataRequest request)
        {

            return _assetRepository.GetAssetMetadata(request);
        }


        [HttpPost]
        public async Task<string> TestAsync()
        {
            try
            {
                var req = await HttpContext.Request.ReadFormAsync();
                if (req.Files == null)
                    return null;

                // Analyze Image
                ImageAnalysis metadatalist = new ImageAnalysis();
                using (var streamimage = req.Files[0].OpenReadStream())
                {
                    metadatalist = await _azureImageAnalyzer.AnalyzImageAsync(streamimage);
                }

                // create folders base on data that Analyzed  
                //var categories = metadatalist.Categories.Select(x => x.Name).ToList();
                creatFolders(new List<string> { "Original" });

                // upload original file in storage
                string filename = Helper.GetRandomBlobName(req.Files[0].FileName , Path.GetExtension(req.Files[0].FileName));
                string filepath = await _azureBlobService.UploadAcync(req.Files, filename, "Original");


                // add Original asset in database
                var parent = _assetRepository.GetEntityByPath("/Original");
                _assetRepository.Add(new AssetModel
                {
                    AssetId = new Guid(),
                    AssetFileName = filename,
                    AssetFilePath = filepath,                    
                    Datetime = DateTime.Now,
                    AssetType = AssetTypes.Image,
                    MetaData = JsonConvert.SerializeObject(metadatalist).ToString(),
                    Parent = parent != null ? parent.AssetId : Guid.Empty
                });
                _assetRepository.Complete();


                // Social Network
                #region reflection call all social netwrok requiured
                //var type = typeof(ISocialNetwork);
                //var types = AppDomain.CurrentDomain.GetAssemblies()
                //    .SelectMany(s => s.GetTypes())
                //    .Where(p => type.IsAssignableFrom(p) )
                //    .Select(x =>new { x.FullName , x.Name}).Where(x=>x.Name !="ISocialNetwork").ToList();

                //foreach (var socialclass in types)
                //{
                //    Type typeName = Type.GetType(socialclass.FullName);
                //    MethodInfo myMethod = typeName.GetMethod("CreateImagesAsync");
                //    object socialclassinstance = Activator.CreateInstance(typeName,null, _azureImageConverter);


                //    List<ImageScaled> imageConverteds = new List<ImageScaled>();
                //    creatFolders(new List<string> { socialclass.Name });
                //    var task = (Task) myMethod.Invoke(socialclassinstance, new object[] { filepath });


                //    await task.ConfigureAwait(false);

                //    var resultProperty = task.GetType().GetProperty("Result");



                //    foreach (var item in imageConverteds)
                //    {
                //        byte[] byteArray = Encoding.UTF8.GetBytes(item.imageString);
                //        using (var streamimage = new MemoryStream(byteArray))
                //        {
                //            string socialfilename = Helper.GetRandomBlobName(req.Files[0].FileName);
                //            string socialfilepath = await _azureBlobService.UploadStreamAcync(streamimage, socialfilename, socialclass.Name);

                //            // add in database
                //            var parentsocial = _assetRepository.GetEntityByPath("/" + socialclass.Name);
                //            _assetRepository.Add(new AssetModel
                //            {
                //                AssetId = new Guid(),
                //                AssetFileName = socialfilename,
                //                AssetFilePath = socialfilepath,
                //                Datetime = DateTime.Now,
                //                AssetType = AssetTypes.Image,
                //                MetaData = JsonConvert.SerializeObject(metadatalist).ToString(),
                //                Parent = parentsocial != null ? parentsocial.AssetId : Guid.Empty
                //            });
                //            _assetRepository.Complete();
                //        }
                //    }
                //}

                #endregion


                List<ImageScaled> imageConverteds = new List<ImageScaled>();
                                
                FaceBook faceBook = new FaceBook(null, _azureImageConverter);
                imageConverteds = await faceBook.CreateImagesAsync(filepath);

                foreach (var item in imageConverteds)
                {
                    string fullfplderpath = (faceBook.GetBaseImageFolders() + "/" + item.GetFolder);
                    creatFolders(fullfplderpath.Split('/').ToList());

                    var bytes = Convert.FromBase64String(item.ImageBase64);
                    var stream = new MemoryStream(bytes);

                    string socialfilename = Helper.GetRandomBlobName(req.Files[0].FileName, item.Extention.ToString());
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
                        Parent = parentsocial != null ? parentsocial.AssetId : Guid.Empty
                    });
                    _assetRepository.Complete();
                }      
                return "Asset uploded successfully.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private  void creatFolders(List<string> folders)
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

