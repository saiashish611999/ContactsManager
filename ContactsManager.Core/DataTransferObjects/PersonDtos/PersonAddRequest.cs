using ContactsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTransferObjects.PersonDtos;
public sealed class PersonAddRequest
{
    [Required]
    [StringLength(maximumLength:50, MinimumLength =3)]
    public string? PersonName { get; set; }

    [Required]
    [EmailAddress]
    public string? EmailAddress { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceivesNewsLetters { get; set; }
}
