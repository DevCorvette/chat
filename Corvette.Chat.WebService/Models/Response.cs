using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Corvette.Chat.WebService.Models
{
    /// <summary>
    /// Web service response without body.
    /// </summary>
    public class Response
    {
        public bool IsSuccess { get; set; }

        public IReadOnlyList<ErrorModel> Errors { get; set; }

        /// <summary>
        /// Use this constructor when an action is successfully completed.
        /// </summary>
        public Response()
        {
            IsSuccess = true;
            Errors = Array.Empty<ErrorModel>();
        }

        /// <summary>
        /// Use this constructor when an action has business logic error.
        /// </summary>
        public Response(IReadOnlyList<ErrorModel> errors)
        {
            IsSuccess = false;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented = true});
        }
    }

    /// <summary>
    /// Web service response with body.
    /// </summary>
    public class Response<T> : Response
    {
        public T Body { get; }

        /// <summary>
        /// Use this constructor when an action is successfully completed and you have body for response.
        /// </summary>
        public Response(T body)
        {
            Body = body;
        }
    }
}