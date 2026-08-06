using AutoFixture;
using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Extensions;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;

namespace ContactsManager.Tests.UnitTests.ServiceTests;
public sealed class PersonsServiceTests
{
    private readonly IPersonsService personsService;
    private readonly Mock<IPersonsRepository> personsRepositoryMock;
    private readonly IPersonsRepository personsRepository;
    private readonly IFixture fixture;

    public PersonsServiceTests()
    {
        fixture = new Fixture();

        personsRepositoryMock = new Mock<IPersonsRepository>();

        personsRepository = personsRepositoryMock.Object;

        personsService = new PersonsService(personsRepository);
    }

    #region AddPerson
    // when null request, throw argument null exception
    [Fact]
    public async Task AddPerson_ShouldThrowArgumentNullExcepetion_WhenNullRequest()
    {
        Func<Task> action = async () =>
        {
            await personsService.AddPerson(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    // when null names, throw argumen exception
    [Theory]
    [InlineData("ashish", null)]
    [InlineData(null, "ashish@gmail.com")]
    [InlineData("ashish", "ashish")]
    public async Task AddPerson_ShouldThrowArgumentException_IfImproperDetails(string? personName, string? email)
    {
        PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            .With(x => x.PersonName, personName)
            .With(x => x.EmailAddress, email)
            .Create();

        Func<Task> action = async () =>
        {
            await personsService.AddPerson(personAddRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    // when proper details, return response
    [Fact]
    public async Task AddPerson_ShouldReturnResponse_WhenProperDetails()
    {
        PersonAddRequest personAddRequest = fixture.Build<PersonAddRequest>()
            .With(x => x.EmailAddress, "ashish@gmail.com")
            .Create();

        Person person = fixture.Build<Person>()
            .With(x => x.EmailAddress, personAddRequest.EmailAddress)
            .Without(x => x.Country)
            .Create();

        personsRepositoryMock.Setup(method => method.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

        PersonResponse response = await personsService.AddPerson(personAddRequest);

        response.PersonId.Should().Be(person.PersonId);

        response.PersonName.Should().Be(person.PersonName);

        response.EmailAddress.Should().Be(person.EmailAddress);

        response.DateOfBirth.Should().Be(person.DateOfBirth);

        response.Gender.Should().Be(person.Gender);

        response.CountryId.Should().Be(person.CountryId);

        response.Address.Should().Be(person.Address);

        response.ReceivesNewsLetters.Should().Be(person.ReceivesNewsLetters);

        personsRepositoryMock.Verify(method => method.AddPerson(It.IsAny<Person>()), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetAllPersons
    [Fact]
    public async Task GetAllPersons_ShouldReturnEmptyObject_IfNoPersons()
    {
        personsRepositoryMock.Setup(method => method.GetAllPersons())
            .ReturnsAsync(new List<Person>());

        List<PersonResponse> persons = await personsService.GetAllPersons();

        persons.Should().BeEmpty();

        personsRepositoryMock.Verify(method => method.GetAllPersons(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllPersons_ShouldReturnPersonsList_IfPersonsExists()
    {
        List<Person> persons = fixture.Build<Person>()
            .Without(p => p.Country)
            .CreateMany(5)
            .ToList();

        personsRepositoryMock.Setup(method => method.GetAllPersons())
            .ReturnsAsync(persons);

        List<PersonResponse> response = await personsService.GetAllPersons();

        response.Should().NotBeEmpty();

        response.Count().Should().Be(5);

        personsRepositoryMock.Verify(method => method.GetAllPersons(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetPersonByPersonId
    // when null person id, throw argument null exception
    [Fact]
    public async Task GetPersonByPersonId_ShouldThrowArgumentNullException_IfNullPersonId()
    {
        Func<Task> action = async () =>
        {
            await personsService.GetPersonByPersonId(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    // when invalid person id, return null
    [Fact]
    public async Task GetPersonByPersonId_ShouldReturnNull_WhenInvalidPersonId()
    {
        personsRepositoryMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid?>()))
            .ReturnsAsync(null as Person);

        PersonResponse? response = await personsService.GetPersonByPersonId(Guid.NewGuid());

        response.Should().BeNull();

        personsRepositoryMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid?>()), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    // when valid person id, return response
    [Fact]
    public async Task GetPersonByPersonId_ShouldReturnResponse_WhenValidPersonId()
    {
        Person person = fixture.Build<Person>()
            .With(p => p.EmailAddress, "ashish@gmail.com")
            .Without(p => p.Country)
            .Create();

        personsRepositoryMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid?>()))
            .ReturnsAsync(person);

        PersonResponse? response = await personsService.GetPersonByPersonId(Guid.NewGuid());

        response.Should().NotBeNull();

        response.PersonId.Should().Be(person.PersonId);

        response.PersonName.Should().Be(person.PersonName);

        response.EmailAddress.Should().Be(person.EmailAddress);

        response.DateOfBirth.Should().Be(person.DateOfBirth);

        response.Gender.Should().Be(person.Gender);

        response.CountryId.Should().Be(person.CountryId);

        response.Address.Should().Be(person.Address);

        response.ReceivesNewsLetters.Should().Be(person.ReceivesNewsLetters);

        personsRepositoryMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid?>()), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetFilteredPersons
    // when null search string, it shoudl return allpersons
    [Fact]
    public async Task GetFilteredPersons_ShouldReturnAllPersons_WhenNullSearchString()
    {
        List<Person> persons = fixture.Build<List<Person>>().Create();

        personsRepositoryMock.Setup(method => method.GetAllPersons())
            .ReturnsAsync(persons);

        List<PersonResponse> filteredPersons = await personsService.GetFilteredPersons(null, null);

        filteredPersons.Count().Should().Be(persons.Count());

        filteredPersons.Should().BeEquivalentTo(persons.Select(person => person.AsPersonResposne()));

        personsRepositoryMock.Verify(method => method.GetAllPersons(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();

    }

    // when search string, it should return filtered persons
    [Fact]
    public async Task GetFilteredPersons_ShouldReturnFilteredPersons_WhenSearchString()
    {
        List<Person> persons = new List<Person>()
    {
        fixture.Build<Person>()
            .With(p => p.PersonName, "Sai Ashish")
            .Without(p => p.Country)
            .Create(),

        fixture.Build<Person>()
            .With(p => p.PersonName, "Praveen")
            .Without(p => p.Country)
            .Create()
    };

        personsRepositoryMock.Setup(method => method.GetAllPersons())
            .ReturnsAsync(persons);

        List<PersonResponse> filteredPersons = await personsService
            .GetFilteredPersons(nameof(PersonResponse.PersonName), "Sai");

        filteredPersons.Count().Should().Be(1);

        filteredPersons[0].PersonName.Should().Be(persons[0].PersonName);

        personsRepositoryMock.Verify(method => method.GetAllPersons(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetSortedPersons
    [Theory]
    [InlineData(SortOrder.ASCENDING)]
    [InlineData(SortOrder.DESCENDING)]
    public void GetSortedPersons_ShouldReturnPersonsInOrder_WhenSortOrder(SortOrder sortOrder)
    {
        List<PersonResponse> persons = fixture.Create<List<PersonResponse>>();

        List<PersonResponse> personResponses = personsService.GetSortedPersons(persons, nameof(PersonResponse.PersonName), sortOrder);

        if (sortOrder == SortOrder.ASCENDING)
        {
            personResponses.Should().BeInAscendingOrder(p => p.PersonName);
        }
        else
        {
            personResponses.Should().BeInDescendingOrder(p => p.PersonName);
        }

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region UpdatePerson
    [Fact]
    public async Task UpdatePerson_ShouldThrowsArgumentNullException_IfNullUpdateRequest()
    {
        Func<Task> action = async () =>
        {
            await personsService.UpdatePerson(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    // when null names, throw argumen exception
    [Theory]
    [InlineData("ashish", null)]
    [InlineData(null, "ashish@gmail.com")]
    [InlineData("ashish", "ashish")]
    public async Task UpdatePerson_ShouldThrowArgumentException_IfImproperDetails(string? personName, string? email)
    {
        PersonUpdateRequest personUpdateRequest = fixture.Build<PersonUpdateRequest>()
            .With(x => x.PersonName, personName)
            .With(x => x.EmailAddress, email)
            .Create();

        Func<Task> action = async () =>
        {
            await personsService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePerson_ShouldThrowInvalidDataException_IfInvalidPersonId()
    {
        PersonUpdateRequest personUpdateRequest = fixture.Build<PersonUpdateRequest>()
            .With(x => x.EmailAddress,"ashish@gmail.com" )
            .Create();

        personsRepositoryMock.Setup(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()))
            .ReturnsAsync(null as Person);

        Func<Task> action = async () =>
        {
            await personsService.UpdatePerson(personUpdateRequest);
        };

        await action.Should().ThrowAsync<InvalidDataException>();

        personsRepositoryMock.Verify(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdatePerson_ShouldReturnResponse_IfValidPerson()
    {
        PersonUpdateRequest personUpdateRequest = fixture.Build<PersonUpdateRequest>()
            .With(x => x.EmailAddress, "ashish@gmail.com")
            .Create();

        Person person = fixture.Build<Person>()
            .Without(p => p.Country)
            .Create();

        personsRepositoryMock.Setup(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        personsRepositoryMock.Setup(method => method.SaveChanges()).Verifiable();

        PersonResponse personResponse = await personsService.UpdatePerson(personUpdateRequest);

        personResponse.PersonId.Should().Be(person.PersonId);

        personsRepositoryMock.Verify(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()), Times.Once);

        personsRepositoryMock.Verify(method => method.SaveChanges(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    #endregion

    #region DeletePerson
    [Fact]
    public async Task DeletePerson_ShouldThrowArgumentNullException_WhenNullPersonId()
    {
        Func<Task> action = async () =>
        {
            await personsService.DeletePerson(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeletePerson_ShouldReturnFalse_WhenInvalidPersonId()
    {
        personsRepositoryMock.Setup(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()))
            .ReturnsAsync(null as Person);

        bool isDeleted = await personsService.DeletePerson(Guid.NewGuid());

        isDeleted.Should().BeFalse();

        personsRepositoryMock.Verify(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeletePerson_ShouldReturnTrue_WhenValidPersonId()
    {
        Person person = fixture.Build<Person>()
            .Without(p => p.Country)
            .Create();

        personsRepositoryMock.Setup(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()))
            .ReturnsAsync(person);

        personsRepositoryMock.Setup(method => method.DeletePerson(It.IsAny<Person>())).Verifiable();

        personsRepositoryMock.Setup(method => method.SaveChanges()).Verifiable();

        bool isDeleted = await personsService.DeletePerson(Guid.NewGuid());

        isDeleted.Should().BeTrue();

        personsRepositoryMock.Verify(method => method.GetPersonByPersonIdWithTracking(It.IsAny<Guid>()), Times.Once);

        personsRepositoryMock.Verify(method => method.DeletePerson(It.IsAny<Person>()), Times.Once);

        personsRepositoryMock.Verify(method => method.SaveChanges(), Times.Once);

        personsRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion


}
