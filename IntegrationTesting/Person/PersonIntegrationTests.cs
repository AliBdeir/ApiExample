using Microsoft.Data.SqlClient;
using RestSharp;
using Dapper;
using Configuration;
using ServiceDefinitions;

namespace IntegrationTesting.Person
{
    [TestFixture]
    public class PersonIntegrationTests
    {
        private RestClient _client;
        private const string BaseUrl = "http://localhost:7151/api/Testing";

        [SetUp]
        public async Task Setup()
        {
            _client = new RestClient(BaseUrl);
            await SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await CleanDatabase();
        }

        private async Task SeedDatabase()
        {
            using var connection = new SqlConnection(ConfigurationHelper.GetConnectionString("Testing"));
            await connection.ExecuteAsync("DELETE FROM Person"); // Clean slate
            await connection.ExecuteAsync(@"
                INSERT INTO Person (Name, Age) VALUES
                ('John Doe', 30),
                ('Jane Doe', 25)");
        }

        private async Task CleanDatabase()
        {
            using var connection = new SqlConnection(ConfigurationHelper.GetConnectionString("Testing"));
            await connection.ExecuteAsync("DELETE FROM Person");
        }

        [Test]
        public async Task GetAllPeople_ShouldReturnPeopleList()
        {
            var request = new RestRequest("Person", Method.Get);
            var response = await _client.ExecuteAsync<List<FrontendPerson>>(request);

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            List<FrontendPerson>? people = response.Data;
            Assert.Multiple(() =>
            {
                Assert.That(people, Is.Not.Null);
                Assert.That(people, Has.Count.EqualTo(2));
                Assert.That(people![0].Age, Is.EqualTo(30));
                Assert.That(people[0].Name, Is.EqualTo("John Doe"));
                Assert.That(people[1].Age, Is.EqualTo(25));
                Assert.That(people[1].Name, Is.EqualTo("Jane Doe"));
            });
        }

        [Test]
        public async Task GetPerson_ShouldReturnPerson()
        {
            int testPersonId = 1;
            var request = new RestRequest($"Person/{testPersonId}", Method.Get);
            var response = await _client.ExecuteAsync<FrontendPerson>(request);
            var person = response.Data;
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(person, Is.Not.Null);
                Assert.That(person!.Name, Is.EqualTo("John Doe"));
                Assert.That(person.Age, Is.EqualTo(30));
            });
        }

        [Test]
        public async Task AddPerson_ShouldReturnNewPersonId()
        {
            var request = new RestRequest("Person", Method.Post);
            request.AddJsonBody(new FrontendPerson() { Age = 20, Name = "John"});

            var response = await _client.ExecuteAsync<IdResponse>(request);
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
                Assert.That(response.Data, Is.Not.Null);
                Assert.That(response.Data!.Id, Is.GreaterThan(0));
            });
        }

        [Test]
        public async Task DeletePerson_ShouldReturnSuccess()
        {
            int testPersonId = 1; // Assuming a test person with ID 1 exists in the seed data
            var request = new RestRequest($"Person/{testPersonId}", Method.Delete);
            var response = await _client.ExecuteAsync(request);
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }
    }

    public record IdResponse(int Id);

}