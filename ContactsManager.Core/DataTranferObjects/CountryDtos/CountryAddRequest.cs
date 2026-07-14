using ContactsManager.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTranferObjects.CountryDtos;
public sealed class CountryAddRequest
{
    [Required]
    [StringLength(maximumLength: 25, MinimumLength = 3)]
    public string? CountryName { get; set;}

    public Country ToEntity()
    {
        return new Country()
        {
            CountryName = this.CountryName
        };
    }
}
