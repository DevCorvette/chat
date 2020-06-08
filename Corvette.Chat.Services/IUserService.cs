using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Models;

namespace Corvette.Chat.Services
{
    /// <summary>
    /// Service that contains all of the user business logic.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new chat user.
        /// </summary>
        /// <param name="name">Unique user name</param>
        /// <exception cref="ArgumentOutOfRangeException">When the name is null or white space</exception>
        /// <exception cref="ChatServiceException">When a user with that name is already exist</exception>
        Task<UserModel> CreateUserAsync(string name);

        /// <summary>
        /// Updates the user's name.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="name">User's new name</param>
        /// <exception cref="ArgumentOutOfRangeException">When user id is default or user name is null or white space</exception>
        /// <exception cref="ChatServiceException">When a user with that name is already exist</exception>
        /// <exception cref="EntityNotFoundException">When a user was not found by id in the database</exception>
        Task UpdateUserNameAsync(Guid userId, string name);
        
        /// <summary>
        /// Returns a user by id.
        /// </summary>
        /// <param name="id">User id</param>
        /// <exception cref="EntityNotFoundException">When a user was not found by id in the database</exception>
        /// <exception cref="ArgumentOutOfRangeException">When user id is default</exception>
        Task<UserModel> GetUserAsync(Guid id);

        /// <summary>
        /// Returns users filtered by the search string.
        /// Returns all users if the search string is null.
        /// </summary>
        Task<IReadOnlyList<UserModel>> GetUsersAsync(string? search);

        /// <summary>
        /// Removes users from the database.
        /// </summary>
        /// <param name="userIds">List of user ids which will remove</param>
        /// <exception cref="ArgumentOutOfRangeException">When a users id list is empty</exception>
        /// <exception cref="EntityNotFoundException">When a user was not found by id in the database</exception>
        Task RemoveUsersAsync(IReadOnlyList<Guid> userIds);

        /// <summary>
        /// Returns true when a user with the current name is already exist in the database.
        /// </summary>
        /// <param name="username">User name</param>
        /// <exception cref="ArgumentOutOfRangeException">When the username is null or white space</exception>
        Task<bool> IsUserNameUsedAsync(string username);
    }
}