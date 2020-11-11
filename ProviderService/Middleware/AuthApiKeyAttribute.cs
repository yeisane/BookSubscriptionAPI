using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProviderService.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeader = "ApiKey";
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var _unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var _httpContextAccessor = context.HttpContext.RequestServices.GetRequiredService<IHttpContextAccessor>();

            //get the host DNS where the call is coming from
            string host = _httpContextAccessor.HttpContext.Request.Host.Value;

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var passedApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            //check if the API key matches the DNS
            var apiKey = (await _unitOfWork.RegisteredApiUsers.FindAsync(x => x.ApiKey.Equals(passedApiKey))).FirstOrDefault();

            if (apiKey == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (apiKey.Dns != host)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            await next();
        }
    }
}
