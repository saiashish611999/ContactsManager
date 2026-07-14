using ContactsManager.Core.Entities;

namespace ContactsManager.Core.RepositoryContracts;

/// <summary>
///  contract to be implemented by PersonsRepository
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    /// method responsible to add person to database
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<Person> AddPersonAsync(Person person);

    /// <summary>
    /// method responsibel to get all persons from database
    /// </summary>
    /// <returns></returns>
    Task<List<Person>> GetAllPersonsAsync();

    /// <summary>
    /// method responsible to get a person by person id
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<Person?> GetPersonByPersonIdAsync(Guid? personId);

    /// <summary>
    /// method responsible to delete the person from database
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<bool> DeletePersonAsync(Guid? personId);

    /// <summary>
    /// method responsible to get the person with id and country and tracking
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<Person?> GetPersonByPersonIdWithTrackingAsync(Guid? personId);

    /// <summary>
    /// method responsible to save changes
    /// </summary>
    /// <returns></returns>
    Task SaveChangesAsync();
}
