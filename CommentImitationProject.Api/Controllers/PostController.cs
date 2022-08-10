using System;
using System.Threading.Tasks;
using CommentImitationProject.Models;
using CommentImitationProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAll());
        }

        [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetById(Guid postId)
        {
            try
            {
                return Ok(await _postService.GetById(postId));
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

        [HttpGet("GetPostsByUserId/{userId:guid}")]
        public async Task<IActionResult> GetPostsByUserId(Guid userId)
        {
            try
            {
                return Ok(await _postService.GetUserPostsById(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostModel model)
        {
            try
            {
                await _postService.Create(model.PostId, model.Text);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdatePostModel model)
        {
            try
            {
                await _postService.Update(model.PostId, model.Text);

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

        [HttpDelete("{postId:guid}")]
        public async Task<IActionResult> Delete(Guid postId)
        {
            try
            {
                await _postService.Delete(postId);

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