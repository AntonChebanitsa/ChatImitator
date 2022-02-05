// using System;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
//
// namespace CommentImitationProject.Controllers
// {
//     public class PostController : Controller
//     {
//         [HttpGet]
//         public Task<IActionResult> GetAll()
//         {
//             // get all
//             return Task.FromResult(Ok("asd"));
//         }
//
//         [HttpGet("{postId}")]
//         public Task<IActionResult> GetByPostId(Guid postId)
//         {
//             return Task.FromResult(Ok("asd"));
//         }
//
//         [HttpPost]
//         public Task<IActionResult> Create([FromBody])
//         {
//             return new Task<IActionResult>();
//         }
//
//         [HttpPut]
//         public Task<IActionResult> Edit([FromBody] Guid postId, string text)
//         {
//             return new Task<IActionResult>();
//         }
//
//         [HttpPatch]
//         public Task<IActionResult> Update()
//         {
//             return new Task<IActionResult>();
//         }
//
//         [HttpDelete("{postId}")]
//         public Task<IActionResult> Delete(Guid postId)
//         {
//             return new Task<IActionResult>();
//         }
//     }
// }