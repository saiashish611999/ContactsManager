using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.Entities;

namespace ContactsManager.Core.RepositoryContracts;

/// <summary>
/// contract to be implemented by CountriesRepository
/// </summary>
public interface ICountriesRepository
{
    /// <summary>
    /// method responsible to return true if the country exists
    /// </summary>
    /// <param name="countryName"></param>
    /// <returns></returns>
    Task<bool> DoesCountryExistAsync(string? countryName);

    /// <summary>
    /// method responsible to add a new country to database
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    Task<Country> AddCountryAsync(Country country);

    /// <summary>
    /// method responsible to return all countries from database
    /// </summary>
    /// <returns></returns>
    Task<List<Country>> GetAllCountriesAsync();

    /// <summary>
    /// method responsible to return a country by its id
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    Task<Country?> GetCountryByCountryIdAsync(Guid? countryId);
}
