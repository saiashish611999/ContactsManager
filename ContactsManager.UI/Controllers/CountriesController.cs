using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContactsManager.UI.Controllers;

[Route("[controller]")]
[Controller]
public sealed class CountriesController: Controller
{
    private readonly ICountriesService countriesService;

    public CountriesController(ICountriesService countriesService)
    {
        this.countriesService = countriesService;
    }

    #region Index
    [Route("[action]")]
    public async Task<IActionResult> Index()
    {
        List<CountryResponse> allCountries = await countriesService.GetAllCountriesAsync();

        return View("Index", allCountries);
    }
    #endregion

    #region Create
    [Route("[action]")]
    [HttpGet]
    public IActionResult Create()
    {
        return View("Create");
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CountryAddRequest countryAddRequest)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", countryAddRequest);
        }
        CountryResponse countryResponse = await countriesService.AddCountryAsync(countryAddRequest);

        return RedirectToAction("Index", "Countries");
    }
    #endregion

    #region Upload

    [Route("[action]")]
    [HttpGet]
    public IActionResult UploadCountries()
    {
        return View("Upload");
    }

    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> UploadCountries([FromForm] IFormFile excelFile)
    {
        if (excelFile is null || excelFile.Length == 0)
        {
            ViewBag.ErrorMessage = "no content in uploaded file";

            return View("Upload");
        }

        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "unsupported file. xlsx file is expected";

            return View("Upload");
        }

        await countriesService.UploadCountriesFromExcel(excelFile);

        return RedirectToAction("Index", "Countries");
    }
    #endregion

    #region Delete
    [Route("[action]/{countryId:guid}")]
    [HttpGet]
    public async Task<IActionResult> Delete(Guid countryId)
    {
        CountryResponse? country = await countriesService.GetCountryByCountryIdAsync(countryId);

        return View("Delete", country);
    }

    [Route("[action]/{countryId:guid}")]
    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] Guid countryId)
    {
        CountryResponse? existingCountry = await countriesService.GetCountryByCountryIdAsync(countryId);

        if (existingCountry is null)
        {
            return RedirectToAction("Index", "Countries");
        }

        bool isDeleted = await countriesService.DeleteCountryAsync(countryId);

        return RedirectToAction("Index", "Countries");
    }
    #endregion
}
