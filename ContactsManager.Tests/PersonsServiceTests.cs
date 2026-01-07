using ContactsManager.Entities;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTO;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;
using Xunit.Abstractions;

namespace ContactsManager.Tests;
public sealed class PersonsServiceTests
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    public PersonsServiceTests(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService(false);
        _personsService = new PersonsService(false, _countriesService);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPersonTests
    // when null PersonAddRequest is provided, then ArgumentNullException is thrown
    [Fact]
    public void AddPerson_NullPersonAddRequest()
    {
        PersonAddRequest? personAddRequest = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personsService.AddPerson(personAddRequest);
        });
    }
    // when null PersonName is provided, then ArgumentException is thrown
    [Fact]
    public void AddPerson_NullPersonName()
    {
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = null
        };
        Assert.Throws<ArgumentException>(() =>
        {
            _personsService.AddPerson(personAddRequest);
        });
    }
    // when valid PersonAddRequest is provided, then PersonResponse is returned with correct values
    [Fact]
    public void AddPerson_ValidPersonDetails()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);  
        PersonAddRequest? personAddRequest =  new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        Assert.NotNull(personResponse);
        Assert.Equal(personAddRequest.PersonName, personResponse.PersonName);
    }
    #endregion

    #region GetAllPersonsTests
    // when no persons return, return empty
    [Fact]
    public void GetAllPersons_NoPersonExists()
    {
        List<PersonResponse> persons = _personsService.GetAllPersons();
        Assert.Empty(persons);
    }
    // when persons exists, return list of persons
    [Fact]
    public void GetAllPersons_PersonsExists()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? firstRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse firstResponse = _personsService.AddPerson(firstRequest);
        PersonAddRequest? secondRequest = new PersonAddRequest()
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse secondResposne = _personsService.AddPerson(secondRequest);

        List<PersonResponse> persons = _personsService.GetAllPersons();
        Assert.Contains(firstResponse, persons);
        Assert.Contains(secondResposne, persons);
    }
    #endregion

    #region GetPersonByPersonId
    // when null personId, throw ArgumentNullException
    [Fact]
    public void GetPersonByPersonId_NullPersonId()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personsService.GetPersonByPersonId(null);
        });
    }
    // when invalid personId, return null
    [Fact]
    public void GetPersonByPersonId_InvalidPersonId()
    {
        PersonResponse? person = _personsService.GetPersonByPersonId("Invalid PersonId");
        Assert.Null(person);
    }
    // when valid personId, return exact matched object
    [Fact]
    public void GetPersonByPersonId_ValidPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };

        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        _testOutputHelper.WriteLine(personResponse.ToString());
        PersonResponse? person = _personsService.GetPersonByPersonId(personResponse.PersonId);
        _testOutputHelper.WriteLine(person?.ToString());
        Assert.NotNull(person);
        Assert.Equal(personResponse.PersonId, person.PersonId);

    }
    #endregion

    #region GetFilteredPersonsTests
    // when searchBy is PersonName and searchString is empty, return all persons
    [Fact]
    public void GetFilteredPersons_NullSearchString()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? firstRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse firstResponse = _personsService.AddPerson(firstRequest);
        PersonAddRequest? secondRequest = new PersonAddRequest()
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse secondResposne = _personsService.AddPerson(secondRequest);

        List<PersonResponse> filteredPersons = _personsService.GetFilteredPersons(searchBy: nameof(PersonResponse.PersonName), "");
        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        foreach (var person in filteredPersons)
        {
            Assert.Contains(person, filteredPersons);
        }
    }
    [Theory]
    [InlineData("", 2)]        // empty search → return all
    [InlineData("pra", 1)]     // filtered search → 1 result
    public void GetFilteredPersons_ByPersonName(
    string searchString,
    int expectedCount)
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest
        {
            CountryName = "some"
        };
        CountryResponse country =
            _countriesService.AddCountry(countryAddRequest);

        _personsService.AddPerson(new PersonAddRequest
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        });

        _personsService.AddPerson(new PersonAddRequest
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        });

        // Act
        List<PersonResponse> filteredPersons =
            _personsService.GetFilteredPersons(
                searchBy: nameof(PersonResponse.PersonName),
                searchString);

        // Assert
        Assert.Equal(expectedCount, filteredPersons.Count);

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            Assert.All(filteredPersons, person =>
                Assert.Contains(
                    searchString,
                    person.PersonName!,
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    // when searchBy is PersonName and searchString is some, return filtered persons
    [Fact]
    public void GetFilteredPersons_SearchString()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? firstRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse firstResponse = _personsService.AddPerson(firstRequest);
        PersonAddRequest? secondRequest = new PersonAddRequest()
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse secondResposne = _personsService.AddPerson(secondRequest);

        List<PersonResponse> filteredPersons = _personsService.GetFilteredPersons(searchBy: nameof(PersonResponse.PersonName), "pra");
        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        allPersons = allPersons.Where(person => person.PersonName!.Contains("pra", StringComparison.OrdinalIgnoreCase)).ToList();
        foreach (var person in filteredPersons)
        {
            Assert.Contains(person, filteredPersons);
        }
    }
    #endregion

    #region GetSortedPersonsTests
    // when sortBy is PersonName and SortOrder is Ascendting, return list of persons sorted ascendion
    [Theory]
    [InlineData(SortOrderEnum.Asc)]
    [InlineData(SortOrderEnum.Desc)]
    public void GetSortedPersons_AscendingOrder(SortOrderEnum sortOrder)
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? firstRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse firstResponse = _personsService.AddPerson(firstRequest);
        PersonAddRequest? secondRequest = new PersonAddRequest()
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse secondResposne = _personsService.AddPerson(secondRequest);
        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(allPersons, sortBy: nameof(PersonResponse.PersonName), sortOrder);
        _testOutputHelper.WriteLine("sorted persons");
        foreach (var person in sortedPersons)
        {
            _testOutputHelper.WriteLine(person.ToString()); 
        }
        if (sortOrder == SortOrderEnum.Asc)
        {
            allPersons = allPersons.OrderBy(person => person.PersonName).ToList();
        }
        else
        {
            allPersons = allPersons.OrderByDescending(person => person.PersonName).ToList();
        }
            _testOutputHelper.WriteLine("sorted all persons");
        foreach (var person in allPersons)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }
        for (int index = 0; index < allPersons.Count; index++)
        {
            Assert.Equal(allPersons[index], sortedPersons[index]);
        }
    }
    // when sortBy is PersonName and SortOrder is Descending, return list of persons sorted descending
    [Fact]
    public void GetSortedPersons_DescendingOrder()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? firstRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse firstResponse = _personsService.AddPerson(firstRequest);
        PersonAddRequest? secondRequest = new PersonAddRequest()
        {
            PersonName = "praveen",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse secondResposne = _personsService.AddPerson(secondRequest);
        List<PersonResponse> allPersons = _personsService.GetAllPersons();
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(allPersons, sortBy: nameof(PersonResponse.PersonName), SortOrderEnum.Desc);
        allPersons = allPersons.OrderByDescending(person => person.PersonName).ToList();
        for (int index = 0; index < allPersons.Count; index++)
        {
            Assert.Equal(allPersons[index], sortedPersons[index]);
        }
    }
    #endregion

    #region UpdatePersonTests
    // when person update request is null, throw ArgumentNullException
    [Fact]
    public void UpdatePerson_NullPersonUpdateRequest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personsService.UpdatePerson(null);
        });

    }
    // when person name is null throw ArgumentException
    [Fact]
    public void UpdatePerson_NullPersonNameAndEmail()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _personsService.UpdatePerson(new PersonUpdateRequest() { PersonName = null, Email = null });
        });
    }
    // when valid person details return the updated person
    [Fact]
    public void UpdatePerson_ValidPersoonDetails()
    {

        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse responseFromAdd = _personsService.AddPerson(personAddRequest);
        PersonUpdateRequest updateRequest = responseFromAdd.ConvetToPersonUpdateRequest();
        updateRequest.PersonName = "something something";
        PersonResponse? responseFromUpdate = _personsService.UpdatePerson(updateRequest);
        PersonResponse? responseFromGet = _personsService.GetPersonByPersonId(responseFromUpdate!.PersonId);
        Assert.Equal(responseFromGet!.PersonName, responseFromUpdate.PersonName);
    }
    #endregion

    #region DeletePersonTests
    // when null personId, throw ArgumentNullException
    [Fact]
    public void DeletePerson_NullPersonId()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personsService.DeletePerson(null);
        });
    }
    // when Invalid personId, return false
    [Fact]
    public void DeletePerson_InvalidPersonId()
    {
        bool isDeleted = _personsService.DeletePerson("Invalid PersonID");
        Assert.False(isDeleted);
    }
    // when valid personId, return true
    [Fact]
    public void DeletePerson_ValidPersonId()
    {
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        { CountryName = "some" };
        CountryResponse country = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            PersonName = "sai ashish",
            Email = "something",
            Gender = ServiceContracts.Enums.GenderOptions.Male,
            DateOfBirth = new DateTime(1999, 01, 06),
            Address = "some address",
            CountryId = country.CountryId,
            ReceiveNewsLetters = true
        };
        PersonResponse responseFromAdd = _personsService.AddPerson(personAddRequest);
        bool isDeleted = _personsService.DeletePerson(responseFromAdd.PersonId);
        Assert.True(isDeleted);
    }
    #endregion
}
