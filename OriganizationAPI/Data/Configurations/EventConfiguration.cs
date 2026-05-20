using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OriganizationAPI.Models;

namespace OriganizationAPI.Data.Configurations
{
	public class EventConfiguration : IEntityTypeConfiguration<Event>
	{
		public void Configure(EntityTypeBuilder<Event> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Title).IsRequired().HasMaxLength(150);
			builder.Property(e => e.Description).IsRequired(false).HasMaxLength(500);
			builder.Property(e => e.Date).IsRequired();
			builder.Property(e => e.Location).IsRequired();
			builder.Property(e => e.BannerImage).IsRequired(false);

			builder.HasOne(e => e.Organizer)
				.WithMany(o => o.Events)
				.HasForeignKey(e => e.OrganizerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(e => e.Tickets)
				.WithOne(t => t.Event)
				.HasForeignKey(t => t.EventId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
