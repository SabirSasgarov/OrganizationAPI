using System.ComponentModel.DataAnnotations;
using Organization.MVC.Models.EventViewModels;

namespace Organization.MVC.Models.TicketViewModels
{
	public class TicketCreateVM
	{
		[Required]
		public int EventId { get; set; }
		[Required]
		public string Type { get; set; } = null!;
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
		public decimal Price { get; set; }
		[Required]
		[Range(1,int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
		public int QuantityAvailable { get; set; }
		public List<EventViewModel>? Events { get; set; }
	}
}
