using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MovieBookingProject;
using MovieBookingProject.Interfaces;
using MovieBookingProject.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
// Add services to the container.
builder.Services.Configure<ConnectionWithMongoDb>(builder.Configuration.GetSection(nameof(ConnectionWithMongoDb)));
builder.Services.AddSingleton<IConnectionWithMongoDb>(config => config.GetRequiredService<IOptions<ConnectionWithMongoDb>>().Value);
builder.Services.AddSingleton<IMongoClient>(config => new MongoClient(builder.Configuration.GetValue<string>("ConnectionWithMongoDb:ConnectionString")));

builder.Services.AddScoped<IMovieInterface, MovieRepository>();
builder.Services.AddScoped<ITicketInterface, TicketRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Created By Amit Tomar")),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo() { Title = "Dev-MovieBookingApp", Version = "1.0.0" });
    s.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Name = "Authorization",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"safsfs$d.fdf76d#hg.fytbju76t7g\""
    });
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "BearerAuth",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new string[] {}
                }});
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
