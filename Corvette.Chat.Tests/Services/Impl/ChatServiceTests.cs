using System.Threading.Tasks;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Services;
using Corvette.Chat.Services.Impl;
using Corvette.Chat.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using NFluent;
using Xunit;

namespace Corvette.Chat.Tests.Services.Impl
{
    public class ChatServiceTests : BaseTest
    {
        private readonly IChatService _service;

        private readonly UserEntity _owner;

        private readonly UserEntity _member;
        
        public ChatServiceTests()
        {
            // create chat servise
            var provider = GetServiceCollection<IChatService, ChatService>()
                .BuildServiceProvider();

            _service = provider.GetService<IChatService>();
            
            // create test users
            _owner = new UserEntity {Name = "Owner"};
            _member = new UserEntity {Name = "Member"};

            // save users
            var context = CreateContext();
            context.Add(_owner);
            context.Add(_member);
            context.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateChatAsync_CreatePrivateChat_CorrectData()
        {
            // arrange
            var creator = new UserModel(_owner);
            
            // act
            var chat = await _service.CreateChatAsync(creator, null, new[] {_member.Id}, true);

            // assert
            Check.That(chat).IsNotNull();
            Check.That(chat.IsPrivate).IsTrue();
            Check.That(chat.Name).IsEqualTo(_member.Name);
        }
        
        [Fact]
        public async Task CreateChatAsync_CreatePublicChat_CorrectData()
        {
            // arrange
            var creator = new UserModel(_owner);
            const string name = "chat name";
            
            // act
            var chat = await _service.CreateChatAsync(creator, name, new[] {_member.Id}, false);

            // assert
            Check.That(chat).IsNotNull();
            Check.That(chat.IsPrivate).IsFalse();
            Check.That(chat.Name).IsEqualTo(name);
        }
    }
}