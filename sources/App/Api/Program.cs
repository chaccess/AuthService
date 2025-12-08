using Api.Middleware;
using Application.Common.Settings;
using Application.CQRS.Commands.SendCode;
using Application.Interfaces;
using Application.Services.AuthService;
using Application.Services.AuthService.Mapping;
using Application.Services.VerificationCodesService;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

//Authentication

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<VerificationCodesSettings>(builder.Configuration.GetSection("VerificationCodes"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IVerificationCodesService, VerificationCodesService>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserMappingProfile>();
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendCodeCommand).Assembly));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    var rsa = RSA.Create(2048);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        //ValidIssuer = "localhost",
        //ValidAudience = "localhost",
        IssuerSigningKey = new RsaSecurityKey(rsa),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
    };
    o.IncludeErrorDetails = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // разрешённый фронтенд
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // если используются куки/авторизация
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
