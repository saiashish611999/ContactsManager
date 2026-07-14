using AutoFixture;
using ContactsManager.Core.DataTranferObjects.CountryDtos;
using ContactsManager.Core.Entities;
using ContactsManager.Core.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using FluentAssertions;
using Moq;

namespace ContactsManager.Tests.ServiceTests;
public sealed class CountriesServiceTests
{
    private readonly IFixture fixture;
    private readonly ICountriesService countriesService;
    private readonly ICountriesRepository countriesRepository;
    private readonly Mock<ICountriesRepository> countriesRepositoryMock;

    public CountriesServiceTests()
    {
        this.fixture = new Fixture();

        this.countriesRepositoryMock = new Mock<ICountriesRepository>();

        this.countriesRepository = countriesRepositoryMock.Object;

        this.countriesService = new CountriesService(countriesRepository);
    }

    #region AddCountryAsyncTests
    // when null request, throw argument null exception
    [Fact]
    public async Task AddCountry_ShouldThrowArgumentNullException_IfNullRequest()
    {
        Func<Task> action = async () =>
        {
            await countriesService.AddCountryAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when invalid request, throw argument exception
    [Fact]
    public async Task AddCountry_ShouldThrowArgumentException_IfNullName()
    {
        Func<Task> action = async () =>
        {
            await countriesService.AddCountryAsync(new CountryAddRequest() { CountryName = null });

        };

        await action.Should().ThrowAsync<ArgumentException>();

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when country exists, throw argument exception
    [Fact]
    public async Task AddCountry_ShouldArgumentException_IfCountryExists()
    {
        CountryAddRequest requestMock = fixture.Create<CountryAddRequest>();

        countriesRepositoryMock.Setup(method => method.DoesCountryExistAsync(It.IsAny<string?>()))
            .ReturnsAsync(true);

        Func<Task> action = async () =>
        {
            await countriesService.AddCountryAsync(requestMock);
        };

        await action.Should().ThrowAsync<ArgumentException>();

        countriesRepositoryMock.Verify(method => method.DoesCountryExistAsync(It.IsAny<string?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when proper details, return response
    [Fact]
    public async Task AddCountry_ShouldReturnCountryResponse_IfProperDetails()
    {
        CountryAddRequest requestMock = fixture.Create<CountryAddRequest>();

        Country countryMock = requestMock.ToEntity();

        countryMock.CountryId = Guid.NewGuid();

        countriesRepositoryMock.Setup(method => method.DoesCountryExistAsync(It.IsAny<string?>()))
            .ReturnsAsync(false);

        countriesRepositoryMock.Setup(method => method.AddCountryAsync(It.IsAny<Country>()))
            .ReturnsAsync(countryMock);

        CountryResponse response = await countriesService.AddCountryAsync(requestMock);

        response.CountryId.Should().NotBe(Guid.Empty);

        response.CountryName.Should().NotBe(null);

        countriesRepositoryMock.Verify(method => method.DoesCountryExistAsync(It.IsAny<string?>()), Times.Once);

        countriesRepositoryMock.Verify(method => method.AddCountryAsync(It.IsAny<Country>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetAllCountriesAsyncTests
    // when no countries, return empty list
    [Fact]
    public async Task GetAllCountriesAsync_ShouldRetrunEmpty_IfNoCountriesExists()
    {
        countriesRepositoryMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(new List<Country>());

        List<CountryResponse> countries = await countriesService.GetAllCountriesAsync();

        countries.Should().BeEmpty();

        countriesRepositoryMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when countries exist, return list of countries
    [Fact]
    public async Task GetAllCountriesAsync_ShouldReturnListOfCountries_IfCountriesExists()
    {
        List<Country> countriesMock = fixture.Build<Country>().Without(prop =>prop.Persons).CreateMany(10).ToList();

        countriesRepositoryMock.Setup(method => method.GetAllCountriesAsync())
            .ReturnsAsync(countriesMock);

        List<CountryResponse> countries = await countriesService.GetAllCountriesAsync();

        countries.Should().NotBeEmpty();

        countries.Count.Should().Be(countriesMock.Count);

        countriesRepositoryMock.Verify(method => method.GetAllCountriesAsync(), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetCountryByCountryIdAsyncTests
    // when null countryId, throw argument null exception
    [Fact]
    public async Task GetCountryByCountryIdAsync_ShouldThrowArgumentNullException_NullCountryId()
    {
        Func<Task> action = async () =>
        {
            await countriesService.GetCountryByCountryIdAsync(null);
        };

        await action.Should().ThrowAsync<ArgumentNullException>();

        countriesRepositoryMock.VerifyNoOtherCalls();

    }

    // when invalid countryId, return null
    [Fact]
    public async Task GetCountryByCountryId_ShouldReturnNull_IfInvalidCountryId()
    {
        countriesRepositoryMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(null as Country);

        CountryResponse? response = await countriesService.GetCountryByCountryIdAsync(Guid.NewGuid());

        response.Should().BeNull();

        countriesRepositoryMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }

    // when valid countryId, return country response
    [Fact]
    public async Task GetCountryByCountryId_ShouldReturnCountryResponse_IfValidCountryId()
    {
        Country countryMock = fixture.Build<Country>().Without(prop => prop.Persons).Create();

        countriesRepositoryMock.Setup(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid?>()))
            .ReturnsAsync(countryMock);

        CountryResponse? response = await countriesService.GetCountryByCountryIdAsync(Guid.NewGuid());

        response.Should().NotBeNull();

        response!.CountryId.Should().Be(countryMock.CountryId);

        response.CountryName.Should().Be(countryMock.CountryName);

        countriesRepositoryMock.Verify(method => method.GetCountryByCountryIdAsync(It.IsAny<Guid?>()), Times.Once);

        countriesRepositoryMock.VerifyNoOtherCalls();
    }
    #endregion
}
