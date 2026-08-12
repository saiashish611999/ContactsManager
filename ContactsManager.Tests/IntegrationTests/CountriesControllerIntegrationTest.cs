using ContactsManager.Core.DataTransferObjects.CountryDtos;
using ContactsManager.Tests.WebApplicationFactory;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit.Abstractions;

namespace ContactsManager.Tests.IntegrationTests;
public sealed class CountriesControllerIntegrationTest: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;
    private readonly ITestOutputHelper writer;

    public CountriesControllerIntegrationTest(CustomWebApplicationFactory factory, ITestOutputHelper writer)
    {
        client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        this.writer = writer;
    }

    #region Index
    [Fact]
    public async Task Index_ShouldReturnView()
    {
        // arrange

        // act
        HttpResponseMessage responseMessage = await client.GetAsync("/countries/index");

        // assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        // fizzler
        string responseString =  await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseString);

        var document = html.DocumentNode;

        document.QuerySelector("table.countries").Should().NotBeNull();
    }
    #endregion

    #region Create
    [Fact]
    public async Task Create_GET_ShouldReturnView()
    {
        HttpResponseMessage responseMessage = await client.GetAsync("/countries/create");

        string? content = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(content);

        var document = html.DocumentNode;

        document.QuerySelector("div.create-country").Should().NotBeNull();
    }

    [Fact]
    public async Task Create_POST_ShouldReturnSameViewIfModelStateError()
    {
        // arrange
        var formData = new Dictionary<string, string?>()
        {
            { "CountryName", "as"}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/countries/create", content);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        string? rawContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(rawContent);

        var document = html.DocumentNode;

        var countryNameError = document.QuerySelector("span.country-name-error");

        countryNameError.Should().NotBeNull();

        countryNameError.InnerText.Should().Be("Country Name is not in a correct format");
    }

    [Fact]
    public async Task Create_POST_ShouldRedirectToIndexAction()
    {
        var formData = new Dictionary<string, string?>()
        {
            { "CountryName", "India"}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/countries/create", content);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.Found);

        responseMessage.Headers.Location.Should().NotBeNull();

        responseMessage.Headers.Location!.ToString().Should().Be("/Countries/Index");

        //responseMessage.RequestMessage!.RequestUri!.AbsoluteUri.Should().Be("/Countries/Index");

        //string? rawContent = await responseMessage.Content.ReadAsStringAsync();

        //writer.WriteLine(rawContent);

        //HtmlDocument html = new HtmlDocument();

        //html.LoadHtml(rawContent);

        //var document = html.DocumentNode;

        //var countryName = document.QuerySelector("td.country-name");

        //countryName.Should().NotBeNull();

        //countryName.InnerText.Should().Be("India");
    }

    #endregion

    #region Delete
    [Fact]
    public async Task Delete_GET_ShouldReturnRedirectToActionIfCountryIsNull()
    {
        HttpResponseMessage responseMessage = await client.GetAsync($"/countries/delete/{Guid.NewGuid()}");

        responseMessage.StatusCode.Should().Be(HttpStatusCode.Found);

        responseMessage.Headers.Location.Should().NotBeNull();

        responseMessage.Headers.Location!.ToString().Should().Be("/Countries/Index");

        //responseMessage.RequestMessage!.RequestUri!.AbsolutePath.Should().Be("/Countries/Index");

        //string? rawContent = await responseMessage.Content.ReadAsStringAsync();

        //writer.WriteLine(rawContent);

        //HtmlDocument html = new HtmlDocument();

        //html.LoadHtml(rawContent);

        //var document = html.DocumentNode;

        //document.QuerySelector("td.country-name").InnerText.Should().Be("India");
    }

    [Fact]
    public async Task Delete_GET_ShouldReturnDeleteViewifPersonExists()
    {
        // arrange
        HttpResponseMessage responseMessage = await client.GetAsync("/Countries/Delete/22222222-2222-2222-2222-222222222222");

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        string? rawCount = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(rawCount);

        var document = html.DocumentNode;

        var countryName = document.QuerySelector("div.country-name");

        countryName.Should().NotBeNull();

        countryName.InnerText.Should().Contain("Iran");
    }

    [Fact]
    public async Task Delete_POST_ShouldRedirectToIndexActionMethod()
    {
        HttpResponseMessage responseMessage = await client.PostAsync("/Countries/DeleteConfirmed/11111111-1111-1111-1111-111111111111", null);

        responseMessage.StatusCode.Should().Be(HttpStatusCode.Found);

        responseMessage.Headers.Location.Should().NotBeNull();

        responseMessage.Headers.Location!.Should().Be("/Countries/Index");
    }
    #endregion
}
