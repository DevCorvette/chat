using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Corvette.Chat.Logic;
using Corvette.Chat.Logic.Exceptions;
using Corvette.Chat.Logic.Models;
using Corvette.Chat.WebService.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace Corvette.Chat.WebService.Helpers
{
    public class AuthHelper
    {
        private readonly IUserService _userService;
        
        private readonly WebServiceConfiguration _webServiceConfiguration;

        private const string UserIdKey = "UserId";

        public AuthHelper(IUserService userService, WebServiceConfiguration webServiceConfiguration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _webServiceConfiguration = webServiceConfiguration ?? throw new ArgumentNullException(nameof(webServiceConfiguration));
        }

        /// <summary>
        /// Generates JWT token from app options and user's data.
        /// </summary>
        public string GetAuthToken(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            // generate token
            var claims = new List<Claim>
            {
                new Claim(UserIdKey, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
            };
            
            var now = DateTime.UtcNow;
            
            var jwt = new JwtSecurityToken(
                issuer: _webServiceConfiguration.AuthOptions.Issuer,
                audience: _webServiceConfiguration.AuthOptions.Audience,
                notBefore: now,
                expires: now.AddDays(_webServiceConfiguration.AuthOptions.LifeDays),
                claims: claims,
                signingCredentials: new SigningCredentials(_webServiceConfiguration.AuthOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        /// Returns user by UserId from JWT.
        /// </summary>
        public Task<UserModel> GetUserAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            
            var claim = context.User.Claims.SingleOrDefault(x => x.Type == UserIdKey);
            if (claim == null) throw new AuthenticationException($"JWT claim by type: {UserIdKey} not found");
            
            if (!Guid.TryParse(claim.Value, out var userId))
                throw new AuthenticationException($"Can't parse {userId}");

            try
            {
                return _userService.GetUserAsync(userId);
            }
            catch (EntityNotFoundException e)
            {
                throw new AuthenticationException(e.Message);
            }
        }
    }
}