using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.UI.Filters.ActionFilters;

public sealed class PersonsListActionFilter : IActionFilter
{
    private readonly ILogger<PersonsListActionFilter> logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        this.logger = logger;
    }
    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("{MethodName} of {ActionFilterName} executing ", 
            nameof(OnActionExecuted), 
            nameof(PersonsListActionFilter));

        PersonsController personsController = (PersonsController)context.Controller;

        Dictionary<string, string> searchByList = new Dictionary<string, string>()
        {
            { "Name", nameof(PersonResponse.PersonName)},
            { "Email", nameof(PersonResponse.EmailAddress)},
            { "Gender", nameof(PersonResponse.Gender)},
            { "Date Of Birth", nameof(PersonResponse.DateOfBirth)},
            { "Age", nameof(PersonResponse.Age)},
            { "country", nameof(PersonResponse.CountryName)},
            { "Address", nameof(PersonResponse.Address)},
        };

        personsController.ViewBag.SearchByList = searchByList;

        IDictionary<string, object?>? items = (IDictionary<string, object?>?) context.HttpContext.Items["arguments"];

        if (items != null)
        {
            if (items.ContainsKey("searchBy"))
            {
                personsController.ViewBag.CurrentSearchBy = items["searchBy"];
            }

            if (items.ContainsKey("searchString"))
            {
                personsController.ViewBag.CurrentSearchString = items["searchString"];
            }

            if(items.ContainsKey("sortBy"))
            {
                personsController.ViewBag.CurrentSortBy = items["sortBy"];
            }

            if (items.ContainsKey("sortOrder"))
            {
                personsController.ViewBag.CurrentSortOrder = items["sortOrder"];
            }
        }

    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("{MethodName} of {ActionFilterName} is executing ", 
            nameof(OnActionExecuting), 
            nameof(PersonsListActionFilter));

        if (!context.ActionArguments.ContainsKey("sortBy") ||
            string.IsNullOrWhiteSpace(Convert.ToString(context.ActionArguments["sortBy"])))
        {
            context.ActionArguments["sortBy"] = nameof(PersonResponse.PersonName);
        }

        if (!context.ActionArguments.ContainsKey("sortOrder") ||
            context.ActionArguments["sortOrder"] == null)
        {
            context.ActionArguments["sortOrder"] = SortOrder.ASCENDING;
        }

        context.HttpContext.Items["arguments"] = context.ActionArguments;

    }
}
