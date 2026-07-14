using AutoFixture;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;

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
}
