using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFunctionApp
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            } catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            // Do whatever you want here
            var response = context.Features.Get<HttpResponseData>();
            response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            // can add data, headers, etc.
        }
    }
}
