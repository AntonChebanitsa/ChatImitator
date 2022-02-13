using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> GetById(Guid id);

        Task<IEnumerable<CommentDto>> GetAll();

        Task<IEnumerable<CommentDto>> GetCommentsByUserId(Guid userId);

        Task<IEnumerable<CommentDto>> GetCommentsByPostId(Guid postId);

        Task Create(string text, Guid userId, Guid postId);

        Task Update(Guid id, string text);

        Task Delete(Guid id);
    }
}