namespace OriganizationAPI.Profiles
{
	public class MapperProfile : Profile
	{
		public MapperProfile(IHttpContextAccessor httpContextAccessor) 
		{
			var request = httpContextAccessor?.HttpContext?.Request;
			var baseUrl = request == null
				? string.Empty
				: new UriBuilder
				{
					Scheme = request.Scheme,
					Host = request.Host.Host,
					Port = request.Host.Port ?? -1
				}.Uri.AbsoluteUri;
			//Event
			CreateMap<EventCreateDto, Event>()
				.ForMember(dest => dest.BannerImage, opt => opt.MapFrom(src => src.File!.SaveFile("wwwroot/images/banners")));
			CreateMap<Event, EventReturnDto>()
				.ForMember(dest => dest.BannerImage, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.BannerImage) ? null : $"{baseUrl}images/banners/{src.BannerImage}"));
			
			
			//Organizer(return)
			CreateMap<Organizer, OrganizerReturnDto>()
				.ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.LogoUrl) ? null : $"{baseUrl}images/logos/{src.LogoUrl}"));
			CreateMap<OrganizerCreateDto, Organizer>()
				.ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.File!.SaveFile("wwwroot/images/logos")));

			//Ticket(return)
			CreateMap<Ticket, TicketReturnDto>();
			CreateMap<TicketCreateDto, Ticket>();

			CreateMap<Event, EventInTicketReturnDto>();
			CreateMap<Ticket, TicketInEventReturnDto>();
			CreateMap<Organizer, OrganizerInEventReturnDto>();
			CreateMap<Event, EventInOrganizerReturnDto>();
		}
	}
}
