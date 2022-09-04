using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        private readonly DbSet<Post> _table;

        public PostRepository(ProjectContext projectContext) : base(projectContext)
        {
            _table = projectContext.Posts;
        }

        public async Task<IEnumerable<Post>> GetPostsByUserId(Guid userId)
        {
            return await _table.Where(x => x.AuthorId.Equals(userId)).ToListAsync();
        }
    }
}