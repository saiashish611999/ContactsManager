using AutoFixture;
using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rotativa.AspNetCore;

namespace ContactsManager.Tests.ControllerTests;
public sealed class PersonsControllerTests
{
    private readonly IPersonsService personsService;
    private readonly Mock<IPersonsService> personsServiceMock;
    private readonly ICountriesService countriesService;
    private readonly Mock<ICountriesService> countriesServiceMock;
    private readonly IFixture fixture;

    public PersonsControllerTests()
    {
        fixture = new Fixture();

        personsServiceMock = new Mock<IPersonsService>();

        personsService = personsServiceMock.Object;

        countriesServiceMock = new Mock<ICountriesService>();

        countriesService = countriesServiceMock.Object;
    }

    #region Index
    [Fact]
    public async Task Index_Get_ReturnsView()
    {
        List<PersonResponse> personsObject = fixture.Build<PersonResponse>()
            .Without(prop => prop.CountryId)
            .CreateMany(5)
            .ToList();

        personsServiceMock.Setup(method => method.GetFilteredPersonsAsync(It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(personsObject);

        personsServiceMock.Setup(method => method.GetSortedPersonsAsync(It.IsAny<List<PersonResponse>>(),
            It.IsAny<string?>()
            , It.IsAny<SortOrder>()))
             .Returns(personsObject.OrderBy(p => p.PersonName).ToList());

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Index(
            fixture.Create<string?>(),
            fixture.Create<string?>(),
            fixture.Create<SortOrder>(),
            fixture.Create<string?>());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeEquivalentTo(personsObject.OrderBy(p => p.PersonName).ToList());

        result.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();

        personsServiceMock.Verify(method => method.GetFilteredPersonsAsync(It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);

        personsServiceMock.Verify(method => method.GetSortedPersonsAsync(
            It.IsAny<List<PersonResponse>>(),
            It.IsAny<string?>(),
            It.IsAny<SortOrder>())
        ,Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Create
    [Fact]
    public async Task Create_Get_ReturnsCreateView()
    {
        List<CountryResponse> countriesObject = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countriesObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Create();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_Post_ReturnsCreateViewIfModelStateErrors()
    {
        List<CountryResponse> countriesObject = fixture.Create<List<CountryResponse>>();

        PersonAddRequest personAddRequestObject = fixture.Build<PersonAddRequest>()
            .With(prop => prop.Email, "saiashish@gmail.com")
            .With(prop => prop.Gender, Gender.MALE)
            .Create();

        countriesServiceMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countriesObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        personsController.ModelState.AddModelError("PersonName", "Person Name is not valid");

        IActionResult actionResult = await personsController.Create(personAddRequestObject);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().Be(personAddRequestObject);

        result.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();

        result.ViewName.Should().Be("Create");

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_Post_ReturnsRedirectToActionIfNoModelError()
    {
        PersonResponse person = fixture.Create<PersonResponse>();

        PersonAddRequest personAddRequestObject = fixture.Build<PersonAddRequest>()
           .With(prop => prop.Email, "saiashish@gmail.com")
           .With(prop => prop.Gender, Gender.MALE)
           .Create();

        personsServiceMock.Setup(method => method.AddPersonAsync(It.IsAny<PersonAddRequest?>()))
            .ReturnsAsync(person);

        PersonsController personsController = new PersonsController(personsService, countriesService);       

        IActionResult actionResult = await personsController.Create(personAddRequestObject);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Never);

        personsServiceMock.Verify(method => method.AddPersonAsync(It.IsAny<PersonAddRequest?>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Update
    [Fact]
    public async Task Update_Get_ReturnRedirectToActionIfPersonIsNull()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Update(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_Get_ReturnViewIfPersonExists()
    {
        List<CountryResponse> countriesObject = fixture.Create<List<CountryResponse>>();

        PersonResponse personResponseObject = fixture.Build<PersonResponse>()
            .With(prop => prop.Email, "ashish@gmail.com")
            .With(prop => prop.Gender, Gender.MALE.ToString())
            .Create();

        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(personResponseObject);

        countriesServiceMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countriesObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Update(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Update");

        result.ViewData.Model.Should().BeEquivalentTo(personResponseObject.ToUpdateRequest());

        result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_Post_ReturnsViewIfModelStateIsInvalid()
    {
        List<CountryResponse> countriesObject = fixture.Create<List<CountryResponse>>();

        PersonUpdateRequest updateRequestObject = fixture.Build<PersonUpdateRequest>()
            .With(prop => prop.Email, "ashish@gmail.com")
            .With(prop => prop.Gender, Gender.MALE)
            .Create();

        countriesServiceMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countriesObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        personsController.ModelState.AddModelError("PersonName", "invalid data");

        IActionResult actionResult = await personsController.Update(Guid.NewGuid(), updateRequestObject);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Update");

        result.ViewData.Model.Should().BeEquivalentTo(updateRequestObject);

        result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_Post_ReturnsRedirectToActionIfPersonIsNull()
    {
        PersonUpdateRequest updateRequestObject = fixture.Build<PersonUpdateRequest>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .With(prop => prop.Gender, Gender.MALE)
           .Create();

        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Update(Guid.NewGuid(), updateRequestObject);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Update_Post_ReturnsRedirectToActionIfPersonExists()
    {
        PersonResponse personResponseObject = fixture.Build<PersonResponse>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .With(prop => prop.Gender, Gender.MALE.ToString())
           .Create();

        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
           .ReturnsAsync(personResponseObject);

        personsServiceMock.Setup(method => method.UpdatePersonAsync(It.IsAny<PersonUpdateRequest>()))
            .ReturnsAsync(personResponseObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Update(Guid.NewGuid(), personResponseObject.ToUpdateRequest());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        countriesServiceMock.VerifyNoOtherCalls();

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.Verify(method => method.UpdatePersonAsync(It.IsAny<PersonUpdateRequest>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_Get_ReturnsRedirectToActionIfNoPersonExists()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Delete(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Get_ReturnsRedirectToActionIfPersonExists()
    {
        PersonResponse personResponseObject = fixture.Build<PersonResponse>()
           .With(prop => prop.Email, "ashish@gmail.com")
           .With(prop => prop.Gender, Gender.MALE.ToString())
           .Create();

        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(personResponseObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.Delete(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Delete");

        result.ViewData.Model.Should().BeEquivalentTo(personResponseObject.ToUpdateRequest());

        result.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Post_ReturnsRedirectToActionIfNoPersonExists()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as PersonResponse);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Post_ReturnsRedirectToActionIfPersonExists()
    {
        personsServiceMock.Setup(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as PersonResponse);

        personsServiceMock.Setup(method => method.DeletePersonAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(true);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult = await personsController.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Persons");

        personsServiceMock.Verify(method => method.GetPersonByPersonIdAsync(It.IsAny<Guid?>()), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetPersonsPdf
    [Fact]
    public async Task GetPersonsPdf_Get_ReturnsViewAsPdf()
    {
        List<PersonResponse> personsObject = fixture.Create<List<PersonResponse>>();

        personsServiceMock.Setup(method => method.GetAllPersonsAsync())
            .ReturnsAsync(personsObject);

        PersonsController personsController = new PersonsController(personsService, countriesService);

        IActionResult actionResult =  await personsController.GetPersonsPdf();

        ViewAsPdf result = Assert.IsType<ViewAsPdf>(actionResult);

        result.ViewName.Should().Be("PersonsPDF");

        result.ViewData.Model.Should().BeAssignableTo<List<PersonResponse>>();

        result.ViewData.Model.Should().BeEquivalentTo(personsObject);

        personsServiceMock.Verify(method => method.GetAllPersonsAsync(), Times.Once);

        personsServiceMock.VerifyNoOtherCalls();

        countriesServiceMock.VerifyNoOtherCalls();

    }
    #endregion
}
