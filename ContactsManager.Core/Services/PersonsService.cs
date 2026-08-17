using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Extensions;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace ContactsManager.Core.Services;
public sealed class PersonsService : IPersonsService
{
    private readonly IPersonsRepository personsRepository;

    private readonly ILogger<PersonsService> logger;

    private const string ServiceName = nameof(PersonsService);

    private readonly IDiagnosticContext diagnosticContext;
    public PersonsService(
        IPersonsRepository personsRepository,
        ILogger<PersonsService> logger,
        IDiagnosticContext diagnosticContext
        )
    {
        this.personsRepository = personsRepository;

        this.logger = logger;

        this.diagnosticContext = diagnosticContext;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(AddPerson), ServiceName);

        ArgumentNullException.ThrowIfNull(personAddRequest);

        ValidateRequest.ValidateRequestObject(personAddRequest);

        Person person = await personsRepository.AddPerson(
            new Person()
            {
                PersonName = personAddRequest.PersonName,
                EmailAddress = personAddRequest.EmailAddress,
                DateOfBirth = personAddRequest.DateOfBirth,
                Gender = personAddRequest.Gender,
                CountryId = personAddRequest.CountryId,
                Address = personAddRequest.Address,
                ReceivesNewsLetters = personAddRequest.ReceivesNewsLetters
            });

        PersonResponse resposne = person.AsPersonResposne();

        return resposne;
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(DeletePerson), ServiceName);

        ArgumentNullException.ThrowIfNull(personId);

        Person? person = await personsRepository.GetPersonByPersonIdWithTracking(personId);

        if (person is null)
        {
            return false;
        }

        await personsRepository.DeletePerson(person);

        await personsRepository.SaveChanges();

        return true;
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetAllPersons), ServiceName);

        List<Person> persons = await personsRepository.GetAllPersons();

        if (persons.Count == 0)
        {
            return new List<PersonResponse>();
        }

        List<PersonResponse> response = persons.Select(person => person.AsPersonResposne()).ToList();

        return response;
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string? searchBy, string? searchString)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetFilteredPersons), ServiceName);

        List<PersonResponse> allPersons = await GetAllPersons();


        diagnosticContext.Set("Persons", allPersons);

        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
        {
            return allPersons;
        }

        PropertyInfo? prop = typeof(PersonResponse).GetProperty(searchBy);

        if (prop is null)
        {
            return allPersons;
        }

        List<PersonResponse> filteredPersons = allPersons.Where(person =>
        {
            var value = prop.GetValue(person, null);

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

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetPersonByPersonId), ServiceName);

        ArgumentNullException.ThrowIfNull(personId);

        Person? person = await personsRepository.GetPersonByPersonId(personId);

        if (person is null)
        {
            return null;
        }

        PersonResponse response = person.AsPersonResposne();

        return response;
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetPersonsCSV), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);

        csvWriter.WriteHeader<PersonResponse>();

        await csvWriter.NextRecordAsync();

        List<PersonResponse> allPersons = await GetAllPersons();

        await csvWriter.WriteRecordsAsync(allPersons);

        await csvWriter.FlushAsync();

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsCSVAdvanced()
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetPersonsCSVAdvanced), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        StreamWriter streamWriter = new StreamWriter(memoryStream);

        CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);

        CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration, leaveOpen: true);

        csvWriter.WriteField("PersonName");

        csvWriter.WriteField("EmailAddress");

        csvWriter.WriteField("Gender");

        List<PersonResponse> allPersons = await GetAllPersons();

        await csvWriter.NextRecordAsync();

        foreach (PersonResponse person in allPersons)
        {
            csvWriter.WriteField(person.PersonName);

            csvWriter.WriteField(person.EmailAddress);

            csvWriter.WriteField(person.Gender.ToString());

            await csvWriter.NextRecordAsync();
        }

        await csvWriter.FlushAsync();

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetPersonsExcel), ServiceName);

        MemoryStream memoryStream = new MemoryStream();

        using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("Persons");

            workSheet.Cells["A1"].Value = "PersonName";

            workSheet.Cells["B1"].Value = "EmailAddress";

            workSheet.Cells["C1"].Value = "Gender";

            // styling header cells
            using (ExcelRange headerCells = workSheet.Cells["A1:C1"])
            {
                headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                headerCells.Style.Fill.BackgroundColor.SetColor(Color.Gray);

                headerCells.Style.Font.Bold = true;
            }

            int rowNumber = 2;

            List<PersonResponse> allPersons = await GetAllPersons();

            foreach (PersonResponse person in allPersons)
            {
                workSheet.Cells[$"A{rowNumber}"].Value = person.PersonName;

                workSheet.Cells[$"B{rowNumber}"].Value = person.EmailAddress;

                workSheet.Cells[$"C{rowNumber}"].Value = person.Gender.ToString();

                rowNumber++;
            }

            workSheet.Cells[$"A1:C{rowNumber}"].AutoFitColumns();

            await excelPackage.SaveAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }

    public List<PersonResponse> GetSortedPersons(
        List<PersonResponse> allPersons, 
        string? sortBy, 
        SortOrder sortOrder)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(GetSortedPersons), ServiceName);

        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        PropertyInfo? prop = typeof(PersonResponse).GetProperty(sortBy);

        if(prop is null)
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersons = sortOrder is SortOrder.ASCENDING ?
            allPersons.OrderBy(person => prop.GetValue(person)).ToList() :
            allPersons.OrderByDescending(person => prop.GetValue(person)).ToList();

        return sortedPersons;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ServiceName}", nameof(UpdatePerson), ServiceName);

        ArgumentNullException.ThrowIfNull(personUpdateRequest);

        ValidateRequest.ValidateRequestObject(personUpdateRequest);

        Person? person = await personsRepository.GetPersonByPersonIdWithTracking(personUpdateRequest.PersonId);

        if (person is null)
        {
            throw new InvalidDataException("person is null");
        }

        person.PersonName = personUpdateRequest.PersonName;
        person.EmailAddress = personUpdateRequest.EmailAddress;
        person.DateOfBirth = personUpdateRequest.DateOfBirth;
        person.Address = personUpdateRequest.Address;
        person.Gender = personUpdateRequest.Gender;
        person.ReceivesNewsLetters = personUpdateRequest.ReceivesNewsLetters;
        person.CountryId = personUpdateRequest.CountryId;

        await personsRepository.SaveChanges();

        PersonResponse response = person.AsPersonResposne();

        return response;
    }
    
}
