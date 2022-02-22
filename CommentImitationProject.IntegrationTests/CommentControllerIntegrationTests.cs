using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommentImitationProject.IntegrationTests
{
    [TestFixture]
    public class CommentControllerIntegrationTests : IntegrationTestsFixture
    {
        [Test]
        public async Task GetAll_WithoutComments_ShouldReturnEmptyCollectionAndOk()
        {
            // Act
            await CleanDatabase();

            var response = await Client.GetAsync(CommentUrl);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await response.Content.ReadAsStringAsync());
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_WithComments_ShouldReturnEmptyCollectionAndOk()
        {
            // Arrange
            await FillDatabaseWithComments(2);

            // Act
            var response = await Client.GetAsync(CommentUrl);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await response.Content.ReadAsStringAsync());

            result.Count().Should().BeGreaterThan(0);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task GetById_CommentNotExists_ShouldReturnNotFound(Guid commentId)
        {
            // Act
            var response = await Client.GetAsync(CommentUrl + commentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetById_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(CommentUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetById_CommentExist_ShouldReturnOkAndModel()
        {
            // Arrange
            await FillDatabaseWithComments();

            var getAllResponse = await Client.GetAsync(CommentUrl);
            var getAll = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await getAllResponse.Content.ReadAsStringAsync()).ToList();
            var expectedComment = getAll.First();

            // Act
            var response = await Client.GetAsync(CommentUrl + expectedComment.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<CommentDto>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedComment);
        }

        [Test]
        public async Task GetCommentsByPostId_CommentsNotExists_ShouldReturnOkAndEmptyCollection()
        {
            // Arrange
            await CleanDatabase();
            await FillDatabaseWithPosts();

            var postsResponse = await Client.GetAsync(PostUrl);
            var post = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsResponse.Content.ReadAsStringAsync()).First();

            // Act
            var response = await Client.GetAsync(CommentUrl + GetCommentsByPostIdUrl + post.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCommentsByUserId_CommentsNotExists_ShouldReturnOkAndEmptyCollection()
        {
            // Arrange
            await CleanDatabase();
            await FillDatabaseWithUsers();

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            // Act
            var response = await Client.GetAsync(CommentUrl + GetCommentsByUserIdUrl + user.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCommentsByPostId_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(CommentUrl + GetCommentsByPostIdUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetCommentsByUserId_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(CommentUrl + GetCommentsByUserIdUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Create_TextIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            await CleanDatabase();
            await FillDatabaseWithPosts();

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            var postsResponse = await Client.GetAsync(PostUrl);
            var post = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsResponse.Content.ReadAsStringAsync()).First();

            var model = new CreateCommentModel
            {
                Text = string.Empty,
                PostId = post.Id,
                UserId = user.Id
            };

            var commentsBeforeResponse = await Client.GetAsync(PostUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsBeforeResponse.Content.ReadAsStringAsync());

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync(CommentUrl, content);

            // Assert
            var commentsAfterResponse = await Client.GetAsync(PostUrl);
            var commentsAfter = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            commentsAfter.Count().Should().Be(commentsBefore.Count());
        }

        [Test, AutoData]
        public async Task Create_IncorrectAuthorId_ShouldReturnBadRequest(string text)
        {
            // Arrange
            await FillDatabaseWithPosts();

            var postsResponse = await Client.GetAsync(PostUrl);
            var post = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsResponse.Content.ReadAsStringAsync()).First();

            var model = new CreateCommentModel
            {
                Text = text,
                PostId = post.Id,
                UserId = Guid.Empty
            };

            var commentsBeforeResponse = await Client.GetAsync(PostUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsBeforeResponse.Content.ReadAsStringAsync());

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync(CommentUrl, content);

            // Assert
            var commentsAfterResponse = await Client.GetAsync(PostUrl);
            var commentsAfter = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            commentsAfter.Count().Should().Be(commentsBefore.Count());
        }

        [Test, AutoData]
        public async Task Create_IncorrectPostId_ShouldReturnBadRequest(string text)
        {
            // Arrange
            await FillDatabaseWithPosts();

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            var model = new CreateCommentModel
            {
                Text = text,
                PostId = Guid.Empty,
                UserId = user.Id
            };

            var commentsBeforeResponse = await Client.GetAsync(PostUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsBeforeResponse.Content.ReadAsStringAsync());

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync(CommentUrl, content);

            // Assert
            var commentsAfterResponse = await Client.GetAsync(PostUrl);
            var commentsAfter = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            commentsAfter.Count().Should().Be(commentsBefore.Count());
        }
        
        [Test, AutoData]
        public async Task Create_ValidRequest_ShouldReturnBadRequest(string text)
        {
            // Arrange
            await FillDatabaseWithPosts();

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            var postsResponse = await Client.GetAsync(PostUrl);
            var post = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsResponse.Content.ReadAsStringAsync()).First();

            var model = new CreateCommentModel
            {
                Text = text,
                PostId = post.Id,
                UserId = user.Id
            };

            var commentsBeforeResponse = await Client.GetAsync(CommentUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsBeforeResponse.Content.ReadAsStringAsync());

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync(CommentUrl, content);

            // Assert
            var commentsAfterResponse = await Client.GetAsync(CommentUrl);
            var commentsAfter = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            commentsAfter.Count().Should().BeGreaterThan(commentsBefore.Count());
        }
        
        [Test, AutoData]
        public async Task Update_EmptyId_ShouldReturnBadRequest(string text)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {Guid.Empty, text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(CommentUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Update_EmptyText_ShouldReturnBadRequest(Guid commentId)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {postId = commentId, text = string.Empty}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(CommentUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Test, AutoData]
        public async Task Update_CommentNotExist_ShouldReturnNotFound(Guid commentId, string text)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {commentId, text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(CommentUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test, AutoData]
        public async Task Update_ValidData_ShouldReturnOkAndContainUpdatedModel(string text)
        {
            // Arrange
            await FillDatabaseWithComments();

            var allCommentsResponse = await Client.GetAsync(CommentUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await allCommentsResponse.Content.ReadAsStringAsync()).ToList();
            var expectedComment = commentsBefore.First();
            expectedComment.Text = text;

            var content = new StringContent(JsonConvert.SerializeObject(new UpdateCommentModel {CommentId = expectedComment.Id, Text = text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(CommentUrl, content);

            // Assert
            var getCommentByIdResponse = await Client.GetAsync(CommentUrl + expectedComment.Id);
            var resultComment = JsonConvert.DeserializeObject<CommentDto>(await getCommentByIdResponse.Content.ReadAsStringAsync());

            resultComment.Text.Should().Be(text);
            resultComment.Id.Should().Be(expectedComment.Id);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Test, AutoData]
        public async Task Delete_CommentNotExist_ShouldReturnNotFound(Guid commentId)
        {
            // Act
            var response = await Client.DeleteAsync(CommentUrl + commentId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_SendEmptyGuid_ShouldReturnNotFound()
        {
            // Act
            var response = await Client.DeleteAsync(CommentUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Delete_ValidData_ShouldReturnOk()
        {
            // Arrange
            await FillDatabaseWithComments(2);

            var commentsBeforeResponse = await Client.GetAsync(CommentUrl);
            var commentsBefore = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsBeforeResponse.Content.ReadAsStringAsync()).ToList();
            var existCommentId = commentsBefore.First().Id;

            // Act
            var response = await Client.DeleteAsync(CommentUrl + existCommentId);

            // Assert
            var commentsAfterResponse = await Client.GetAsync(CommentUrl);
            var commentsAfter = JsonConvert.DeserializeObject<IEnumerable<CommentDto>>(await commentsAfterResponse.Content.ReadAsStringAsync()).ToList();

            commentsAfter.Should().NotContain(x => x.Id.Equals(existCommentId));
            commentsAfter.Count.Should().BeLessThan(commentsBefore.Count);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}