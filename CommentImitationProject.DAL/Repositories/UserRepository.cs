using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        protected UserRepository(ProjectContext dbProjectContext) : base(dbProjectContext)
        {
        }
    }
}