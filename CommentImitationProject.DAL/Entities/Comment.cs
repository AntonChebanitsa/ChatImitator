using System;

namespace CommentImitationProject.DAL.Entities
{
    public class Comment : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime LastEditDate { get; set; }

        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; }
    }
}