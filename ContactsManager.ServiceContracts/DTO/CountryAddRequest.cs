using ContactsManager.Entities;

namespace ContactsManager.ServiceContracts.DTO;

/// <summary>
/// DTO class used to add a new country
/// </summary>
public sealed class CountryAddRequest
{
    public string? CountryName { get; set; }

    public Country ConvertToCountryEntity()
    {
        return new Country()
        {
            CountryName = this.CountryName
        };
    }

}
