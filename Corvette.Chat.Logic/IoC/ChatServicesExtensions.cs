using System;
using Corvette.Chat.Data;
using Corvette.Chat.Logic.Impl;
using Curiosity.Configuration;
using Curiosity.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace Corvette.Chat.Logic.IoC
{
    public static class ChatServicesExtensions
    {
        /// <summary>
        /// Adds to IoC the Corvette Chat services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="action">Action for set database options</param>
        /// <exception cref="ArgumentOutOfRangeException">When the connection string is null or white space</exception>
        public static IServiceCollection AddCorvetteChat(this IServiceCollection services,  Action<DbOptions> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            
            var options = new DbOptions();
            action.Invoke(options);
            
            return AddCorvetteChat(services, options);
        }
        
        /// <summary>
        /// Adds to IoC the Corvette Chat services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="options">Database options</param>
        /// <exception cref="ArgumentOutOfRangeException">When the connection string is null or white space</exception>
        public static IServiceCollection AddCorvetteChat(this IServiceCollection services, DbOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            // validate options
            var errors = options.Validate(nameof(options));
            if (errors.Count > 0)
                throw new ConfigurationValidationException("Wrong data base options", errors);

            services.AddSingleton(options);
            
            services.AddSingleton<IChatDataContextFactory, ChatDataContextFactory>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMemberService, MemberService>();

            return services;
        }
    }
}