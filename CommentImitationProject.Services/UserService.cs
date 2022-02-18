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
        private const string IncorrectParameterMessage = "Incorrect parameter";
        private const string UserNotExistMessage = "User with this id doesn't exist";
        private const string NicknameShouldBeNotEmptyMessage = "Nickname should be not empty";

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
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(userId)}: {userId}");

            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new NullReferenceException(UserNotExistMessage);

            return _mapper.Map<UserDto>(user);
        }

        public async Task Create(string nickname)
        {
            if (string.IsNullOrEmpty(nickname))
                throw new ArgumentException(NicknameShouldBeNotEmptyMessage);

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
            if (userId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(userId)}: {userId}");

            if (string.IsNullOrEmpty(nickname))
                throw new ArgumentException(NicknameShouldBeNotEmptyMessage);

            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new NullReferenceException(UserNotExistMessage);

            user.NickName = nickname;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(userId)}: {userId}");

            var user = await _unitOfWork.Users.GetById(userId);

            if (user == null)
                throw new NullReferenceException(UserNotExistMessage);

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CommitAsync();
        }
    }
}