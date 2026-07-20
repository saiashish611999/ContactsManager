using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace ContactsManager.Core.Services;
public sealed class CountriesService: ICountriesService
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

    public async Task<CountryResponse> AddCountryAsync(CountryAddRequest? countryAddRequest)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(AddCountryAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(countryAddRequest);

        ValidationHelper.ValidateRequest(countryAddRequest);

        bool doesCountryExist = await countriesRepository.DoesCountryExistAsync(countryAddRequest.CountryName);

        if (doesCountryExist)
        {
            throw new ArgumentException("country already exists!!!");
        }

        Country country = countryAddRequest.ToEntity();

        Country countryAdded = await countriesRepository.AddCountryAsync(country);

        CountryResponse response = countryAdded.ToCountryResponse();

        return response;
    }

    public async Task<bool> DeleteCountryAsync(Guid countryId)
    {

        logger.LogInformation("reached {methodName} of {serviceName}", nameof(DeleteCountryAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(countryId);

        Country? existingCountry = await countriesRepository.GetCountryByCountryIdAsync(countryId);

        if (existingCountry is null)
        {
            return false;
        }

        bool isDeleted = await countriesRepository.DeleteCountry(countryId);

        return isDeleted;
    }

    public async Task<List<CountryResponse>> GetAllCountriesAsync()
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetAllCountriesAsync), ServiceName);

        List<Country> countries = await countriesRepository.GetAllCountriesAsync();

        List<CountryResponse> response = countries.Select(c => c.ToCountryResponse()).ToList();

        return response;
    }

    public async Task<CountryResponse?> GetCountryByCountryIdAsync(Guid? countryId)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetCountryByCountryIdAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(countryId);

        Country? country = await countriesRepository.GetCountryByCountryIdAsync(countryId);

        if (country is null)
        {
            return null;
        }

        CountryResponse response = country.ToCountryResponse();

        return response;
    }

    public async Task<int> UploadCountriesFromExcel(IFormFile formFile)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(UploadCountriesFromExcel), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        int noOfRowsInserted = 0;

        await formFile.CopyToAsync(memoryStream);

        using (ExcelPackage package = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets["countries"];

            int rowCount = worksheet.Dimension.Rows;

            for (int index = 2; index <= rowCount; index++)
            {
                string? cellValue = worksheet.Cells[$"A{rowCount}"].Value.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    CountryAddRequest countryAddRequest = new CountryAddRequest()
                    {
                        CountryName = cellValue
                    };

                    await AddCountryAsync(countryAddRequest);

                    noOfRowsInserted++;
                }
            }
        }

        return noOfRowsInserted;
    }
}
