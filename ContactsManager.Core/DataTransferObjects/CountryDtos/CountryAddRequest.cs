using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTransferObjects.CountryDtos;
public sealed class CountryAddRequest
{
    [StringLength(maximumLength: 18, MinimumLength = 3, ErrorMessage = "Country Name is not in a correct format")]
    [Required(ErrorMessage = "Country Name is a required field")]
    public string? CountryName { get; set; }
}
