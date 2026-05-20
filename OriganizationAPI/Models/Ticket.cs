using OriganizationAPI.Models.Common;

namespace OriganizationAPI.Models
{
	public class Ticket : BaseEntity
	{
		public int EventId { get; set; }
		public Event Event { get; set; } = null!;
		public string Type { get; set; } = null!;
		public decimal Price {  get; set; }
		public int QuantityAvailable { get; set; }
	}
}
