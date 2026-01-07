using ContactsManager.Entities;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTO;

namespace ContactsManager.Services;
public sealed class CountriesService : ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService(bool initialize = true)
    {
        _countries = new List<Country>();

        #region MockData
        if (initialize)
        {
            _countries.AddRange(new List<Country>
        {
            new Country
            {
                CountryId = "country_7f1a3c2e-4b6a-4b5f-9a1d-3c8f1c6c9e12",
                CountryName = "India"
            },
            new Country
            {
                CountryId = "country_2d9c8b41-6f3e-4b7e-b9d2-1a8e9c5f4a33",
                CountryName = "United States"
            },
            new Country
            {
                CountryId = "country_91a4d7f8-3c2b-4e9a-8d1f-6b5c4a2e7f11",
                CountryName = "United Kingdom"
            },
            new Country
            {
                CountryId = "country_5b8e2d3c-1a7f-4c9e-9d2b-8a6f3e4c1b22",
                CountryName = "Canada"
            },
            new Country
            {
                CountryId = "country_c3a1f9e4-8d2b-4a6c-9e7f-1b5d8c2a4e55",
                CountryName = "Australia"
            },
            new Country
            {
                CountryId = "country_e4b7c2a1-9f6d-4a8e-b3c5-2d1f9e8a7b66",
                CountryName = "Germany"
            }
        });
        }
        #endregion
    }


    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        if (countryAddRequest is null)
            throw new ArgumentNullException(nameof(countryAddRequest));
        if (countryAddRequest.CountryName is null)
            throw new ArgumentException("CountryName should not be null");
        if (_countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0)
            throw new ArgumentException("Country already exists");

        Country country = countryAddRequest.ConvertToCountryEntity();
        country.CountryId = $"country_{Guid.NewGuid()}";
        _countries.Add(country);
        CountryResponse countryResponse = country.ConvertToCountryResponse();
        return countryResponse;
    }

    public List<CountryResponse> GetAllCountries()
    {
        List<CountryResponse> countries = _countries.Select(country => country.ConvertToCountryResponse()).ToList();
        return countries;
    }

    public CountryResponse? GetCountryByCountryId(string? countryId)
    {
        if(countryId is null)
            throw new ArgumentNullException(nameof(countryId));
        Country? country = _countries.FirstOrDefault(country => country.CountryId == countryId);
        return country is not null ? country.ConvertToCountryResponse() : null;

    }
}
