using ContactsManager.ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.ServiceContracts.DTO;
public sealed class PersonUpdateRequest
{
    public string? PersonId { get; set; }

    [Required(ErrorMessage = "PersonName is a required field")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email is a required field")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions Gender { get; set; }
    public string? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}
