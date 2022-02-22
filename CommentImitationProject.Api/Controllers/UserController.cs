using System;
using System.Threading.Tasks;
using CommentImitationProject.DTO;
using CommentImitationProject.Models;
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
            return Ok(await _userService.GetAll());
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            try
            {
                return Ok(await _userService.GetById(userId));
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string nickName)
        {
            try
            {
                await _userService.Create(nickName);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] UserDto user)
        {
            try
            {
                _userService.Edit(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateUserModel model)
        {
            try
            {
                await _userService.Update(model.UserId, model.Nickname);

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

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            try
            {
                await _userService.Delete(userId);

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