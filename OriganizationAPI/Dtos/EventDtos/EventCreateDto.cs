namespace OriganizationAPI.Dtos.EventDtos
{
	public class EventCreateDto
	{
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = null!;
		public IFormFile? File { get; set; }
		public int OrganizerId { get; set; }
	}
	public class EventCreateDtoValidator : AbstractValidator<EventCreateDto>
	{
		public EventCreateDtoValidator()
		{
			RuleFor(e => e.Title)
				.NotEmpty().WithMessage("Title is required!")
				.MaximumLength(150).WithMessage("Title can not exceed 150 characters!");

			RuleFor(e => e.Description)
				.MaximumLength(500).WithMessage("Description can not exceed 500 characters!");

			RuleFor(e => e.Date)
				.Must(date => date.Date > DateTime.UtcNow).WithMessage("Events can not be in the past!");

			RuleFor(e => e.Location)
				.NotEmpty().WithMessage("Location is required!");

			RuleFor(e => e.File)
				.Must(file => file?.Length < 2 * 1024 * 1024).WithMessage("File can not be greater than 2 MBs!")
				.When(e => e.File != null);

		}
	}
}
