using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using System.Reflection;

namespace ContactsManager.Core.Services;
public sealed class PersonsService : IPersonsService
{
    private readonly IPersonsRepository personsRepository;

    public PersonsService(IPersonsRepository personsRepository)
    {
        this.personsRepository = personsRepository;
    }

    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest);

        ValidationHelper.ValidateRequest(personAddRequest);

        Person personToBeAdded = await personsRepository.AddPersonAsync(personAddRequest.ToEntity());

        PersonResponse personResponse = personToBeAdded.ToResponse();

        return personResponse;
    }

    public async Task<bool> DeletePersonAsync(Guid? personID)
    {
        ArgumentNullException.ThrowIfNull(personID);

        Person? person = await personsRepository.GetPersonByPersonIdAsync(personID);

        if (person is null)
        {
            return false;
        }

        bool isDeleted = await personsRepository.DeletePersonAsync(personID);

        return isDeleted;
    }

    public async Task<List<PersonResponse>> GetAllPersonsAsync()
    {
        List<Person> persons = await personsRepository.GetAllPersonsAsync();

        List<PersonResponse> repsonse = persons.Select(p => p.ToResponse()).ToList();

        return repsonse;
    }

    public async Task<List<PersonResponse>> GetFilteredPersonsAsync(string? searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersonsAsync();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return allPersons;
        }

        PropertyInfo? propInfo = typeof(PersonResponse).GetProperty(searchBy);

        if (propInfo is null)
        {
            return allPersons;
        }

        List<PersonResponse> filteredPersons = allPersons.Where(person =>
        {
            var value = propInfo.GetValue(person, null);

            if (value is null)
            {
                return false;
            }

            if (searchBy == nameof(PersonResponse.Gender))
            {
                return value.ToString()!.Equals(searchString, StringComparison.OrdinalIgnoreCase);
            }

            return value.ToString()!.Contains(searchString, StringComparison.OrdinalIgnoreCase);

        }).ToList();

        return filteredPersons;
    }

    public async Task<PersonResponse?> GetPersonByPersonIdAsync(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(personId);

        Person? person = await personsRepository.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return null;
        }

        PersonResponse resposne = person.ToResponse();

        return resposne;
    }

    public Task<List<PersonResponse>> GetSortedPersonsAsync(List<PersonResponse> allPersons, string? sortBy, SortOrder sortOrder)
    {
        throw new NotImplementedException();
    }

    public Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest? personUpdateRequest)
    {
        throw new NotImplementedException();
    }
}
