using BackendNETAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using BackendNETAPI.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger services

// Configure DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Corrected line: use 'builder.Services' instead of 'services'
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//     //  .AddEntityFrameworkStores<MyDbContext>()
//       .AddDefaultTokenProviders();


//services.AddScoped<IUserService, UserService>(); // Register the user service

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
                   policy.WithOrigins("https://witty-cliff-0cb39d610.5.azurestaticapps.net")
                  .AllowAnyHeader() // Allow any header
                  .AllowAnyMethod(); // Allow any HTTP method
        });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured.");
}

if (string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("JWT Issuer is not configured.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

//// Configure Kestrel to listen on port 5274
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5274); // Listen on port 5274 for any IP address
});

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{
//    app.UseSwagger(); // Enable Swagger in development
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineOinkMarket-v1");
//        c.RoutePrefix = string.Empty;  // Makes Swagger available at the root (e.g., /swagger)
//    });
//}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineOinkMarket-v1");
    });
}


app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseRouting();

app.UseCors("AllowSpecificOrigins"); // Enable CORS policy

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// Serve static files from wwwroot/uploads
app.UseStaticFiles();


app.MapControllers(); // Map API controllers

// Run the application
app.Run();
