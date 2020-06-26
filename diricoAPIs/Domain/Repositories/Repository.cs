using diricoAPIs.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace diricoAPIs.Domain.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private diricoDBContext _context;

        public Repository(diricoDBContext context)
        {
            _context = context;
        }

        public void Add(TEntity entity)
        {
           _context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public int Complete()
        {
            
                return _context.SaveChanges();
            
        }

        public TEntity Get(Guid Id)
        {
            return _context.Set<TEntity>().Find(Id);
        }

        public IEnumerable<TEntity> GetData(GetListRequest request)
        {           
            return _context.Set<TEntity>()//.Where(predicate)
                .Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void RemoveAll()
        {
            _context.Set<TEntity>().RemoveRange(_context.Set<TEntity>());
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
        }
    }
}
