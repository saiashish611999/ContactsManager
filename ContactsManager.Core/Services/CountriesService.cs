using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Extensions;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace ContactsManager.Core.Services;
public sealed class CountriesService : ICountriesService
{
    private readonly ICountriesRepository countriesRepository;

    private readonly ILogger<CountriesService> logger;

    private const string ServiceName = nameof(CountriesService);
    public CountriesService(
        ICountriesRepository countriesRepository,
        ILogger<CountriesService> logger)
    {
        this.countriesRepository = countriesRepository;

        this.logger = logger;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(AddCountry), ServiceName);

        ArgumentNullException.ThrowIfNull(countryAddRequest);

        ValidateRequest.ValidateRequestObject(countryAddRequest);

        bool isCountryExists = await countriesRepository.IsCountryExists(countryAddRequest.CountryName);

        if (isCountryExists)
        {
            throw new ArgumentException("country already exists!!!");
        }

        Country country = new Country()
        {
            CountryName = countryAddRequest.CountryName
        };

        Country addedCountry = await countriesRepository.AddCountry(country);

        CountryResponse response = addedCountry.AsCountryResponse();

        return response;
    }

    public async Task<bool> DeleteCountry(Guid countryId)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(DeleteCountry), ServiceName);

        bool isDeleted = await countriesRepository.DeleteCountry(countryId);

        return isDeleted;
    }

    public async Task<List<CountryResponse>> GetAllCountries()
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetAllCountries), ServiceName);

        List<Country> countries = await countriesRepository.GetAllCountries();

        List<CountryResponse> response = countries.Select(c => c.AsCountryResponse()).ToList();

        return response;
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetCountryByCountryId), ServiceName);

        ArgumentNullException.ThrowIfNull(countryId);

        Country? country = await countriesRepository.GetCountryByCountryId(countryId);

        if (country is null)
        {
            return null;
        }

        CountryResponse response = country.AsCountryResponse();

        return response;
    }

    public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(UploadCountriesFromExcel), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        int noOfRowsInserted = 0;

        await formFile.CopyToAsync(memoryStream);

        using (ExcelPackage package = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets["Countries"];

            int rowCount = worksheet.Dimension.Rows;

            for (int index = 2; index <= rowCount; index++)
            {
                string? cellValue = worksheet.Cells[$"A{rowCount}"].Value.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    CountryAddRequest countryAddRequest = new()
                    {
                        CountryName = cellValue
                    };

                    await AddCountry(countryAddRequest);

                    noOfRowsInserted++;
                }
            }
        }

        return noOfRowsInserted;
    }
}
