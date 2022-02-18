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
            var posts = await _unitOfWork.Posts.GetAll();

            return posts.Count == 0
                ? new List<PostDto>()
                : posts.Select(x => _mapper.Map<PostDto>(x));
        }

        public async Task<PostDto> GetById(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException(postId.ToString());

            var post = await _unitOfWork.Posts.GetById(postId);

            return post == null
                ? throw new NullReferenceException()
                : _mapper.Map<PostDto>(post);
        }

        public async Task<IEnumerable<PostDto>> GetUserPostsById(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(userId.ToString());

            var posts = (await _unitOfWork.Posts.GetPostsByUserId(userId)).ToList();

            return !posts.Any()
                ? new List<PostDto>()
                : posts.Select(x => _mapper.Map<PostDto>(x));
        }

        public async Task Create(Guid userId, string text)
        {
            if (string.IsNullOrEmpty(text) || userId == Guid.Empty)
            {
                throw new ArgumentException();
            }

            var post = new Post {Text = text, PublicationDate = DateTime.UtcNow, AuthorId = userId};

            await _unitOfWork.Posts.CreateAsync(post);
            await _unitOfWork.CommitAsync();
        }

        public async Task Update(Guid postId, string text)
        {
            if (postId == Guid.Empty || string.IsNullOrEmpty(text))
                throw new ArgumentException();

            var post = await _unitOfWork.Posts.GetById(postId);

            if (post == null)
                throw new NullReferenceException();

            post.Text = text;

            _unitOfWork.Posts.Update(post);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new AggregateException(postId.ToString());

            var post = await _unitOfWork.Posts.GetById(postId);

            if (post == null)
                throw new NullReferenceException();

            _unitOfWork.Posts.Remove(post);
            await _unitOfWork.CommitAsync();
        }
    }
}