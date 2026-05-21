namespace OriganizationAPI.Models
{
	public class Event: BaseEntity
	{
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public string Location { get; set; } = null!;
		public string? BannerImage {  get; set; }
		public int OrganizerId { get; set; }
		public Organizer Organizer { get; set; } = null!;
		public List<Ticket> Tickets { get; set; } = new List<Ticket>();
	}
}
