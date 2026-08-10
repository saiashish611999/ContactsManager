using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Extensions;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;

namespace ContactsManager.Core.Services;
public sealed class CountriesService : ICountriesService
{
    private readonly ICountriesRepository countriesRepository;
    public CountriesService(ICountriesRepository countriesRepository)
    {
        this.countriesRepository = countriesRepository;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        ArgumentNullException.ThrowIfNull(countryAddRequest);

        ValidateRequest.ValidateRequestObject(countryAddRequest);

        bool isCountryExists = await countriesRepository.IsCountryExists(countryAddRequest.CountryName);

        if (isCountryExists)
        {
            throw new ArgumentException("country already exists!!!");
        }

        Country country = new Country()
        {
            CountryName = countryAddRequest.CountryName
        };

        Country addedCountry = await countriesRepository.AddCountry(country);

        CountryResponse response = addedCountry.AsCountryResponse();

        return response;
    }

    public async Task<bool> DeleteCountry(Guid countryId)
    {
        bool isDeleted = await countriesRepository.DeleteCountry(countryId);

        return isDeleted;
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        List<Country> countries = await countriesRepository.GetAllCountries();

        List<CountryResponse> response = countries.Select(c => c.AsCountryResponse()).ToList();

        return response;
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        ArgumentNullException.ThrowIfNull(countryId);

        Country? country = await countriesRepository.GetCountryByCountryId(countryId);

        if (country is null)
        {
            return null;
        }

        CountryResponse response = country.AsCountryResponse();

        return response;
    }
}
