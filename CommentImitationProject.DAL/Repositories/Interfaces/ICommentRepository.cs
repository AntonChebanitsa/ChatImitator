using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DAL.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetPostComments(Guid postId);

        Task<List<Comment>> GetUserComments(Guid userId);
    }
}