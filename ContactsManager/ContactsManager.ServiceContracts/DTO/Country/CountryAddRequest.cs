namespace ContactsManager.ServiceContracts.DTO.Country;

using ContactsManager.Entities;

/// <summary>
/// This class represents the request model for adding a new country.
/// </summary>
public class CountryAddRequest
{
    public string? CountryName { get; set; }

    /// <summary>
    /// This method converts the request model to the domain model.
    /// </summary>
    /// <returns> returns the object of the Country domain class</returns>
    public Country ConvertToCountry()
    {
        return new Country()
        {
            CountryName = CountryName
        };
    }
}
