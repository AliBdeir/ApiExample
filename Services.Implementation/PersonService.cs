using Microsoft.Data.SqlClient;
using ServiceDefinitions;
using System.Data;
using Microsoft.Extensions.Logging;
using Azure.Core;
using Dapper;

namespace ServicesImplementation
{
    public class PersonService(SqlConnection db, ILoggerFactory loggerFactory) : IPersonService
    {
        private readonly ILogger logger = loggerFactory.CreateLogger<PersonService>();
        public async Task<int> AddPerson(FrontendPerson person)
        {
            logger.LogInformation("Adding person of name {name} and age {age}", person.Name, person.Age);
            await db.OpenAsync();
            int insertedId = await db.QuerySingleAsync<int>("INSERT Into [Person] ([Name], [Age]) OUTPUT Inserted.PersonId VALUES (@Name, @Age)", new
            {
                person.Name,
                person.Age
            });
            return insertedId;
        }

        public async Task DeletePerson(int personId)
        {
            logger.LogInformation("Deleting person of ID {personId}", personId);
            await db.OpenAsync();
            await db.OpenAsync();
            await db.ExecuteAsync("DELETE FROM [Person] WHERE PersonId = @personId", new { personId });
        }

        public async Task<ICollection<FrontendPerson>> GetPeople()
        {
            logger.LogInformation("Getting all people");
            await db.OpenAsync();
            var persons = await db.QueryAsync<FrontendPerson>("SELECT * FROM [Person]");
            return persons.ToList();
        }

        public async Task<FrontendPerson> GetPerson(int personId)
        {
            logger.LogInformation("Getting Person of id {id}", personId);
            await db.OpenAsync();
            var person = await db.QuerySingleAsync<FrontendPerson>("select * from [Person] where PersonId = @personId", new
            {
                personId
            });
            return person;
        }
    }
}
