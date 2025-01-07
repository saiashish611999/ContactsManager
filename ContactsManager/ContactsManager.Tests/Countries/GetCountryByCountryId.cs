using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services;

namespace ContactsManager.Tests.Countries;

public class GetCountryByCountryId
{
    private readonly ICountriesService _countriesService;
    public GetCountryByCountryId()
    {
        _countriesService = new CountriesService();
    }

    // 1. When CountryId is null or empty, then it should throw ArgumentException
    [Fact]
    public void GetCountryByCountryId_WhenCountryIdIsNullOrEmpty()
    {
        // Arrange
        Guid? countryId = Guid.Empty;

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.GetCountryById(countryId);
        });
    }
    // 2. When CountryId is not found, then it should return null
    [Fact]
    public void GetCountryByCountryId_WhenCountryIsnotFound()
    {
        // Arrange
        Guid? countryId = Guid.NewGuid();

        // Act
        CountryResponse? response = _countriesService.GetCountryById(countryId);

        // Assert
        Assert.Null(response);
    }
    // 3. When CountryId is found, then it should return the countryResponse
    [Fact]
    public void GetCountryByCountryId_WhenCountryExists()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = "India"
        };
        
        CountryResponse? responseFromAdd  = _countriesService.AddCountry(countryAddRequest);
        CountryResponse? responseFromGetId = _countriesService.GetCountryById(responseFromAdd.CountryId);

        // Assert
        Assert.Equal(responseFromAdd, responseFromGetId);
        Assert.Equal(responseFromAdd.CountryId, responseFromGetId.CountryId);
        Assert.Equal(responseFromAdd.CountryName, responseFromGetId.CountryName);

    }
}
