using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.UI.Filters.ActionFilters;

public sealed class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
{
    private readonly ILogger<ResponseHeaderActionFilter> logger;
    private readonly string key;
    private readonly string value;
    public int Order { set; get; }

    public ResponseHeaderActionFilter(
        ILogger<ResponseHeaderActionFilter> logger,
        string key,
        string value,
        int order)
    {
        this.logger = logger;
        this.key = key;
        this.value = value;
        this.Order = order;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        logger.LogInformation("reached to {MethodName} of {ActionFilterName}",
            nameof(OnActionExecuted),
            nameof(ResponseHeaderActionFilter));

        context.HttpContext.Response.Headers[key] = value;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        logger.LogInformation("reached to {MethodName} of {ActionFilterName}", 
            nameof(OnActionExecuting), 
            nameof(ResponseHeaderActionFilter));

        
    }
}
