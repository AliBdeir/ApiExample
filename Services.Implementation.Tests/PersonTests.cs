using Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ServiceDefinitions;
using ServicesImplementation;

namespace Services.Implementation.Tests
{
    public class PersonTests
    {
        private SqlConnection connection = null!;
        private IPersonService personService = null!;

        private readonly string testUserName = "Test User";
        private readonly int testUserAge = 21;

        [SetUp]
        public async Task Setup()
        {
            this.connection = new(ConfigurationHelper.GetConnectionString("Testing"));
            await this.connection.OpenAsync();
            await connection.ExecuteAsync("DBCC CHECKIDENT ('[Person]', RESEED,0)\nDELETE Person"); // Can also create stored procedure to do this
            await connection.ExecuteAsync("INSERT INTO [Person] (Name, Age) values (@testUserName, @testUserAge)", new
            {
                testUserName,
                testUserAge
            });
            await this.connection.CloseAsync();
            personService = new PersonService(this.connection, new LoggerFactory());
        }

        [Test]
        public async Task Test_AddPerson_InsertsPerson()
        {
            FrontendPerson person = new() { Age = 80, Name = "John Doe" };
            int insertedId = await personService!.AddPersonAsync(person);
            (int age, string name) = await connection.QuerySingleAsync<(int age, string name)>("Select [age], [name] from Person where personId = @insertedId", new
            {
                insertedId
            });
            Assert.Multiple(() =>
            {
                Assert.That(insertedId, Is.GreaterThan(0));
                Assert.That(age, Is.EqualTo(person.Age));
                Assert.That(name, Is.EqualTo(person.Name));
            });
        }

        [Test]
        public async Task Test_GetPersonById_ReturnsPerson()
        {
            var person = await personService.GetPersonAsync(1);
            Assert.Multiple(() =>
            {
                Assert.That(person.Name, Is.EqualTo("Test User"));
                Assert.That(person.Age, Is.EqualTo(21));
            });
        }

        [Test]
        public async Task Test_DeletePerson_DeletesPerson()
        {
            await personService.DeletePersonAsync(1);
            var found = await connection.QuerySingleOrDefaultAsync<object>("select * from Person where PersonId = 1");
            Assert.That(found, Is.Null);
        }

        [Test]
        public async Task Test_GetMultiplePeople()
        {
            var people = await personService.GetPeopleAsync();
            Assert.That(people, Has.Count.GreaterThan(0));
        }


        [TearDown]
        public async Task TearDown()
        {
            await connection.CloseAsync();
            personService = null!;
        }

    }
}