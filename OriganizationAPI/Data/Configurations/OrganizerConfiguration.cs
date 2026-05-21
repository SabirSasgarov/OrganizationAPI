namespace OriganizationAPI.Data.Configurations
{
	public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
	{
		public void Configure(EntityTypeBuilder<Organizer> builder)
		{
			builder.HasKey(o => o.Id);
			builder.Property(o => o.Name).IsRequired().HasMaxLength(100);
			builder.Property(o => o.Email).IsRequired();
			builder.Property(o => o.Phone).IsRequired(false).HasMaxLength(20);
			builder.Property(o => o.LogoUrl).IsRequired(false);
		}
	}
}
