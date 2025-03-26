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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Maspster configuration 
TypeAdapterConfig<Court, CourtDto>.NewConfig()
    .Ignore(dest => dest.Images);
TypeAdapterConfig<Court, CourtDto>.NewConfig()
    .Map(court => court.Images, courtDto => !string.IsNullOrEmpty(courtDto.Images)
        ? courtDto.Images.Split(new[] { ',' }, StringSplitOptions.None).ToList()
        : new List<string>());
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
builder.Services.AddCors(option =>
{
    option.AddPolicy("policy", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyHeader().AllowAnyMethod());
});


builder.Services.AddSingleton<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<IRepositoryManager, RepositoryManager>();

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
