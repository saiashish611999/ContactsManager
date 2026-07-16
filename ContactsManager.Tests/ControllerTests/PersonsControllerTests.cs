using AutoFixture;
using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.DataTranferObjects.PersonDtos;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    #endregion
}
