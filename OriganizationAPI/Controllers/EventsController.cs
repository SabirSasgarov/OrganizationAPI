using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OriganizationAPI.Data.Contexts;
using OriganizationAPI.Dtos.EventDtos;
using OriganizationAPI.Dtos.TicketDtos;
using OriganizationAPI.Extensions;
using OriganizationAPI.Models;

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

			return Ok(events);
		}
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] EventCreateDto eventCreateDto)
		{
			var validationResult = await validationRules.ValidateAsync(eventCreateDto);
			if (!validationResult.IsValid)
			{
				return BadRequest(validationResult.Errors);
			}

			if (await context.Events.AnyAsync(e => e.Title == eventCreateDto.Title))
			{
				return BadRequest("A event with the given title already exists!");
			}
			var eventt= mapper.Map<Event>(eventCreateDto);
			await context.Events.AddAsync(eventt);
			await context.SaveChangesAsync();
			return Created();
		}
		[HttpPost("{id}/banner")]
		public async Task<IActionResult> Post(int id,IFormFile banner)
		{
			if (banner == null) return BadRequest("Choose a file!");
			var existingEvent = await context.Events.FindAsync(id);
			if (existingEvent == null) return NotFound("There is no such event with given id!");
			if(existingEvent.BannerImage != null)
			{
				FileExtension.DeleteFile("wwwroot/images/banners", existingEvent.BannerImage);
			}
			existingEvent.BannerImage = banner.SaveFile("wwwroot/images/banners");
			await context.SaveChangesAsync();
			return Ok();
		}
		[HttpGet("{id}/tickets")]
		public async Task<IActionResult> Get(int id)
		{
			var existingEvent = await context.Events
				.Include(e => e.Tickets)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound("No such event!");
			var tickets = existingEvent.Tickets;
			if (tickets == null && tickets?.Count == 0) return Ok("There is no tickets!");

			var eventReturnDto = mapper.Map<EventReturnDto>(existingEvent);

			return Ok(eventReturnDto);
		}

		[HttpGet("{id}/organizer")]
		public async Task<IActionResult> GetOrganizer(int id)
		{
			var existingEvent = await context.Events
				.Include(e => e.Organizer)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound("No such event!");
			var organizer = existingEvent.Organizer;
			if (organizer == null) return Ok("There is no organizer for that event!");

			var eventReturnDto = mapper.Map<EventReturnDto>(existingEvent);

			return Ok(eventReturnDto);
		}

		[HttpPost("{id}/tickets")]
		public async Task<IActionResult> Post(int id, TicketCreateDto ticketCreateDto)
		{
			var existingEvent = await context.Events
				.Include(e => e.Tickets)
				.FirstOrDefaultAsync(e => e.Id == id);

			if (existingEvent == null) return NotFound("No such event!");
			var tickets = existingEvent.Tickets;
			foreach(var ticket in tickets)
			{
				if (ticket.Type == ticketCreateDto.Type)
					return BadRequest("Already Exists!");
			}

			var newTicket = mapper.Map<Ticket>(ticketCreateDto);
			await context.Tickets.AddAsync(newTicket);
			await context.SaveChangesAsync();

			return Created();
		}
	}
}
