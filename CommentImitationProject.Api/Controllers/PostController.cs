using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{postId:guid}")]
        public Task<IActionResult> GetByPostId(Guid postId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] string text, Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        public Task<IActionResult> Update([FromBody] Guid postId, string text)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{postId:guid}")]
        public Task<IActionResult> Delete(Guid postId)
        {
            throw new NotImplementedException();
        }
    }
}