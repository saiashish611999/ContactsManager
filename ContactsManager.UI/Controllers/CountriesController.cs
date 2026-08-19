using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Filters.ActionFilters;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers;

[Controller]
[Route("[controller]")]
public sealed class CountriesController : Controller
{
    private readonly ICountriesService countriesService;
    
    private const string ControllerName = nameof(CountriesController);

    private ILogger<CountriesController> logger;

    public CountriesController(ICountriesService countriesService, ILogger<CountriesController> logger)
    {
        this.countriesService = countriesService;

        this.logger = logger;
    }

    #region Index

    [Route("[action]")]
    [HttpGet]
    [TypeFilter(typeof(CountriesListActionFilter))]
    public async Task<IActionResult> Index()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Index), ControllerName);

        List<CountryResponse> allCountries = await countriesService.GetAllCountries();

        return View("Index", allCountries);
    }

    #endregion

    #region Create
    [HttpGet]
    [Route("[action]")]
    public IActionResult Create()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Create), ControllerName);

        return View("Create");
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create(CountryAddRequest countryAddRequest)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Create), ControllerName);

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
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Delete), ControllerName);

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
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(Delete), ControllerName);

        bool isDeleted = await countriesService.DeleteCountry(countryId);

        return RedirectToAction("Index", "Countries");
    }
    #endregion

    #region UploadCountries

    [Route("[action]")]
    [HttpGet]
    public IActionResult UploadCountries()
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(UploadCountries), ControllerName);

        return View("Upload");
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> UploadCountries([FromForm] IFormFile formFile)
    {
        logger.LogInformation("Reached {MethodName} of {ControllerName}", nameof(formFile), ControllerName);

        if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "please upload either .xlsx or .xls file";

            return View("Upload");
        }

        if (formFile is null || formFile.Length == 0)
        {
            ViewBag.ErrorMessage = "No content in uploaded file";

            return View("Upload");
        }

        await countriesService.UploadCountriesFromExcel(formFile);

        return RedirectToAction("Index", "Countries");
    }
    #endregion
}
