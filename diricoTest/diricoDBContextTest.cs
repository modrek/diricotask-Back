using diricoAPIs.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Text;

namespace diricoTest
{
    public  class DiricoDBContextTest : IDisposable
    {
        public diricoDBContext _context;
        public DiricoDBContextTest()
        {
            var options = new DbContextOptionsBuilder<diricoDBContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
            _context = new diricoDBContext(options);

            _context.Database.EnsureCreated();

            DiricoDBContextInitializer.Initialize(_context);
        }

        
        public void Dispose()
        {
            _context.Database.EnsureDeleted();

            _context.Dispose();
        }
    }
}
