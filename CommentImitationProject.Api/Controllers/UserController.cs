using System;
using System.Threading.Tasks;
using CommentImitationProject.DAL.Entities;
using CommentImitationProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommentImitationProject.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _userService.GetAll());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            try
            {
                return Ok(await _userService.GetById(userId));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string nickName)
        {
            try
            {
                await _userService.Create(nickName);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] User user)
        {
            try
            {
                _userService.Edit(user);

                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] Guid userId, string nickName)
        {
            try
            {
                await _userService.Update(userId, nickName);

                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            try
            {
                await _userService.Delete(userId);

                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}