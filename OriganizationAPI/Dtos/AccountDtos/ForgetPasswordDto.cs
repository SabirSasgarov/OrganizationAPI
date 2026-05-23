namespace OriganizationAPI.Dtos.AccountDtos
{
	public class ForgetPasswordDto
	{
		public string Email { get; set; } = null!;
	}

	public class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
	{
		public ForgetPasswordDtoValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Invalid email format.");
		}
	}

}
