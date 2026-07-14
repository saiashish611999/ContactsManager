using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// contract to be implemented by PersonsService
/// </summary>
public interface IPersonsService
{
    /// <summary>
    /// method responsible to add person
    /// </summary>
    /// <param name="personAddRequest"></param>
    /// <returns></returns>
    Task<PersonResponse> AddPersonAsync(PersonAddRequest? personAddRequest);

    /// <summary>
    /// method responsible to get all persons
    /// </summary>
    /// <returns></returns>
    Task<List<PersonResponse>> GetAllPersonsAsync();

    /// <summary>
    /// method responsible to get person by personId
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    Task<PersonResponse?> GetPersonByPersonIdAsync(Guid? personId);

    /// <summary>
    /// method responsible to get filtered persons based on searchBy and searchString
    /// </summary>
    /// <param name="searchBy"></param>
    /// <param name="searchString"></param>
    /// <returns></returns>
    Task<List<PersonResponse>> GetFilteredPersonsAsync(string? searchBy, string? searchString);

    /// <summary>
    /// method responsible to get sorted persons based on sortBy and sortOrder
    /// </summary>
    /// <param name="allPersons"></param>
    /// <param name="sortBy"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
    List<PersonResponse> GetSortedPersonsAsync(List<PersonResponse> allPersons, string? sortBy, SortOrder sortOrder);

    /// <summary>
    /// method responsible to update person
    /// </summary>
    /// <param name="personUpdateRequest"></param>
    /// <returns></returns>
    Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    /// method responsible to delete person by personID
    /// </summary>
    /// <param name="personID"></param>
    /// <returns></returns>
    Task<bool> DeletePersonAsync(Guid? personID);

    /// <summary>
    /// method responsible to get the persons csv
    /// </summary>
    /// <returns></returns>
    Task<MemoryStream> GetPersonsCsv();

    /// <summary>
    /// method responsible to get the persons csv advanced
    /// </summary>
    /// <returns></returns>
    Task<MemoryStream> GetPersonsCsvAdvanced();

    /// <summary>
    /// method responsible to get the persons excel
    /// </summary>
    /// <returns></returns>
    Task<MemoryStream> GetPersonsExcel();
}
