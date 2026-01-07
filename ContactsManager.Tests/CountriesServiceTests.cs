using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTO;
using ContactsManager.Services;
using Xunit.Abstractions;

namespace ContactsManager.Tests;
public sealed class CountriesServiceTests
{
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;
    public CountriesServiceTests(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService(false);
        _testOutputHelper = testOutputHelper;  
    }

    #region AddCountryTests
    // when CountryAddRequest is null, then ArgumentNullException is thrown
    [Fact]
    public void AddCountry_NullCountryAddRequest()
    {
        CountryAddRequest? countryAddRequest = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _countriesService.AddCountry(countryAddRequest);
        });
    }
    // when CountryName is null or empty, then ArgumentException is thrown
    [Fact]
    public void AddCountry_NullCountryName()
    {
        CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = null };
        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(countryAddRequest);
        });
    }
    // when CountryName is duplicate, then ArgumentException is thrown
    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "India" };
        _countriesService.AddCountry(countryAddRequest);
        Assert.Throws<ArgumentException>(() =>
        {
            _countriesService.AddCountry(countryAddRequest);
        });
    }
    // when valid CountryAddRequest is provided, then CountryResponse is returned with correct values
    [Fact]
    public void AddCountry_ValidCountryAddRequest()
    {
        CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "India" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        Assert.NotNull(countryResponse.CountryId);
        Assert.Equal(countryAddRequest.CountryName, countryResponse.CountryName);
        Assert.Contains(countryResponse, countries);
    }
    #endregion

    #region GetAllCountriesTests
    // when no countries are added, then empty list is returned
    [Fact]
    public void GetAllCountries_NoCountriesExists()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        Assert.Empty(countries);
    }
    // when countries are added, then list of CountryResponse is returned with correct values
    [Fact]
    public void GetAllCountries_CountriesExists()
    {
        CountryAddRequest? firstCountryAddRequest = new CountryAddRequest() { CountryName = "India" };
        CountryAddRequest? secondCountryAddRequest = new CountryAddRequest() { CountryName = "USA" };
        CountryResponse firstCountryResponse = _countriesService.AddCountry(firstCountryAddRequest);
        CountryResponse secondCountryResponse = _countriesService.AddCountry(secondCountryAddRequest);
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        Assert.Contains(firstCountryResponse, countries);
        Assert.Contains(secondCountryResponse, countries);
    }
    #endregion

    #region GetCountryByCountryIdTests
    // if null countryId is provided, then throw ArgumentNullException
    [Fact]
    public void GetCountryByCountryId_NullCountryId()
    {
        string? countryId = null;
        Assert.Throws<ArgumentNullException>(() =>
        {
            _countriesService.GetCountryByCountryId(countryId);
        });
    }
    // if invalid countryId is provided, then return null
    [Fact]
    public void GetCountryByCountryId_InvalidCountryId()
    {
        string? countryId = "invalid country id";
        CountryResponse? country = _countriesService.GetCountryByCountryId(countryId);
        Assert.Null(country);
    }
    // if valid countryId is provided, then return CountryResponse with correct values
    [Fact]
    public void GetCountryByCountryId_ValidCountryId()
    {
        CountryAddRequest? countryAddRequest = new CountryAddRequest() { CountryName = "India" };
        CountryResponse countryFromAdd = _countriesService.AddCountry(countryAddRequest);
        _testOutputHelper.WriteLine(countryFromAdd.ToString());
        CountryResponse? countryFromGet = _countriesService.GetCountryByCountryId(countryFromAdd.CountryId!);
        _testOutputHelper.WriteLine(countryFromGet?.ToString());
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        Assert.NotNull(countryFromGet);
        Assert.Equal(countryFromAdd, countryFromGet);
        Assert.Contains(countryFromGet, countries);
    }
    #endregion
}
