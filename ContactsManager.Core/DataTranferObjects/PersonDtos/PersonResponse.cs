using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;

namespace ContactsManager.Core.DataTranferObjects.PersonDtos;
public sealed class PersonResponse
{
    public Guid PersonId { get; set; }
    public required string PersonName { get; set; }
    public required string Email { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public double? Age { get; set; }

    public PersonUpdateRequest ToUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonId = this.PersonId,
            PersonName = this.PersonName,
            Email = this.Email,
            Gender = (Gender) Enum.Parse(typeof(Gender), this.Gender!),
            DateOfBirth = this.DateOfBirth,
            Address = this.Address,
            CountryId = this.CountryId,
        };
    }
}

public static class PersonExtensions
{
    public static PersonResponse ToResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            Gender = person.Gender,
            DateOfBirth = person.DateOfBirth,
            Address = person.Address,
            CountryId = person.CountryId,
            CountryName = person.Country?.CountryName,
            Age = (person.DateOfBirth is not null) ?
            Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) :
            null
        };
    }
}
