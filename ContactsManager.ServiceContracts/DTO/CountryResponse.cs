using ContactsManager.Entities;

namespace ContactsManager.ServiceContracts.DTO;

/// <summary>
/// DTO class used to return country information
/// </summary>
public sealed class CountryResponse
{
    public string? CountryId { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CountryResponse response &&
               CountryId == response.CountryId &&
               CountryName == response.CountryName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CountryId, CountryName);
    }

    public override string ToString()
    {
        return $"CountryId:{this.CountryId}, CountryName:{this.CountryName}";
    }
}

/// <summary>
/// An extension class to convert Country entity to CountryResponse DTO
/// </summary>
public static class CountryExtensions
{
    public static CountryResponse ConvertToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}