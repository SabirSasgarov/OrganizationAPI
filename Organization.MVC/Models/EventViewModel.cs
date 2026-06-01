namespace Organization.MVC.Models
{
	public class EventViewModel
	{
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = null!;
		public string? BannerImage { get; set; }
		public int OrganizerId { get; set; }
		public OrganizerInEventReturnDto Organizer { get; set; } = null!;
		public List<TicketInEventReturnDto>? Tickets { get; set; }
	}
	public class OrganizerInEventReturnDto
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Phone { get; set; }
		public string? LogoUrl { get; set; }
	}
	public class TicketInEventReturnDto
	{
		public string Type { get; set; } = null!;
		public decimal Price { get; set; }
		public int QuantityAvailable { get; set; }
	}
}

