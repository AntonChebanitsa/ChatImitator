using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(ProjectContext dbProjectContext) : base(dbProjectContext)
        {
        }
    }
}