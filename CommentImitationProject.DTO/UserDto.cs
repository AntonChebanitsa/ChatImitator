using System;
using System.Collections.Generic;
using CommentImitationProject.DAL.Entities;

namespace CommentImitationProject.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string NickName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}