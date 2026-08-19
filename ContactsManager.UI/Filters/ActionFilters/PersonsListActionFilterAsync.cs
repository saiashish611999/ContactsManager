using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.UI.Filters.ActionFilters;

public sealed class PersonsListActionFilterAsync : IAsyncActionFilter, IOrderedFilter
{
    public int Order { set; get; }
    private readonly ILogger<PersonsListActionFilterAsync> logger;

    public PersonsListActionFilterAsync(ILogger<PersonsListActionFilterAsync> logger)
    {
        this.logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // on action executing logic
        if (!context.ActionArguments.ContainsKey("sortBy") ||
            string.IsNullOrWhiteSpace(Convert.ToString(context.ActionArguments["sortBy"])))
        {
            context.ActionArguments["sortBy"] = nameof(PersonResponse.PersonName);
        }

        if (!context.ActionArguments.ContainsKey("sortOrder") ||
            context.ActionArguments["sortOrder"] is null)
        {
            context.ActionArguments["sortOrder"] = SortOrder.ASCENDING;
        }

        await next();

        // on action executed logic
        PersonsController controller = (PersonsController)context.Controller;

        controller.ViewBag.CurrentSortOrder = context.ActionArguments["sortOrder"];

        controller.ViewBag.CurrentSortBy = context.ActionArguments["sortBy"];

        controller.ViewBag.CurrentSearchBy = context.ActionArguments["searchBy"];

        controller.ViewBag.CurrentSearchString = context.ActionArguments["searchString"];

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

        controller.ViewBag.SearchByList = searchByList;
    }
}
