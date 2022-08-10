using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DbSet<T> _table;

        protected BaseRepository(DbContext context)
        {
            _table = context.Set<T>();
        }

        public async Task<List<T>> GetAll()
        {
            return await _table.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            var entity = await _table.FirstOrDefaultAsync(x => x.Id.Equals(id));

            return entity;
        }

        public async Task CreateAsync(T entity)
        {
            await _table.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _table.Update(entity);
        }

        public void Remove(T entity)
        {
            _table.Remove(entity);
        }
    }
}