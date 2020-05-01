using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Models;
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
        public async Task<UserModel> CreateUserAsync(string name)
        {
            _logger.LogDebug($"{nameof(CreateUserAsync)} started for new user name: {name}.");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentOutOfRangeException(nameof(name), "Can't be null or empty.");
            if (await IsUserNameUsedAsync(context, name)) throw new ChatServiceException($"A user name: {name} is already used.");
            
            // create
            var user = new UserEntity
            {
                Name = name,
            };
            context.Add(user);
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"User with id: {user.Id}, name: {user.Name} was successfully created.");
            return new UserModel(user);
        }

        /// <inheritdoc/>
        public async Task UpdateUserNameAsync(Guid userId, string name)
        {
            _logger.LogDebug($"{nameof(UpdateUserNameAsync)} started for user with id: {userId} new name: {name}.");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (userId == default) throw new ArgumentOutOfRangeException(nameof(userId), "Can't be default.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentOutOfRangeException(nameof(name), "Can't be null or empty.");
            if (await IsUserNameUsedAsync(context, name, userId)) throw new ChatServiceException($"A user name: {name} is already used.");

            // get
            var user = await context.Users
                           .Where(x => x.Id == userId)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException($"User by id: {userId} was not found");
            
            // update
            user.Name = name;
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"{nameof(UpdateUserNameAsync)} successfully updated username: {user.Name} for user with id: {user.Id}.");
        }

        /// <inheritdoc/>
        public async Task<UserModel> GetUserAsync(Guid id)
        {
            await using var context = _contextFactory.CreateContext();

            // check
            if (id == default) throw new ArgumentOutOfRangeException(nameof(id), "Can't be default.");

            // get
            var user = await context.Users
                           .SingleOrDefaultAsync(x => x.Id == id)
                       ?? throw new EntityNotFoundException($"User by id: {id} was not found");
            
            return new UserModel(user);
        }

        /// <inheritdoc/>
        public async Task RemoveUsersAsync(IReadOnlyList<Guid> userIds)
        {
            _logger.LogDebug($"{nameof(RemoveUsersAsync)} started for users id: {string.Join(" ,", userIds)}");
            await using var context = _contextFactory.CreateContext();

            // check
            if (userIds.Count <= 0) throw new ArgumentOutOfRangeException(nameof(userIds), "Can't be empty.");
            
            foreach (var userId in userIds)
            {
                var user = await context.Users
                               .Include(x => x.OwnChats)
                               .Where(x => x.Id == userId)
                               .SingleOrDefaultAsync()
                           ?? throw new EntityNotFoundException($"User by id: {userId} was not found.");
               
                if (user.OwnChats?.Count != 0)
                    throw new ChatServiceException($"Can't remove a user with id: {userId} because he owns some chats.");
                
                // remove
                context.Users.Remove(user);
            }

            // save
            await context.SaveChangesAsync();
            _logger.LogInformation($"{nameof(RemoveUsersAsync)} successfully removed users with id: {string.Join(" ,", userIds)}");
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserNameUsedAsync(string username)
        {
            await using var context = _contextFactory.CreateContext();

            if (string.IsNullOrWhiteSpace(username)) 
                throw new ArgumentOutOfRangeException(nameof(username), "Can't be null or empty.");
            
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