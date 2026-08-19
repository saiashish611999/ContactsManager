using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.UI.Filters.ActionFilters;


public sealed class CountriesListActionFilter : IActionFilter
{
    private readonly ILogger<CountriesListActionFilter> logger;

    public CountriesListActionFilter(ILogger<CountriesListActionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("{MethodName} of {ActionFilterName} executed", nameof(OnActionExecuted), nameof(CountriesListActionFilter));        
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("{MethodName} of {ActionFilterName} executed", nameof(OnActionExecuting), nameof(CountriesListActionFilter));
    }
}
