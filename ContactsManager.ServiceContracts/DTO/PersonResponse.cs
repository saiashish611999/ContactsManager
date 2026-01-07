using ContactsManager.Entities;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.ServiceContracts.DTO;
public sealed class PersonResponse
{
    public string? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
    public string? CountryName { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is PersonResponse response &&
               PersonId == response.PersonId &&
               PersonName == response.PersonName &&
               CountryId == response.CountryId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PersonId, PersonName, CountryId);
    }

    public override string ToString()
    {
        return $"PersonId:{this.PersonId}, PersonName:{this.PersonName}";
    }

    public PersonUpdateRequest ConvetToPersonUpdateRequest()
    {
        return new PersonUpdateRequest()
        {
            PersonId = PersonId,
            PersonName = PersonName,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender!),
            CountryId = CountryId,
            Address = Address,
            ReceiveNewsLetters = ReceiveNewsLetters
        };
    }
}

public static class PersonExtensions
{
    public static PersonResponse ConvertToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryId = person.CountryId,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters,
            Age = person.DateOfBirth.HasValue ? (DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25 : null
        };
    }


}
