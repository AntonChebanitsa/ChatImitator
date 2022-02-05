using CommentImitationProject.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommentImitationProject.DAL
{
    public class ProjectContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<User> Users { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }
    }
}