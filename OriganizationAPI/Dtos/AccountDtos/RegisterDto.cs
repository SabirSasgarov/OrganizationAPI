namespace OriganizationAPI.Dtos.AccountDtos
{
	public class RegisterDto
	{
		public string? FullName { get; set; }
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string ConfirmPassword { get; set; } = null!;
	}
	public class RegisterDtoValidator : AbstractValidator<RegisterDto>
	{
		public RegisterDtoValidator()
		{
			RuleFor(e => e.FullName)
				.MaximumLength(100).WithMessage("Full name can not exceed 100 characters!");
			RuleFor(e => e.UserName)
				.NotEmpty().WithMessage("User name is required!")
				.MaximumLength(50).WithMessage("User name can not exceed 50 characters!");
			RuleFor(e => e.Email)
				.NotEmpty().WithMessage("Email is required!")
				.EmailAddress().WithMessage("Enter valid email address!");
			RuleFor(e => e.Password)
				.NotEmpty().WithMessage("Password is required!")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters long!");
			RuleFor(e => e.ConfirmPassword)
				.Equal(e => e.Password).WithMessage("Passwords do not match!");
		}
	}
}
