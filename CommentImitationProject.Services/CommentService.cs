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

        public async Task<CommentDto> GetConcreteComment(Guid id)
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

        public IEnumerable<CommentDto> GetAllComments()
        {
            var entities = _unitOfWork.Comments.GetAll();

            return entities.Select(x => _mapper.Map<CommentDto>(x));
        }

        public IEnumerable<CommentDto> GetUserComments(Guid userId)
        {
            var comments = _unitOfWork.Comments.GetUserComments(userId);

            return comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public IEnumerable<CommentDto> GetPostComments(Guid postId)
        {
            var comments = _unitOfWork.Comments.GetPostComments(postId);

            return comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public IEnumerable<CommentDto> GetByUserAndPost(Guid postId, Guid userId)
        {
            var comments = _unitOfWork.Comments.GetByPostAndUser(postId, userId);

            return comments.Select(x => _mapper.Map<CommentDto>(x));
        }

        public async Task CreateComment(string text, Guid userId, Guid postId)
        {
            var newComment = new Comment {AuthorId = userId, PostId = postId, Text = text, LastEditDate = DateTime.UtcNow};

            await _unitOfWork.Comments.CreateAsync(newComment);
            await _unitOfWork.CommitAsync();
        }

        public async Task EditComment(Guid id, string text)
        {
            try
            {
                var commentToEdit = await _unitOfWork.Comments.GetById(id);
                commentToEdit.Text = text;
                commentToEdit.LastEditDate = DateTime.UtcNow;

                _unitOfWork.Comments.Update(commentToEdit);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task DeleteComment(Guid id)
        {
            try
            {
                var commentToDelete = await _unitOfWork.Comments.GetById(id);

                _unitOfWork.Comments.Remove(commentToDelete);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task DeleteUserComments(Guid userId)
        {
            var userComments = _unitOfWork.Comments.GetUserComments(userId);

            _unitOfWork.Comments.RemoveRange(userComments);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeletePostComments(Guid postId)
        {
            var userComments = _unitOfWork.Comments.GetPostComments(postId);

            _unitOfWork.Comments.RemoveRange(userComments);
            await _unitOfWork.CommitAsync();
        }
    }
}