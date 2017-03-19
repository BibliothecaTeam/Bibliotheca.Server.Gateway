using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public async Task<IList<UserDto>> Get()
        {
            var projects = await _userService.GetUsersAsync();
            return projects;
        }

        [HttpGet("{id}")]
        public async Task<UserDto> Get(string id)
        {
            var project = await _userService.GetUserAsync(id);
            return project;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            await _userService.CreateUserAsync(user);
            return Created($"/projects/{user.Id}", user);
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> Put(string id, [FromBody] UserDto user)
        {
            await _userService.UpdateUserAsync(id, user);
            return Ok();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
