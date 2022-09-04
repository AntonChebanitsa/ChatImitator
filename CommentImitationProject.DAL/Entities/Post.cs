using System;
using System.Collections.Generic;

namespace CommentImitationProject.DAL.Entities
{
    public class Post : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime PublicationDate { get; set; }

        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}