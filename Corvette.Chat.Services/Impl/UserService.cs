using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.DTO;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Services.Impl
{
    /// <inheritdoc/>
    public sealed class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly IChatDataContextFactory _contextFactory;

        public UserService(
            ILogger<UserService> logger, 
            IChatDataContextFactory contextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }


        /// <inheritdoc/>
        public async Task<UserDto> CreateAsync(string name)
        {
            await using var context = _contextFactory.CreateContext();

            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentOutOfRangeException(nameof(name), "Can't be null or empty.");
            
            if (await IsUserNameUsedAsync(context, name)) 
                throw new ChatServiceException($"A user name: {name} is already used.");

            
            var user = new UserEntity
            {
                Name = name,
            };

            context.Add(user);
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"User with id: {user.Id}, name: {user.Name} was successfully created.");

            return new UserDto(user);
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(Guid userId, string name)
        {
            await using var context = _contextFactory.CreateContext();

            if (userId == default) 
                throw new ArgumentOutOfRangeException(nameof(userId), "Can't be default.");
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentOutOfRangeException(nameof(name), "Can't be null or empty.");
            
            if (await IsUserNameUsedAsync(context, name, userId))
                throw new ChatServiceException($"A user name: {name} is already used.");

            
            var user = await context.Users.GetByIdAsync(userId);
            user.Name = name;
            
            _logger.LogInformation($"Username: {user.Name} was successfully updated for user with id: {user.Id}.");

            await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetAsync(Guid id)
        {
            if (id == default) throw new ArgumentOutOfRangeException(nameof(id), "Can't be default.");
            
            
            await using var context = _contextFactory.CreateContext();
            var user = await context.Users.GetByIdAsync(id, true);
            
            return new UserDto(user);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(IReadOnlyList<Guid> userIds)
        {
            if (userIds.Count <= 0) throw new ArgumentOutOfRangeException(nameof(userIds), "Can't be empty.");
            
            
            await using var context = _contextFactory.CreateContext();
            foreach (var userId in userIds)
            {
                var user = await context.Users.GetByIdAsync(userId);
                context.Users.Remove(user);
            }

            await context.SaveChangesAsync();
            
            _logger.LogInformation($"Users with id[{string.Join(" ,", userIds)}] was successfully removed");
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserNameUsedAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentOutOfRangeException(nameof(username), "Can't be null or empty.");

            await using var context = _contextFactory.CreateContext();
            return await IsUserNameUsedAsync(context, username);
        }

        private async Task<bool> IsUserNameUsedAsync(ChatDataContext context, string username, Guid? userId = null)
        {
            return await context.Users
                .Where(x => x.Name.Equals(username.Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(x => userId == null || x.Id != userId)
                .AnyAsync();
        }
    }
}