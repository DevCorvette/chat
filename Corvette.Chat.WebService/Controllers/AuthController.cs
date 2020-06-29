using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Corvette.Chat.Logic;
using Corvette.Chat.WebService.Models;
using Corvette.Chat.WebService.Models.Auth;
using Corvette.Chat.WebService.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Corvette.Chat.WebService.Controllers
{
    [Route("chat/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        private readonly AppSettings _appSettings;

        public AuthController(IUserService userService, AppSettings appSettings)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        [HttpGet]
        public Response Test()
        {
            return new Response();
        }

        /// <summary>
        /// Returns authorization token for user by user's login and secret key.
        /// </summary>
        [HttpPost]
        public async Task<Response<string>> Auth([FromBody] AuthModel model)
        {
            // get and check user
            var user = await _userService.GetUserAsync(model.Login, model.Key);
            
            // generate token
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            };
            
            var now = DateTime.UtcNow;
            
            var jwt = new JwtSecurityToken(
                issuer: _appSettings.AuthSettings.Issuer,
                audience: _appSettings.AuthSettings.Audience,
                notBefore: now,
                expires: now.AddDays(_appSettings.AuthSettings.LifeDays),
                claims: claims,
                signingCredentials: new SigningCredentials(_appSettings.AuthSettings.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            var token =  new JwtSecurityTokenHandler().WriteToken(jwt);
            return new Response<string>(token);
        }
    }
}