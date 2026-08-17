using AutoFixture;
using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactsManager.Tests.UnitTests.ControllerTests;
public sealed class CountriesControllerTests
{
    private readonly ICountriesService countriesService;
    private readonly Mock<ICountriesService> countriesServiceMock;
    private readonly IFixture fixture;
    private readonly ILogger<CountriesController> logger;

    public CountriesControllerTests()
    {
        countriesServiceMock = new Mock<ICountriesService>();

        countriesService = countriesServiceMock.Object;

        logger = new Mock<ILogger<CountriesController>>().Object;

        fixture = new Fixture();
    }

    [Fact]
    public async Task Index_GET_ShouldReturnIndexView()
    {
        List<CountryResponse> allCountries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(allCountries);

        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult = await controller.Index();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeEquivalentTo(allCountries);

        result.ViewName.Should().Be("Index");

        countriesServiceMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Create_GET_ShouldReturnCreateView()
    {
        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult =  controller.Create();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_POST_ShouldReturnCreateViewIfModelStateErrors()
    {
        CountriesController controller = new CountriesController(countriesService, logger);

        controller.ModelState.AddModelError("CountryName", "CountryName is not valid");

        CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

        IActionResult actionResult = await controller.Create(countryAddRequest);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeAssignableTo<CountryAddRequest>();

        result.ViewData.Model.Should().BeEquivalentTo(countryAddRequest);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_POST_ShouldReturnRedirectToActionWhenNoModelStateErrors()
    {
        CountryResponse countryResponse = fixture.Create<CountryResponse>();

        countriesServiceMock.Setup(method => method.AddCountry(It.IsAny<CountryAddRequest>()))
            .ReturnsAsync(countryResponse);

        CountriesController controller = new CountriesController(countriesService, logger);

        CountryAddRequest countryAddRequest = fixture.Create<CountryAddRequest>();

        IActionResult actionResult = await controller.Create(countryAddRequest);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.AddCountry(It.IsAny<CountryAddRequest>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task Delete_GET_ShouldReturnRedirectToActionResultIfNullCountry()
    {
        countriesServiceMock.Setup(method => method.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(null as CountryResponse);

        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.GetCountryByCountryId(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();

    }

    [Fact]
    public async Task Delete_GET_ShouldReturnViewResultIfCountryExists()
    {
        CountryResponse countryResponse = fixture.Create<CountryResponse>();

        countriesServiceMock.Setup(method => method.GetCountryByCountryId(It.IsAny<Guid>()))
            .ReturnsAsync(countryResponse);

        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult = await controller.Delete(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeAssignableTo<CountryResponse>();

        result.ViewData.Model.Should().BeEquivalentTo(countryResponse);

        result.ViewName.Should().Be("Delete");

        countriesServiceMock.Verify(method => method.GetCountryByCountryId(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteConfirmed_POST_ShouldReturnRedirectToActionIfCountryExists()
    {
        countriesServiceMock.Setup(method => method.DeleteCountry(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult = await controller.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.DeleteCountry(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteConfirmed_POST_ShouldReturnRedirectToActionIfNoCountryExists()
    {
        countriesServiceMock.Setup(method => method.DeleteCountry(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        CountriesController controller = new CountriesController(countriesService, logger);

        IActionResult actionResult = await controller.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.DeleteCountry(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }
}
