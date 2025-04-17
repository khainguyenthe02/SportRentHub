using Dapper;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Repositories;
using SportRentHub.Services.Interfaces;
using SportRentHub.Services;
using ERBeeVisionMaster.Entities.Extensions;
using Microsoft.OpenApi.Models;
using Mapster;
using Microsoft.Extensions.Hosting;
using SportRentHub.Entities.Models;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.Extensions;
using SportRentHub.Entities;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();


// Add services to the container.

builder.Services.AddControllers();

//Maspster configuration 
TypeAdapterConfig<Court, CourtDto>.NewConfig()
	.Map(dest => dest.Images, src =>
		!string.IsNullOrEmpty(src.Images)
		? src.Images.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
		: new List<string>());

TypeAdapterConfig<CourtDto, Court>.NewConfig()
	.Map(dest => dest.Images, src =>
		src.Images != null && src.Images.Any()
		? string.Join(",", src.Images)
		: string.Empty);
TypeAdapterConfig.GlobalSettings.Compile();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sport_Rent_Hub", Version = "v1" });
    c.ConfigureDateTime();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
            }
        });

});
builder.Services.AddCors(options =>
{
    options.AddPolicy("policy", policy =>
    {
        policy.WithOrigins("https://learn-eight-ebon.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddSingleton<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<IRepositoryManager, RepositoryManager>();
builder.Services.AddHostedService<BookingAutoUpdateService>();
builder.Services.AddHttpsRedirection(options =>
{
	options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
	options.HttpsPort = 443;
});

DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();
app.UseCors("policy");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
