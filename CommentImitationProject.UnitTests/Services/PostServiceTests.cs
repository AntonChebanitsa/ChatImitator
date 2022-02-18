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
    [TestFixture]
    public class PostServiceTests : ServicesTestFixture
    {
        private IPostService _postService;

        [SetUp]
        public void Setup()
        {
            _postService = new PostService(MockUnitOfWork.Object, MockMapper.Object);
        }

        [Test]
        public async Task GetAll_NoPosts_ShouldReturnEmptyCollection()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetAll())
                .ReturnsAsync(new List<Post>());

            // Act
            var result = await _postService.GetAll();

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.GetAll());
            result.Should().BeEquivalentTo(new List<Post>());
        }

        [Test]
        public async Task GetAll_WithSomePosts_ShouldReturnCollection()
        {
            // Arrange
            var posts = Fixture.Build<Post>()
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Posts.GetAll())
                .ReturnsAsync(posts);

            var expectedPosts = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<PostDto>(It.IsAny<Post>()))
                .Returns(() => expectedPosts[calls])
                .Callback(() => calls++);

            // Act
            var result = await _postService.GetAll();

            // Assert
            result.Should().BeEquivalentTo(expectedPosts);
        }

        [Test, AutoData]
        public async Task GetById_PostExists_ShouldReturnUser(Guid postId)
        {
            // Arrange
            var post = Fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            var expectedPost = new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            };

            MockMapper.Setup(x => x.Map<PostDto>(post))
                .Returns(expectedPost);

            // Act
            var result = await _postService.GetById(postId);

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.GetById(postId));
            result.Should().BeEquivalentTo(expectedPost);
        }

        [Test, AutoData]
        public void GetById_NoPost_ShouldThrowNullReferenceException(Guid postId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.GetById(postId));
        }

        [Test]
        public void GetById_EmptyGuid_ShouldThrowArgumentException()
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetById(Guid.Empty))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _postService.GetById(Guid.Empty));
        }

        [Test, AutoData]
        public async Task GetUserPostsById_NoPosts_ShouldReturnEmptyCollection(Guid userId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetPostsByUserId(userId))
                .ReturnsAsync(new List<Post>());

            // Act
            var result = await _postService.GetUserPostsById(userId);

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.GetPostsByUserId(userId));
            result.Should().BeEquivalentTo(new List<Post>());
        }

        [Test, AutoData]
        public async Task GetUserPostsById_WithSomePosts_ShouldReturnCollection(Guid userId)
        {
            // Arrange
            var posts = Fixture.Build<Post>()
                .With(x => x.AuthorId, userId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .CreateMany().ToList();

            MockUnitOfWork.Setup(x => x.Posts.GetPostsByUserId(userId))
                .ReturnsAsync(posts);

            var expectedPosts = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            }).ToList();

            var calls = 0;
            MockMapper.Setup(x => x.Map<PostDto>(It.IsAny<Post>()))
                .Returns(() => expectedPosts[calls])
                .Callback(() => calls++);

            // Act
            var result = await _postService.GetUserPostsById(userId);

            // Assert
            result.Should().BeEquivalentTo(expectedPosts);
        }

        [Test, AutoData]
        public void Create_PostCreated_VerifyUoW(Guid userId, string text)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.CreateAsync(It.IsAny<Post>()));

            // Act 
            _postService.Create(userId, text);

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.CreateAsync(It.IsAny<Post>()));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public async Task Update_PostUpdated_VerifyUoW(Guid postId, string text)
        {
            // Arrange
            var post = Fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            // Act
            await _postService.Update(postId, text);

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.Update(post));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public void Update_NoUser_ShouldThrowArgumentException(Guid postId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _postService.Update(postId, string.Empty));
        }

        [Test, AutoData]
        public void Update_NoUser_ShouldThrowNullReferenceException(Guid postId, string text)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.Update(postId, text));
        }

        [Test, AutoData]
        public async Task Delete_PostUpdated_VerifyUoW(Guid postId)
        {
            // Arrange
            var post = Fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            // Act
            await _postService.Delete(postId);

            // Assert
            MockUnitOfWork.Verify(x => x.Posts.Remove(post));
            MockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public void Delete_NoUser_ShouldThrowNullReferenceException(Guid postId)
        {
            // Arrange
            MockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.Delete(postId));
        }
    }
}