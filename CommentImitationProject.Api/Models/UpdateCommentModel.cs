using System;

namespace CommentImitationProject.Models
{
    public class UpdateCommentModel
    {
        public Guid CommentId { get; set; }

        public string Text { get; set; }
    }
}