namespace ContactsManager.Entities;
public sealed class Person
{
    public string? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? CountryId { get; set; }  
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}
