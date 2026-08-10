using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
public sealed class CountriesController : Controller
{
    private readonly ICountriesService countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        this.countriesService = countriesService;
    }

    #region Index

    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<CountryResponse> allCountries = await countriesService.GetAllCountries();

        return View("Index", allCountries);
    }

    #endregion

    #region Create
    [HttpGet]
    [Route("[action]")]
    public IActionResult Create()
    {
        return View("Create");
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(CountryAddRequest countryAddRequest)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", countryAddRequest);
        }

        CountryResponse addedCountry = await countriesService.AddCountry(countryAddRequest);

        return RedirectToAction("Index", "Countries");
    }
    #endregion

    #region Delete
    [Route("[action]/{countryId:guid}")]
    [HttpGet]
    public async Task<IActionResult> Delete([FromRoute] Guid countryId)
    {
        CountryResponse? existingCountry = await countriesService.GetCountryByCountryId(countryId);

        if (existingCountry is null)
        {
            return RedirectToAction("Index", "Countries");
        }

        return View("Delete", existingCountry);
    }

    [Route("[action]/{countryId:guid}")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] Guid countryId)
    {
        bool isDeleted = await countriesService.DeleteCountry(countryId);

        return RedirectToAction("Index", "Countries");
    }
    #endregion
}
