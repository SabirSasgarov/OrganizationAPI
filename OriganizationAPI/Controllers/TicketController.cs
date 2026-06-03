namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketController(
		AppDbContext context,
		IValidator<TicketCreateDto> ticketValidationRules,
		IMapper mapper) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var tickets = await context.Tickets
				.ProjectTo<TicketReturnDto>(mapper.ConfigurationProvider)
				.ToListAsync();
			return Ok(tickets);
		}
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] TicketCreateDto ticketCreateDto)
		{
			if (ticketValidationRules != null)
			{
				var validationResult = await ticketValidationRules.ValidateAsync(ticketCreateDto);
				if (!validationResult.IsValid)
				{
					return BadRequest(validationResult.Errors);
				}
			}

			var existingEvent = await context.Events.FindAsync(ticketCreateDto.EventId);
			if (existingEvent == null) return NotFound("There is no such event!");
			foreach (var ticket in existingEvent!.Tickets)
			{
				if (ticket.Type == ticketCreateDto.Type)
					return BadRequest("The type already exists!");
			}
			var newTicket = mapper.Map<Ticket>(ticketCreateDto);
			await context.Tickets.AddAsync(newTicket);
			await context.SaveChangesAsync();
			return Created();
		}
	}
}
