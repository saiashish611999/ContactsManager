using ContactsManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure;
public class DatabaseContext: DbContext
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Person> Persons => Set<Person>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ContactsManager");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

}
