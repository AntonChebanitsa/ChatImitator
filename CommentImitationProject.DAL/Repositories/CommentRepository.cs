using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL.Repositories
{
    public class CommentRepository : BaseRepository<Comment>, ICommentRepository
    {
        private readonly DbSet<Comment> _table;

        public CommentRepository(ProjectContext projectContext) : base(projectContext)
        {
            _table = projectContext.Comments;
        }

        public async Task<List<Comment>> GetPostComments(Guid postId)
        {
            return await _table.Where(x => x.PostId.Equals(postId)).ToListAsync();
        }

        public async Task<List<Comment>> GetUserComments(Guid userId)
        {
            return await _table.Where(x => x.AuthorId.Equals(userId)).ToListAsync();
        }
    }
}