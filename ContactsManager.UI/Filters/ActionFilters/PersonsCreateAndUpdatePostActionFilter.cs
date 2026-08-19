using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactsManager.UI.Filters.ActionFilters;

public sealed class PersonsCreateAndUpdatePostActionFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order { get; set; }
    private readonly ILogger<PersonsCreateAndUpdatePostActionFilter> logger;
    private readonly ICountriesService countriesService;

    public PersonsCreateAndUpdatePostActionFilter(
        ILogger<PersonsCreateAndUpdatePostActionFilter> logger,
        int order,
        ICountriesService countriesService)
    {
        this.logger = logger;
        this.Order = order;
        this.countriesService = countriesService;
    }   

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // on action executing
        if (context.Controller is PersonsController personsController)
        {
            if (!personsController.ModelState.IsValid)
            {
                List<CountryResponse> allCountries = await countriesService.GetAllCountries();

                personsController.ViewBag.Countries = allCountries.Select(country =>
                {
                    return new SelectListItem()
                    {
                        Value = country.CountryId.ToString(),
                        Text = country.CountryName
                    };
                });

                var personAddRequest = context.ActionArguments["personAddRequest"];

                context.Result = personsController.View(personAddRequest);
                return;
                // a non null value will be short circuit the next subsecuent filter and prevents execution of next filters and action methods
            }
        }

        await next();

        // on action executed
    }
}
