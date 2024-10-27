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
using BackendNETAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger services

// Configure DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")  // Allow your Angular app's URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();  // Important: Allow credentials
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

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    // Enable Swagger in Development and Staging
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineOinkMarket-v1");
          c.DocumentTitle = "Local Swagger UI"; 
    });
}
//else if (app.Environment.IsProduction())
//{
//    // Optional: Enable Swagger in Production, but protect it
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineOinkMarket-v1");

//        // Optional: Add basic auth or limit access
//        c.RoutePrefix = string.Empty; // To make swagger available at root
//        c.DocumentTitle = "Production Swagger UI"; // Optional customization
//    });
//}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseRouting();
app.MapHub<NotificationHub>("/notificationHub");

app.UseCors("AllowSpecificOrigins"); // Enable CORS policy
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware
app.UseStaticFiles();
app.MapControllers(); // Map API controllers
app.Run();
