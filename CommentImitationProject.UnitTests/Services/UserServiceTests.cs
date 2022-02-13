using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using AutoMapper;
using CommentImitationProject.DAL;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Services;
using CommentImitationProject.Services.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CommentImitationProject.UnitTests.Services
{
    public class UserServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private IUserService _userService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetAll_NoUsers_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Users.GetAll())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAll();

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.GetAll());
            result.Should().BeEquivalentTo(new List<UserDto>());
        }

        [Test]
        public async Task GetAll_WithSomeUsers_ShouldReturnCollection()
        {
            // Arrange
            var users = _fixture.Build<User>()
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .CreateMany().ToList();

            _mockUnitOfWork.Setup(x => x.Users.GetAll())
                .ReturnsAsync(users);

            var expectedUsers = users.Select(user => new UserDto
            {
                Id = user.Id,
                NickName = user.NickName,
                RegistrationDate = user.RegistrationDate
            }).ToList();

            var calls = 0;
            _mockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
                .Returns(() => expectedUsers[calls])
                .Callback(() => calls++);

            // Act
            var result = await _userService.GetAll();

            // Assert
            result.Should().BeEquivalentTo(expectedUsers);
        }

        [Test, AutoData]
        public async Task GetById_UserExists_ShouldReturnUser(Guid userId)
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            _mockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            var expectedUser = new UserDto
            {
                Id = user.Id,
                RegistrationDate = user.RegistrationDate,
                NickName = user.NickName
            };

            _mockMapper.Setup(x => x.Map<UserDto>(user))
                .Returns(expectedUser);

            // Act
            var result = await _userService.GetById(userId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.GetById(userId));
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public void GetById_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userService.GetById(new Guid()));
        }

        [Test, AutoData]
        public void Create_UserCreated_VerifyUoW(string nickName)
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Users.CreateAsync(It.IsAny<User>()));

            // Act 
            _userService.Create(nickName);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.CreateAsync(It.IsAny<User>()));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Edit_UserEdited_VerifyUoW()
        {
            // Arrange
            var user = _fixture.Build<User>()
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            _mockUnitOfWork.Setup(x => x.Users.Update(It.IsAny<User>()));

            // Act 
            _userService.Edit(user);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.Update(user));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public async Task Update_UserUpdated_VerifyUoW(Guid userId, string nickName)
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            _mockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.Update(userId, nickName);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.Update(user));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }
        
        [Test]
        public void Update_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userService.Update(new Guid(), string.Empty));
        }
        
        [Test, AutoData]
        public async Task Delete_UserDeleted_VerifyUoW(Guid userId)
        {
            // Arrange
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            _mockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.Delete(userId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Users.Remove(user));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }
        
        [Test]
        public void Delete_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userService.Delete(new Guid()));
        }
    }
}