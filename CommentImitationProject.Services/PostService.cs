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
        private const string IncorrectParameterMessage = "Incorrect parameter";
        private const string PostNotExistMessage = "Post with this id doesn't exist";
        private const string NicknameShouldBeNotEmptyMessage = "Post text should be not empty";

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
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(postId)}: {postId}");

            var post = await _unitOfWork.Posts.GetById(postId);

            if (post == null)
                throw new NullReferenceException(PostNotExistMessage);

            return _mapper.Map<PostDto>(post);
        }

        public async Task<IEnumerable<PostDto>> GetUserPostsById(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(userId)}: {userId}");

            var posts = (await _unitOfWork.Posts.GetPostsByUserId(userId)).ToList();

            return !posts.Any()
                ? new List<PostDto>()
                : posts.Select(post => _mapper.Map<PostDto>(post));
        }

        public async Task Create(Guid userId, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(NicknameShouldBeNotEmptyMessage);

            if (userId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(userId)}: {userId}");

            var post = new Post {Text = text, PublicationDate = DateTime.UtcNow, AuthorId = userId};

            await _unitOfWork.Posts.CreateAsync(post);
            await _unitOfWork.CommitAsync();
        }

        public async Task Update(Guid postId, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(NicknameShouldBeNotEmptyMessage);

            if (postId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(postId)}: {postId}");

            var post = await _unitOfWork.Posts.GetById(postId);

            if (post == null)
                throw new NullReferenceException(PostNotExistMessage);

            post.Text = text;

            _unitOfWork.Posts.Update(post);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException($"{IncorrectParameterMessage} {nameof(postId)}: {postId}");

            var post = await _unitOfWork.Posts.GetById(postId);

            if (post == null)
                throw new NullReferenceException(PostNotExistMessage);

            _unitOfWork.Posts.Remove(post);
            await _unitOfWork.CommitAsync();
        }
    }
}