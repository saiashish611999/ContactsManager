using AutoFixture;
using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.DataTransferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactsManager.Tests.UnitTests.ControllerTests;
public sealed class PersonsControllerTests
{
    private readonly ICountriesService countriesService;
    private readonly Mock<ICountriesService> countriesServiceMock;
    private readonly IPersonsService personsService;
    private readonly Mock<IPersonsService> personsServiceMock;
    private readonly IFixture fixture;
    private readonly ILogger<PersonsController> logger;

    public PersonsControllerTests()
    {
        countriesServiceMock = new Mock<ICountriesService>();

        countriesService = countriesServiceMock.Object;

        personsServiceMock = new Mock<IPersonsService>();

        personsService = personsServiceMock.Object;

        fixture = new Fixture();

        logger = new Mock<ILogger<PersonsController>>().Object;
    }

    #region Index

    [Fact]
    public async Task Index_GET_ShouldReturnViewResult()
    {
        List<PersonResponse> filteredPersons = fixture.Create<List<PersonResponse>>();

        personsServiceMock.Setup(method => method.GetFilteredPersons(It.IsAny<string?>(), It.IsAny<string>()))
            .ReturnsAsync(filteredPersons);

        List<PersonResponse> sortedPersons = filteredPersons.OrderByDescending(person => person.PersonName).ToList();

        personsServiceMock.Setup(method => method.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string?> (), It.IsAny<SortOrder>()))
            .Returns(sortedPersons);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Index(
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<SortOrder>());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();

        result.ViewData.Model.Should().BeEquivalentTo(sortedPersons);

        result.ViewName.Should().Be("Index");

        personsServiceMock.Verify(method => method.GetFilteredPersons(It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);

        personsServiceMock.Verify(method => method.GetSortedPersons(It.IsAny<List<PersonResponse>>(),It.IsAny<string?>(), It.IsAny<SortOrder>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();

    }

    #endregion

    #region Create
    [Fact]
    public async Task Create_GET_ShouldReturnViewResult()
    {
        List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(countries);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Create();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_POST_ShouldReturnViewResultIfModelStateIsNotValid()
    {
        List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(countries);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        controller.ModelState.AddModelError("PersonName", "Person Name is not valid");

        PersonAddRequest personAddRequest = fixture.Create<PersonAddRequest>();

        IActionResult actionResult = await controller.Create(personAddRequest);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        result.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

        result.ViewData.Model.Should().BeEquivalentTo(personAddRequest);

        countriesServiceMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_POST_ShouldReturnRedirectToActionResult()
    {
        personsServiceMock.Setup(method => method.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(fixture.Create<PersonResponse>());

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        PersonAddRequest personAddRequest = fixture.Create<PersonAddRequest>();

        IActionResult actionResult = await controller.Create(personAddRequest);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.AddPerson(It.IsAny<PersonAddRequest>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Update
    [Fact]
    public async Task Update_GET_ShouldReturnRedirectToActionIfPersonDoesntExists()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Update(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_GET_ShouldReturnViewResultIfPersonExists()
    {
        PersonResponse personResponse = fixture.Create<PersonResponse>();

        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(personResponse);

        List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(countries);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Update(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Update");

        result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.Verify(method => method.GetAllCountries(), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task Update_POST_ShouldReturnViewResultIfModelStateError()
    {
        List<CountryResponse> allCountries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(allCountries);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        controller.ModelState.AddModelError("PersonName", "Person Name is invalid");

        PersonUpdateRequest personUpdateRequest = fixture.Create<PersonUpdateRequest>();

        IActionResult actionResult = await controller.Update(Guid.NewGuid(), personUpdateRequest);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Update");

        result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

        result.ViewData.Model.Should().BeEquivalentTo(personUpdateRequest);

        countriesServiceMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task Update_POST_ShouldReturnRedirectToActionResultIfNoModelStateErrors()
    {
        PersonResponse personResponse = fixture.Create<PersonResponse>();

        personsServiceMock.Setup(method => method.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
            .ReturnsAsync(personResponse);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        PersonUpdateRequest personUpdateRequest = fixture.Create<PersonUpdateRequest>();

        IActionResult actionResult = await controller.Update(Guid.NewGuid(), personUpdateRequest);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.UpdatePerson(It.IsAny<PersonUpdateRequest>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();


    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_GET_ShouldReturnRedirectToActionIfNullPerson()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);
        
        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task Delete_GET_ShouldReturnViewResultIfPersonExists()
    {
        PersonResponse personResponse = fixture.Create<PersonResponse>();

        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(personResponse);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        DeletePersonResponse deletePersonResponse = new DeletePersonResponse()
        {
            PersonId = personResponse.PersonId,
            PersonName = personResponse.PersonName,
            EmailAddress = personResponse.EmailAddress
        };

        result.ViewName.Should().Be("Delete");

        result.ViewData.Model.Should().BeAssignableTo<DeletePersonResponse>();

        result.ViewData.Model.Should().BeEquivalentTo(deletePersonResponse);

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_POST_ShouldReturnRedirectToActionIfPersonIsNull()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_POST_ShouldReturnRedirectToActionIfPersonExists()
    {
        PersonResponse personResponse = fixture.Create<PersonResponse>();

        personsServiceMock.Setup(method => method.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(personResponse);

        personsServiceMock.Setup(method => method.DeletePerson(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        PersonsController controller = new PersonsController(countriesService, personsService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid(), fixture.Create<DeletePersonResponse>());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonId(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.Verify(method => method.DeletePerson(It.IsAny<Guid>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion
}
