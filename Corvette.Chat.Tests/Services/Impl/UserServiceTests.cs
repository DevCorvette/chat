using System;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services;
using Corvette.Chat.Services.Exceptions;
using Corvette.Chat.Services.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NFluent;
using Xunit;

namespace Corvette.Chat.Tests.Services.Impl 
{
    public class UserServiceTests : BaseTest
    {
        private readonly IUserService _service;
        
        public UserServiceTests()
        {
            var provider = GetServiceCollection<IUserService, UserService>()
                .BuildServiceProvider();

            _service = provider.GetService<IUserService>();
        }

        /// <summary>
        /// Checks that the method returns new user with correct data.
        /// </summary>
        [Fact]
        public async Task CreateAsync_CreateUser_CorrectUser()
        {
            // arrange
            const string userName = "Test User";
            const string login = "Login";
            const string key = "key";

            // act
            var user = await _service.CreateUserAsync(userName, login, key);

            // assert
            var entity = await CreateContext().Users
                .Where(x => x.Id == user.Id)
                .SingleOrDefaultAsync();
            
            Check.That(user).IsNotNull();
            Check.That(user.Id).Not.Equals(default(Guid));
            Check.That(user.Name).Equals(userName);
            Check.That(entity).IsNotNull();
            Check.That(entity.Login).Equals(login);
            Check.That(entity.SecretKey).Equals(key);
        }

        /// <summary>
        /// Checks that the method throws Exception when user name is already used.
        /// </summary>
        [Fact]
        public async Task CreateAsync_CheckName_ThrowException()
        {
            // arrange
            var context = CreateContext();
            context.Add(new UserEntity
            {
                Name = "Existed User",
            });
            await context.SaveChangesAsync();

            // act & assert
            Check
                .ThatAsyncCode(async() => await _service.CreateUserAsync(" existed user ", "login", "key"))
                .Throws<ChatServiceException>();
        }

        /// <summary>
        /// Checks that the method updated user name in data base.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UpdateUserName_NewUserName()
        {
            // arrange
            var context = CreateContext();
            var user = new UserEntity
            {
                Name = "Existed User",
                Login = "login",
                SecretKey = "key",
            };
            context.Add(user);
            await context.SaveChangesAsync();

            const string newName = "New Name";
            const string newLogin = "NewLogin";
            const string newKey = "NewKey";
            
            // act
            await _service.UpdateUserAsync(user.Id, newName, newLogin, newKey);

            // assert
            var updatedUser = await CreateContext().Users
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            Check.That(updatedUser).IsNotNull();
            Check.That(updatedUser!.Name).Equals(newName);
            Check.That(updatedUser!.Login).Equals(newLogin);
            Check.That(updatedUser!.SecretKey).Equals(newKey);
        }

        /// <summary>
        /// Checks that the method remove user.
        /// </summary>
        [Fact]
        public async Task RemoveAsync_RemoveUser_UserRemoved()
        {
            // arrange
            var context = CreateContext();
            var user = new UserEntity
            {
                Name = "Existed User",
                Login = "login",
                SecretKey = "key",
            };
            context.Add(user);
            await context.SaveChangesAsync();
            
            // act
            await _service.RemoveUsersAsync(new[] {user.Id});

            // assert
            var removedUser = await CreateContext().Users
                .FirstOrDefaultAsync(x => x.Id == user.Id);

            Check.That(removedUser).IsNull();
        }
    }
}