namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class OrganizerController(AppDbContext context,
		IMapper mapper,
		IValidator<OrganizerCreateDto> organizerValidationRules) : ControllerBase
	{
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var organizers = await context.Organizers
				.ProjectTo<OrganizerReturnDto>(mapper.ConfigurationProvider)
				.ToListAsync();
			return Ok(organizers);
		}
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] OrganizerCreateDto organizerCreateDto)
		{
			if(organizerValidationRules != null)
			{
				var validationResult = await organizerValidationRules.ValidateAsync(organizerCreateDto);
				if (!validationResult.IsValid)
				{
					return BadRequest(validationResult.Errors);
				}
			}
			

			if (await context.Organizers.AnyAsync(e => e.Name == organizerCreateDto.Name))
			{
				return BadRequest("A organizer with the given name already exists!");
			}
			var organizer = mapper.Map<Organizer>(organizerCreateDto);
			await context.Organizers.AddAsync(organizer);
			await context.SaveChangesAsync();
			return Created();
		}
		[Authorize(Roles = "Admin")]
		[HttpPatch("{id}/logo")]
		public async Task<IActionResult> Post(int id, IFormFile logo)
		{
			if (logo == null) return BadRequest("Choose a file!");
			var existingOrganizer = await context.Organizers.FindAsync(id);
			if (existingOrganizer == null) return NotFound("There is no such organizer with given id!");
			if (existingOrganizer.LogoUrl != null)
			{
				FileExtension.DeleteFile("wwwroot/images/logos", existingOrganizer.LogoUrl);
			}
			existingOrganizer.LogoUrl = logo.SaveFile("wwwroot/images/logos");
			await context.SaveChangesAsync();
			return Ok();
		}

		[HttpGet("{id}/events")]
		public async Task<IActionResult> Get(int id)
		{
			var existingOrganizer = await context.Organizers
				.Include(e => e.Events)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingOrganizer == null) return NotFound("No such organizer!");
			var events = existingOrganizer.Events;
			if (events == null && events?.Count == 0) return Ok("There is no events!");

			var organizerReturnDto = mapper.Map<OrganizerReturnDto>(existingOrganizer);

			return Ok(organizerReturnDto);
		}
	}
}
