using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.DataTranferObjects.PersonDtos;
public sealed class PersonAddRequest
{
    [Required]
    [StringLength(maximumLength:25, MinimumLength = 3)]
    public required string PersonName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    public Gender Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }

    public Person ToEntity()
    {
        return new Person()
        {
            PersonName = this.PersonName,
            Email = this.Email,
            Gender = this.Gender.ToString(),
            DateOfBirth = this.DateOfBirth,
            Address = this.Address,
            CountryId = this.CountryId
        };
    }
}
