using ContactsManager.Entities;
using ContactsManager.ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.ServiceContracts.DTO;
public sealed class PersonAddRequest
{
    [Required(ErrorMessage = "PersonName is a required field")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email is a required field")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Date)]
    [Required]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public GenderOptions Gender { get; set; }

    [Required]
    public string? CountryId { get; set; }

    [Required]
    public string? Address { get; set; }

    [Required]
    public bool ReceiveNewsLetters { get; set; }

    public Person ConvertToPersonEntity()
    {
        return new Person()
        {
            PersonName = this.PersonName,
            Email = this.Email,
            DateOfBirth = this.DateOfBirth,
            Gender = this.Gender.ToString(),
            CountryId = this.CountryId,
            Address = this.Address,
            ReceiveNewsLetters = this.ReceiveNewsLetters
        };
    }
}
