using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Infrastructure.Repositories;
public sealed class CountriesRepository: ICountriesRepository
{
    private readonly DatabaseContext database;
    public CountriesRepository(DatabaseContext database)
    {
        this.database = database;
    }

    public async Task<Country> AddCountryAsync(Country country)
    {
        await database.AddAsync(country);

        await database.SaveChangesAsync();

        return country;
    }

    public async Task<bool> DoesCountryExistAsync(string? countryName)
    {
        ArgumentNullException.ThrowIfNull(countryName);

        bool doesCountryExist = await database
            .Countries
            .AnyAsync(c => c.CountryName == countryName);

        return doesCountryExist;

    }

    public async Task<List<Country>> GetAllCountriesAsync()
    {
        List<Country> countries = await database.Countries.ToListAsync();

        return countries;
    }

    public async Task<Country?> GetCountryByCountryIdAsync(Guid? countryId)
    {
        ArgumentNullException.ThrowIfNull(countryId);

        Country? country = await database.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);

        return country;
    }

    public async Task<bool> DeleteCountry(Guid? countryId)
    {
        int rowsAffected = await database.Countries.Where(country => country.CountryId == countryId).ExecuteDeleteAsync();

        return rowsAffected > 0 ? true : false;
    }
}
