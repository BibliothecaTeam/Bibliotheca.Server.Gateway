using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which manages users.
    /// /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IUsersService _userService;

        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userService">Users service.</param>
        /// <param name="authorizationService">Authorization service.</param>
        public UsersController(IUsersService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Get list of users.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all users defined in ths authorization service.
        /// </remarks>
        /// <returns>List of users.</returns>
        [HttpGet]
        [Authorize("CanManageUsers")]
        [ProducesResponseType(200, Type = typeof(IList<UserDto>))]
        public async Task<IList<UserDto>> Get()
        {
            var user = await _userService.GetUsersAsync();
            return user;
        }

        /// <summary>
        /// Get specific user.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about specific user.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <returns>User data.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        public async Task<IActionResult> Get(string id)
        {
            var authorization = await _authorizationService.AuthorizeAsync(User, new UserDto { Id = id }, Operations.Read);
            if (!authorization.Succeeded)
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

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating new user information.
        /// </remarks>
        /// <param name="user">User information.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [Authorize("CanManageUsers")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post([FromBody] UserDto user)
        {
            await _userService.CreateUserAsync(user);
            return Created($"/users/{user.Id}", user);
        }

        /// <summary>
        /// Update user information.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating user information.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <param name="user">User information.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{id}")]
        [Authorize("CanManageUsers")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string id, [FromBody] UserDto user)
        {
            await _userService.UpdateUserAsync(id, user);
            return Ok();
        }

        /// <summary>
        /// Delete user.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting user.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{id}")]
        [Authorize("CanManageUsers")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        /// <summary>
        /// Refresh access token.
        /// </summary>
        /// <remarks>
        /// Endpoint is resposible for saving new access token for user.
        /// </remarks>
        /// <param name="id">User id.</param>
        /// <param name="accessToken">New access token.</param>
        /// <returns>If token saved successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{id}/refreshToken")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> RefreshAccessToken(string id, [FromBody] AccessTokenDto accessToken)
        {
            var authorization = await _authorizationService.AuthorizeAsync(User, new UserDto { Id = id }, Operations.Update);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _userService.RefreshTokenAsync(id, accessToken);
            return Ok();
        }
    }
}
