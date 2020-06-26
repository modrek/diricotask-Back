using diricoAPIs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diricoTest
{
    class DiricoDBContextInitializer
    {
        public static void Initialize(diricoDBContext context)
        {
            if (context.Assets.Any())
            {
                return;
            }

            Seed(context);
        }

        private static void Seed(diricoDBContext context)
        {
            Guid a1 = Guid.NewGuid();
            context.Assets.Add(new AssetModel
            {
                AssetId = a1,
                AssetFileName = "Folder1",
                AssetFilePath = "http://pic1.com/Folder1",
                AssetType = AssetTypes.Folder,
                Datetime = DateTime.Now,
            });

            Guid a2 = Guid.NewGuid();
            context.Assets.Add(new AssetModel
            {
                AssetId = a2,
                AssetFileName = "Folder2",
                AssetFilePath = "http://pic1.com/Folder1/Folder2",
                AssetType = AssetTypes.Folder,
                Datetime = DateTime.Now,
                Parent=a1
            });

            context.Assets.Add(new AssetModel
            {
                AssetId = Guid.NewGuid(),
                AssetFileName = "pic1",
                AssetFilePath = "http://pic1.com/Folder1/pic1.png",
                AssetType = AssetTypes.Image,
                Datetime = DateTime.Now,
                Parent = a2
            });

            
            context.SaveChanges();
        }
    }
}
