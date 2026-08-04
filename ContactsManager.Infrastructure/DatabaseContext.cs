using ContactsManager.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure;
public class DatabaseContext: DbContext
{
    public DbSet<Country> Countries => Set<Country>();
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ContactsManager");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

}
