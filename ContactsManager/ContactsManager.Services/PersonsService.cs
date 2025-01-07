using ContactsManager.Entities;
using ContactsManager.ServiceContracts.DTO.Person;
using ContactsManager.ServiceContracts.ServiceContracts;
using ContactsManager.Services.Helpers;

namespace ContactsManager.Services;

public class PersonsService : IPersonsService
{
    private readonly List<Persons> _persons;
    public PersonsService()
    {
        _persons = new List<Persons>();
    }
    public PersonResponse? AddPerson(PersonAddRequest? perosnAddRequest)
    {
        // validation when person add request is null
        if (perosnAddRequest == null)
        {
            throw new ArgumentNullException(nameof(perosnAddRequest));
        }

        // validation when parameters of person are null
        ModelValidator.ModelValidationHelper(perosnAddRequest);

        // validation when person is duplicate
        if (_persons.Where(person => person.PersonName == perosnAddRequest.PersonName).Count() > 0)
        {
            throw new ArgumentException("Person already exists", nameof(perosnAddRequest.PersonName));
        }

        // covnert to person
        Persons person = perosnAddRequest.ConvertToPerosn();
        person.PersonId = Guid.NewGuid();
        _persons.Add(person);

        return person.ConvertToPersonResponse();
        
        
    }
}
