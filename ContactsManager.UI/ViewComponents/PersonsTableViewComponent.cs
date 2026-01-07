using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.ViewComponents;

[ViewComponent]
public sealed class PersonsTableViewComponent:ViewComponent
{
    private readonly IPersonsService _personsService;
    public PersonsTableViewComponent(IPersonsService personsService)
    {
        _personsService = personsService;
    }

    public IViewComponentResult Invoke(string searchBy,
                                       string searchString,
                                       string sortBy,
                                       SortOrderEnum sortOrder)
    {
        var persons = _personsService.GetFilteredPersons(searchBy, searchString);
        var sorted = _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        return View(sorted);
    }
}
