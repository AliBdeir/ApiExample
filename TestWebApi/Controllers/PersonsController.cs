using Microsoft.AspNetCore.Mvc;
using ServiceDefinitions;

namespace TestWebApi.Controllers
{
    // !! This will NOT work when deployed as it is relying on a local SQL Server database!
    [ApiController]
    [Route("Persons")]
    public class PersonsController(IPersonService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPeople()
        {
            var people = await service.GetPeopleAsync();
            return Ok(people);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await service.GetPersonAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] FrontendPerson person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int id = await service.AddPersonAsync(person);
            return CreatedAtAction(nameof(GetPerson), new { id }, new { id });
            // ^ https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/201
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            await service.DeletePersonAsync(id);
            return NoContent();
        }

        [HttpDelete("Throw")]
        public IActionResult Throw()
        {
            throw new Exception("Throwing!");
        }
    }
}
