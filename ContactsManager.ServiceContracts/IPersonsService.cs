using ContactsManager.ServiceContracts.DTO;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.ServiceContracts;
public interface IPersonsService
{
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);
    List<PersonResponse> GetAllPersons();
    PersonResponse? GetPersonByPersonId(string? personId);
    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
    List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder);
    PersonResponse? UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    bool DeletePerson(string? personId);
}
