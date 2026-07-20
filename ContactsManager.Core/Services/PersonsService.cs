using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using CsvHelper;
using System.Reflection;
using System.Globalization;
using CsvHelper.Configuration;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.Extensions.Logging;

namespace ContactsManager.Core.Services;
public sealed class PersonsService : IPersonsService
{
    private readonly IPersonsRepository personsRepository;

    private readonly ILogger<PersonsService> logger;

    private const string ServiceName = nameof(PersonsService);

    public PersonsService(
        IPersonsRepository personsRepository,
        ILogger<PersonsService> logger)
    {
        this.personsRepository = personsRepository;

        this.logger = logger;
    }

    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest? personAddRequest)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(AddPersonAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(personAddRequest);

        ValidationHelper.ValidateRequest(personAddRequest);

        Person personToBeAdded = await personsRepository.AddPersonAsync(personAddRequest.ToEntity());

        PersonResponse personResponse = personToBeAdded.ToResponse();

        return personResponse;
    }

    public async Task<bool> DeletePersonAsync(Guid? personID)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(DeletePersonAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(personID);

        Person? person = await personsRepository.GetPersonByPersonIdAsync(personID);

        if (person is null)
        {
            return false;
        }

        bool isDeleted = await personsRepository.DeletePersonAsync(personID);

        return isDeleted;
    }

    public async Task<List<PersonResponse>> GetAllPersonsAsync()
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetAllPersonsAsync), ServiceName);

        List<Person> persons = await personsRepository.GetAllPersonsAsync();

        List<PersonResponse> repsonse = persons.Select(p => p.ToResponse()).ToList();

        return repsonse;
    }

    public async Task<List<PersonResponse>> GetFilteredPersonsAsync(string? searchBy, string? searchString)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetFilteredPersonsAsync), ServiceName);

        List<PersonResponse> allPersons = await GetAllPersonsAsync();

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return allPersons;
        }

        PropertyInfo? propInfo = typeof(PersonResponse).GetProperty(searchBy);

        if (propInfo is null)
        {
            return allPersons;
        }

        List<PersonResponse> filteredPersons = allPersons.Where(person =>
        {
            var value = propInfo.GetValue(person, null);

            if (value is null)
            {
                return false;
            }

            if (searchBy == nameof(PersonResponse.Gender))
            {
                return value.ToString()!.Equals(searchString, StringComparison.OrdinalIgnoreCase);
            }

            return value.ToString()!.Contains(searchString, StringComparison.OrdinalIgnoreCase);

        }).ToList();

        return filteredPersons;
    }

    public async Task<PersonResponse?> GetPersonByPersonIdAsync(Guid? personId)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetPersonByPersonIdAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(personId);

        Person? person = await personsRepository.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return null;
        }

        PersonResponse resposne = person.ToResponse();

        return resposne;
    }

    public async Task<MemoryStream> GetPersonsCsv()
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetPersonsCsv), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvWriter writer = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, true);

        writer.WriteHeader<PersonResponse>();

        await writer.NextRecordAsync();

        List<PersonResponse> allPersons = await GetAllPersonsAsync();

        await writer.WriteRecordsAsync(allPersons);

        await writer.FlushAsync();

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsCsvAdvanced()
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetPersonsCsvAdvanced), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture);

        CsvWriter writer = new CsvWriter(streamWriter, configuration, true);

        writer.WriteField(nameof(PersonResponse.PersonName));

        writer.WriteField(nameof(PersonResponse.Email));

        writer.WriteField(nameof(PersonResponse.Gender));

        await writer.NextRecordAsync();

        List<PersonResponse> allPersons = await GetAllPersonsAsync();

        foreach (PersonResponse person in allPersons)
        {
            writer.WriteField(person.PersonName);

            writer.WriteField(person.Email);

            writer.WriteField(person.Gender);

            await writer.NextRecordAsync();
        }

        await writer.FlushAsync();

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetPersonsExcel), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        using (ExcelPackage package = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Persons");

            worksheet.Cells["A1"].Value = "Person Name";

            worksheet.Cells["B1"].Value = "Email";

            worksheet.Cells["C1"].Value = "Gender";

            using (ExcelRange headerCells = worksheet.Cells["A1:C1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                headerCells.Style.Fill.BackgroundColor.SetColor(Color.Gray);

                headerCells.Style.Font.Bold = true;
            }

            int dataRowNumber = 2;

            List<PersonResponse> allPersons = await GetAllPersonsAsync();

            foreach (PersonResponse person in allPersons)
            {
                worksheet.Cells[$"A{dataRowNumber}"].Value = person.PersonName;

                worksheet.Cells[$"B{dataRowNumber}"].Value = person.Email;

                worksheet.Cells[$"C{dataRowNumber}"].Value = person.Gender;

                dataRowNumber++;
            }

            worksheet.Cells[$"A1:C{dataRowNumber}"].AutoFitColumns();

            await package.SaveAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }

    public List<PersonResponse> GetSortedPersonsAsync(List<PersonResponse> allPersons, string? sortBy, SortOrder sortOrder)
    {

        logger.LogInformation("reached {methodName} of {serviceName}", nameof(GetSortedPersonsAsync), ServiceName);

        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        PropertyInfo? propInfo = typeof(PersonResponse).GetProperty(sortBy);

        if (propInfo is null)
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersons = sortOrder == SortOrder.ASCENDING ?
            allPersons.OrderBy(person => propInfo.GetValue(person)).ToList() :
            allPersons.OrderByDescending(person => propInfo.GetValue(person)).ToList();

        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest? personUpdateRequest)
    {
        logger.LogInformation("reached {methodName} of {serviceName}", nameof(UpdatePersonAsync), ServiceName);

        ArgumentNullException.ThrowIfNull(personUpdateRequest);

        ValidationHelper.ValidateRequest(personUpdateRequest);

        Person? person = await personsRepository.GetPersonByPersonIdWithTrackingAsync(personUpdateRequest.PersonId);

        if (person is null)
        {
            throw new InvalidDataException("person id doesn't exist");
        }

        person.PersonName = personUpdateRequest.PersonName;

        person.Email = personUpdateRequest.Email;

        person.Gender = personUpdateRequest.Gender.ToString();

        person.Address = personUpdateRequest.Address;

        person.DateOfBirth = personUpdateRequest.DateOfBirth;

        person.CountryId = personUpdateRequest.CountryId;

        await personsRepository.SaveChangesAsync();

        return person.ToResponse();
    }
}
