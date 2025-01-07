
using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services;

namespace ContactsManager.Tests.Countries;

public class GetAllCountriesTests
{
    private readonly ICountriesService _countriesService;
    public GetAllCountriesTests()
    {
        _countriesService = new CountriesService();
    }

    // 1. When there are no countries, then it should return an empty list
    [Fact]
    public void GetAllCountries_WhenNoCountries()
    {
        // Act
        List<CountryResponse>? responseFromGetAllCountriesMethod =  _countriesService.GetAllCountries();

        // Assert
        Assert.Empty(responseFromGetAllCountriesMethod);
        
    }

    [Fact]
    public void GetAllCountries_WhenCountriesExists()
    {
        // Arrange
        List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
        {
            new CountryAddRequest()
            {
                CountryName = "India"
            },
            new CountryAddRequest()
            {
                CountryName = "USA"
            }
        };

        List<CountryResponse> responseFromAdd = new List<CountryResponse>();
        foreach (var country in countryAddRequests)
        {
            CountryResponse? response = _countriesService.AddCountry(country);
            responseFromAdd.Add(response);

        }

        List<CountryResponse>? responseFromGet = new List<CountryResponse>();
        responseFromGet = _countriesService.GetAllCountries();

        // Assert
        foreach (CountryResponse expectedCountry in responseFromAdd)
        {
            Assert.Contains(expectedCountry, responseFromGet);
        }
    }
    // 2. When there are countries, then it should return the list of countries
}
