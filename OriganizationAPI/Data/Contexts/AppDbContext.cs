using Microsoft.EntityFrameworkCore;
using OriganizationAPI.Models;

namespace OriganizationAPI.Data.Contexts
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Event> Events { get; set; }
		public DbSet<Organizer> Organizers { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}
	}
}
