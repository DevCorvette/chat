using System;
using Corvette.Chat.Data;
using Corvette.Chat.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace Corvette.Chat.Services.IoC
{
    public static class ChatServicesExtensions
    {
        /// <summary>
        /// Adds to IoC the Corvette Chat services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="connectionString">Connection string of chat's database</param>
        /// <exception cref="ArgumentOutOfRangeException">When the connection string is null or white space</exception>
        public static IServiceCollection AddChatServices(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) 
                throw new ArgumentOutOfRangeException(nameof(connectionString));
            
            services.AddSingleton<IChatDataContextFactory>(new ChatDataContextFactory(connectionString));
            
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMemberService, MemberService>();

            return services;
        }
    }
}