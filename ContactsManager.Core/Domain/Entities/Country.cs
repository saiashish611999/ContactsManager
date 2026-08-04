namespace ContactsManager.Core.Domain.Entities;
public sealed class Country
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
    public ICollection<Person> Persons { get; set; } = [];
}
