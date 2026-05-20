using FluentValidation;

namespace OriganizationAPI.Dtos.AccountDtos
{
	public class LoginDto
	{
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
	public class LoginDtoValidator : AbstractValidator<LoginDto>
	{
		public LoginDtoValidator() 
		{
			RuleFor(e => e.Username)
				.NotEmpty().WithMessage("User name is required!")
				.MaximumLength(50).WithMessage("User name can not exceed 50 characters!");
			RuleFor(e => e.Password)
				.NotEmpty().WithMessage("Password is required!")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters long!");
		}
	}
}
