using ContactsManager.Core.Entities;

namespace ContactsManager.Core.DataTranferObjects.CountryDtos;
public sealed class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
}

public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}
