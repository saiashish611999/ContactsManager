using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTransferObjects.CountryDtos;
public sealed class CountryAddRequest
{
    [StringLength(maximumLength: 18, MinimumLength = 3)]
    [Required]
    public string? CountryName { get; set; }
}
