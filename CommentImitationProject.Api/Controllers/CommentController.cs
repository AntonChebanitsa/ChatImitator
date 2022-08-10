using System;
using System.Threading.Tasks;
using CommentImitationProject.Models;
using CommentImitationProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _commentService.GetAll());
        }

        [HttpGet("{commentId:guid}")]
        public async Task<IActionResult> GetById(Guid commentId)
        {
            try
            {
                return Ok(await _commentService.GetById(commentId));
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCommentsByPostId/{postId:guid}")]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId)
        {
            try
            {
                return Ok(await _commentService.GetCommentsByPostId(postId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCommentsByUserId/{userId:guid}")]
        public async Task<IActionResult> GetCommentsByUserId(Guid userId)
        {
            try
            {
                return Ok(await _commentService.GetCommentsByUserId(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentModel model)
        {
            try
            {
                await _commentService.Create(model.Text, model.UserId, model.PostId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateCommentModel model)
        {
            try
            {
                await _commentService.Update(model.CommentId, model.Text);

                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{commentId:guid}")]
        public async Task<IActionResult> Delete(Guid commentId)
        {
            try
            {
                await _commentService.Delete(commentId);

                return Ok();
            }
            catch (NullReferenceException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}