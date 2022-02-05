// using System;
// using CommentImitationProject.DAL.Entities;
// using Microsoft.AspNetCore.Mvc;
//
// namespace CommentImitationProject.Controllers
// {
//     public class UserController : Controller
//     {
//         [HttpGet]
//         public Task<IActionResult> GetAll()
//         {
//             // get all users
//             return Task.FromResult(Ok("asd"));
//         }
//
//         [HttpGet("{id}")]
//         public Task<IActionResult> GetById(Guid postId)
//         {
//             //get user by id
//             return Task.FromResult(Ok("asd"));
//         }
//
//         [HttpPost]
//         public Task<IActionResult> Create([FromBody] User user)
//         {
//             //add new user to db
//             return new Task<IActionResult>();
//         }
//
//         [HttpPut]
//         public Task<IActionResult> Edit([FromBody] User user)
//         {
//             return new Task<IActionResult>();
//         }
//
//         [HttpPatch]
//         [Route "update-email"]
//         public Task<IActionResult> Update([FromBody] Guid userId, string email)
//         {
//             return new Task<IActionResult>();
//         }
//
//         [HttpDelete("{id}")]
//         public Task<IActionResult> Delete(Guid userId)
//         {
//             return new Task<IActionResult>();
//         }
//     }
// }