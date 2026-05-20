using Microsoft.EntityFrameworkCore;
using OriganizationAPI.Data.Contexts;
using OriganizationAPI.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(opt => opt.AddProfile(new MapperProfile(new HttpContextAccessor())));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
