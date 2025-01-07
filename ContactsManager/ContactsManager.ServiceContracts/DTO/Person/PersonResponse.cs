using ContactsManager.Entities;

namespace ContactsManager.ServiceContracts.DTO.Person;

public class PersonResponse
{
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool RecieveNewLetters { get; set; }
    public string? Country { get; set; }
    public double? Age { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(PersonResponse))
        {
            return false;
        }    
        PersonResponse person = (PersonResponse)obj;
        return PersonId == person.PersonId &&
               Email == person.Email &&
               DateOfBirth == person.DateOfBirth &&
               Gender == person.Gender &&
               CountryId == person.CountryId &&
               Address == person.Address &&
               RecieveNewLetters == person.RecieveNewLetters &&
               Country == person.Country &&
               Age == person.Age &&
               PersonName == person.PersonName;

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class PersonExtensions
{
    public static PersonResponse ConvertToPersonResponse(this Persons person)
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
            RecieveNewLetters = person.RecieveNewLetters,
            Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365) : null,

        };

    }
}
