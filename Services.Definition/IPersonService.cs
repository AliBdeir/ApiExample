namespace ServiceDefinitions
{
    public interface IPersonService
    {
        /// <summary>
        /// Gets all people from the database
        /// </summary>
        /// <returns>A collection of Person</returns>
        Task<ICollection<FrontendPerson>> GetPeople();
        /// <summary>
        /// Gets a single person from the database
        /// </summary>
        /// <param name="personId">The ID of the requested person</param>
        /// <returns>A single person</returns>
        Task<FrontendPerson> GetPerson(int personId);
        /// <summary>
        /// Inserts a person into the database
        /// </summary>
        /// <param name="person">The person to be inserted, ID will be ignored.</param>
        /// <returns>The ID of the inserted person.</returns>
        Task<int> AddPerson(FrontendPerson person);
        /// <summary>
        /// Deletes a person from the database
        /// </summary>
        /// <param name="personId">The ID of the person to be deleted</param>
        Task DeletePerson(int personId);
    }

    public class FrontendPerson
    {
        public int PersonId { get; set; }
        public required string Name { get; set; }
        public required int Age { get; set; }
    }

}
