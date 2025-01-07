using ContactsManager.ServiceContracts.DTO.Country;

namespace ContactsManager.ServiceContracts.ServiceContracts;

/// <summary>
/// Service contract for the countries service.
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// This method is responsible for adding a new country.
    /// </summary>
    /// <param name="countryAddRequest">represents the country object to be added</param>
    /// <returns> returns the same country as country response</returns>
    CountryResponse? AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// This method is responsible for getting all the countries.
    /// </summary>
    /// <returns> returns the list of all countries in database</returns>
    List<CountryResponse>? GetAllCountries();

    /// <summary>
    /// This method is responsible for getting the country by its id.
    /// </summary>
    /// <param name="countryId"> requires the id of the country</param>
    /// <returns> returns the country response object</returns>
    CountryResponse? GetCountryById(Guid? countryId);
}
