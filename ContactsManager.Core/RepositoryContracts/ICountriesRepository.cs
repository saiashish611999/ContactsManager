using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.RepositoryContracts;

/// <summary>
/// contract should be implemented by CountriesRepository
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// method responsible to check if country exists in database by country name
    /// </summary>
    /// <param name="countryName"></param>
    /// <returns></returns>
    Task<bool> IsCountryExists(string? countryName);

    /// <summary>
    /// method responsible to add country in database
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    Task<Country> AddCountry(Country country);

    /// <summary>
    /// method responsible to get all countries from database
    /// </summary>
    /// <returns></returns>
    Task<List<Country>> GetAllCountries();

    /// <summary>
    /// method responsible to get country by country id from database
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    Task<Country?> GetCountryByCountryId(Guid? countryId);

    /// <summary>
    /// method responsible to delete country from database
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    Task<bool> DeleteCountry(Guid countryId);
}
