using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services;

namespace ContactsManager.Tests.Countries;

public class AddCountryTests
{
    private readonly ICountriesService _countriesService;
    public AddCountryTests()
    {
        _countriesService = new CountriesService();
    }

    // 1. When CountryAddRequest is null, then it should throw ArgumentNullException
    [Fact]
    public void AddCountry_WhenCountryAddRequestIsNull()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = null;

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            // Act
            _countriesService.AddCountry(countryAddRequest);
        });

    }
    // 2. When the CountryName is null or empty, then it should throw ArgumentException
    [Fact]
    public void AddCountry_WhenCountryNameIsNullOrEmpty()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = null
        };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _countriesService.AddCountry(countryAddRequest);
        });
    }
    // 3. When the CountryName is Duplicate, It should throw ArgumentException
    [Fact]
    public void AddCountry_WhenCountryNameIsExists()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = "India"
        };
        CountryAddRequest? countryAddRequestDuplicate = new CountryAddRequest()
        {
            CountryName = "India"
        };

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _countriesService.AddCountry(countryAddRequest);
            _countriesService.AddCountry(countryAddRequestDuplicate);
        });
    }
    // 4. When the Country is added successfully, then it should return the CountryResponse
    [Fact]
    public void AddCountry_WhenProperCountryDetailsSubmitted()
    {
        // Arrange
        CountryAddRequest? countryAddRequest = new CountryAddRequest()
        {
            CountryName = "India"
        };

        // Act
        CountryResponse? responseFromAddMethod =  _countriesService.AddCountry(countryAddRequest);
        List<CountryResponse>? countryResponses = _countriesService.GetAllCountries();

        // Assert
        Assert.NotNull(responseFromAddMethod);
        Assert.Equal(countryAddRequest.CountryName, responseFromAddMethod.CountryName);
        Assert.True(responseFromAddMethod.CountryId != Guid.Empty);
        Assert.Contains(responseFromAddMethod, countryResponses);
    }
}
