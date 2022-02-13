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
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetAll()
        {
            var comments = await _unitOfWork.Comments.GetAll();

            return comments.Count == 0
                ? new List<CommentDto>()
                : comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task<CommentDto> GetById(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetById(id);

            return comment == null
                ? throw new NullReferenceException()
                : _mapper.Map<CommentDto>(comment);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByUserId(Guid userId)
        {
            var comments = await _unitOfWork.Comments.GetUserComments(userId);

            return !comments.Any()
                ? new List<CommentDto>()
                : comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByPostId(Guid postId)
        {
            var comments = await _unitOfWork.Comments.GetPostComments(postId);

            return !comments.Any()
                ? new List<CommentDto>()
                : comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task Create(string text, Guid userId, Guid postId)
        {
            var newComment = new Comment {AuthorId = userId, PostId = postId, Text = text, LastEditDate = DateTime.UtcNow};

            await _unitOfWork.Comments.CreateAsync(newComment);
            await _unitOfWork.CommitAsync();
        }

        public async Task Update(Guid id, string text)
        {
            var comment = await _unitOfWork.Comments.GetById(id);

            if (comment == null)
                throw new NullReferenceException();

            comment.Text = text;
            comment.LastEditDate = DateTime.UtcNow;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid id)
        {
            var comment = await _unitOfWork.Comments.GetById(id);

            if (comment == null)
                throw new NullReferenceException();

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.CommitAsync();
        }
    }
}