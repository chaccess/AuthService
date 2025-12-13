using Api.Middleware;
using Application.Common.Settings;
using Application.CQRS.Commands.CreateUser;
using Application.Interfaces;
using Application.Services.AuthService;
using Application.Services.UserService;
using Application.Services.UserService.Mapping;
using Application.Services.VerificationCodesService;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Authentication

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Key));
builder.Services.Configure<VerificationCodesSettings>(builder.Configuration.GetSection(VerificationCodesSettings.Key));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IVerificationCodesService, VerificationCodesService>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserMappingProfile>();
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "localhost",
        ValidAudience = "localhost",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration[$"{JwtSettings.Key}:{nameof(JwtSettings.JwtSecretString)}"])
            ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
    o.IncludeErrorDetails = true;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OnlyAnonymous", policy =>
    {
        policy.RequireAssertion(context => context.User?.Identity?.IsAuthenticated != true);
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

//EntityFramework

builder.Services.AddDbContext<AuthDbContext>(
    opt => opt.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

// Add middleware
app.UseExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
