using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ProjectContext dbProjectContext) : base(dbProjectContext)
        {
        }
    }
}