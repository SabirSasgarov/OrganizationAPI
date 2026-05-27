namespace Organization.Tests
{
	public class ApiIntegrationTests : IClassFixture<TestWebApplicationFactory>
	{
		private readonly HttpClient _client;

		public ApiIntegrationTests(TestWebApplicationFactory factory)
		{
			_client = factory.CreateClient();
		}

		[Fact]
		public async Task GetEvents_ThroughHttpPipeline_ReturnsOk()
		{
			var response = await _client.GetAsync("/api/Events");
			var body = await response.Content.ReadAsStringAsync();

			Assert.True(response.IsSuccessStatusCode, body);
			Assert.Equal("[]", body);
		}

		[Fact]
		public async Task GetOrganizers_WhenUnauthenticated_ReturnsUnauthorized()
		{
			var response = await _client.GetAsync("/api/Organizer");

			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}
	}
}
