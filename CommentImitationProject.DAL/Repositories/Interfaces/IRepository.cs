using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DAL.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, IBaseEntity
    {
        IQueryable<T> GetAll();

        Task<T> GetById(Guid id);

        Task CreateAsync(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}