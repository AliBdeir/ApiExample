using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestFunctionApp
{
    public class TestFunction(ILogger<TestFunction> logger, IConfiguration configuration)
    {
        [Function("TestFunction")]
        public async Task<HttpResponseData> GetAllPeople([HttpTrigger(AuthorizationLevel.Function, "get", Route = "TestFunction")] HttpRequestData req)
        {
            logger.LogInformation("Test");
            string name = configuration["AppName"] ?? throw new Exception();
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(new
            {
                appName = name,
            });
            return response;
        }
    }
}
