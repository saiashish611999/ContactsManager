﻿

namespace ContactsManager.Entities;

/// <summary>
/// Domain class for Person
/// </summary>
public class Persons
{
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool RecieveNewLetters { get; set; }
}