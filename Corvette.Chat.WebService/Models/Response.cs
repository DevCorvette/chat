using System;
using System.Collections.Generic;

namespace Corvette.Chat.WebService.Models
{
    /// <summary>
    /// Web service response without body.
    /// </summary>
    public class Response
    {
        public bool IsSuccess { get; }
        
        public IReadOnlyList<ErrorModel> Errors { get; }

        /// <summary>
        /// Use this constructor when an action is successfully completed.
        /// </summary>
        public Response()
        {
            IsSuccess = true;
            Errors = new ErrorModel[0];
        }

        /// <summary>
        /// Use this constructor when an action has business logic error.
        /// </summary>
        public Response(IReadOnlyList<ErrorModel> errors)
        {
            IsSuccess = false;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
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