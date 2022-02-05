using AutoMapper;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;

namespace CommentImitationProject.Services
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Comment, CommentDto>().ReverseMap();
            // CreateMap<User, UserDto>().ReverseMap();
            // CreateMap<Post, PostDto>().ReverseMap();
        }
    }
}