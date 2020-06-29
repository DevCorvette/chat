using System.Linq;
using System.Threading.Tasks;
using Corvette.Chat.WebService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Corvette.Chat.WebService.Middleware
{
    /// <summary>
    /// Checks that requested model is valid
    /// </summary>
    public class ModelFilter : IAsyncActionFilter
    {
        /// <summary>
        /// When model is invalid return json response with errors
        /// </summary>
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modelState = context.ModelState;

            if (modelState.IsValid)
            {
                return next();
            }
            else
            {
                var errors = modelState
                    .SelectMany(item => item.Value.Errors
                        .Select(e => new ErrorModel(e.ErrorMessage, item.Key)))
                    .ToList();

                context.Result = new JsonResult(new Response(errors));
                return Task.CompletedTask;
            }
        }
    }
}