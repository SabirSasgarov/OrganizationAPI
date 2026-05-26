namespace Organization.Tests
{
	public class EventsControllerTests
	{
		[Fact]
		public async Task Get_ReturnsOkResult()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var context = new AppDbContext(options);
			var mapperMock = new Mock<IMapper>();
			var validatorMock = new Mock<IValidator<EventCreateDto>>();

			var controller = new EventsController(context, mapperMock.Object, validatorMock.Object);

			// Act
			var result = await controller.Get();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.NotNull(okResult.Value);
		}

		[Fact]
		public async Task Post_MissingOrganizer_ReturnsNotFound()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var context = new AppDbContext(options);
			var mapperMock = new Mock<IMapper>();
			var validatorMock = new Mock<IValidator<EventCreateDto>>();
			var dto = new EventCreateDto { Title = "Test Event", OrganizerId = 99 }; // 99 doesn't exist

			validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult()); // Valid dto

			var controller = new EventsController(context, mapperMock.Object, validatorMock.Object);

			// Act
			var result = await controller.Post(dto);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("There is no such organizer with given id!", notFoundResult.Value);
		}
	}
}