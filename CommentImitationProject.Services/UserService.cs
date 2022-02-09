using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommentImitationProject.DAL;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Services.Interfaces;

namespace CommentImitationProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _unitOfWork.Users.GetAll();

            return users.Select(x => _mapper.Map<UserDto>(x));
        }

        public async Task<UserDto> GetById(Guid userId)
        {
            var user = await _unitOfWork.Users.GetById(userId);

            return _mapper.Map<UserDto>(user);
        }

        public async Task Create(string nickName)
        {
            var user = new User {NickName = nickName, RegistrationDate = DateTime.UtcNow.Date};

            await _unitOfWork.Users.CreateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public void Edit(User user)
        {
            _unitOfWork.Users.Update(user);
        }

        public async Task Update(Guid userId, string nickName)
        {
            try
            {
                var user = await _unitOfWork.Users.GetById(userId);

                user.NickName = nickName;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CommitAsync();
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Entity with this id doesn't exists");
            }
        }

        public async Task Delete(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetById(userId);

                _unitOfWork.Users.Remove(user);
                await _unitOfWork.CommitAsync();
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException("Entity with this id doesn't exists");
            }
        }
    }
}