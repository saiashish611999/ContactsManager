using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Extensions;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using System.Reflection;

namespace ContactsManager.Core.Services;
public sealed class PersonsService : IPersonsService
{
    private readonly IPersonsRepository personsRepository;
    public PersonsService(
        IPersonsRepository personsRepository
        )
    {
        this.personsRepository = personsRepository;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest);

        ValidateRequest.ValidateRequestObject(personAddRequest);

        Person person = await personsRepository.AddPerson(
            new Person()
            {
                PersonName = personAddRequest.PersonName,
                EmailAddress = personAddRequest.EmailAddress,
                DateOfBirth = personAddRequest.DateOfBirth,
                Gender = personAddRequest.Gender,
                CountryId = personAddRequest.CountryId,
                Address = personAddRequest.Address,
                ReceivesNewsLetters = personAddRequest.ReceivesNewsLetters
            });

        PersonResponse resposne = person.AsPersonResposne();

        return resposne;
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        List<Person> persons = await personsRepository.GetAllPersons();

        if (persons.Count == 0)
        {
            return new List<PersonResponse>();
        }

        List<PersonResponse> response = persons.Select(person => person.AsPersonResposne()).ToList();

        return response;
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersons();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return allPersons;
        }

        PropertyInfo? prop = typeof(PersonResponse).GetProperty(searchBy);

        if (prop is null)
        {
            return allPersons;
        }

        List<PersonResponse> filteredPersons = allPersons.Where(person =>
        {
            var value = prop.GetValue(person, null);

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

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(personId);

        Person? person = await personsRepository.GetPersonByPersonId(personId);

        if (person is null)
        {
            return null;
        }

        PersonResponse response = person.AsPersonResposne();

        return response;
    }
}
