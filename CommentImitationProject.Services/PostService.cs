using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommentImitationProject.DAL;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.DTO;
using CommentImitationProject.Services.Interfaces;

namespace CommentImitationProject.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostDto>> GetAll()
        {
            var entities = await _unitOfWork.Posts.GetAll();

            return entities.Select(x => _mapper.Map<PostDto>(x));
        }

        public async Task<IEnumerable<PostDto>> GetUserPostsById(Guid userId)
        {
            var entities = await _unitOfWork.Posts.GetPostsByUserId(userId);

            return entities.Select(x => _mapper.Map<PostDto>(x));
        }

        public async Task<PostDto> GetById(Guid postId)
        {
            var entity = await _unitOfWork.Posts.GetById(postId);

            return _mapper.Map<PostDto>(entity);
        }

        public async Task Create(Guid userId, string text)
        {
            var entity = new Post
            {
                Text = text,
                PublicationDate = DateTime.UtcNow,
                AuthorId = userId
            };

            await _unitOfWork.Posts.CreateAsync(entity);
        }

        public async Task Update(Guid postId, string text)
        {
            var entity = await _unitOfWork.Posts.GetById(postId);

            entity.Text = text;

            _unitOfWork.Posts.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid postId)
        {
            var entity = await _unitOfWork.Posts.GetById(postId);

            _unitOfWork.Posts.Remove(entity);
        }
    }
}