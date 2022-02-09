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

        public async Task<CommentDto> GetCommentById(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Comments.GetById(id);

                return _mapper.Map<CommentDto>(entity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CommentDto>> GetAllComments()
        {
            var entities = await _unitOfWork.Comments.GetAll();

            return entities.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task<IEnumerable<CommentDto>> GetUserCommentsByUserId(Guid userId)
        {
            var comments = await _unitOfWork.Comments.GetUserComments(userId);

            return comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task<IEnumerable<CommentDto>> GetPostCommentsByPostId(Guid postId)
        {
            var comments = await _unitOfWork.Comments.GetPostComments(postId);

            return comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task Create(string text, Guid userId, Guid postId)
        {
            var newComment = new Comment {AuthorId = userId, PostId = postId, Text = text, LastEditDate = DateTime.UtcNow};

            await _unitOfWork.Comments.CreateAsync(newComment);
            await _unitOfWork.CommitAsync();
        }

        public async Task Edit(Guid id, string text)
        {
            var commentToEdit = await _unitOfWork.Comments.GetById(id);
            commentToEdit.Text = text;
            commentToEdit.LastEditDate = DateTime.UtcNow;

            _unitOfWork.Comments.Update(commentToEdit);
            await _unitOfWork.CommitAsync();
        }

        public async Task Delete(Guid id)
        {
            var commentToDelete = await _unitOfWork.Comments.GetById(id);

            _unitOfWork.Comments.Remove(commentToDelete);
            await _unitOfWork.CommitAsync();
        }
    }
}