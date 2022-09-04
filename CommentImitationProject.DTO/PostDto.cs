using System;
using System.Collections.Generic;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DTO
{
    public class PostDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime PublicationDate { get; set; }

        public User Author { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}