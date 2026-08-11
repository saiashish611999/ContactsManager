using ContactsManager.Tests.WebApplicationFactory;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace ContactsManager.Tests.IntegrationTests;
public sealed class CountriesControllerIntegrationTest: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;

    public CountriesControllerIntegrationTest(CustomWebApplicationFactory factory)
    {
        client = factory.CreateClient();
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
}
