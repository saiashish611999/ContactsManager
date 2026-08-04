using AutoFixture;
using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;

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


}
