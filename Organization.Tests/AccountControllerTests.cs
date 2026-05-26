namespace Organization.Tests
{
	public class AccountControllerTests
	{
		[Fact]
		public async Task Register_InvalidData_ReturnsBadRequest()
		{
			// Arrange
			var registerValidatorMock = new Mock<IValidator<RegisterDto>>();
			var loginValidatorMock = new Mock<IValidator<LoginDto>>();
			var resetValidatorMock = new Mock<IValidator<ResetPasswordDto>>();
			var forgetValidatorMock = new Mock<IValidator<ForgetPasswordDto>>();

			var store = new Mock<IUserStore<AppUser>>();
			var userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

			// Mocking services simply to initialize controller
			var jwtServiceMock = new Mock<JwtService>(null); // Normally we inject configuration

			var dto = new RegisterDto();
			var validationResult = new ValidationResult([new ValidationFailure("Email", "Required")]);
			registerValidatorMock.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(validationResult);

			var controller = new AccountController(
				registerValidatorMock.Object,
				loginValidatorMock.Object,
				resetValidatorMock.Object,
				forgetValidatorMock.Object,
				userManagerMock.Object,
				null, // RefreshTokenService
				null // JwtService
			);

			// Act
			var result = await controller.Register(dto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}