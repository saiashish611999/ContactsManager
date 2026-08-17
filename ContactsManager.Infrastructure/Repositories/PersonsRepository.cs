using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactsManager.Infrastructure.Repositories;
public class PersonsRepository : IPersonsRepository
{
    private readonly DatabaseContext database;

    private const string RepositoryName = nameof(PersonsRepository);

    private readonly ILogger<PersonsRepository> logger;
    public PersonsRepository(DatabaseContext database,
        ILogger<PersonsRepository> logger)
    {
        this.database = database;

        this.logger = logger;
    }

    public async Task<Person> AddPerson(Person person)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(AddPerson), RepositoryName);

        await database.Persons.AddAsync(person);

        await database.SaveChangesAsync();

        return person;
    }

    public Task DeletePerson(Person person)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(DeletePerson), RepositoryName);

        database.Persons.Remove(person);

        return Task.CompletedTask;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetAllPersons), RepositoryName);

        List<Person> persons = await database
            .Persons
            .AsNoTracking()
            .Include(p => p.Country)
            .ToListAsync();

        return persons;
    }

    public async Task<Person?> GetPersonByPersonId(Guid? personId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetPersonByPersonId), RepositoryName);

        ArgumentNullException.ThrowIfNull(nameof(personId));

        Person? person = await database.Persons
            .AsNoTracking()
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        return person;
    }

    public async Task<Person?> GetPersonByPersonIdWithTracking(Guid? personId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetPersonByPersonIdWithTracking), RepositoryName);

        Person? person = await database.Persons
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personId);

        return person;
    }

    public async Task SaveChanges()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(SaveChanges), RepositoryName);

        await database.SaveChangesAsync();
    }
}
