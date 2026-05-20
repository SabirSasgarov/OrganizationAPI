using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OriganizationAPI.Data.Contexts;
using OriganizationAPI.Dtos.OrganizerDtos;
using OriganizationAPI.Dtos.TicketDtos;
using OriganizationAPI.Models;

namespace OriganizationAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TicketController(AppDbContext context, IMapper mapper) : ControllerBase
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
			var existingEvent = await context.Events.FindAsync(ticketCreateDto.EventId);
			if (existingEvent != null) return NotFound("There is no such event!");
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
