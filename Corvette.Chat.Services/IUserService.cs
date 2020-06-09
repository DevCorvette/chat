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
        /// <param name="login">Unique user login that is used for user identity</param>
        /// <param name="secretKey">A key that is used for user identity</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ChatServiceException"></exception>
        Task<UserModel> CreateUserAsync(string name, string login, string secretKey);

        /// <summary>
        /// Updates the user's data.
        /// Updates only those data which has a value.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="name">User's new name</param>
        /// <param name="login"></param>
        /// <param name="secretKey"></param>
        /// <exception cref="ChatServiceException"></exception>
        /// <exception cref="EntityNotFoundException"></exception>
        Task UpdateUserAsync(Guid userId, string? name, string? login, string? secretKey);
        
        /// <summary>
        /// Returns a user by id.
        /// </summary>
        /// <param name="id">User id</param>
        /// <exception cref="EntityNotFoundException">When a user was not found by id in the database</exception>
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
        /// Returns true when a user with the username has already existed in the database.
        /// </summary>
        /// <param name="username">User name</param>
        /// <exception cref="ArgumentNullException"></exception>
        Task<bool> IsUserNameUsedAsync(string username);

        /// <summary>
        /// Returns true when a user with the login has already existed in the database.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Task<bool> IsLoginUsedAsync(string login);
    }
}