namespace ServiceDefinitions
{
    public interface IPersonService
    {
        /// <summary>
        /// Gets all people from the database
        /// </summary>
        /// <returns>A collection of Person</returns>
        Task<ICollection<FrontendPerson>> GetPeopleAsync();
        /// <summary>
        /// Gets a single person from the database
        /// </summary>
        /// <param name="personId">The ID of the requested person</param>
        /// <returns>A single person</returns>
        Task<FrontendPerson> GetPersonAsync(int personId);
        /// <summary>
        /// Inserts a person into the database
        /// </summary>
        /// <param name="person">The person to be inserted, ID will be ignored.</param>
        /// <returns>The ID of the inserted person.</returns>
        Task<int> AddPersonAsync(FrontendPerson person);
        /// <summary>
        /// Deletes a person from the database
        /// </summary>
        /// <param name="personId">The ID of the person to be deleted</param>
        Task DeletePersonAsync(int personId);
    }

    public class FrontendPerson
    {
        public int PersonId { get; set; }
        public required string Name { get; set; }
        public required int Age { get; set; }
    }

}
