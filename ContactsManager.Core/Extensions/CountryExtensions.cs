using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.Extensions;
public static class CountryExtensions
{
    public static CountryResponse AsCountryResponse(this Country country)
    {
        return new CountryResponse()
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}
