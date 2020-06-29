using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Corvette.Chat.Logic.Exceptions;
using Corvette.Chat.WebService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Corvette.Chat.WebService.Middleware
{
    /// <summary>
    /// Middleware for handling a business logic exceptions
    /// </summary>
    public class ExceptionFilterMiddleware
    {
        private readonly ILogger<ExceptionFilterMiddleware> _logger;
        
        private readonly RequestDelegate _next;
        
        /// <summary>
        /// Create a new ExceptionFilterMiddleware
        /// </summary>
        public ExceptionFilterMiddleware(RequestDelegate next, ILogger<ExceptionFilterMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Catches exception, writes a error to the log.
        /// If exception is <see cref="ChatLogicException"/> returns response with error information
        /// else throws it up.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.Request.ContentType == "application/json")
                    context.Request.EnableBuffering(); // for reading a request from a body
                
                await _next.Invoke(context);
                
            }
            catch (ChatLogicException ex)
            {
                _logger.LogWarning(ex, $"Caught BLL exception: '{ex.GetType().Name}' from request: {await DescribeAsync(context.Request)}");

                // write response
                var response = new Response(new[] {new ErrorModel(ex.Message, null)});

                context.Response.Clear();
                context.Response.StatusCode = (int) HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Unexpected exception occured from request: {await DescribeAsync(context.Request)}");
                throw;
            }
        }

        private async ValueTask<string> DescribeAsync(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out var token);
            
            var body = "";
            if (request.ContentType == "application/json" && request.Body.CanSeek)
            {
                using var reader = new StreamReader(request.Body);
                request.Body.Seek(0, SeekOrigin.Begin);
                body = await reader.ReadToEndAsync();
            }

            return $"\nPath: {request.Path} " +
                   $"\nMethod: {request.Method} " +
                   $"\nToken: {token} " +
                   $"\nContentType: {request.ContentType} " +
                   $"\nQuery: {request.QueryString} " +
                   $"\nBody: {body}";
        }
    }
}