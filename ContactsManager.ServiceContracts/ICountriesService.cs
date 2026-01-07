using ContactsManager.ServiceContracts.DTO;

namespace ContactsManager.ServiceContracts;

/// <summary>
/// Represents the contract for country-related operations
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the data store
    /// </summary>
    /// <param name="countryAddRequest">country object to add</param>
    /// <returns>returns the country object after adding it</returns>
    CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Get all countries from the data store
    /// </summary>
    /// <returns>returns the list of country objects as response</returns>
    List<CountryResponse> GetAllCountries();

    /// <summary>
    /// Get country by country id
    /// </summary>
    /// <param name="countryId">string value represents countryId</param>
    /// <returns>returns country object if exists or null</returns>
    CountryResponse? GetCountryByCountryId(string? countryId);
}
