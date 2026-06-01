namespace Organization.MVC.Models
{
	public class OrganizerViewModel
	{
		public string Name { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Phone { get; set; }
		public string? LogoUrl { get; set; }
		public List<EventInOrganizerReturnDto>? Events { get; set; }
	}
	public class EventInOrganizerReturnDto
	{
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = null!;
		public string? BannerImage { get; set; }
	}
}
