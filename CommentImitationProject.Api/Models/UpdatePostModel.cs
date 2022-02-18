using System;

namespace CommentImitationProject.Models
{
    public class UpdatePostModel
    {
        public Guid PostId { get; set; }

        public string Text { get; set; }
    }
}