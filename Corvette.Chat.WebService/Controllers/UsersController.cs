using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Corvette.Chat.Logic;
using Corvette.Chat.Logic.Models;
using Corvette.Chat.WebService.Models;
using Corvette.Chat.WebService.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corvette.Chat.WebService.Controllers
{
    [Route("chat/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Returns all existing users.
        /// Searches users by name if the search string has value.
        /// </summary>
        /// <param name="search">Search by name</param>
        [HttpGet]
        public async Task<Response<IReadOnlyList<UserModel>>> GetUsers(string? search)
        {
            var users = await _userService.GetUsersAsync(search);
            return new Response<IReadOnlyList<UserModel>>(users);
        }

        /// <summary>
        /// Returns user by id.
        /// </summary>
        /// <param name="id">User id</param>
        [HttpGet("{id}")]
        public async Task<Response<UserModel>> GetUser(Guid id)
        {
            var user = await _userService.GetUserAsync(id);
            return new Response<UserModel>(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        [HttpPost("new")]
        [AllowAnonymous]
        public async Task<Response<UserModel>> CreateUser([FromBody] CreateUserRequest request)
        {
            var user = await _userService.CreateUserAsync(request.Name, request.Login, request.Key);
            return new Response<UserModel>(user);
        }
        
        /// <summary>
        /// Updates an existing user
        /// </summary>
        [HttpPost("update")]
        public async Task<Response> UpdateUser([FromBody] UpdateUserRequest request)
        {
            await _userService.UpdateUserAsync(request.UserId, request.Name, request.Login, request.Key);
            return new Response();
        }

        /// <summary>
        /// Removes users by id.
        /// </summary>
        /// <param name="ids">User ids</param>
        [HttpDelete("delete")]
        public async Task<Response> RemoveUsers([FromBody] [MinLength(1)] IReadOnlyList<Guid> ids)
        {
            await _userService.RemoveUsersAsync(ids);
            return new Response();
        }

        /// <summary>
        /// Returns true if the username is already used.
        /// </summary>
        /// <param name="name">User name</param>
        [HttpGet("name/check")]
        public async Task<Response<bool>> CheckName([Required] [MinLength(1)] string name)
        {
            var isUsed = await _userService.IsUserNameUsedAsync(name);
            return new Response<bool>(isUsed);
        }
        
        /// <summary>
        /// Returns true if the login is already used.
        /// </summary>
        /// <param name="login">User name</param>
        [HttpGet("login/check")]
        public async Task<Response<bool>> CheckLogin([Required] [MinLength(1)] string login)
        {
            var isUsed = await _userService.IsLoginUsedAsync(login);
            return new Response<bool>(isUsed);
        }
    }
}