using ContactsManager.Core.Enums;

namespace ContactsManager.Core.Domain.Entities;
public sealed class Person
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? EmailAddress { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceivesNewsLetters { get; set; }
    public Country? Country { get; set; }
}
