using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Services;
using CommentImitationProject.Services.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CommentImitationProject.UnitTests.Services
{
    public class UserServiceTests : ServicesTestFixture
    {
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            _userService = new UserService(MockUnitOfWork.Object, MockMapper.Object);
        }

        [Test]
        public async Task GetAll_NoUsers_ShouldReturnEmptyCollection()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetAll())
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _userService.GetAll();

            // Assert
            MockUnitOfWork.Verify(x => x.Users.GetAll());
            result.Should().BeEquivalentTo(new List<UserDto>());
        }

        [Test]
        public async Task GetAll_WithSomeUsers_ShouldReturnCollection()
        {
            // Arrange
            var users = Fixture.Build<User>()
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Users.GetAll())
                .ReturnsAsync(users);

            var expectedUsers = users.Select(user => new UserDto
            {
                Id = user.Id,
                NickName = user.NickName,
                RegistrationDate = user.RegistrationDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
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
            var user = Fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            var expectedUser = new UserDto
            {
                Id = user.Id,
                RegistrationDate = user.RegistrationDate,
                NickName = user.NickName
            };

            MockMapper.Setup(x => x.Map<UserDto>(user))
                .Returns(expectedUser);

            // Act
            var result = await _userService.GetById(userId);

            // Assert
            MockUnitOfWork.Verify(x => x.Users.GetById(userId));
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public void GetById_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.GetById(new Guid()));
        }

        [Test, AutoData]
        public void GetById_NoUser_ShouldThrowNullReferenceException(Guid userId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userService.GetById(userId));
        }

        [Test, AutoData]
        public void Create_UserCreated_VerifyUoW(string nickName)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.CreateAsync(It.IsAny<User>()));

            // Act 
            _userService.Create(nickName);

            // Assert
            MockUnitOfWork.Verify(x => x.Users.CreateAsync(It.IsAny<User>()));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Edit_UserEdited_VerifyUoW()
        {
            // Arrange
            var user = Fixture.Build<UserDto>()
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            MockUnitOfWork.Setup(x => x.Users.Update(It.IsAny<User>()));

            // Act 
            _userService.Edit(user);

            // Assert
            MockUnitOfWork.Verify(x => x.Users.Update(It.IsAny<User>()));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public async Task Update_UserUpdated_VerifyUoW(Guid userId, string nickName)
        {
            // Arrange
            var user = Fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.Update(userId, nickName);

            // Assert
            MockUnitOfWork.Verify(x => x.Users.Update(user));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Update_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.Update(new Guid(), string.Empty));
        }

        [Test, AutoData]
        public void Update_NoUser_ShouldThrowNullReferenceException(Guid userId, string nickname)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _userService.Update(userId, nickname));
        }

        [Test, AutoData]
        public void Update_InvalidNickname_ShouldThrowArgumentException(Guid userId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.Update(userId, string.Empty));
        }

        [Test, AutoData]
        public async Task Delete_UserDeleted_VerifyUoW(Guid userId)
        {
            // Arrange
            var user = Fixture.Build<User>()
                .With(x => x.Id, userId)
                .Without(x => x.Comments)
                .Without(x => x.Posts)
                .Create();

            MockUnitOfWork.Setup(x => x.Users.GetById(userId))
                .ReturnsAsync(user);

            // Act
            await _userService.Delete(userId);

            // Assert
            MockUnitOfWork.Verify(x => x.Users.Remove(user));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Delete_InvalidId_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(new Guid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.Delete(new Guid()));
        }

        [Test]
        public void Delete_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Users.GetById(Guid.NewGuid()))
                .ReturnsAsync((User) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userService.Delete(new Guid()));
        }
    }
}