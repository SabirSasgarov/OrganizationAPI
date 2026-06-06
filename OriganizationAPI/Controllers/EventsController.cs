using OriganizationAPI.Handler;

namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventsController(AppDbContext context,
		IMapper mapper,
		IValidator<EventCreateDto> validationRules) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var events = await context.Events
			.ProjectTo<EventReturnDto>(mapper.ConfigurationProvider)
			.ToListAsync();

			//return Ok(events);
			return Ok(ResponseHandler<List<EventReturnDto>>.SuccessResponse(events));
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var eventt = await context.Events
				.ProjectTo<EventReturnDto>(mapper.ConfigurationProvider)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (eventt == null) return NotFound(ResponseHandler<EventReturnDto>.FailureResponse(["Event not found."]));

			return Ok(ResponseHandler<EventReturnDto>.SuccessResponse(eventt));
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] EventCreateDto eventCreateDto)
		{
			var validationResult = await validationRules.ValidateAsync(eventCreateDto);
			if (!validationResult.IsValid)
			{
				//return BadRequest(validationResult.Errors);
				return BadRequest(ResponseHandler<EventCreateDto>.FailureResponse([.. validationResult.Errors.Select(e => e.ErrorMessage)]));
			}

			var existingOrganizer = await context.Organizers.FindAsync(eventCreateDto.OrganizerId);
			if(existingOrganizer == null)
			{
				return NotFound(ResponseHandler<EventCreateDto>.FailureResponse(["Organizer not found."]));
			}

			if (await context.Events.AnyAsync(e => e.Title == eventCreateDto.Title))
			{
				return BadRequest(ResponseHandler<EventCreateDto>.FailureResponse(["An event with the given title already exists."]));
			}
			var eventt= mapper.Map<Event>(eventCreateDto);
			await context.Events.AddAsync(eventt);
			await context.SaveChangesAsync();
			return Ok(ResponseHandler<EventReturnDto>.SuccessResponse(mapper.Map<EventReturnDto>(eventt)));
		}
		[Authorize]
		[HttpPatch("{id}/banner")]
		public async Task<IActionResult> Post(int id,IFormFile banner)
		{
			if (banner == null) return BadRequest(ResponseHandler<EventReturnDto>.FailureResponse(["Choose a file!"]));
			var existingEvent = await context.Events.FindAsync(id);
			if (existingEvent == null) return NotFound(ResponseHandler<EventReturnDto>.FailureResponse(["Event not found."]));
			if(existingEvent.BannerImage != null)
			{
				FileExtension.DeleteFile("wwwroot/images/banners", existingEvent.BannerImage);
			}
			existingEvent.BannerImage = banner.SaveFile("wwwroot/images/banners");
			await context.SaveChangesAsync();
			return Ok(ResponseHandler<EventReturnDto>.SuccessResponse(mapper.Map<EventReturnDto>(existingEvent)));
		}
		[HttpGet("{id}/tickets")]
		public async Task<IActionResult> GetTickets(int id)
		{
			var existingEvent = await context.Events
				.Include(e => e.Tickets)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound(ResponseHandler<EventReturnDto>.FailureResponse(["Event not found."]));
			var tickets = existingEvent.Tickets;
			if (tickets == null && tickets?.Count == 0) return Ok(ResponseHandler<EventReturnDto>.FailureResponse(["There are no tickets for this event."]));

			//var eventReturnDto = mapper.Map<EventReturnDto>(existingEvent);
			var ticketsDto = mapper.Map<List<TicketReturnDto>>(tickets);
			return Ok(ResponseHandler<List<TicketReturnDto>>.SuccessResponse(ticketsDto));
		}

		[HttpGet("{id}/organizer")]
		public async Task<IActionResult> GetOrganizer(int id)
		{
			var existingEvent = await context.Events
				.Include(e => e.Organizer)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound(ResponseHandler<EventReturnDto>.FailureResponse(["Event not found."]));
			var organizer = existingEvent.Organizer;
			if (organizer == null) return Ok(ResponseHandler<EventReturnDto>.FailureResponse(["There is no organizer for that event!"]));

			//var eventReturnDto = mapper.Map<EventReturnDto>(existingEvent);
			var organizerDto = mapper.Map<OrganizerReturnDto>(organizer);

			return Ok(ResponseHandler<OrganizerReturnDto>.SuccessResponse(organizerDto));
		}
		[Authorize]
		[HttpPost("{id}/tickets")]
		public async Task<IActionResult> Post(int id,[FromForm] TicketCreateDto ticketCreateDto)
		{
			var existingEvent = await context.Events
				.Include(e => e.Tickets)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound(ResponseHandler<EventReturnDto>.FailureResponse(["Event not found."]));
			var tickets = existingEvent.Tickets;
			foreach(var ticket in tickets)
			{
				if (ticket.Type == ticketCreateDto.Type)
					return BadRequest(ResponseHandler<EventReturnDto>.FailureResponse(["Ticket type already exists for this event."]));
			}

			var newTicket = mapper.Map<Ticket>(ticketCreateDto);
			await context.Tickets.AddAsync(newTicket);
			await context.SaveChangesAsync();

			return Ok(ResponseHandler<TicketReturnDto>.SuccessResponse(mapper.Map<TicketReturnDto>(newTicket)));
		}
	}
}
