using ContactsManager.ServiceContracts.DTO.Person;

namespace ContactsManager.ServiceContracts.ServiceContracts
{
    public interface IPersonsService
    {
        /// <summary>
        /// Method to add the person
        /// </summary>
        /// <param name="perosnAddRequest"> input to the person</param>
        /// <returns> return person response object</returns>
        PersonResponse? AddPerson(PersonAddRequest? perosnAddRequest);
    }
}
