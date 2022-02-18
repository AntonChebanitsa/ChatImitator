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
    [TestFixture]
    public class UserControllerIntegrationTests : IntegrationTestsFixture
    {
        [Test]
        public async Task GetAll_WithoutUsers_ShouldReturnEmptyCollectionAndOk()
        {
            // Arrange
            await CleanDatabase();

            // Act
            var response = await Client.GetAsync(UserUrl);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAll_WithUsers_ShouldReturnCollectionAndOk()
        {
            // Arrange
            const int count = 2;

            await CleanDatabase();
            await FillDatabaseWithUsers(count);

            // Act
            var response = await Client.GetAsync(UserUrl);

            // Assert
            var result = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await response.Content.ReadAsStringAsync());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Count().Should().Be(count);
        }

        [Test, AutoData]
        public async Task GetById_UserNotExists_ShouldReturnNotFound(Guid userId)
        {
            // Act
            var response = await Client.GetAsync(UserUrl + userId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetById_GetExistingUser_ReturnUserAndOk()
        {
            // Arrange
            const int count = 2;

            await FillDatabaseWithUsers(count);

            var getAllResponse = await Client.GetAsync(UserUrl);
            var expectedUser = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await getAllResponse.Content.ReadAsStringAsync()).First();

            // Act
            var getByIdResponse = await Client.GetAsync(UserUrl + expectedUser.Id);

            // Assert
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultUser = JsonConvert.DeserializeObject<UserDto>(await getByIdResponse.Content.ReadAsStringAsync());
            resultUser.Should().BeEquivalentTo(expectedUser);
        }

        [Test]
        public async Task GetById_SendEmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await Client.GetAsync(UserUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Create_WithName_ShouldReturnOkAndContainEntityWithNickname(string nickname)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(nickname), Encoding.UTF8, "application/json");
            var responseBeforeCreate = await Client.GetAsync(UserUrl);
            var modelsBefore = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseBeforeCreate.Content.ReadAsStringAsync()).ToList();

            // Act
            var response = await Client.PostAsync(UserUrl, content);

            // Assert
            var responseAfterCreate = await Client.GetAsync(UserUrl);
            var modelsAfter = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseAfterCreate.Content.ReadAsStringAsync()).ToList();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            modelsAfter.Count.Should().NotBe(modelsBefore.Count);
            modelsAfter.Should().Contain(x => x.NickName.Equals(nickname));
        }

        [Test]
        public async Task Create_WithoutName_ShouldReturnBadRequest()
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(string.Empty), Encoding.UTF8, "application/json");
            var responseBeforeCreate = await Client.GetAsync(UserUrl);
            var modelsBefore = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseBeforeCreate.Content.ReadAsStringAsync()).ToList();

            // Act
            var response = await Client.PostAsync(UserUrl, content);

            // Assert
            var responseAfterCreate = await Client.GetAsync(UserUrl);
            var modelsAfter = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseAfterCreate.Content.ReadAsStringAsync()).ToList();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            modelsAfter.Count.Should().Be(modelsBefore.Count);
        }

        [Test, AutoData]
        public async Task Edit_WhenUserNotExist_ShouldReturnOk(string newNickname)
        {
            // Arrange
            await FillDatabaseWithUsers(2);

            var responseBeforeCreate = await Client.GetAsync(UserUrl);
            var modelsBefore = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseBeforeCreate.Content.ReadAsStringAsync()).ToList();
            var modelToEdit = modelsBefore.First();
            modelToEdit.NickName = newNickname;

            var content = new StringContent(JsonConvert.SerializeObject(modelToEdit), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync(UserUrl, content);

            // Assert
            var responseAfterCreate = await Client.GetAsync(UserUrl);
            var modelsAfter = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await responseAfterCreate.Content.ReadAsStringAsync()).ToList();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            modelsAfter.Count.Should().Be(modelsBefore.Count);
            modelsAfter.Should().Contain(x => x.NickName.Equals(newNickname));
        }

        [Test, AutoData]
        public async Task Update_EmptyId_ShouldReturnBadRequest(string nickname)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {Guid.Empty, nickname}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(UserUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Update_EmptyNickname_ShouldReturnBadRequest(Guid userId)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {userId, string.Empty}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(UserUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test, AutoData]
        public async Task Update_InvalidId_ShouldReturnNotFound(Guid userId, string nickname)
        {
            // Arrange
            var content = new StringContent(JsonConvert.SerializeObject(new {userId, nickname}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(UserUrl, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test, AutoData]
        public async Task Update_ValidData_ShouldReturnOkAndContainUpdatedModel(string nickname)
        {
            // Arrange
            await FillDatabaseWithUsers(5);

            var allUsersResponse = await Client.GetAsync(UserUrl);
            var usersBefore = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await allUsersResponse.Content.ReadAsStringAsync()).ToList();
            var expectedUser = usersBefore.First();
            expectedUser.NickName = nickname;

            var content = new StringContent(JsonConvert.SerializeObject(new UpdateUserModel {UserId = expectedUser.Id, Nickname = nickname}), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(UserUrl, content);

            // Assert
            var userById = await Client.GetAsync(UserUrl + expectedUser.Id);
            var resultUser = JsonConvert.DeserializeObject<UserDto>(await userById.Content.ReadAsStringAsync());

            resultUser.NickName.Should().Be(nickname);
            resultUser.Id.Should().Be(expectedUser.Id);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task Delete_UserNotExist_ShouldReturnNotFound(Guid userId)
        {
            // Act
            var response = await Client.DeleteAsync(UserUrl + userId);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_SendEmptyGuid_ShouldReturnNotFound()
        {
            // Act
            var response = await Client.DeleteAsync(UserUrl + Guid.Empty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Delete_ValidData_ShouldReturnOk()
        {
            // Arrange
            await FillDatabaseWithUsers(2);

            var usersBeforeResponse = await Client.GetAsync(UserUrl);
            var usersBefore = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersBeforeResponse.Content.ReadAsStringAsync()).ToList();
            var existUserId = usersBefore.First().Id;

            // Act
            var response = await Client.DeleteAsync(UserUrl + existUserId);

            // Assert
            var usersAfterResponse = await Client.GetAsync(UserUrl);
            var usersAfter = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await usersAfterResponse.Content.ReadAsStringAsync()).ToList();

            usersAfter.Should().NotContain(x => x.Id.Equals(existUserId));
            usersAfter.Count.Should().BeLessThan(usersBefore.Count);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}