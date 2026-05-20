using FluentValidation;
using Microsoft.Identity.Client;
using OriganizationAPI.Dtos.EventDtos;
using OriganizationAPI.Models;

namespace OriganizationAPI.Dtos.TicketDtos
{
	public class TicketCreateDto
	{
		public int EventId { get; set; }
		public string Type { get; set; } = null!;
		public decimal Price { get; set; }
		public int QuantityAvailable { get; set; }
	}
	public class TicketCreateDtoValidator : AbstractValidator<TicketCreateDto>
	{
		public TicketCreateDtoValidator()
		{
			RuleFor(e => e.EventId)
				.NotEmpty().WithMessage("EventId is required!");

			RuleFor(e => e.Type)
				.NotNull().WithMessage("Type is Required!")
				.MaximumLength(50).WithMessage("Type must have atleast 50 character length!");

			RuleFor(e => e.Price)
				.NotNull().WithMessage("Price can not be null!")
				.PrecisionScale(18, 2, false).WithMessage("Price must have up to 10 digits and 2 decimal places.")
				.Must(price =>  price > 0).WithMessage("Price must be greater than 0!");
			
			RuleFor(e => e.QuantityAvailable)
				.NotNull().WithMessage("Quantity can not be null!")
				.Must(q =>  q > 0).WithMessage("Quantity must be greater than 0!");
		}
	}
}
