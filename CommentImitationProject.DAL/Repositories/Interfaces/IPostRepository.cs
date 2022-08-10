using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DAL.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        Task<IEnumerable<Post>> GetPostsByUserId(Guid userId);
    }
}