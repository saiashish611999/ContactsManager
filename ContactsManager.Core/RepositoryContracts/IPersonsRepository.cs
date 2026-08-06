using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.RepositoryContracts;

/// <summary>
/// contract to be implmeneted by PersonsRepository
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    /// method responsible to add person to database
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// method responsible to get all persons
    /// </summary>
    /// <returns></returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    /// method responsible to get person by person id
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<Person?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// method responsible to update the person
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<Person?> GetPersonByPersonIdWithTracking(Guid? personId);
    
    /// <summary>
    /// method is responsible to save the changes to database
    /// </summary>
    /// <returns></returns>
    Task SaveChanges();

    /// <summary>
    /// method responsible to delete person
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task DeletePerson(Person person);
}
