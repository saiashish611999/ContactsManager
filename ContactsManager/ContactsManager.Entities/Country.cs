namespace ContactsManager.Entities;

/// <summary>
/// This class represents the domain model for storing the country details.
/// </summary>
public class Country
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }
}
