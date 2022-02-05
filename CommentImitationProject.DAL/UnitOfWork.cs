using System.Threading.Tasks;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectContext _projectContext;

        public UnitOfWork(IUserRepository users, IPostRepository posts, ICommentRepository comments, ProjectContext projectContext)
        {
            Users = users;
            Posts = posts;
            Comments = comments;
            _projectContext = projectContext;
        }

        public async Task CommitAsync()
        {
            await _projectContext.SaveChangesAsync();
        }

        public IPostRepository Posts { get; }
        public IUserRepository Users { get; }
        public ICommentRepository Comments { get; }
    }
}