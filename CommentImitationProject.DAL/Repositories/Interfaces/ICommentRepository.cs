using System;
using System.Linq;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DAL.Repositories.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        IQueryable<Comment> GetPostComments(Guid postId);

        IQueryable<Comment> GetUserComments(Guid userId);

        IQueryable<Comment> GetByPostAndUser(Guid postId, Guid userId);
    }
}