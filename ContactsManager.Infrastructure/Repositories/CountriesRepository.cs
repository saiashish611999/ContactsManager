using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;
public sealed class CountriesRepository : ICountriesRepository
{
    private readonly DatabaseContext database;
    public CountriesRepository(DatabaseContext database)
    {
        this.database = database;
    }

    public async Task<Country> AddCountry(Country country)
    {
        await database.Countries.AddAsync(country);

        await database.SaveChangesAsync();

        return country;
    }

    public async Task<bool> DeleteCountry(Guid countryId)
    {
        int rowsAffected = await database.Countries.Where(country => country.CountryId == countryId)
            .ExecuteDeleteAsync();

        return rowsAffected > 0;
    }

    public async Task<List<Country>> GetAllCountries()
    {
        List<Country> countries = await database
            .Countries
            .AsNoTracking()
            .ToListAsync();

        return countries;
    }

    public async Task<Country?> GetCountryByCountryId(Guid? countryId)
    {
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
        if (countryName is null)
        {
            throw new ArgumentNullException(nameof(countryName));
        }

        bool isExists = await database.Countries.AnyAsync(c => c.CountryName == countryName);

        return isExists;
    }
}
