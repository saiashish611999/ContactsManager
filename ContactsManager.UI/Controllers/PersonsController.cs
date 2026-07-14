using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
public sealed class PersonsController : Controller
{
    private readonly IPersonsService personsService;

    private readonly ICountriesService countriesService;

    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        this.personsService = personsService;

        this.countriesService = countriesService;
    }

    private async Task LoadCountries()
    {
        List<CountryResponse> countries = await countriesService.GetAllCountriesAsync();

        ViewBag.Countries = countries.Select(country =>
        {
            return new SelectListItem()
            {
                Text = country.CountryName,
                Value = country.CountryId.ToString()
            };
        });
    }

    [Route("[action]")]
    [Route("/")]
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchBy,
        string? searchString,
        SortOrder sortOrder = SortOrder.ASCENDING,
        string? sortBy = nameof(PersonResponse))
    {
        Dictionary<string, string> searchByList = new Dictionary<string, string>()
        {
            { "Name", nameof(PersonResponse.PersonName)},
            { "Email", nameof(PersonResponse.Email)},
            { "Gender", nameof(PersonResponse.Gender)},
            { "Date Of Birth", nameof(PersonResponse.DateOfBirth)},
            { "Age", nameof(PersonResponse.Age)},
            { "country", nameof(PersonResponse.CountryName)},
            { "Address", nameof(PersonResponse.Address)}
        };

        ViewBag.searchByList = searchByList;

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersonsAsync(searchBy, searchString);

        ViewBag.CurrentSearchBy = searchBy;

        ViewBag.CurrentSearchString = searchString;

        List<PersonResponse> sortedPersons = personsService.GetSortedPersonsAsync(filteredPersons, sortBy, sortOrder);

        ViewBag.CurrentSortBy = sortBy;

        ViewBag.CurrentSortOrder = sortOrder;

        return View("Index", sortedPersons);
    }

    #region Create
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
        await LoadCountries();

        return View("Create");
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(PersonAddRequest? personAddRequest)
    {
        if (personAddRequest is null || !ModelState.IsValid)
        {
            await LoadCountries();

            return View("Create");
        }

        await personsService.AddPersonAsync(personAddRequest);

        return RedirectToAction("Index", "Persons");
    }
    #endregion
}
