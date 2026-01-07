using ContactsManager.Entities;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTO;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services.Helper;

namespace ContactsManager.Services;
public sealed class PersonsService : IPersonsService
{
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;

    public PersonsService(ICountriesService countriesService, 
                          bool initialize = true
                          )
    {
        _persons = new List<Person>();
        _countriesService = countriesService;

        #region MockData
        if (initialize)
        {
            _persons.AddRange(new List<Person>
        {
            new Person
            {
                PersonId = "person_1a2b3c4d-1111-4aaa-9bbb-000000000001",
                PersonName = "Sai Ashish",
                Email = "sai.ashish@gmail.com",
                DateOfBirth = new DateTime(1999, 01, 06),
                Gender = "Male",
                CountryId = "country_7f1a3c2e-4b6a-4b5f-9a1d-3c8f1c6c9e12", // India
                Address = "Hyderabad, India",
                ReceiveNewsLetters = true
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-2222-4aaa-9bbb-000000000002",
                PersonName = "Praveen Kumar",
                Email = "praveen@gmail.com",
                DateOfBirth = new DateTime(1998, 05, 12),
                Gender = "Male",
                CountryId = "country_7f1a3c2e-4b6a-4b5f-9a1d-3c8f1c6c9e12",
                Address = "Bangalore, India",
                ReceiveNewsLetters = false
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-3333-4aaa-9bbb-000000000003",
                PersonName = "John Smith",
                Email = "john.smith@usa.com",
                DateOfBirth = new DateTime(1990, 03, 20),
                Gender = "Male",
                CountryId = "country_2d9c8b41-6f3e-4b7e-b9d2-1a8e9c5f4a33", // USA
                Address = "New York, USA",
                ReceiveNewsLetters = true
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-4444-4aaa-9bbb-000000000004",
                PersonName = "Emily Johnson",
                Email = "emily@usa.com",
                DateOfBirth = new DateTime(1995, 07, 18),
                Gender = "Female",
                CountryId = "country_2d9c8b41-6f3e-4b7e-b9d2-1a8e9c5f4a33",
                Address = "California, USA",
                ReceiveNewsLetters = false
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-5555-4aaa-9bbb-000000000005",
                PersonName = "Oliver Brown",
                Email = "oliver@uk.com",
                DateOfBirth = new DateTime(1988, 11, 05),
                Gender = "Male",
                CountryId = "country_91a4d7f8-3c2b-4e9a-8d1f-6b5c4a2e7f11", // UK
                Address = "London, UK",
                ReceiveNewsLetters = true
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-6666-4aaa-9bbb-000000000006",
                PersonName = "Sophia Williams",
                Email = "sophia@uk.com",
                DateOfBirth = new DateTime(1992, 09, 10),
                Gender = "Female",
                CountryId = "country_91a4d7f8-3c2b-4e9a-8d1f-6b5c4a2e7f11",
                Address = "Manchester, UK",
                ReceiveNewsLetters = false
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-7777-4aaa-9bbb-000000000007",
                PersonName = "Liam Wilson",
                Email = "liam@canada.com",
                DateOfBirth = new DateTime(1991, 02, 25),
                Gender = "Male",
                CountryId = "country_5b8e2d3c-1a7f-4c9e-9d2b-8a6f3e4c1b22", // Canada
                Address = "Toronto, Canada",
                ReceiveNewsLetters = true
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-8888-4aaa-9bbb-000000000008",
                PersonName = "Emma Taylor",
                Email = "emma@canada.com",
                DateOfBirth = new DateTime(1996, 06, 30),
                Gender = "Female",
                CountryId = "country_5b8e2d3c-1a7f-4c9e-9d2b-8a6f3e4c1b22",
                Address = "Vancouver, Canada",
                ReceiveNewsLetters = true
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-9999-4aaa-9bbb-000000000009",
                PersonName = "Noah Miller",
                Email = "noah@australia.com",
                DateOfBirth = new DateTime(1994, 12, 14),
                Gender = "Male",
                CountryId = "country_c3a1f9e4-8d2b-4a6c-9e7f-1b5d8c2a4e55", // Australia
                Address = "Sydney, Australia",
                ReceiveNewsLetters = false
            },
            new Person
            {
                PersonId = "person_1a2b3c4d-aaaa-4aaa-9bbb-000000000010",
                PersonName = "Mia Anderson",
                Email = "mia@germany.com",
                DateOfBirth = new DateTime(1993, 04, 08),
                Gender = "Female",
                CountryId = "country_e4b7c2a1-9f6d-4a8e-b3c5-2d1f9e8a7b66", // Germany
                Address = "Berlin, Germany",
                ReceiveNewsLetters = true
            }
        });
        }
        #endregion
    }

