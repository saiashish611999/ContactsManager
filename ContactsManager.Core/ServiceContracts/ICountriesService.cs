using ContactsManager.Core.DataTranferObjects.CountryDtos;
using Microsoft.AspNetCore.Http;

namespace ContactsManager.Core.ServiceContracts;

/// <summary>
/// contract to be implemented by CountriesService
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// method responsible to add the country
    /// </summary>
    /// <param name="countryAddRequest"></param>
    /// <returns></returns>
    Task<CountryResponse> AddCountryAsync(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// method responsible to get all countries
    /// </summary>
    /// <returns></returns>
    Task<List<CountryResponse>> GetAllCountriesAsync();

    /// <summary>
    /// method repsonsible to get country by countryId
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns></returns>
    Task<CountryResponse?> GetCountryByCountryIdAsync(Guid? countryId);

    /// <summary>
    /// method responsile to add countries from excel file
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    Task<int> UploadCountriesFromExcel(IFormFile formFile);
}
