using Microsoft.AspNetCore.WebUtilities;
using SkillBuddy.Entity;
using SkillBuddy.Core;
// using SkillBuddy.ui.Middleware;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Http.Features;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load config from appsettings.json
builder.Configuration
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

IConfiguration Configuration = builder.Configuration;

// Add framework services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// === 🔑 Inject Cryptography ===
string key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
             ?? throw new InvalidOperationException("ENCRYPTION_KEY is missing!");
string saltBase64 = Environment.GetEnvironmentVariable("ENCRYPTION_SALT")
             ?? throw new InvalidOperationException("ENCRYPTION_SALT is missing!");
byte[] salt = Convert.FromBase64String(saltBase64);

// Register Cryptography as singleton (DI ready)
builder.Services.AddSingleton(new Cryptography(key, salt));

// === Database setup ===
var dataConnection = new SkillBuddy.data.DataBaseManager.DataConnection();
dataConnection.ConnectionString = Configuration["ConnectionStrings:DefaultConnection"]
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Debug log (do not print in production)
Console.WriteLine("Connection String Program: " + dataConnection.ConnectionString);

var app = builder.Build();

// === Middleware ===
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
