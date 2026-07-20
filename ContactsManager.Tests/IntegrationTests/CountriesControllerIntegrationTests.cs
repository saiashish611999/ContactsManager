using ContactsManager.Tests.WebApplicationFactory;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace ContactsManager.Tests.IntegrationTests;
public sealed class CountriesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public CountriesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        this.client = factory.CreateClient();
    }

    #region Index
    [Fact] 
    public async Task Index_ToReturnView()
    {
        // act
        HttpResponseMessage responseMessage = await client.GetAsync("/Countries/Index");

        // assertion
        responseMessage.IsSuccessStatusCode.Should().BeTrue();

        string responseBody =  await responseMessage.Content.ReadAsStringAsync();

        HtmlDocument html = new HtmlDocument();

        html.LoadHtml(responseBody);

        var document = html.DocumentNode;

        document.QuerySelectorAll("table.countries").Should().NotBeNullOrEmpty();
    }
    #endregion
}
