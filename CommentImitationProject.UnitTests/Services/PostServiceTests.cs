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
    [TestFixture]
    public class PostServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IMapper> _mockMapper;
        private IPostService _postService;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(_mockUnitOfWork.Object, _mockMapper.Object);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetAll_NoPosts_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Posts.GetAll())
                .ReturnsAsync(new List<Post>());

            // Act
            var result = await _postService.GetAll();

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.GetAll());
            result.Should().BeEquivalentTo(new List<Post>());
        }

        [Test]
        public async Task GetAll_WithSomePosts_ShouldReturnCollection()
        {
            // Arrange
            var posts = _fixture.Build<Post>()
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .CreateMany().ToList();

            _mockUnitOfWork.Setup(x => x.Posts.GetAll())
                .ReturnsAsync(posts);

            var expectedPosts = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            }).ToList();

            var calls = 0;
            _mockMapper.Setup(x => x.Map<PostDto>(It.IsAny<Post>()))
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
            var post = _fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            _mockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            var expectedPost = new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            };

            _mockMapper.Setup(x => x.Map<PostDto>(post))
                .Returns(expectedPost);

            // Act
            var result = await _postService.GetById(postId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.GetById(postId));
            result.Should().BeEquivalentTo(expectedPost);
        }

        [Test]
        public void GetById_NoPost_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Posts.GetById(new Guid()))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.GetById(new Guid()));
        }

        [Test, AutoData]
        public async Task GetUserPostsById_NoPosts_ShouldReturnEmptyCollection(Guid userId)
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Posts.GetPostsByUserId(userId))
                .ReturnsAsync(new List<Post>());

            // Act
            var result = await _postService.GetUserPostsById(userId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.GetPostsByUserId(userId));
            result.Should().BeEquivalentTo(new List<Post>());
        }

        [Test, AutoData]
        public async Task GetUserPostsById_WithSomePosts_ShouldReturnCollection(Guid userId)
        {
            // Arrange
            var posts = _fixture.Build<Post>()
                .With(x => x.AuthorId, userId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .CreateMany().ToList();

            _mockUnitOfWork.Setup(x => x.Posts.GetPostsByUserId(userId))
                .ReturnsAsync(posts);

            var expectedPosts = posts.Select(post => new PostDto
            {
                Id = post.Id,
                Text = post.Text,
                PublicationDate = post.PublicationDate
            }).ToList();

            var calls = 0;
            _mockMapper.Setup(x => x.Map<PostDto>(It.IsAny<Post>()))
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
            _mockUnitOfWork.Setup(x => x.Posts.CreateAsync(It.IsAny<Post>()));

            // Act 
            _postService.Create(userId, text);

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.CreateAsync(It.IsAny<Post>()));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test, AutoData]
        public async Task Update_PostUpdated_VerifyUoW(Guid postId, string text)
        {
            // Arrange
            var post = _fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            _mockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            // Act
            await _postService.Update(postId, text);

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.Update(post));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Update_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Posts.GetById(new Guid()))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.Update(new Guid(), string.Empty));
        }

        [Test, AutoData]
        public async Task Delete_PostUpdated_VerifyUoW(Guid postId)
        {
            // Arrange
            var post = _fixture.Build<Post>()
                .With(x => x.Id, postId)
                .Without(x => x.Author)
                .Without(x => x.Comments)
                .Create();

            _mockUnitOfWork.Setup(x => x.Posts.GetById(postId))
                .ReturnsAsync(post);

            // Act
            await _postService.Delete(postId);

            // Assert
            _mockUnitOfWork.Verify(x => x.Posts.Remove(post));
            _mockUnitOfWork.Verify(x => x.CommitAsync());
        }

        [Test]
        public void Delete_NoUser_ShouldThrowNullReferenceException()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Posts.GetById(new Guid()))
                .ReturnsAsync((Post) null);

            // Assert
            Assert.ThrowsAsync<NullReferenceException>(async () => await _postService.Delete(new Guid()));
        }
    }
}