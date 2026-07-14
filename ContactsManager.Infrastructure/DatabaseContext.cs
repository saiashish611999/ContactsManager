using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure;
public sealed class DatabaseContext: DbContext
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
