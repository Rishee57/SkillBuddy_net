using Microsoft.AspNetCore.WebUtilities;
using SkillBuddy.Entity;
using SkillBuddy.Core;
// using SkillBuddy.ui.Middleware;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load config from appsettings.json
builder.Configuration
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration Configuration = builder.Configuration;

// Add framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillBuddy API", Version = "v1" });
});

// === 🔑 Inject Cryptography ===
// Try environment variables first, then fallback to appsettings for development
string key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
             ?? Configuration["Encryption:Key"]
             ?? throw new InvalidOperationException("ENCRYPTION_KEY is missing from both environment variables and appsettings!");

string saltBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_SALT")
                   ?? Configuration["Encryption:Salt"]
                   ?? throw new InvalidOperationException("ENCRYPTION_SALT is missing from both environment variables and appsettings!");

byte[] salt = Convert.FromBase64String(saltBase64);

// Register Cryptography as singleton (DI ready)
builder.Services.AddSingleton(new Cryptography(key, salt));

// === Database setup ===
var dataConnection = new SkillBuddy.data.DataBaseManager.DataConnection();
dataConnection.ConnectionString = Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Debug log (do not print in production)
Console.WriteLine("Connection String Program: " + dataConnection.ConnectionString);

// Register Email Publisher
builder.Services.AddScoped<SkillBuddy.ui.Services.EmailPublisher>();

var app = builder.Build();

// === Middleware ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillBuddy API v1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection(); // Usually handled by reverse proxy or API gateway in Docker
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
