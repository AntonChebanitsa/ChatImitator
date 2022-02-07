using System;
using System.Collections.Generic;

namespace CommentImitationProject.DAL.Entities
{
    public class User : IBaseEntity
    {
        public Guid Id { get; set; }

        public string NickName { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}