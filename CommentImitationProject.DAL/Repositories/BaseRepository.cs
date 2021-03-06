using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        public readonly DbSet<T> _table;

        public BaseRepository(DbContext context)
        {
            _table = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _table;
        }

        public async Task<T> GetById(Guid id)
        {
            var entity = await _table.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (entity == null)
            {
                throw new ArgumentException("Entity with this id doesn't exist");
            }

            return entity;
        }

        public async Task CreateAsync(T entity)
        {
            await _table.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            if (entity != null)
            {
                _table.Remove(entity);
            }
            else
            {
                throw new NullReferenceException("This entity does not exist");
            }
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _table.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            if (entity != null)
            {
                _table.Update(entity);
            }
            else
            {
                throw new NullReferenceException("This entity does not exist");
            }
        }
    }
}