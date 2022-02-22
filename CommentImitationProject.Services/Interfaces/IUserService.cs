using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAll();

        Task<UserDto> GetById(Guid userId);

        Task Create(string nickname);

        Task Edit(UserDto user);

        Task Update(Guid userid, string nickname);

        Task Delete(Guid userId);
    }
}