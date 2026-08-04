using AutoFixture;
using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;

namespace ContactsManager.Tests.UnitTests.ServiceTests;
public class CountriesServiceTests
{
    private readonly IFixture fixture;
    private readonly Mock<ICountriesRepository> countriesRepositoryMock;
    private readonly ICountriesRepository countriesRepository;
    private readonly ICountriesService countriesService;

    public CountriesServiceTests()
    {
        fixture = new Fixture();

        countriesRepositoryMock = new Mock<ICountriesRepository>();

        countriesRepository = countriesRepositoryMock.Object;

        this.countriesService = new CountriesService(countriesRepository); 
    }

    #region AddCountry
    // when null request, throw argument null exception
    [Fact]
    public async Task AddCountry_ShouldThrowArgumentNullException_WhenNullRequest()
    {
        Func<Task> action = async () =>
        {
            await countriesService.AddCountry(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when null name, throw argument null exception
    [Fact]
    public async Task AddCountry_ShouldThrowArgumentException_WhenNullCountryName()
    {
        CountryAddRequest? countryAddRequestObject = fixture.Build<CountryAddRequest>()
            .With(prop => prop.CountryName, null as string)
            .Create();

        Func<Task> action = async () =>
        {
            await countriesService.AddCountry(countryAddRequestObject);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when duplicate name, throw argument exception
    [Fact]
    public async Task AddCountry_ShouldThrowArgumentException_WhenDuplicateCountryName()
    {
        CountryAddRequest countryAddRequestObject = fixture.Create<CountryAddRequest>();

        countriesRepositoryMock.Setup(method => method.IsCountryExists(It.IsAny<string?>()))
            .ReturnsAsync(true);

        Func<Task> action = async () =>
        {
            await countriesService.AddCountry(countryAddRequestObject);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        countriesRepositoryMock.Verify(method => method.IsCountryExists(It.IsAny<string?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when valid request, return country response
    [Fact]
    public async Task AddCountry_ShouldReturnCountryResponse_WhenValidRequest()
    {
        CountryAddRequest countryAddRequestObject = fixture.Create<CountryAddRequest>();

        Country countryObject = fixture.Create<Country>();

        countriesRepositoryMock.Setup(method => method.IsCountryExists(It.IsAny<string?>()))
            .ReturnsAsync(false);

        countriesRepositoryMock.Setup(method => method.AddCountry(It.IsAny<Country>()))
            .ReturnsAsync(countryObject);

        CountryResponse response = await countriesService.AddCountry(countryAddRequestObject);

        response.CountryId.Should().Be(countryObject.CountryId);

        response.CountryName.Should().Be(countryObject.CountryName);

        countriesRepositoryMock.Verify(method => method.IsCountryExists(It.IsAny<string?>()), Times.Once);

        countriesRepositoryMock.Verify(method => method.AddCountry(It.IsAny<Country>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetAllCountries
    // when no countries, return empty list
    [Fact]
    public async Task GetAllCountries_ShouldReturnEmptyList_WhenNoCountries()
    {
        countriesRepositoryMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(new List<Country>());

        List<CountryResponse> countries = await countriesService.GetAllCountries();

        countries.Should().BeEmpty();

        countries.Count().Should().Be(0);

        countriesRepositoryMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when countries exists, return list of countries
    [Fact]
    public async Task GetAllCountries_ShouldReturnListOfCountries_WhenCountriesExists()
    {
        List<Country> countriesList = fixture.Create<List<Country>>();

        countriesRepositoryMock.Setup(method => method.GetAllCountries())
            .ReturnsAsync(countriesList);

        List<CountryResponse> response = await countriesService.GetAllCountries();

        response.Count().Should().Be(countriesList.Count());

        response.Should().NotBeEmpty();

        countriesRepositoryMock.Verify(method => method.GetAllCountries(), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetCountryByCountryId
    // when null id, throw argument null exception
    [Fact]
    public async Task GetCountryByCountryId_ShouldThrowArgumentNullException_WhenNullId()
    {
        Func<Task> action = async () =>
        {
            await countriesService.GetCountryByCountryId(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when invalid id, return null
    [Fact]
    public async Task GetCountryByCountryId_ShouldReturnNull_WhenInvalidId()
    {
        countriesRepositoryMock.Setup(method => method.GetCountryByCountryId(It.IsAny<Guid?>()))
            .ReturnsAsync(null as Country);

        CountryResponse? response = await countriesService.GetCountryByCountryId(Guid.NewGuid());

        response.Should().BeNull();

        countriesRepositoryMock.Verify(method => method.GetCountryByCountryId(It.IsAny<Guid?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when valid id, retur country response
    [Fact]
    public async Task GetCountryByCountryId_ShouldReturnCountryResponse_WhenValidCountryId()
    {
        Country countryObject = fixture.Create<Country>();

        countriesRepositoryMock.Setup(method => method.GetCountryByCountryId(It.IsAny<Guid?>()))
            .ReturnsAsync(countryObject);

        CountryResponse? response = await countriesService.GetCountryByCountryId(Guid.NewGuid());

        response.Should().NotBeNull();

        response.CountryId.Should().Be(countryObject.CountryId);

        response.CountryName.Should().Be(countryObject.CountryName);

        countriesRepositoryMock.Verify(method => method.GetCountryByCountryId(It.IsAny<Guid?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion
}
