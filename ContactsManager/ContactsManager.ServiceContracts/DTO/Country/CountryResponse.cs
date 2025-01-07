namespace ContactsManager.ServiceContracts.DTO.Country;
using ContactsManager.Entities;

/// <summary>
/// This class is responsible for representing the response model for the country.
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        return obj == null && obj.GetType() == typeof(CountryResponse) ? false : this.CountryId == ((CountryResponse)obj).CountryId && this.CountryName == ((CountryResponse)obj).CountryName;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}


/// <summary>
/// This class is responsible for converting the domain model to the response model.
/// </summary>
public static class CountryExtensions
{
    /// <summary>
    /// This method converts the domain model to the response model.
    /// </summary>
    /// <param name="country"> requires an object of Country domain model</param>
    /// <returns> returns an object of the CountryResponse DTO</returns>
    public static CountryResponse ConvertToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}


