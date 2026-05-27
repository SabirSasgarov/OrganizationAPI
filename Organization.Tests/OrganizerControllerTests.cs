namespace Organization.Tests
{
	public class OrganizerControllerTests
	{
		[Fact]
		public async Task Get_ReturnsOkResult_WithOrganizersList()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var context = new AppDbContext(options);
			var mapper = TestMapperFactory.Create();
			var validatorMock = new Mock<IValidator<OrganizerCreateDto>>();

			context.Organizers.Add(new OriganizationAPI.Models.Organizer { Id = 1, Name = "Test Organizer 1", Email = "first@example.com" });
			context.Organizers.Add(new OriganizationAPI.Models.Organizer { Id = 2, Name = "Test Organizer 2", Email = "second@example.com" });
			await context.SaveChangesAsync();

			// Mocking ProjectTo is complex, typically we just test if Ok Object result returns (Integration tests are better for ProjectTo mapping)
			// But for unit test flow we will initialize the controller
			var controller = new OrganizerController(context, mapper, validatorMock.Object);

			// Act
			var result = await controller.Get();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.NotNull(okResult.Value);
		}

		[Fact]
		public async Task Post_InvalidData_ReturnsBadRequest()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;
			var context = new AppDbContext(options);
			var mapperMock = new Mock<IMapper>();
			var validatorMock = new Mock<IValidator<OrganizerCreateDto>>();
			var dto = new OrganizerCreateDto { Name = "" };

			var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
			validatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(validationResult);

			var controller = new OrganizerController(context, mapperMock.Object, validatorMock.Object);

			// Act
			var result = await controller.Post(dto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}
