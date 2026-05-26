using Microsoft.AspNetCore.Mvc;
using Moq;
namespace Organization.Tests
{
	public class TicketControllerTests
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
			var validatorMock = new Mock<IValidator<TicketCreateDto>>();

			var controller = new TicketController(context, validatorMock.Object, mapperMock.Object);

			// Act
			var result = await controller.Get();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.NotNull(okResult.Value);
		}
	}
}