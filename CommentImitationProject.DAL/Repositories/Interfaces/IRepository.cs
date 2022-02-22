using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DAL.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, IBaseEntity
    {
        Task<List<T>> GetAll();

        Task<T> GetById(Guid id);

        Task CreateAsync(T entity);

        void Remove(T entity);

        void Update(T entity);
    }
}