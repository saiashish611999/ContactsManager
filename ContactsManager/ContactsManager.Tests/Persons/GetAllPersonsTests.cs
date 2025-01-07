using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.DTO.Person;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services;

namespace ContactsManager.Tests.Persons;

public class GetAllPersonsTests
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    public GetAllPersonsTests()
    {
        _countriesService = new CountriesService();
        _personsService = new PersonsService();
    }

    // 1. When Persons are null, return null
    [Fact]
    public void GetAllPersons_WhenNoPersons()
    {
        // Act
        List<PersonResponse>? personResponses = _personsService.GetAllPersons();

        // Assert
        Assert.Empty(personResponses);
    }

    // 2. returns list of person response object when there are persons in database
    [Fact]
    public void GetAllPersons_WhenPersonsExist()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = "India"
        };
        
        CountryResponse? responseFromAdd = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "John Doe",
            Address = "123, ABC Street",
            Email = "something@gmail.com",
            CountryId = responseFromAdd.CountryId,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Entities.Enums.Gender.male,
            RecieveNewLetters = true
        };

        PersonAddRequest? personAddRequestDuplicate = new PersonAddRequest()
        {
            PersonName = "John",
            Address = "123, ABCD Street",
            Email = "something2@gmail.com",
            CountryId = responseFromAdd.CountryId,
            DateOfBirth = new DateTime(1991, 1, 1),
            Gender = Entities.Enums.Gender.others,
            RecieveNewLetters = true
        };

        List<PersonResponse> personResponsesFromAdd = new List<PersonResponse>()
        {
            _personsService.AddPerson(personAddRequest),
            _personsService.AddPerson(personAddRequestDuplicate)
        };
        
        List<PersonResponse> personResponsesFromGet = _personsService.GetAllPersons();

        // Assert
        foreach (var expected in personResponsesFromAdd)
        {
            Assert.Contains(expected, personResponsesFromGet);
        }

    }
}
