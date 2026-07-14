using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;

namespace ContactsManager.Core.Services;
public sealed class CountriesService: ICountriesService
{
    private readonly ICountriesRepository countriesRepository;

    public CountriesService(ICountriesRepository countriesRepository)
    {
        this.countriesRepository = countriesRepository;
    }

    public async Task<CountryResponse> AddCountryAsync(CountryAddRequest? countryAddRequest)
    {
        ArgumentNullException.ThrowIfNull(countryAddRequest);

        ValidationHelper.ValidateRequest(countryAddRequest);

        bool doesCountryExist = await countriesRepository.DoesCountryExistAsync(countryAddRequest.CountryName);

        if (doesCountryExist)
        {
            throw new ArgumentException("country already exists!!!");
        }

        Country country = countryAddRequest.ToEntity();

        Country countryAdded = await countriesRepository.AddCountryAsync(country);

        CountryResponse response = countryAdded.ToCountryResponse();

        return response;
    }

    public async Task<List<CountryResponse>> GetAllCountriesAsync()
    {
        List<Country> countries = await countriesRepository.GetAllCountriesAsync();

        List<CountryResponse> response = countries.Select(c => c.ToCountryResponse()).ToList();

        return response;
    }

    public async Task<CountryResponse?> GetCountryByCountryIdAsync(Guid? countryId)
    {
        ArgumentNullException.ThrowIfNull(countryId);

        Country? country = await countriesRepository.GetCountryByCountryIdAsync(countryId);

        if (country is null)
        {
            return null;
        }

        CountryResponse response = country.ToCountryResponse();

        return response;
    }
}
