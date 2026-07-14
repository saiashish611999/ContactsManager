using AutoFixture;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;
using OfficeOpenXml;

namespace ContactsManager.Tests.ServiceTests;
public sealed class PersonsServiceTests
{
    private readonly IFixture fixture;
    private readonly IPersonsService personsService;
    private readonly IPersonsRepository personsRepository;
    private readonly Mock<IPersonsRepository> personsReposiotryMock;

    public PersonsServiceTests()
    {
        fixture = new Fixture();

        personsReposiotryMock = new Mock<IPersonsRepository>();

        personsRepository = personsReposiotryMock.Object;

        personsService = new PersonsService(personsRepository);

        ExcelPackage.License.SetNonCommercialPersonal("Sai Ashish");
    }

    #region AddPersons
    // when null request, throw argument null exception
    [Fact]
    public async Task AddPerson_ShouldThrowArgumentNullException_IfNullRequest()
    {
        Func<Task> action = async () =>
        {
            await personsService.AddPersonAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when null parameters, throw argument exception
    [Theory]
    [InlineData("ashish", "ashsih")]
    [InlineData("ashish", null)]
    [InlineData(null, "ashish@gmail.com")]
    public async Task AddPerson_ShouldThrowArgumentException_IfNullParameters(string? Name, string? Email)
    {
        PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            .With(person => person.PersonName, Name)
            .With(person => person.Email, Email)
            .With(person => person.Gender, Core.Enums.Gender.MALE)
            .Create();

        Func<Task> action = async () =>
        {
            await personsService.AddPersonAsync(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when proper request, return proper response
    [Fact]
    public async Task AddPerson_ShouldReturnProperResponse_IfProperRequest()
    {
        PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            .With(person => person.Email, "ashish@gmail.com")
            .With(person => person.Gender, Core.Enums.Gender.MALE)
            .Create();

        Person person = personAddRequest.ToEntity();

        personsReposiotryMock.Setup(method => method.AddPersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(person);

        PersonResponse response = await personsService.AddPersonAsync(personAddRequest);

        response.PersonName.Should().Be(person.PersonName);

        personsReposiotryMock.Verify(method => method.AddPersonAsync(It.IsAny<Person>()), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetAllPersons
    // when no persons, return empty object
    [Fact]
    public async Task GetAllPersons_ShouldReturnEmptyObject_WhenNoPersonsExists()
    {
        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync())
            .ReturnsAsync(new List<Person>());

        List<PersonResponse> persons = await personsService.GetAllPersonsAsync();

        persons.Should().BeEmpty();

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when persons exists, return persons
    [Fact]
    public async Task GetAllPersons_ShoudlReturnPersonsList_WhenPersonsExists()
    {
        List<Person> persons = fixture.Build<Person>().Without(prop => prop.Country).CreateMany(5).ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync())
            .ReturnsAsync(persons);

        List<PersonResponse> allPersons = await personsService.GetAllPersonsAsync();

        allPersons.Should().NotBeEmpty();

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetPersonByPersonId
    // when null personId, throw argument null exception
    [Fact]
    public async Task GetPersonByPersonId_ShouldThrowArgumentNullException_IfNullId()
    {
        Func<Task> action = async () =>
        {
            await personsService.GetPersonByPersonIdAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Never);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when invalid person id, return null
    [Fact]
    public async Task GetPersonByPersonId_ShouldReturnNull_InvalidPersonId()
    {
        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as Person);

        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(Guid.NewGuid());

        person.Should().BeNull();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when valid person id, return person as response
    [Fact]
    public async Task GetPersonByPersonId_ShouldReturnPersonResponse_ValidPersonId()
    {
        Person personMockObject = fixture.Build<Person>()
            .Without(c => c.Country)
            .Create();

        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(personMockObject);

        PersonResponse? person = await personsService.GetPersonByPersonIdAsync(Guid.NewGuid());

        person.Should().NotBeNull();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region DeletePerson
    // when null person id, throw argument null exception
    [Fact]
    public async Task DeletePerson_ShouldThrowArgumentNullException_NullPersonId()
    {
        Func<Task> action = async () =>
        {
            await personsService.DeletePersonAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when invalid person id, return false
    [Fact]
    public async Task DeletePerson_ShouldReturnFalse_IfInvalidPersonId()
    {
        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as Person);

        bool isDeleted = await personsService.DeletePersonAsync(Guid.NewGuid());

        isDeleted.Should().BeFalse();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.Verify(method => method.DeletePersonAsync(It.IsAny<Guid?>()), Times.Never);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    // when valid person id, return true
    [Fact]
    public async Task DeletePerson_ShouldReturnTrue_IfValidPersonId()
    {
        Person personMock = fixture.Build<Person>()
            .Without(c => c.Country)
            .Create();

        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(personMock);

        personsReposiotryMock.Setup(method => method.DeletePersonAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(true);

        bool isDeleted = await personsService.DeletePersonAsync(Guid.NewGuid());

        isDeleted.Should().BeTrue();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.Verify(method => method.DeletePersonAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetFilteredPersons
    [Fact]
    public async Task GetFilteredPersons_ShouldReturnAllPersons_IfEmptySearchString()
    {
        List<Person> personsMockObject = fixture.Build<Person>()
            .With(prop => prop.Email, "ashish@gmail.com")
            .Without(prop => prop.Country)
            .CreateMany(5)
            .ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync()).ReturnsAsync(personsMockObject);

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersonsAsync(nameof(PersonResponse.PersonName), "");

        filteredPersons.Count.Should().Be(5);

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetFilteredPersons_ShouldReturnAPerson_IfSearchString()
    {
        List<Person> personsMockObject = fixture.Build<Person>()
            .With(prop => prop.Email, "ashish@gmail.com")
            .Without(prop => prop.Country)
            .CreateMany(1)
            .ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync()).ReturnsAsync(personsMockObject);

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersonsAsync(nameof(PersonResponse.Email), "Ash");

        filteredPersons.Count.Should().Be(1);

        filteredPersons.Should().OnlyContain(p => p.Email.Contains("ash", StringComparison.OrdinalIgnoreCase));

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetSortedPersons
    [Theory]
    [InlineData(SortOrder.ASCENDING)]
    [InlineData(SortOrder.DESCENDING)]
    public void GetSortedPersons_SortedPersonsResponse(SortOrder sortOrder)
    {
        List<PersonResponse> personsResponseMockObject = fixture.Build<PersonResponse>()
            .CreateMany(10)
            .ToList();

        List<PersonResponse> sortedPersons = personsService.GetSortedPersonsAsync(personsResponseMockObject, nameof(PersonResponse.PersonName), sortOrder);

        if (sortOrder == SortOrder.ASCENDING)
        {
            sortedPersons.Should().BeInAscendingOrder(p => p.PersonName);
        }
        else
        {
            sortedPersons.Should().BeInDescendingOrder(p => p.PersonName);
        }

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_ThrowArgumentNullException_NullRequest()
    {
        Func<Task> action = async () =>
        {
            await personsService.UpdatePersonAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("ashish", null)]
    [InlineData(null, "ashish@gmail.com")]
    [InlineData("ashish", "ashish")]
    public async Task UpdatePerson_ThrowsArgumentException_NullOrInvalidParameters(string? Name, string? Email)
    {
        PersonUpdateRequest personUpdateRequestMock = fixture.Build<PersonUpdateRequest>()
            .With(prop => prop.PersonName, Name)
            .With(prop => prop.Email, Email)
            .Without(prop => prop.CountryId)
            .Create();

        Func<Task> action = async () =>
        {
            await personsService.UpdatePersonAsync(personUpdateRequestMock);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePerson_ThrowsInvalidOperationException_InvalidPersonId()
    {
        PersonUpdateRequest updateRequestMock = fixture.Build<PersonUpdateRequest>()
            .With(prop => prop.Email, "ashish@gmail.com")
            .Create();

        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdWithTrackingAsync(It.IsAny<Guid?>()))
             .ReturnsAsync(null as Person);

        Func<Task> action = async () =>
        {
            await personsService.UpdatePersonAsync(updateRequestMock);
        };

        await action.Should().ThrowAsync<InvalidDataException>();

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdWithTrackingAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.Verify(method => method.SaveChangesAsync(), Times.Never);

        personsReposiotryMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task UpdatePerson_ReturnsPersonResponse_ValidPersonId()
    {
        Person personMockObject = fixture.Build<Person>()
            .With(prop => prop.PersonName, "ashish")
            .With(prop => prop.Email, "ashish@gmail.com")
            .With(prop => prop.Gender, "MALE")
            .Without(prop => prop.Country)
            .Create();

        personsReposiotryMock.Setup(method => method.GetPersonByPersonIdWithTrackingAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(personMockObject);

        personsReposiotryMock.Setup(method => method.SaveChangesAsync()).Returns(Task.CompletedTask);

        PersonUpdateRequest updateRequest = new()
        {
            PersonId = personMockObject.PersonId,
            PersonName = personMockObject.PersonName!,
            Email = "praveen@gmail.com",
            Gender = (Gender)Enum.Parse(typeof(Gender), personMockObject.Gender!),
            Address = personMockObject.Address,
            CountryId = personMockObject.CountryId,
            DateOfBirth = personMockObject.DateOfBirth
        };

        PersonResponse response = await personsService.UpdatePersonAsync(updateRequest);

        response.Email.Should().Be(updateRequest.Email);

        personsReposiotryMock.Verify(method => method.GetPersonByPersonIdWithTrackingAsync(It.IsAny<Guid?>()), Times.Once);

        personsReposiotryMock.Verify(method => method.SaveChangesAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetPersonsCsv
    [Fact]
    public async Task GetPersonsCsv_ShouldReturnMemoryStream()
    {
        List<Person> personsMockObject = fixture.Build<Person>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .Without(prop => prop.Country)
           .CreateMany(5)
           .ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync()).ReturnsAsync(personsMockObject);

        MemoryStream memoryStream = await personsService.GetPersonsCsv();

        memoryStream.Should().BeOfType<MemoryStream>();

        memoryStream.Should().NotBeNull();

        memoryStream.Length.Should().BeGreaterThan(0);

        using StreamReader reader = new StreamReader(memoryStream);

        string? csv = await reader.ReadToEndAsync();

        csv.Should().Contain("PersonName");

        csv.Should().Contain("Email");

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }

    #endregion

    #region GetPersonsCsvAdvanced
    [Fact]
    public async Task GetPersonsCsvAdvanced_ReturnsMemoryStreamContainsData()
    {
        List<Person> personsMockObject = fixture.Build<Person>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .Without(prop => prop.Country)
           .CreateMany(10)
           .ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync()).ReturnsAsync(personsMockObject);

        MemoryStream memoryStream = await personsService.GetPersonsCsvAdvanced();

        memoryStream.Length.Should().BeGreaterThan(0);

        memoryStream.Should().NotBeNull();

        using StreamReader reader = new StreamReader(memoryStream);

        string? csv = await reader.ReadToEndAsync();

        csv.Should().Contain("PersonName");

        csv.Should().Contain("Email");

        csv.Should().Contain("Gender");

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetPersonsExcel
    [Fact]
    public async Task GetPersonsExcel_ShouldReturnMemoryStream()
    {
        List<Person> personsMockObject = fixture.Build<Person>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .Without(prop => prop.Country)
           .CreateMany(10)
           .ToList();

        personsReposiotryMock.Setup(method => method.GetAllPersonsAsync()).ReturnsAsync(personsMockObject);

        MemoryStream memoryStream = await personsService.GetPersonsExcel();

        memoryStream.Should().NotBeNull();

        memoryStream.Length.Should().BeGreaterThan(0);

        memoryStream.Position = 0;

        ExcelPackage.License.SetNonCommercialPersonal("Tests");

        using ExcelPackage package = new ExcelPackage(memoryStream);

        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

        worksheet.Should().NotBeNull();

        worksheet.Cells["A1"].Text.Should().Be("Person Name");

        worksheet.Cells["B1"].Text.Should().Be("Email");
        
        worksheet.Cells["C1"].Text.Should().Be("Gender");       

        personsReposiotryMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsReposiotryMock.VerifyNoOtherCalls();
    }
    #endregion
}
