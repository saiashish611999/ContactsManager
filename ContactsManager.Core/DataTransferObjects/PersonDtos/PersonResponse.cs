using ContactsManager.Core.Enums;

namespace ContactsManager.Core.DataTransferObjects.PersonDtos;
public sealed class PersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? EmailAddress { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceivesNewsLetters { get; set; }
    public string? CountryName { get; set; }
    public double? Age { get; set; }
}
