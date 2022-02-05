using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> GetConcreteComment(Guid id);

        IEnumerable<CommentDto> GetAllComments();

        IEnumerable<CommentDto> GetUserComments(Guid userId);

        IEnumerable<CommentDto> GetPostComments(Guid postId);

        IEnumerable<CommentDto> GetByUserAndPost(Guid postId, Guid userId);

        Task CreateComment(string text, Guid userId, Guid postId);

        Task EditComment(Guid id, string text);

        Task DeleteComment(Guid id);

        Task DeleteUserComments(Guid userId);

        Task DeletePostComments(Guid postId);
    }
}