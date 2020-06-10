using System.Threading.Tasks;
using Corvette.Chat.Data.Entities;
using Corvette.Chat.Logic;
using Corvette.Chat.Logic.Impl;
using Corvette.Chat.Logic.Models;
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
            // create chat service
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
        public async Task CreatePrivateChatAsync_CorrectData()
        {
            // arrange
            var creator = new UserModel(_owner);
            
            // act
            var chat = await _service.CreatePrivateChatAsync(creator, _member.Id);

            // assert
            Check.That(chat).IsNotNull();
            Check.That(chat.IsPrivate).IsTrue();
            Check.That(chat.Name).IsEqualTo(_member.Name);
        }
        
        [Fact]
        public async Task CreatePublicChatAsync_CorrectData()
        {
            // arrange
            var creator = new UserModel(_owner);
            const string name = "chat name";
            
            // act
            var chat = await _service.CreatePublicChatAsync(creator, name);

            // assert
            Check.That(chat).IsNotNull();
            Check.That(chat.IsPrivate).IsFalse();
            Check.That(chat.Name).IsEqualTo(name);
        }
    }
}