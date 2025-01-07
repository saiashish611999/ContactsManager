using ContactsManager.Entities;
using ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.ServiceContracts.ServiceContracts;

namespace ContactsManager.Services;

public class CountriesService : ICountriesService
{
    #region Data Storage
    // data storage
    private readonly List<Country> _countries ;
    public CountriesService()
    {
        _countries = new List<Country>();
    }
    #endregion

    #region AddCountry
    public CountryResponse? AddCountry(CountryAddRequest? countryAddRequest)
    {
        // validation if countryAddRequest is null
        if (countryAddRequest == null)
        {
            throw new ArgumentNullException(nameof(countryAddRequest));
        }

        // validation if country name is null or empty
        if (string.IsNullOrEmpty(countryAddRequest.CountryName))
        {
            throw new ArgumentException("Country name is required", nameof(countryAddRequest.CountryName));
        }

        // validation if country name is duplicate
        if (_countries.Where(country => country.CountryName == countryAddRequest.CountryName).Count() > 0)
        {
            throw new ArgumentException("Country name already exists", nameof(countryAddRequest.CountryName));
        }

        // convert countryAddRequest to Country
        Country country = countryAddRequest.ConvertToCountry();
        country.CountryId = Guid.NewGuid();
        
        // add country to the list
        _countries.Add(country);       
       
        // return the countryResponse
        return country.ConvertToCountryResponse();
        
    }


    #endregion

    #region GetAllCountries
    public List<CountryResponse>? GetAllCountries()
    {
        return _countries.Select(country => country.ConvertToCountryResponse()).ToList();
    }


    #endregion

    #region GetCountryById
    public CountryResponse? GetCountryById(Guid? countryId)
    {
        if (countryId == Guid.Empty || countryId == null)
        {
            throw new ArgumentException("Country Id is required", nameof(countryId));
        }

        Country? country = _countries.Where(country => country.CountryId == countryId).FirstOrDefault();

        return country == null? null: country.ConvertToCountryResponse();
    }
    #endregion
}
