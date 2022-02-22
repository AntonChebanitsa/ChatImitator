using System;

namespace CommentImitationProject.Models
{
    public class UpdateUserModel
    {
        public Guid UserId { get; set; }

        public string Nickname { get; set; }
    }
}