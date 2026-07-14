using ContactsManager.Core.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;
public sealed class PersonsRepository : IPersonsRepository
{
    private readonly DatabaseContext database;

    public PersonsRepository(DatabaseContext database)
    {
        this.database = database;
    }

    public async Task<Person> AddPersonAsync(Person person)
    {
        await database.Persons.AddAsync(person);

        await database.SaveChangesAsync();

        return person;
    }

    public async Task<bool> DeletePersonAsync(Guid? personId)
    {
        int rows =  await database.Persons
            .Where(person => person.PersonId == personId)
            .ExecuteDeleteAsync();

        return rows > 0;
    }

    public async Task<List<Person>> GetAllPersonsAsync()
    {
        List<Person> persons = await database.Persons
            .Include(person => person.Country)
            .AsNoTracking()
            .ToListAsync();

        return persons;
    }

    public async Task<Person?> GetPersonByPersonIdAsync(Guid? personId)
    {
        Person? person = await database.Persons
            .Include(person => person.Country)
            .AsNoTracking()
            .FirstOrDefaultAsync(person => person.PersonId == personId);

        return person;
    }

    public async Task<Person?> GetPersonByPersonIdWithTrackingAsync(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(nameof(personId));

        Person? person = await database.Persons
            .Include(person => person.Country)
            .FirstOrDefaultAsync(person => person.PersonId == personId);

        return person;
    }

    public async Task SaveChangesAsync()
    {
        await database.SaveChangesAsync();
    }
}
