using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-CUSTOM-HEADER", "ContactsManager", 3})]
public sealed class PersonsController: Controller
{
    private readonly ICountriesService countriesService;
    private readonly IPersonsService personsService;
    private const string ControllerName = nameof(PersonsController);
    private readonly ILogger<PersonsController> logger;

    public PersonsController(
        ICountriesService countriesService,
        IPersonsService personsService,
        ILogger<PersonsController> logger)
    {
        this.countriesService = countriesService;

        this.personsService = personsService;

        this.logger = logger;
    }

    #region PrivateMethods
    private async Task LoadCountries()
    {
        List<CountryResponse> allCountries = await countriesService.GetAllCountries();

        ViewBag.Countries = allCountries.Select(country =>
        {
            return new SelectListItem()
            {
                Value = country.CountryId.ToString(),
                Text = country.CountryName,
            };
        });
    }
    #endregion

    #region Index

    [Route("/")]
    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(PersonsListActionFilter))]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Custom-Key-Method", "random value", 1})]
    public async Task<IActionResult> Index(
        [FromQuery] string? searchBy,
        [FromQuery] string? searchString,
        [FromQuery] string? sortBy,
        [FromQuery] SortOrder sortOrder)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Index), ControllerName);

        //Dictionary<string, string> searchByList = new Dictionary<string, string>()
        //{
        //    { "Name", nameof(PersonResponse.PersonName)},
        //    { "Email", nameof(PersonResponse.EmailAddress)},
        //    { "Gender", nameof(PersonResponse.Gender)},
        //    { "Date Of Birth", nameof(PersonResponse.DateOfBirth)},
        //    { "Age", nameof(PersonResponse.Age)},
        //    { "country", nameof(PersonResponse.CountryName)},
        //    { "Address", nameof(PersonResponse.Address)},            
        //};

        //ViewBag.SearchByList = searchByList;

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersons(searchBy, searchString);

       // ViewBag.CurrentSearchBy = searchBy;

       // ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = personsService.GetSortedPersons(
            filteredPersons,
            sortBy,
            sortOrder);

        // ViewBag.CurrentSortBy = sortBy;

        // ViewBag.CurrentSortOrder = sortOrder;


        return View("Index", sortedPersons);
    }
    #endregion

    #region Create
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Create), ControllerName);

        await LoadCountries();

        return View("Create");
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(PersonsCreateAndUpdatePostActionFilter), Arguments = new object[] { 1})]
    public async Task<IActionResult> Create(PersonAddRequest? personRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Create), ControllerName);

        //if (personRequest is null || !ModelState.IsValid)
        //{
        //    await LoadCountries();

        //    return View("Create", personRequest);
        //}

        PersonResponse addedPerson = await personsService.AddPerson(personRequest);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region Update
    [Route("[action]/{personId:guid}")]
    [HttpGet]
    public async Task<IActionResult> Update([FromRoute] Guid personId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Update), ControllerName);

        PersonResponse? existingPerson = await personsService.GetPersonByPersonId(personId);

        if (existingPerson is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        await LoadCountries();

        PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = existingPerson.PersonId,
            PersonName = existingPerson.PersonName,
            EmailAddress = existingPerson.EmailAddress,
            CountryId = existingPerson.CountryId,
            Address = existingPerson.Address,
            DateOfBirth = existingPerson.DateOfBirth,
            Gender = existingPerson.Gender,
            ReceivesNewsLetters = existingPerson.ReceivesNewsLetters
        };

        return View("Update", personUpdateRequest);        
    }

    [Route("[action]/{personId:guid}")]
    [HttpPost]
    [TypeFilter(typeof(PersonsCreateAndUpdatePostActionFilter), Arguments = new object[] { 1})]
    public async Task<IActionResult> Update(
        [FromRoute] Guid personId, 
        [FromForm] PersonUpdateRequest? personRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Update), ControllerName);

        //if (personRequest is null || !ModelState.IsValid)
        //{
        //    await LoadCountries();

        //    return View("Update", personRequest);
        //}

        PersonResponse updatedPerson = await personsService.UpdatePerson(personRequest);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region Delete
    [Route("[action]/{personId:guid}")]
    [HttpGet]
    public async Task<IActionResult> Delete([FromRoute] Guid personId)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Delete), ControllerName);

        PersonResponse? existingPerson = await personsService.GetPersonByPersonId(personId);

        if (existingPerson is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        DeletePersonResponse personResponse = new DeletePersonResponse()
        {
            PersonId = existingPerson.PersonId,
            PersonName = existingPerson.PersonName,
            EmailAddress = existingPerson.EmailAddress
        };

        return View("Delete", personResponse);
    }

    [Route("[action]/{personId:guid}")]
    [HttpPost]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid personId,
        [FromForm] DeletePersonResponse deletePerson)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Delete), ControllerName);

        PersonResponse? existingPerson = await personsService.GetPersonByPersonId(personId);

        if (existingPerson == null)
        {
            return RedirectToAction("Index", "Persons");
        }

        bool isValid = await personsService.DeletePerson(personId);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region PersonsPDF
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> PersonsPDF()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(PersonsPDF), ControllerName);

        List<PersonResponse> allPersons = await personsService.GetAllPersons();

        return new ViewAsPdf("PersonsPDF", allPersons, ViewData)
        {
            PageMargins = new Margins()
            {
                Top = 20,
                Left = 20,
                Right = 20,
                Bottom = 20
            },
            PageOrientation = Orientation.Landscape
        };
    }
    #endregion

    #region PersonsCSV
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> PersonsCSV()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(PersonsCSV), ControllerName);

        MemoryStream memoryStream = await personsService.GetPersonsCSV();

        return File(memoryStream, "application/octet-stream", "persons.csv");
    }
    #endregion

    #region PersonsAdvancedCSV
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> PersonsAdvancedCSV()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(PersonsAdvancedCSV), ControllerName);

        MemoryStream memoryStream = await personsService.GetPersonsCSVAdvanced();

        return File(memoryStream, "application/octet-stream", "persons_advanced.csv");
    }
    #endregion

    #region PersonsExcel
    public async Task<IActionResult> GetPersonsExcel()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(GetPersonsExcel), ControllerName);

        MemoryStream memoryStream = await personsService.GetPersonsExcel();

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }
    #endregion
}
