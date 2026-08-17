using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactsManager.Infrastructure.Repositories;
public sealed class CountriesRepository : ICountriesRepository
{
    private readonly DatabaseContext database;

    private const string RepositoryName = nameof(CountriesRepository);

    private readonly ILogger<CountriesRepository> logger;
    public CountriesRepository(
        DatabaseContext database,
        ILogger<CountriesRepository> logger)
    {
        this.database = database;

        this.logger = logger;
    }

    public async Task<Country> AddCountry(Country country)
    {
        logger.LogInformation("Reached {MethodName} of {RepositoryName}", nameof(AddCountry), RepositoryName);

        await database.Countries.AddAsync(country);

        await database.SaveChangesAsync();

        return country;
    }

    public async Task<bool> DeleteCountry(Guid countryId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(DeleteCountry), RepositoryName);

        var country = await database.Countries
        .FirstOrDefaultAsync(c => c.CountryId == countryId);

        if (country != null)
        {
            database.Countries.Remove(country);
            await database.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetAllCountries), RepositoryName);
            
        List<Country> countries = await database
            .Countries
            .AsNoTracking()
            .ToListAsync();

        return countries;
    }

    public async Task<Country?> GetCountryByCountryId(Guid? countryId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetCountryByCountryId), RepositoryName);

        Country? country = await database
            .Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CountryId == countryId);

        if (country is null)
        {
            return null;
        }

        return country;
    }

    public async Task<bool> IsCountryExists(string? countryName)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(IsCountryExists), RepositoryName);

        if (countryName is null)
        {
            throw new ArgumentNullException(nameof(countryName));
        }

        bool isExists = await database.Countries.AnyAsync(c => c.CountryName == countryName);

        return isExists;
    }
}
