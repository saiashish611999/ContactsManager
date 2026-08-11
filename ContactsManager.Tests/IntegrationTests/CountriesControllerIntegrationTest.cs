using ContactsManager.Tests.WebApplicationFactory;
using FluentAssertions;

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
        HttpResponseMessage responseMessage = await client.GetAsync("/persons/index");

        // assert
        responseMessage.IsSuccessStatusCode.Should().BeTrue();
    }
    #endregion
}
