namespace ContactsManager.Core.Entities;
public sealed class Person
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
}
