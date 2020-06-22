using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Models
{
    public class diricoDBContext : DbContext
    {
        public diricoDBContext(DbContextOptions<diricoDBContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // beasouse of Run  OnModelCreating of IdentityDbContext that inheritance from it.
            //base.OnModelCreating(modelBuilder);

            // seed method
            modelBuilder.Entity<UserModel>().HasData(new UserModel { UserId = Guid.NewGuid(), FirstName = "FirstName", LastName = "LastName", UserName = "Admin" });
        }

        #region define Datasets

        public DbSet<UserModel> Users { get; set; }
        public DbSet<AssetModel> Assets { get; set; }

        #endregion define Datasets


    }
}
