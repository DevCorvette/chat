using System;
using System.Threading.Tasks;
using Corvette.Chat.Logic;
using Corvette.Chat.WebService.Helpers;
using Corvette.Chat.WebService.Models;
using Corvette.Chat.WebService.Models.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Corvette.Chat.WebService.Controllers
{
    [Route("chat/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        private readonly AuthHelper _authHelper;

        public AuthController(
            IUserService userService, 
            AuthHelper authHelper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authHelper = authHelper ?? throw new ArgumentNullException(nameof(authHelper));
        }

        /// <summary>
        /// Returns authorization token for user by user's login and secret key.
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> Auth([FromBody] AuthModel model)
        {
            // get and check user
            var user = await _userService.GetUserAsync(model.Login, model.Key);
            
            var token = _authHelper.GetAuthToken(user);
            return new Response<string>(token);
        }
    }
}