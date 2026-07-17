using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
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

    #region PrivateMethods
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

    #endregion

    #region Index

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
    #endregion

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

            return View("Create", personAddRequest);
        }

        await personsService.AddPersonAsync(personAddRequest);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region Edit
    [HttpGet]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid? personId)
    {
        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        await LoadCountries();

        PersonUpdateRequest updateRequest = person.ToUpdateRequest();

        return View("Update", updateRequest);
    }

    [HttpPost]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid? personId, [FromForm] PersonUpdateRequest? personUpdateRequest)
    {
        if (personUpdateRequest is null || !ModelState.IsValid)
        {
            await LoadCountries();

            return View("Update", personUpdateRequest);
        }

        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        PersonResponse updatedResponse = await personsService.UpdatePersonAsync(personUpdateRequest);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region Delete
    [HttpGet]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid personId)
    {
        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        return View("Delete", person.ToUpdateRequest());
    }

    [HttpPost]
    [Route("[action]/{personId:guid}")]
    public async Task<IActionResult> DeleteConfirmed(
        [FromRoute] Guid personId)
    {
        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(personId);

        if (person is null)
        {
            return RedirectToAction("Index", "Persons");
        }

        await personsService.DeletePersonAsync(personId);

        return RedirectToAction("Index", "Persons");
    }
    #endregion

    #region GetPersonsPdf
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> GetPersonsPdf()
    {
        List<PersonResponse> allPersons = await personsService.GetAllPersonsAsync();

        return new ViewAsPdf("PersonsPDF", allPersons, ViewData)
        {
            PageMargins = new Margins()
            {
                Top = 20,
                Right = 20,
                Left = 20,
                Bottom = 20
            },
            PageOrientation = Orientation.Landscape
        };
    }
    #endregion

    #region GetPersonsCsv
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetPersonsCsv()
    {
        MemoryStream memoryStream = await personsService.GetPersonsCsv();

        return File(memoryStream, "application/octet-stream", "persons.csv");
    }
    #endregion

    #region GetPersonsCsvAdvanced
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetPersonsCsvAdvanced()
    {
        MemoryStream memoryStream = await personsService.GetPersonsCsvAdvanced();

        return File(memoryStream, "application/octet-stream", "persons-advanced.csv");
    }
    #endregion

    #region GetPersonsExcel
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetPersonsExcel()
    {
        MemoryStream memoryStream = await personsService.GetPersonsExcel();

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
    }
    #endregion
}
