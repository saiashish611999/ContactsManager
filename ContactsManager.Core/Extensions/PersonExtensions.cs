using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;

namespace ContactsManager.Core.Extensions;
public static class PersonExtensions
{
    public static PersonResponse AsPersonResposne(this Person person)
    {
        return new PersonResponse()
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            EmailAddress = person.EmailAddress,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            CountryId = person.CountryId,
            Address = person.Address,
            ReceivesNewsLetters = person.ReceivesNewsLetters,
            CountryName = person.Country?.CountryName,
            Age = person.DateOfBirth is not null ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 2) : null
        };
    }
}
