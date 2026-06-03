namespace Organization.MVC.Models.TicketViewModels
{
	public class TicketViewModel
	{
		public int EventId { get; set; }
		public EventInTicketReturnDto? Event { get; set; }
		public string Type { get; set; } = null!;
		public decimal Price { get; set; }
		public int QuantityAvailable { get; set; }
	}
	public class EventInTicketReturnDto()
	{
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = null!;
		public string? BannerImage { get; set; }
	}
}
