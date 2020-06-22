using diricoAPIs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Repositories
{
    public interface IAssetRepository : IRepository<AssetModel>
    {

        List<FolderResponse> GetFolders(FolderRequest request);

        List<FolderContentResponse> GetFolderContents(FolderContentRequest request);

        MetadataResponse GetAssetMetadata(MetadataRequest request);

        AssetModel GetEntityByPath(string path);
    }
}
