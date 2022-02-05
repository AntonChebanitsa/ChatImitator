using System;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DTO
{
    public class CommentDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime LastEditDate { get; set; }

        public Post Post { get; set; }

        public User Author { get; set; }
    }
}