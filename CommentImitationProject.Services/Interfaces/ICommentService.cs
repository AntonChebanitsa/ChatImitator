using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> GetCommentById(Guid id);

        Task<IEnumerable<CommentDto>> GetAll();

        Task<IEnumerable<CommentDto>> GetUserCommentsByUserId(Guid userId);

        Task<IEnumerable<CommentDto>> GetPostCommentsByPostId(Guid postId);

        Task Create(string text, Guid userId, Guid postId);

        Task Edit(Guid id, string text);

        Task Delete(Guid id);
    }
}