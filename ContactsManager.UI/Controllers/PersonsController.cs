using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
public sealed class PersonsController: Controller
{
    private readonly ICountriesService countriesService;
    private readonly IPersonsService personsService;

    public PersonsController(
        ICountriesService countriesService,
        IPersonsService personsService)
    {
        this.countriesService = countriesService;

        this.personsService = personsService;
    }

    #region Index

    [Route("/")]
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] string? searchBy,
        [FromQuery] string? searchString,
        [FromQuery] string? sortBy,
        [FromQuery] SortOrder sortOrder)
    {
        Dictionary<string, string> searchByList = new Dictionary<string, string>()
        {
            { "Name", nameof(PersonResponse.PersonName)},
            { "Email", nameof(PersonResponse.EmailAddress)},
            { "Gender", nameof(PersonResponse.Gender)},
            { "Date Of Birth", nameof(PersonResponse.DateOfBirth)},
            { "Age", nameof(PersonResponse.Age)},
            { "country", nameof(PersonResponse.CountryName)},
            { "Address", nameof(PersonResponse.Address)},
            { "ReceiveNewsLetters", nameof(PersonResponse.ReceivesNewsLetters)}
        };

        ViewBag.SearchByList = searchByList;

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersons(searchBy, searchString);

        ViewBag.CurrentSearchBy = searchBy;

        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = personsService.GetSortedPersons(
            filteredPersons,
            sortBy,
            sortOrder);

        ViewBag.CurrentSortBy = sortBy;

        ViewBag.CurrentSortOrder = sortOrder;


        return View("Index", sortedPersons);
    }
    #endregion

    #region Create
    #endregion
}
