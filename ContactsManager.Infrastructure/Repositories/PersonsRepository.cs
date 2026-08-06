using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;
public class PersonsRepository : IPersonsRepository
{
    private readonly DatabaseContext database;
    public PersonsRepository(DatabaseContext database)
    {
        this.database = database;
    }

    public async Task<Person> AddPerson(Person person)
    {
        await database.Persons.AddAsync(person);

        await database.SaveChangesAsync();

        return person;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        List<Person> persons = await database
            .Persons
            .AsNoTracking()
            .Include(p => p.Country)
            .ToListAsync();

        return persons;
    }

    public async Task<Person?> GetPersonByPersonId(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(nameof(personId));

        Person? person = await database.Persons
            .AsNoTracking()
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        return person;
    }

    public async Task<Person?> GetPersonByPersonIdWithTracking(Guid? personId)
    {
        Person? person = await database.Persons
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        return person;
    }

    public async Task SaveChanges()
    {
        await database.SaveChangesAsync();
    }
}
