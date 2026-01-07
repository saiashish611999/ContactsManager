using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTO;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[Controller]")]
public sealed class PersonsController : Controller
{
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;
    public PersonsController(IPersonsService personsService,
                             ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }
    [Route("/")]
    [Route("[Action]")]
    public IActionResult Index(string searchBy,
                               string searchString,
                               string sortBy = nameof(PersonResponse.PersonName),
                               SortOrderEnum sortOrder = SortOrderEnum.Asc)
    {
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name"},
            { nameof(PersonResponse.Email), "Email"},
            { nameof(PersonResponse.DateOfBirth), "Date of Birth"},
            { nameof(PersonResponse.Gender), "Gender"},
            { nameof(PersonResponse.CountryName), "Country"},
            { nameof(PersonResponse.Address), "Address"},
            { nameof(PersonResponse.ReceiveNewsLetters), "Receive news letters"},
            { nameof(PersonResponse.Age), "Age"}

        };

        // filtering persons
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;
        List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);

        // sorting persons
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder;
        List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        return View("Index", sortedPersons);
    }

    [HttpGet("[Action]")]
    public IActionResult SearchTable(string searchBy,
                                     string searchString,
                                     string sortBy = nameof(PersonResponse.PersonName),
                                     SortOrderEnum sortOrder = SortOrderEnum.Asc)
    {
        return ViewComponent("PersonsTable", new
        {
            searchBy,
            searchString,
            sortBy,
            sortOrder
        });
    }


    [HttpGet("[Action]")]
    public IActionResult Create()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryId });
        return View("Create");
    }

    [HttpPost("[Action]")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryId });
            ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
            return View("Create");
        }
        PersonResponse response = _personsService.AddPerson(personAddRequest);
        return RedirectToAction("Index", "Persons");
    }

    [HttpGet("[Action]")]
    public IActionResult Edit(string personId)
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryId });
        PersonResponse? person = _personsService.GetPersonByPersonId(personId);
        PersonUpdateRequest updateRequest = person!.ConvetToPersonUpdateRequest();
        return View("Edit", updateRequest);
    }

    [HttpPost("[Action]")]
    public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem() { Text = country.CountryName, Value = country.CountryId });
            ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
            return View("Edit");
        }
        PersonResponse? response = _personsService.UpdatePerson(personUpdateRequest);
        return RedirectToAction("Index", "Persons");
    }

    
}
