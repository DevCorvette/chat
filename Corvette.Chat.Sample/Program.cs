using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.Logic.Models;
using Corvette.Chat.WebService.Models;
using Corvette.Chat.WebService.Models.Auth;
using Corvette.Chat.WebService.Models.Users;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.SignalR.Client;

namespace Corvette.Chat.Sample
{
    public class Program
    {
        private const string BaseUrl = "http://localhost:5001/chat";
        
        private const string BaseLogin = "user";
        private const string KeyHash = "d8578edf8458ce06fbc5bb76a58c5ca4";
        private static int _tryLoginCount = 0;
        
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start");
                
                Console.WriteLine("Create a new user");
                var (user, login) = await CreateNewUser();
                
                Console.WriteLine("Authorization");
                var token = await Authorization(login);
                
                Console.WriteLine("Get an interlocutor");
                var interlocutor = await GetInterlocutor(user, token);
                
                Console.WriteLine("Connecting to SignalR");

                var connection = new HubConnectionBuilder()
                    .WithUrl($"{BaseUrl}/hub", opt => opt.AccessTokenProvider = () => Task.FromResult(token))
                    .WithAutomaticReconnect()
                    .Build();

                await connection.StartAsync();
                Console.WriteLine("Connection established");
                
                Console.WriteLine("Creating a new private chat");
                var privateChat = await connection.InvokeAsync<ChatModel>("CreatePrivateChat", interlocutor.Id);
                Console.WriteLine($"Created private chat: ({privateChat}) with user (id: {interlocutor.Id})");
                
                Console.WriteLine("Creating a new public chat");
                var publicChat = await connection.InvokeAsync<ChatModel>("CreatePublicChat", "New public chat");
                Console.WriteLine($"Created public chat: ({publicChat})");
                
                Console.WriteLine("Renaming public chat");
                await connection.InvokeAsync("RenameChat", publicChat.Id, "Just a public chat");
                publicChat = await connection.InvokeAsync<ChatModel>("GetChat", publicChat.Id);
                Console.WriteLine($"Public chat was successfully renamed: \"{publicChat.Name}\"");



                var list = await connection.InvokeAsync<IReadOnlyList<ChatModel>>("GetAllChats");
                
                Console.WriteLine($"Got {list.Count} chats");

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }
            
            Console.WriteLine("Finish");
        }

        private static T GetBody<T>(Response<T> response)
        {
            if (!response.IsSuccess)
                throw new InvalidOperationException($"Errors in response: {response}");

            return response.Body;
        }
        
        private static async Task<(UserModel user, string login)> CreateNewUser()
        {
            string login = null;
            var isNeedToCheck = true;
            
            Console.WriteLine("Check login");
            while (isNeedToCheck)
            {
                login = $"{BaseLogin}_{++_tryLoginCount}";
                    
                var checkResponse = await Url.Combine(BaseUrl, "users/login/check")
                    .SetQueryParam("login", login)
                    .GetJsonAsync<Response<bool>>();
                
                isNeedToCheck = GetBody(checkResponse);
            }

            // create a new user
            Console.WriteLine("Call creation");
            var userResponse = await Url.Combine(BaseUrl, "users/new")
                .PostJsonAsync(new CreateUserRequest
                {
                    Name = login,
                    Login = login,
                    Key = KeyHash
                })
                .ReceiveJson<Response<UserModel>>();

            // check response and extract user
            var user = GetBody(userResponse) ?? throw new InvalidOperationException($"User from response is null. Response: ({userResponse})");
            Console.WriteLine($"User: ({user}) was successfully created");

            return (user, login);
        }
        
        private static async Task<string> Authorization(string login)
        {
            Console.WriteLine("Trying to get JWT");
            var authResponse = await Url.Combine(BaseUrl, "auth")
                .PostJsonAsync(new AuthModel
                {
                    Login = login,
                    Key = KeyHash
                })
                .ReceiveJson<Response<string>>();

            // check response and extract token
            var token = GetBody(authResponse) ?? throw new InvalidOperationException($"Token from response is null. Response: ({authResponse})");
            Console.WriteLine("JWT successfully taken");

            return token;
        }
        
        private static async Task<UserModel> GetInterlocutor(UserModel currentUser, string token)
        {
            Console.WriteLine("Get users list");
            var usersResponse = await Url.Combine(BaseUrl, "users")
                .WithOAuthBearerToken(token)
                .GetJsonAsync<Response<IReadOnlyList<UserModel>>>();

            var users = GetBody(usersResponse);
            Console.WriteLine($"Successfully got {users.Count} users");

            var interlocutor = users.FirstOrDefault(x => x.Id != currentUser.Id);
            if (interlocutor == null)
            {
                (interlocutor, _) = await CreateNewUser();
            }

            Console.WriteLine($"Got interlocutor: ({interlocutor})");
            return interlocutor;
        }

        
    }
}