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

            return users.Count == 0
                ? new List<UserDto>()
                : users.Select(x => _mapper.Map<UserDto>(x));
        }

        public async Task<UserDto> GetById(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(userId.ToString());

            var user = await _unitOfWork.Users.GetById(userId);

            return user == null
                ? throw new NullReferenceException()
                : _mapper.Map<UserDto>(user);
        }

        public async Task Create(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                throw new ArgumentException(nickname);

            var user = new User {NickName = nickname, RegistrationDate = DateTime.UtcNow.Date};

            await _unitOfWork.Users.CreateAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task Edit(UserDto userDto)
        {
            _unitOfWork.Users.Update(_mapper.Map<User>(userDto));
            await _unitOfWork.CommitAsync();
        }

        public async Task Update(Guid userId, string nickname)
        {
            if (userId == Guid.Empty || string.IsNullOrEmpty(nickname))
                throw new ArgumentException(userId == Guid.Empty
                    ? userId.ToString()
                    : nickname);

            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new NullReferenceException();

            user.NickName = nickname;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException(userId.ToString());
            }

            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new NullReferenceException();

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CommitAsync();
        }
    }
}