using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTranferObjects.PersonDtos;
public sealed class PersonUpdateRequest
{
    public Guid PersonId { get; set; }

    [Required]
    [StringLength(maximumLength: 25, MinimumLength = 3)]
    public required string PersonName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    public Gender Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
    
}
