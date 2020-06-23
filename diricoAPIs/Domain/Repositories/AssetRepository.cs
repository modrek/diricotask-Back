using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using diricoAPIs.Domain.Models;

namespace diricoAPIs.Domain.Repositories
{
    public class AssetRepository : Repository<AssetModel> ,IAssetRepository
    {
        private diricoDBContext _context;

        public AssetRepository(diricoDBContext context) : base(context)
        {
            _context = context;
        }

        public MetadataResponse GetAssetMetadata(Guid AssetId)
        {
            try
            {
                var result = _context.Assets.Where(x => x.AssetId == AssetId)
                      .Select(x => new MetadataResponse
                      {
                          Metadata = x.MetaData,
                      })
                      .FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {

                return null;// JsonObjectAttribute("Unexpected Error ." + ex.Message);
            }
        }

        public List<FolderContentResponse> GetFolderContents(Guid FolderId)
        {
            try
            {
                var result = _context.Assets.Where(x => (x.AssetType == AssetTypes.Image || x.AssetType == AssetTypes.Video)
                       && x.Parent == FolderId)
                       .Select(x => new FolderContentResponse
                       {
                           AssetID = x.AssetId,
                           AssetName = x.AssetFileName,
                           AssetPath = x.AssetFilePath,
                           AssetType = x.AssetType
                       })
                       .ToList();


                return result;
            }
            catch (Exception ex)
            {

                return null;// JsonObjectAttribute("Unexpected Error ." + ex.Message);
            }
        }

        public List<FolderResponse> GetFolders(Guid? CurrentLevelKey)
        {
            try
            {
                if (CurrentLevelKey == null)
                    CurrentLevelKey = Guid.Empty;

                var result = _context.Assets.Where(x => x.AssetType == AssetTypes.Folder
                    && x.Parent == CurrentLevelKey )
                    .Select(x => new FolderResponse { FolderId = x.AssetId, FolderName = x.AssetFileName })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                return null;// JsonObjectAttribute("Unexpected Error ." + ex.Message);
            }
        }

        public AssetModel GetEntityByPath(string path)
        {
           return _context.Assets.Where(x => x.AssetFilePath == path).FirstOrDefault();
        }
    }
}
