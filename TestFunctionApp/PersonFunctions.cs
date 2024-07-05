using Dapper;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ServiceDefinitions;

namespace TestFunctionApp
{
    public class PersonFunctions(IPersonService service)
    {
        [Function("GetAllPeople")]
        public async Task<HttpResponseData> GetAllPeople([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Person")] HttpRequestData req)
        {
            SentrySdk.CaptureMessage("Hello, world!", SentryLevel.Error);
            var people = await service.GetPeopleAsync();
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(people);
            return response;
        }


        [Function("GetPerson")]
        public async Task<HttpResponseData> GetPerson([HttpTrigger(AuthorizationLevel.Function, "get", Route = "Person/{id}")] HttpRequestData req,
            int id)
        {
            var person = await service.GetPersonAsync(id);
            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(person);
            return response;
        }


        [Function("AddPerson")]
        public async Task<HttpResponseData> AddPerson([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Person")] HttpRequestData req,
            [FromBody] FrontendPerson person)
        {
            int id = await service.AddPersonAsync(person);
            var res = req.CreateResponse();
            await res.WriteAsJsonAsync(new
            {
                id,
            });
            return res;
        }


        [Function("DeletePerson")]
        public async Task<HttpResponseData> DeletePerson([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Person/{id}")] HttpRequestData req, int id)
        {
            await service.DeletePersonAsync(id);
            return req.CreateResponse();
        }

        [Function("Throw")]
        public HttpResponseData Throw([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Throw")] HttpRequestData req)
        {
            throw new Exception("Throwing!");
        }

    }
}
