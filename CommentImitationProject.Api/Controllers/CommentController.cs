using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommentImitationProject.DTO;
using CommentImitationProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<CommentDto> comments;

            try
            {
                comments = _commentService.GetAllComments();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(comments);
        }

        [HttpGet("postid={postId:guid}")]
        public IActionResult GetByPostId(Guid postId)
        {
            IEnumerable<CommentDto> comments;

            try
            {
                comments = _commentService.GetPostComments(postId);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(comments);
        }

        [HttpGet("userid={userId:guid}")]
        public IActionResult GetByUserId(Guid userId)
        {
            IEnumerable<CommentDto> comments;

            try
            {
                comments = _commentService.GetUserComments(userId);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string text, Guid userId, Guid postId)
        {
            try
            {
                await _commentService.CreateComment(text, userId, postId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Accepted();
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] Guid commentId, string text)
        {
            try
            {
                await _commentService.EditComment(commentId, text);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete(Guid commentId)
        {
            try
            {
                await _commentService.DeleteComment(commentId);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}