using ContactsManager.Core.DataTransferObjects.PersonDtos;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// contract to be implemented by PersonsService
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// method responsible to add person to database
    /// </summary>
    /// <param name="personAddRequest"></param>
    /// <returns></returns>
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// method responsible to get all persons
    /// </summary>
    /// <returns></returns>
    Task<List<PersonResponse>> GetAllPersons();

    /// <summary>
    /// method responsible to get a person by personid
    /// </summary>
    /// <returns></returns>
    Task<PersonResponse?> GetPersonByPersonId(Guid? personId);

    /// <summary>
    /// method responsible to filter the persons
    /// </summary>
    /// <param name="searchBy"></param>
    /// <param name="searchString"></param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString);
}
