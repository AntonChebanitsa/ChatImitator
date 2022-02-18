using System;

namespace CommentImitationProject.Models
{
    public class CreatePostModel
    {
        public Guid PostId { get; set; }

        public string Text { get; set; }
    }
}