    private PersonResponse GetPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ConvertToPersonResponse();
        personResponse.CountryName = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest is null)
            throw new ArgumentNullException(nameof(personAddRequest));
        ValidationHelper.ValidateModel(personAddRequest);
        Person person = personAddRequest.ConvertToPersonEntity();
        person.PersonId = $"person_{Guid.NewGuid()}";
        _persons.Add(person);
       PersonResponse resposne = GetPersonResponse(person);
        return resposne;
    }

    public List<PersonResponse> GetAllPersons()
    {
        List<PersonResponse> persons = _persons.Select(person => GetPersonResponse(person)).ToList();
        return persons;
    }

    public PersonResponse? GetPersonByPersonId(string? personId)
    {
        if (personId is null)
            throw new ArgumentNullException(nameof(personId));

        Person? person = _persons.FirstOrDefault(person => person.PersonId == personId);
        return person is not null ? GetPersonResponse(person) : null;
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = _persons.Select(person => GetPersonResponse(person)).ToList();
        if ((string.IsNullOrEmpty(searchBy) || string.IsNullOrWhiteSpace(searchBy)) && (string.IsNullOrEmpty(searchString) || string.IsNullOrWhiteSpace(searchString)))
            return allPersons;
        
        List<PersonResponse> filteredPersons = searchBy switch
        {
            nameof(PersonResponse.PersonName) => allPersons.Where(person => !string.IsNullOrEmpty(person.PersonName) && person.PersonName.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.Email) => allPersons.Where(person => !string.IsNullOrEmpty(person.Email) && person.Email.Contains(searchString!, StringComparison.Ordinal)).ToList(),
            nameof(PersonResponse.DateOfBirth) => allPersons.Where(person => !string.IsNullOrEmpty(person.DateOfBirth.ToString()) && person.DateOfBirth.ToString()!.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.Gender) => allPersons.Where(person => !string.IsNullOrEmpty(person.Gender) && person.Gender.ToString().StartsWith(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.CountryName) => allPersons.Where(person => !string.IsNullOrEmpty(person.CountryName) && person.CountryName.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.Address) => allPersons.Where(person => !string.IsNullOrEmpty(person.Address) && person.Address.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.ReceiveNewsLetters) => allPersons.Where(person => !string.IsNullOrEmpty(person.ReceiveNewsLetters.ToString()) && person.ReceiveNewsLetters.ToString()!.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            nameof(PersonResponse.Age) => allPersons.Where(person => !string.IsNullOrEmpty(person.Age.ToString()) && person.Age.ToString()!.Contains(searchString!, StringComparison.OrdinalIgnoreCase)).ToList(),
            _ => allPersons
        };

   
        return filteredPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
    {
        if (sortBy is null)
            return allPersons;

        goto ReflectionExample;
        List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.PersonName), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.PersonName).ToList(),
            (nameof(PersonResponse.PersonName), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.PersonName).ToList(),
            (nameof(PersonResponse.Email), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.Email).ToList(),
            (nameof(PersonResponse.Email), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.Email).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.DateOfBirth).ToList(),
            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),
            (nameof(PersonResponse.Gender), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.Gender).ToList(),
            (nameof(PersonResponse.Gender), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.Gender).ToList(),
            (nameof(PersonResponse.CountryName), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.CountryName).ToList(),
            (nameof(PersonResponse.CountryName), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.CountryName).ToList(),
            (nameof(PersonResponse.Address), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.Address).ToList(),
            (nameof(PersonResponse.Address), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.Address).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.ReceiveNewsLetters), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),
            (nameof(PersonResponse.Age), SortOrderEnum.Asc) => allPersons.OrderBy(person => person.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderEnum.Desc) => allPersons.OrderByDescending(person => person.Age).ToList(),
            _ => allPersons
        };

    ReflectionExample:
        var property = typeof(PersonResponse).GetProperty(sortBy);
        if (property is null)
            return allPersons;
        return sortOrder switch
        {
            SortOrderEnum.Asc => allPersons.OrderBy(person => property.GetValue(person, null)).ToList(),
            SortOrderEnum.Desc => allPersons.OrderByDescending(person => property.GetValue(person, null)).ToList(),
            _ => allPersons
        };
    }

    public PersonResponse? UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest is null)
            throw new ArgumentNullException(nameof(personUpdateRequest));
        ValidationHelper.ValidateModel(personUpdateRequest);

        Person? person = _persons.FirstOrDefault(person => person.PersonId == personUpdateRequest.PersonId);
        if (person is null)
            return null;
        person.PersonName = personUpdateRequest.PersonName;
        person.Email = personUpdateRequest.Email;
        person.Gender = personUpdateRequest.Gender.ToString();
        person.DateOfBirth = personUpdateRequest.DateOfBirth;
        person.Address = personUpdateRequest.Address;
        person.CountryId = personUpdateRequest.CountryId;
        person.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

        return GetPersonResponse(person);
    }

    public bool DeletePerson(string? personId)
    {
        if (personId is null)
            throw new ArgumentNullException(nameof(personId));
        Person? person = _persons.FirstOrDefault(person => person.PersonId == personId);
        return person is not null && _persons.Remove(person);
    }
}
