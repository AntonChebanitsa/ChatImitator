using System.Threading.Tasks;
using CommentImitationProject.DAL.Repositories.Interfaces;

namespace CommentImitationProject.DAL
{
    public interface IUnitOfWork
    {
        Task CommitAsync();

        ICommentRepository Comments { get; }

        IUserRepository Users { get; }

        IPostRepository Posts { get; }
    }
}