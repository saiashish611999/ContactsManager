using ContactsManager.Tests.WebApplicationFactory;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace ContactsManager.Tests.IntegrationTests;
public sealed class PersonsControllerIntegrationTests: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public PersonsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        this.client = factory.CreateClient();
    }

    #region Index
    [Fact]
    public async Task Index_ShouldReturnView()
    {
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
}
