using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Logic.Exceptions;
using Corvette.Chat.Logic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.Logic.Impl
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
        public async Task<UserModel> CreateUserAsync(string name, string login, string secretKey)
        {
            _logger.LogDebug($"{nameof(CreateUserAsync)} started for new user name: {name}, login: {login}.");
            await using var context = _contextFactory.CreateContext();

            // checks
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Can't be null or empty.");
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentNullException(nameof(login), "Can't be null or empty.");
            if (string.IsNullOrWhiteSpace(secretKey)) throw new ArgumentNullException(nameof(secretKey), "Can't be null or empty.");
            if (await IsUserNameUsedAsync(context, name)) throw new ChatServiceException($"A user name: {name} is already used.");
            if (await IsLoginUsedAsync(context, name)) throw new ChatServiceException($"A login: {login} is already used.");
            
            // create
            var user = new UserEntity
            {
                Name = name.Trim(),
                Login = login.Trim(),
                SecretKey = secretKey,
            };
            context.Add(user);
            await context.SaveChangesAsync();
            
            _logger.LogInformation($"User with id: {user.Id}, name: {user.Name} was successfully created.");
            return new UserModel(user);
        }

        /// <inheritdoc/>
        public async Task UpdateUserAsync(Guid userId, string? name, string? login, string? secretKey)
        {
            _logger.LogDebug($"{nameof(UpdateUserAsync)} started for user with id: {userId} new name: {name}.");
            await using var context = _contextFactory.CreateContext();

            // get
            var user = await context.Users
                           .Where(x => x.Id == userId)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException($"User by id: {userId} was not found");

            // update
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (await IsUserNameUsedAsync(context, name, userId)) 
                    throw new ChatServiceException($"A user name: {name} is already used.");
                
                user.Name = name.Trim();
                _logger.LogInformation($"{nameof(UpdateUserAsync)} updated username: {user.Name} for user with id: {user.Id}.");
            }
            
            if (!string.IsNullOrWhiteSpace(login))
            {
                if (await IsLoginUsedAsync(context, login, userId))
                    throw new ChatServiceException($"A login: {login} is already used.");

                user.Login = login.Trim();
                _logger.LogInformation($"{nameof(UpdateUserAsync)} updated login: {user.Login} for user with id: {user.Id}.");
            }
            
            if (!string.IsNullOrWhiteSpace(secretKey))
            {
                user.SecretKey = secretKey;
                _logger.LogInformation($"{nameof(UpdateUserAsync)} updated secret key: {user.SecretKey} for user with id: {user.Id}.");
            }           
            
            await context.SaveChangesAsync();
            
            _logger.LogDebug($"{nameof(UpdateUserAsync)} successfully finished.");
        }

        /// <inheritdoc/>
        public async Task<UserModel> GetUserAsync(Guid id)
        {
            await using var context = _contextFactory.CreateContext();

            // get
            var user = await context.Users
                           .SingleOrDefaultAsync(x => x.Id == id)
                       ?? throw new EntityNotFoundException($"User by id: {id} was not found");
            
            return new UserModel(user);
        }

        /// <inheritdoc/>
        public async Task<UserModel> GetUserAsync(string login, string key)
        {
            await using var context = _contextFactory.CreateContext();
            
            // get
            var user = await context.Users
                           .Where(x => x.Login == login)
                           .Where(x => x.SecretKey == key)
                           .SingleOrDefaultAsync()
                       ?? throw new EntityNotFoundException("User was not found. Login or key is incorrect.");
            
            return new UserModel(user);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserModel>> GetUsersAsync(string? search)
        {
            await using var context = _contextFactory.CreateContext();

            var users = await context.Users
                .Where(x => search == null
                            || x.Name.ToLower().Contains(search.ToLower()))
                .ToListAsync();

            return users.ConvertAll(x => new UserModel(x)).ToList();
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
                    throw new ChatLogicException($"Can't remove a user with id: {userId} because he owns some chats.");
                
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
                throw new ArgumentNullException(nameof(username), "Can't be null or empty.");
            
            return await IsUserNameUsedAsync(context, username);
        }
        
        /// <inheritdoc/>
        public async Task<bool> IsLoginUsedAsync(string login)
        {
            await using var context = _contextFactory.CreateContext();

            if (string.IsNullOrWhiteSpace(login)) 
                throw new ArgumentNullException(nameof(login), "Can't be null or empty.");
            
            return await IsUserNameUsedAsync(context, login);
        }

        private async Task<bool> IsUserNameUsedAsync(ChatDataContext context, string username, Guid? userId = null)
        {
            return await context.Users
                .Where(x => x.Name.Trim().ToLower() == username.Trim().ToLower())
                .Where(x => userId == null || x.Id != userId)
                .AnyAsync();
        }
        
        private async Task<bool> IsLoginUsedAsync(ChatDataContext context, string login, Guid? userId = null)
        {
            return await context.Users
                .Where(x => x.Login.Trim().ToLower() == login.Trim().ToLower())
                .Where(x => userId == null || x.Id != userId)
                .AnyAsync();
        }
    }
}