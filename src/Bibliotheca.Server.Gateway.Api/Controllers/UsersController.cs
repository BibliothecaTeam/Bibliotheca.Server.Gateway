using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
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

        private readonly IAuthorizationService _authorizationService;

        public UsersController(IUsersService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpGet()]
        [Authorize("CanManageUsers")]
        public async Task<IList<UserDto>> Get()
        {
            var user = await _userService.GetUsersAsync();
            return user;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new UserDto { Id = id }, Operations.Read);
            if (!isAuthorize)
            {
                return Forbid();
            }

            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return new ObjectResult(user);
        }

        [HttpPost]
        [Authorize("CanManageUsers")]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            await _userService.CreateUserAsync(user);
            return Created($"/users/{user.Id}", user);
        }

        [HttpPut("{id}")]
        [Authorize("CanManageUsers")]
        public async Task<IActionResult> Put(string id, [FromBody] UserDto user)
        {
            await _userService.UpdateUserAsync(id, user);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize("CanManageUsers")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpPut("{id}/refreshToken")]
        public async Task<IActionResult> RefreshAccessToken(string id, [FromBody] AccessTokenDto accessToken)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new UserDto { Id = id }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _userService.RefreshTokenAsync(id, accessToken);
            return Ok();
        }
    }
}
