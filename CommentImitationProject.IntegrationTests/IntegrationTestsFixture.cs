using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using CommentImitationProject.DAL;
using CommentImitationProject.DAL.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommentImitationProject.IntegrationTests
{
    public class IntegrationTestsFixture
    {
        protected readonly Fixture Fixture;
        protected readonly HttpClient Client;
        private readonly WebApplicationFactory<Startup> _appFactory;
        private readonly ProjectContext _context;

        protected const string CommentUrl = "api/Comment/";
        protected const string UserUrl = "api/User/";
        protected const string PostUrl = "api/Post/";
        protected const string GetPostsByUserIdUrl = "GetPostsByUserId/";
        protected const string GetCommentsByPostIdUrl = "GetCommentsByPostId/";  
        protected const string GetCommentsByUserIdUrl = "GetCommentsByUserId/";

        protected IntegrationTestsFixture()
        {
            Fixture = new Fixture();
            _appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DbContextOptions<ProjectContext>));
                        services.AddDbContext<ProjectContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                    });
                });

            _context = _appFactory.Services.CreateScope().ServiceProvider.GetService<ProjectContext>();
            Client = _appFactory.CreateClient();
        }

        protected async Task FillDatabaseWithComments(int count = 1)
        {
            var user = GenerateUser();
            var post = GeneratePost();
            var comments = new List<Comment>();

            for (var i = 0; i < count; i++)
            {
                comments.Add(GenerateComment());
            }

            foreach (var comment in comments)
            {
                comment.Author = user;
                comment.AuthorId = user.Id;

                comment.Post = post;
                comment.PostId = post.Id;
            }

            await _context.Users.AddAsync(user);
            await _context.Posts.AddAsync(post);
            await _context.Comments.AddRangeAsync(comments);
            await _context.SaveChangesAsync();
        }

        protected async Task FillDatabaseWithUsers(int count = 1)
        {
            var users = new List<User>();

            for (var i = 0; i < count; i++)
            {
                users.Add(GenerateUser());
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }

        protected async Task FillDatabaseWithPosts(int count = 1)
        {
            var user = GenerateUser();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var posts = new List<Post>();

            for (var i = 0; i < count; i++)
            {
                posts.Add(GeneratePost());
            }

            foreach (var post in posts)
            {
                post.Author = user;
                post.AuthorId = user.Id;
            }

            await _context.Posts.AddRangeAsync(posts);
            await _context.SaveChangesAsync();
        }

        private User GenerateUser()
        {
            return new()
            {
                NickName = Fixture.Create<string>(),
                RegistrationDate = Fixture.Create<DateTime>()
            };
        }

        private Post GeneratePost()
        {
            return new()
            {
                Text = Fixture.Create<string>(),
                PublicationDate = Fixture.Create<DateTime>()
            };
        }

        protected async Task CleanDatabase()
        {
            await _context.Database.EnsureDeletedAsync();
        }

        private Comment GenerateComment()
        {
            return new()
            {
                Text = Fixture.Create<string>(),
                LastEditDate = Fixture.Create<DateTime>()
            };
        }
    }
}