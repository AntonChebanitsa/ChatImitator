using System;
using System.Linq;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly DbSet<Comment> _table;

        protected CommentRepository(ProjectContext projectContext) : base(projectContext)
        {
            _table = projectContext.Comments;
        }

        public IQueryable<Comment> GetPostComments(Guid postId)
        {
            return _table.Where(x => x.PostId.Equals(postId));
        }

        public IQueryable<Comment> GetUserComments(Guid userId)
        {
            return _table.Where(x => x.AuthorId.Equals(userId));
        }

        public IQueryable<Comment> GetByPostAndUser(Guid postId, Guid userId)
        {
            return _table.Where(x => x.PostId.Equals(postId) && x.AuthorId.Equals(userId));
        }
    }
}