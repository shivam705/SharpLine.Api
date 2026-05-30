using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Data;
using SharpLine.Api.Extensions;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Repositories;
using SharpLine.Api.Services;
using SharpLine.Api.Services.AuthServices;
using SharpLine.Api.Services.IService.IAuthService;
using SharpLine.Api.Services.IService.IFirebaseService;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var path = Path.Combine(builder.Environment.ContentRootPath, "firebase-key.json");

// Add DbContext + Identity

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

// Configure Identity
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Register Repositories
builder.Services.AddScoped<ShopRepository>();
builder.Services.AddScoped<BarberRepository>();
builder.Services.AddScoped<BookingRepository>();
// Register Services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IBarberService, BarberService>();
builder.Services.AddScoped<IBookingService, BookingService>();

//firebase
builder.Services.AddScoped<IFirebaseAuthService, FirebaseAuthService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token with Bearer prefix. Example: Bearer {token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//firebase Configuration
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-key.json")
});


builder.AddAppAuthetication();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --------------------------- ROLE + ADMIN SEEDING ---------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRoles(services);
}

app.Run();
