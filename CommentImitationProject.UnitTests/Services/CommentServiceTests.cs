using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    [TestFixture]
    public class CommentServiceTests : ServicesTestFixture
    {
        private ICommentService _commentService;

        [SetUp]
        public void Setup()
        {
            _commentService = new CommentService(MockUnitOfWork.Object, MockMapper.Object);
        }

        [Test]
        public async Task GetAll_NoComments_ShouldReturnEmptyCollection()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetAll())
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetAll();

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.GetAll());
            result.Should().BeEquivalentTo(new List<Comment>());
        }

        [Test]
        public async Task GetAll_WithSomeComments_ShouldReturnCollection()
        {
            // Arrange
            var comments = Fixture.Build<Comment>()
                .Without(x => x.Author)
                .Without(x => x.Post)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Comments.GetAll())
                .ReturnsAsync(comments);

            var expectedPosts = comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                LastEditDate = comment.LastEditDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(() => expectedPosts[calls])
                .Callback(() => calls++);

            // Act
            var result = await _commentService.GetAll();

            // Assert
            result.Should().BeEquivalentTo(expectedPosts);
        }

        [Test, AutoData]
        public async Task GetById_CommentExists_ShouldReturnUser(Guid commentId)
        {
            // Arrange
            var comment = Fixture.Build<Comment>()
                .With(x => x.Id, commentId)
                .Without(x => x.Author)
                .Without(x => x.Post)
                .Create();

            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync(comment);

            var expectedComment = new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                LastEditDate = comment.LastEditDate
            };

            MockMapper.Setup(x => x.Map<CommentDto>(comment))
                .Returns(expectedComment);

            // Act
            var result = await _commentService.GetById(commentId);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.GetById(commentId));
            result.Should().BeEquivalentTo(expectedComment);
        }

        [Test, AutoData]
        public void GetById_NoComments_ShouldThrowNullReferenceException(Guid commentId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _commentService.GetById(commentId));
        }

        [Test]
        public void GetById_InvalidData_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(new Guid()))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _commentService.GetById(new Guid()));
        }

        [Test, AutoData]
        public async Task GetCommentsByUserId_NoPosts_ShouldReturnEmptyCollection(Guid userId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetUserComments(userId))
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetCommentsByUserId(userId);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.GetUserComments(userId));
            result.Should().BeEquivalentTo(new List<Post>());
        }

        [Test, AutoData]
        public async Task GetCommentsByUserId_WithSomePosts_ShouldReturnCollection(Guid userId)
        {
            // Arrange
            var comments = Fixture.Build<Comment>()
                .With(x => x.AuthorId, userId)
                .Without(x => x.Author)
                .Without(x => x.Post)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Comments.GetUserComments(userId))
                .ReturnsAsync(comments);

            var expectedComments = comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                LastEditDate = comment.LastEditDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(() => expectedComments[calls])
                .Callback(() => calls++);

            // Act
            var result = await _commentService.GetCommentsByUserId(userId);

            // Assert
            result.Should().BeEquivalentTo(expectedComments);
        }

        [Test, AutoData]
        public async Task GetCommentsByPostId_NoPosts_ShouldReturnEmptyCollection(Guid postId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetPostComments(postId))
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetCommentsByPostId(postId);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.GetPostComments(postId));
            result.Should().BeEquivalentTo(new List<Comment>());
        }

        [Test, AutoData]
        public async Task GetCommentsByPostId_WithSomePosts_ShouldReturnCollection(Guid postId)
        {
            // Arrange
            var comments = Fixture.Build<Comment>()
                .With(x => x.PostId, postId)
                .Without(x => x.Author)
                .Without(x => x.Post)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Comments.GetPostComments(postId))
                .ReturnsAsync(comments);

            var expectedComments = comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                LastEditDate = comment.LastEditDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(() => expectedComments[calls])
                .Callback(() => calls++);

            // Act
            var result = await _commentService.GetCommentsByPostId(postId);

            // Assert
            result.Should().BeEquivalentTo(expectedComments);
        }

        [Test, AutoData]
        public void Create_CommentCreated_VerifyUoW(string text, Guid userId, Guid postId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.CreateAsync(It.IsAny<Comment>()));

            // Act 
            _commentService.Create(text, userId, postId);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.CreateAsync(It.IsAny<Comment>()));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public async Task Update_CommentUpdated_VerifyUoW(Guid commentId, string text)
        {
            // Arrange
            var comment = Fixture.Build<Comment>()
                .Without(x => x.Author)
                .Without(x => x.Post)
                .Create();

            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync(comment);

            // Act
            await _commentService.Update(commentId, text);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.Update(comment));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public void Update_NoComment_ShouldThrowNullReferenceException(Guid commentId, string text)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _commentService.Update(commentId, text));
        }

        [Test]
        public void Update_InvalidData_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(new Guid()))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _commentService.Update(new Guid(), string.Empty));
        }

        [Test, AutoData]
        public async Task Delete_CommentUpdated_VerifyUoW(Guid commentId)
        {
            // Arrange
            var comment = Fixture.Build<Comment>()
                .Without(x => x.Author)
                .Without(x => x.Post)
                .Create();

            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync(comment);

            // Act
            await _commentService.Delete(commentId);

            // Assert
            MockUnitOfWork.Verify(x => x.Comments.Remove(comment));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public void Delete_NoComment_ShouldThrowNullReferenceException(Guid commentId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(commentId))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _commentService.Delete(commentId));
        }

        [Test]
        public void Delete_InvalidData_ShouldThrowNullReferenceException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Comments.GetById(new Guid()))
                .ReturnsAsync((Comment) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _commentService.Delete(new Guid()));
        }
    }
}