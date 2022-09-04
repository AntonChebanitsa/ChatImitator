using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using CommentImitationProject.DTO;
using CommentImitationProject.Models;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CommentImitationProject.IntegrationTests
{
    public class PostControllerIntegrationTests : IntegrationTestsFixture
    {
        [Test]
        public async Task GetAll_WithoutPosts_ShouldReturnEmptyCollectionAndOk()
        {
            // Arrange
            await CleanDatabase();

            // Act
            var response = await Client.GetAsync(PostUrl);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_WithUsers_ShouldReturnCollectionAndOk()
        {
            // Arrange
            const int count = 3;

            await CleanDatabase();
            await FillDatabaseWithPosts(count);

            // Act
            var response = await Client.GetAsync(PostUrl);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await response.Content.ReadAsStringAsync()).ToList();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Count.Should().Be(count);
        }

        [Test, AutoData]
        public async Task GetById_PostNotExists_ShouldReturnNotFound(Guid postId)
        {
            // Act
            var response = await Client.GetAsync(PostUrl + postId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetById_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(PostUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetById_PostExist_ShouldReturnOkAndModel()
        {
            // Arrange
            await FillDatabaseWithPosts(2);

            var getAllResponse = await Client.GetAsync(PostUrl);
            var getAll = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await getAllResponse.Content.ReadAsStringAsync()).ToList();
            var expectedPost = getAll.First();

            // Act
            var response = await Client.GetAsync(PostUrl + expectedPost.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<PostDto>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedPost);
        }

        [Test]
        public async Task GetPostsByUserId_PostNotExists_ShouldReturnOkAndEmptyCollection()
        {
            // Arrange
            await CleanDatabase();
            await FillDatabaseWithUsers();

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            // Act
            var response = await Client.GetAsync(PostUrl + GetPostsByUserIdUrl + user.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetPostsByUserId_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(PostUrl + GetPostsByUserIdUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetPostsByUserId_Exist_ShouldReturnOkAndCollection()
        {
            // Arrange
            const int count = 2;

            await CleanDatabase();
            await FillDatabaseWithPosts(count);

            var usersResponse = await Client.GetAsync(UserUrl);
            var user = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersResponse.Content.ReadAsStringAsync()).First();

            // Act
            var response = await Client.GetAsync(PostUrl + GetPostsByUserIdUrl + user.Id);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Count().Should().Be(count);
        }

        [Test]
        public async Task Create_TextIsEmpty_ShouldReturnBadRequest()
        {
            // Arrange
            await FillDatabaseWithUsers();

            var allUsersResponse = await Client.GetAsync(UserUrl);
            var allUsers = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await allUsersResponse.Content.ReadAsStringAsync());
            var author = allUsers.First();

            var model = new CreatePostModel
            {
                Text = string.Empty,
                PostId = author.Id
            };

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postsBeforeResponse = await Client.GetAsync(PostUrl);
            var postsBefore = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsBeforeResponse.Content.ReadAsStringAsync());

            // Act
            var response = await Client.PostAsync(PostUrl, content);

            // Assert
            var postsAfterResponse = await Client.GetAsync(PostUrl);
            var postsAfter = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            postsAfter.Count().Should().Be(postsBefore.Count());
        }

        [Test, AutoData]
        public async Task Create_IncorrectAuthorId_ShouldReturnBadRequest(string text)
        {
            // Arrange
            await FillDatabaseWithUsers();

            var model = new CreatePostModel
            {
                Text = text,
                PostId = Guid.Empty
            };

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postsBeforeResponse = await Client.GetAsync(PostUrl);
            var postsBefore = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsBeforeResponse.Content.ReadAsStringAsync());

            // Act
            var response = await Client.PostAsync(PostUrl, content);

            // Assert
            var postsAfterResponse = await Client.GetAsync(PostUrl);
            var postsAfter = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsAfterResponse.Content.ReadAsStringAsync());

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            postsAfter.Count().Should().Be(postsBefore.Count());
        }

        [Test, AutoData]
        public async Task Create_ValidRequest_ShouldReturnOK(string text)
        {
            // Arrange
            await FillDatabaseWithUsers();

            var allUsersResponse = await Client.GetAsync(UserUrl);
            var allUsers = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await allUsersResponse.Content.ReadAsStringAsync());
            var author = allUsers.First();

            var model = new CreatePostModel
            {
                Text = text,
                PostId = author.Id
            };

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var postsBeforeResponse = await Client.GetAsync(PostUrl);
            var postsBefore = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsBeforeResponse.Content.ReadAsStringAsync());

            // Act
            var response = await Client.PostAsync(PostUrl, content);

            // Assert
            var postsAfterResponse = await Client.GetAsync(PostUrl);
            var postsAfter = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsAfterResponse.Content.ReadAsStringAsync()).ToList();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            postsAfter.Should().Contain(x => x.Text.Equals(text));
            postsAfter.Count.Should().BeGreaterThan(postsBefore.Count());
        }

        [Test, AutoData]
        public async Task Update_EmptyId_ShouldReturnBadRequest(string text)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {Guid.Empty, text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(PostUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Update_EmptyText_ShouldReturnBadRequest(Guid postId)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {postId, text = string.Empty}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(PostUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Update_PostNotExist_ShouldReturnNotFound(Guid postId, string text)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {postId, text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(PostUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test, AutoData]
        public async Task Update_ValidData_ShouldReturnOkAndContainUpdatedModel(string text)
        {
            // Arrange
            await FillDatabaseWithPosts(5);

            var allPostsResponse = await Client.GetAsync(PostUrl);
            var postsBefore = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await allPostsResponse.Content.ReadAsStringAsync()).ToList();
            var expectedPost = postsBefore.First();
            expectedPost.Text = text;

            var content = new StringContent(JsonConvert.SerializeObject(new UpdatePostModel {PostId = expectedPost.Id, Text = text}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(PostUrl, content);

            // Assert
            var getPostById = await Client.GetAsync(PostUrl + expectedPost.Id);
            var resultPost = JsonConvert.DeserializeObject<PostDto>(await getPostById.Content.ReadAsStringAsync());

            resultPost.Text.Should().Be(text);
            resultPost.Id.Should().Be(expectedPost.Id);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task Delete_PostNotExist_ShouldReturnNotFound(Guid userId)
        {
            // Act
            var response = await Client.DeleteAsync(PostUrl + userId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_SendEmptyGuid_ShouldReturnNotFound()
        {
            // Act
            var response = await Client.DeleteAsync(PostUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Delete_ValidData_ShouldReturnOk()
        {
            // Arrange
            await FillDatabaseWithPosts(2);

            var postsBeforeResponse = await Client.GetAsync(PostUrl);
            var postsBefore = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postsBeforeResponse.Content.ReadAsStringAsync()).ToList();
            var existPostId = postsBefore.First().Id;

            // Act
            var response = await Client.DeleteAsync(PostUrl + existPostId);

            // Assert
            var postAfterResponse = await Client.GetAsync(PostUrl);
            var postAfter = JsonConvert.DeserializeObject<IEnumerable<PostDto>>(await postAfterResponse.Content.ReadAsStringAsync()).ToList();

            postAfter.Should().NotContain(x => x.Id.Equals(existPostId));
            postAfter.Count.Should().BeLessThan(postsBefore.Count);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}