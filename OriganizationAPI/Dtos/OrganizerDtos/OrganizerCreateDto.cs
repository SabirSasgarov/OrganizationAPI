namespace OriganizationAPI.Dtos.OrganizerDtos
{
	public class OrganizerCreateDto
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Phone { get; set; }
		public IFormFile? File { get; set; }
	}

	public class OrganizerDtoValidator : AbstractValidator<OrganizerCreateDto>
	{
		public OrganizerDtoValidator()
		{
			RuleFor(e => e.Name)
				.NotEmpty().WithMessage("Name is required!")
				.MaximumLength(100).WithMessage("Name can not exceed 100 characters!");

			RuleFor(e => e.Email)
				.NotEmpty().WithMessage("Email is required!")
				.EmailAddress().WithMessage("Enter valid email address!");

			RuleFor(e => e.Phone)
				.MaximumLength(20).WithMessage("Phone number can not exceed 20 characters!");

			RuleFor(e => e.File)
				.Must(file => file == null || file.Length < 2 * 1024 * 1024).WithMessage("File can not be greater than 2 MBs!");

		}
	}

}
