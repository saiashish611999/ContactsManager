using ContactsManager.Core.Enums;
using ContactsManager.Tests.WebApplicationFactory;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactsManager.Tests.IntegrationTests;
public sealed class PersonsControllerIntegrationTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;
    private readonly CustomWebApplicationFactory factory;

    public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        this.client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        this.factory = factory;
    }

    #region Index
    [Fact]
    public async Task Index_ShouldReturnView()
    {
        await factory.ResetDatabase();

        // act
        HttpResponseMessage responseMessage = await client.GetAsync("/persons/index");

        // assert
        string? content = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(content);

        var document = html.DocumentNode;

        document.QuerySelector("table.persons-table").Should().NotBeNull();
    }
    #endregion

    #region Create
    [Fact]
    public async Task Create_ShouldReturnView()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.GetAsync("/Persons/Create");

        string? content = await responseMessage.Content.ReadAsStringAsync();

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(content);

        var document = html.DocumentNode;

        var personNameLabel = document.QuerySelector("label.person-name-label");

        personNameLabel.Should().NotBeNull();

        personNameLabel.InnerText.Should().Be("PersonName");
    }

    [Fact]
    public async Task Create_POST_ShouldReturnViewIfModelStateErrors()
    {
        await factory.ResetDatabase();

        var formData = new Dictionary<string, string?>()
        {
            { "PersonName", null},
            { "EmailAddress", null},
            { "Gender", Gender.MALE.ToString()},
            { "DateOfBirth", new DateTime(1997, 01, 23).ToString()},
            { "Address", "something"},
            { "ReceiveNewsLetters", true.ToString()},
            { "CountryId", Guid.Parse("22222222-2222-2222-2222-222222222222").ToString()}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/Persons/Create", content);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        string? responseContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseContent);

        var document = html.DocumentNode;

        var personNameError = document.QuerySelector("span.person-name-error");

        personNameError.Should().NotBeNull();

        personNameError.InnerText.Should().Contain("PersonName is Required");
    }

    [Fact]
    public async Task Create_POST_ShouldRedirectToIndexMethod()
    {
        await factory.ResetDatabase();

        var formData = new Dictionary<string, string?>()
        {
            { "PersonName", "Sai Ashish"},
            { "EmailAddress", "Ashish@gmail.com"},
            { "Gender", Gender.MALE.ToString()},
            { "DateOfBirth", new DateTime(1997, 01, 23).ToString()},
            { "Address", "something"},
            { "ReceiveNewsLetters", true.ToString()},
            { "CountryId", Guid.Parse("22222222-2222-2222-2222-222222222222").ToString()}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/Persons/Create", content);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
    }

    #endregion

    #region Update
    [Fact]
    public async Task Update_GET_ShouldRedirectToIndexViewIfPersonIsNull()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.GetAsync($"/Persons/Update/{Guid.NewGuid()}");

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
    }

    [Fact]
    public async Task Update_GET_ShouldReturnUpdateViewIfPersonExists()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.GetAsync("/Persons/Update/190F863F-0D26-4FDD-AC24-3B729724C4F8");

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        string? responseContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseContent);

        var document = html.DocumentNode;

        var personName = document.QuerySelector("input.person-name");

        personName.Should().NotBeNull();

        personName.GetAttributeValue("value", "").Should().Be("Sai Ashish");

    }

    [Fact]
    public async Task Update_POST_ShouldReturnUpdateViewIfModelError()
    {
        await factory.ResetDatabase();

        var formData = new Dictionary<string, string?>()
        {
            { "PersonName", null},
            { "EmailAddress", null},
            { "Gender", Gender.MALE.ToString()},
            { "DateOfBirth", new DateTime(1997, 01, 23).ToString()},
            { "Address", "something"},
            { "ReceiveNewsLetters", true.ToString()},
            { "CountryId", Guid.Parse("22222222-2222-2222-2222-222222222222").ToString()}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/Persons/Update/190F863F-0D26-4FDD-AC24-3B729724C4F8", content);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        string? responseContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseContent);

        var document = html.DocumentNode;

        var personName = document.QuerySelector("span.person-name-error");

        personName.Should().NotBeNull();

        personName.InnerText.Should().Be("PersonName is required field");
    }

    [Fact]
    public async Task Update_POST_ShouldReturnRedirectToIndexView()
    {
        await factory.ResetDatabase();

        var formData = new Dictionary<string, string?>()
        {
            { "PersonId", "190F863F-0D26-4FDD-AC24-3B729724C4F8"},
            { "PersonName", "Sai Ashish"},
            { "EmailAddress", "Ashish@gmail.com"},
            { "Gender", Gender.MALE.ToString()},
            { "DateOfBirth", new DateTime(1997, 01, 23).ToString()},
            { "Address", "something"},
            { "ReceiveNewsLetters", true.ToString()},
            { "CountryId", Guid.Parse("22222222-2222-2222-2222-222222222222").ToString()}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/Persons/Update/190F863F-0D26-4FDD-AC24-3B729724C4F8", content);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
       
    }
    #endregion

    #region Delete
    [Fact]
    public async Task Delete_GET_ShouldReturnRedirectToActionWhenPersonIsNull()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.GetAsync($"/Persons/Delete/{Guid.NewGuid()}");

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
    }

    [Fact]
    public async Task Delete_GET_ShouldReturnDeleteViewIfPersonExists()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.GetAsync("/Persons/Delete/190F863F-0D26-4FDD-AC24-3B729724C4F8");

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        string? responseContent = await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseContent);

        var document = html.DocumentNode;

        var personName = document.QuerySelector("div.person-name");

        personName.Should().NotBeNull();

        personName.InnerHtml.Should().Contain("Sai Ashish");
    }

    [Fact]
    public async Task Delete_POST_ShouldReturnRedirectToActionWhenPersonIsNull()
    {
        await factory.ResetDatabase();

        HttpResponseMessage responseMessage = await client.PostAsync($"/Persons/Delete/{Guid.NewGuid()}", null);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
    }

    [Fact]
    public async Task Delete_POST_ShouldRedirectToActionWhenPersonIsNotNull()
    {
        await factory.ResetDatabase();

        var formData = new Dictionary<string, string?>()
        {
            { "PersonId", "190F863F-0D26-4FDD-AC24-3B729724C4F8"},
            { "PersonName", "Sai Ashish"},
            { "EmailAddress", "saiashish611999@gmail.com"}
        };

        var content = new FormUrlEncodedContent(formData);

        HttpResponseMessage responseMessage = await client.PostAsync("/Persons/Delete/190F863F-0D26-4FDD-AC24-3B729724C4F8", content);

        responseMessage.StatusCode.Should().Be(System.Net.HttpStatusCode.Found);
        
    }
    #endregion
}
