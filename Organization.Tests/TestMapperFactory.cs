namespace Organization.Tests
{
	public static class TestMapperFactory
	{
		public static IMapper Create()
		{
			var configuration = new MapperConfiguration(
				cfg => cfg.AddProfile(new MapperProfile(new HttpContextAccessor())),
				NullLoggerFactory.Instance);

			return configuration.CreateMapper();
		}
	}
}
