using System;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{userId:guid}")]
        public Task<IActionResult> GetById(Guid userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Create([FromBody] User user)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public Task<IActionResult> Edit([FromBody] User user)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{userId:guid}")]
        public Task<IActionResult> Delete(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}