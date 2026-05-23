namespace OriganizationAPI.Dtos.AccountDtos
{
	public class ResetPasswordDto
	{
		public string Password { get; set; } = null!;
		public string NewPassword { get; set; } = null!;
		public string ReNewPassword { get; set; } = null!;
	}

	public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
	{
		public ResetPasswordDtoValidator()
		{
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Current password is required.");
			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("New password is required.")
				.MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
				.NotEqual(x => x.Password).WithMessage("New password must be different from the current password.");
			RuleFor(x => x.ReNewPassword)
				.NotEmpty().WithMessage("Please confirm the new password.")
				.Equal(x => x.NewPassword).WithMessage("The new password and confirmation do not match.");
		}
	}

}
