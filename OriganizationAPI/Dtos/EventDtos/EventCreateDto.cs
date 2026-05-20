using FluentValidation;

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
				.MaximumLength(100).WithMessage("");

			RuleFor(e => e.File)
				.NotNull().WithMessage("File is Required!");


		}
	}
}
