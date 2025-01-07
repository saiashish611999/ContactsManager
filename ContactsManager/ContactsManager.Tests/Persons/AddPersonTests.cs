using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.DTO.Person;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services;

namespace ContactsManager.Tests.Persons;

public class AddPersonTests
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    public AddPersonTests()
    {
        _personsService = new PersonsService();
        _countriesService = new CountriesService();
    }

    // 1. When PersonAddRequest is null, then it should throw ArgumentNullException
    [Fact]
    public void AddPerson_WhenPersonAddRequestIsNull()
    {
        // Arrange
        PersonAddRequest? personAddRequest = null;

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _personsService.AddPerson(personAddRequest);
        });
    }
    // 2. When the parameters of the PersonAddRequest are null, then it should throw ArgumentException
    [Fact]
    public void AddPerson_WhenPersonAddRequestParametersAreNull()
    {
        // Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = null,
            Address = null,
            Email = null,
            CountryId = null,
            DateOfBirth = null,
            Gender = null

        };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personsService.AddPerson(personAddRequest);
        });
    }
    // 3. When the person is duplicate, then it should throw ArgumentException
    [Fact]
    public void AddPerson_WhenPersonIsDuplicate()
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
            PersonName = "John Doe",
            Address = "123, ABC Street",
            Email = "something@gmail.com",
            CountryId = responseFromAdd.CountryId,
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Entities.Enums.Gender.male,
            RecieveNewLetters = true
        };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personsService.AddPerson(personAddRequest);
            _personsService.AddPerson(personAddRequestDuplicate);
        });

    }
    // 4. When the person is added successfully, then it should return the personResponse
    [Fact]
    public void AddPerson_WhenPersonIsAddedSuccessfully()
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

        // Act
        PersonResponse? personResponse = _personsService.AddPerson(personAddRequest);

        // Assert
        Assert.Equal(personResponse.PersonName, personAddRequest.PersonName);
        Assert.NotNull(personResponse.PersonId);
    }
}
