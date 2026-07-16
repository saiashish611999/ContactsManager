using AutoFixture;
using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactsManager.Tests.ControllerTests;
public sealed class CountriesControllerTests
{
    private readonly ICountriesService countriesService;
    private readonly Mock<ICountriesService> countriesServiceMock;
    private readonly IFixture fixture;

    public CountriesControllerTests()
    {
        fixture = new Fixture();

        countriesServiceMock = new Mock<ICountriesService>();

        countriesService = countriesServiceMock.Object;
    }

    #region Index
    [Fact]
    public async Task Index_ShouldReturnIndexView()
    {
        List<CountryResponse> countries = fixture.Create<List<CountryResponse>>();

        countriesServiceMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countries);

        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = await countriesController.Index();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().BeAssignableTo<List<CountryResponse>>();

        result.ViewData.Model.Should().Be(countries);

        countriesServiceMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Create
    [Fact]
    public void Create_Get_ReturnsView()
    {
        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = countriesController.Create();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_Post_ReturnsViewIfModelStateIsNotValid()
    {
        CountryAddRequest countryAddRequestObject = fixture.Create<CountryAddRequest>();

        CountriesController countriesController = new CountriesController(countriesService);

        countriesController.ModelState.AddModelError("CountryName", "Country Name is not valid");

        IActionResult actionResult = await countriesController.Create(countryAddRequestObject);

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Create");

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_Post_ReturnsRedirectToActionIfModelStateIsValid()
    {
        CountryAddRequest countryAddRequestObject = fixture.Create<CountryAddRequest>();

        CountryResponse countryResponseObject = fixture.Create<CountryResponse>();

        countriesServiceMock.Setup(method => method.AddCountryAsync(It.IsAny<CountryAddRequest?>()))
            .ReturnsAsync(countryResponseObject);

        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = await countriesController.Create(countryAddRequestObject);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.AddCountryAsync(It.IsAny<CountryAddRequest>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_Get_ReturnsInvalidDataExceptionIfInvalidCuntryId()
    {
        countriesServiceMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as CountryResponse);

        CountriesController countriesController = new CountriesController(countriesService);

        Func<Task> action = async () =>
        {
            await countriesController.Delete(Guid.NewGuid());
        };

        await action.Should().ThrowAsync<InvalidDataException>();

        countriesServiceMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Get_ReturnsViewIfValidCountryId()
    {
        CountryResponse countryResponseObject = fixture.Create<CountryResponse>();

        countriesServiceMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(countryResponseObject);

        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = await countriesController.Delete(Guid.NewGuid());

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewData.Model.Should().Be(countryResponseObject);

        result.ViewData.Model.Should().BeAssignableTo<CountryResponse>();

        countriesServiceMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Post_IfInvalidCountryIdReturnRedirectToActionIndex()
    {
        countriesServiceMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(null as CountryResponse);

        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = await countriesController.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_Post_IfVlaidCountryIdReturnRedirectToActionIndex()
    {
        CountryResponse countryResponseObject = fixture.Create<CountryResponse>();

        countriesServiceMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(countryResponseObject);

        countriesServiceMock.Setup(method => method.DeleteCountryAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = await countriesController.DeleteConfirmed(Guid.NewGuid());

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(actionResult);

        result.ActionName.Should().Be("Index");

        result.ControllerName.Should().Be("Countries");

        countriesServiceMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.Verify(method => method.DeleteCountryAsync(It.IsAny<Guid>()), Times.Once);

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Upload
    [Fact]
    public void Upload_Get_ReturnsUploadView()
    {
        CountriesController countriesController = new CountriesController(countriesService);

        IActionResult actionResult = countriesController.UploadCountries();

        ViewResult result = Assert.IsType<ViewResult>(actionResult);

        result.ViewName.Should().Be("Upload");

        countriesServiceMock.VerifyNoOtherCalls();
    }
    #endregion
}
