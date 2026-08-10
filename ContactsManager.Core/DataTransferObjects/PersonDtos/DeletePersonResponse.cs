namespace ContactsManager.Core.DataTransferObjects.PersonDtos;
public sealed class DeletePersonResponse
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? EmailAddress { get; set; }
}
