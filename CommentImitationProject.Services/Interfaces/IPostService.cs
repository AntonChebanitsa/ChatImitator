using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAll();

        Task<IEnumerable<PostDto>> GetUserPostsById(Guid userId);

        Task<PostDto> GetById(Guid postId);

        Task Create(Guid userId, string text);

        Task Update(Guid postId, string text);

        Task Delete(Guid postId);
    }
}