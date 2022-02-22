using System;

namespace CommentImitationProject.Models
{
    public class CreateCommentModel
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public string Text { get; set; }
    }
}