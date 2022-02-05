using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        protected PostRepository(ProjectContext dbProjectContext) : base(dbProjectContext)
        {
        }
    }
}