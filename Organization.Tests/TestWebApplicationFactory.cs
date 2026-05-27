namespace Organization.Tests
{
	public class TestWebApplicationFactory : WebApplicationFactory<Program>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment("Testing");

			builder.ConfigureAppConfiguration(configuration =>
			{
				configuration.AddInMemoryCollection(new Dictionary<string, string?>
				{
					["Jwt:Issuer"] = "test-issuer",
					["Jwt:Audience"] = "test-audience",
					["Jwt:Key"] = "integration-test-signing-key-with-enough-length"
				});
			});
		}
	}
}
