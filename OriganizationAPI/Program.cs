using Microsoft.OpenApi.Models;
using OriganizationAPI.Middlewares;

namespace OriganizationAPI
{
	public partial class Program
	{
		private static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				if (builder.Environment.IsEnvironment("Testing"))
				{
					options.UseInMemoryDatabase("OrganizationApiTests");
				}
				else
				{
					options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
				}
			});
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddAutoMapper(opt => opt.AddProfile(new MapperProfile(new HttpContextAccessor())));
			builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
				{
					opt.Password.RequireDigit = true;
					opt.Password.RequireLowercase = true;
					opt.Password.RequireUppercase = false;
					opt.Password.RequireNonAlphanumeric = false;
					opt.Password.RequiredLength = 6;
				})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
			builder.Services.AddValidatorsFromAssembly(typeof(EventCreateDtoValidator).Assembly);
			builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));
			builder.Services.AddAuthentication(opt =>
				{
					opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer("Bearer", options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ClockSkew = TimeSpan.Zero,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
					};
				});
			builder.Services.AddScoped<JwtService>();
			builder.Services.AddScoped<RefreshTokenService>();
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer"
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
				{
				new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
				Array.Empty<string>()
				}
				});
			});

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					policy =>
					{
						policy
							.AllowAnyOrigin() // frontend URL
							.AllowAnyHeader()
							.AllowAnyMethod();
						//if front end sends any cookies use this
						//.AllowCredentials();
						//but this can not be used with allow nay origin so care about that
					});
			});

			var app = builder.Build();

			app.UseMiddleware<ExceptionHandlingMidlleware>();
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
			app.UseCors("AllowAll");
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}
