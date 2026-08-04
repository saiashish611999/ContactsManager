using ContactsManager.Core.DataTransferObjects.CountryDtos;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// contract to be implemented by CountriesService
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// method responsible to add country to database
    /// </summary>
    /// <param name="countryAddRequest"></param>
    /// <returns></returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// method responsible to get all countries from database
    /// </summary>
    /// <returns></returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// method resonsible to get country by country id
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);
}
