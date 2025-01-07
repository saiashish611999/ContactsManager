using ContactsManager.Entities.Enums;
using ContactsManager.Entities;
using System.ComponentModel.DataAnnotations;

namespace ContactsManager.ServiceContracts.DTO.Person;

/// <summary>
/// DTO class for adding person
/// </summary>
public class PersonAddRequest
{
    [Required]
    public string? PersonName { get; set; }

    [Required]
    public string? Email { get; set; }

    [Required]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public Gender? Gender { get; set; }

    [Required]
    public Guid? CountryId { get; set; }

    [Required]
    public string? Address { get; set; }
    public bool RecieveNewLetters { get; set; }

    /// <summary>
    /// method to convert to person entity
    /// </summary>
    /// <returns>returns the response as person entity</returns>
    public Persons ConvertToPerosn()
    {
        return new Persons()
        {
            PersonName = this.PersonName,
            Email = this.Email,
            DateOfBirth = this.DateOfBirth,
            Gender = this.Gender.ToString(),
            CountryId = this.CountryId,
            Address = this.Address,
            RecieveNewLetters = this.RecieveNewLetters
        };
    }
}
