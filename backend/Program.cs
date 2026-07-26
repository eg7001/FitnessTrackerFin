using FitnessTracker.DbContext;
using FitnessTracker.Middleware;
using FitnessTracker.Models;
using FitnessTracker.Services;
using FitnessTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// DEPENDENCIES
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ISetService, SetService>();
builder.Services.AddScoped<IWorkoutExerciseService, WorkoutExerciseService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// TOken Dependency
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataProtection();

builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
            NameClaimType = ClaimTypes.NameIdentifier // ✅ important
        };
    });

// PER FRONTEND
// The frontend proxies /api requests through its own origin in both dev
// (Vite proxy) and prod (nginx proxy), so the browser never makes a
// cross-origin call under normal use - this CORS policy exists as a
// fallback for anyone hitting the API directly from a browser context.
var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(allowedOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Blunt brute-force login/register attempts: 5 requests per minute per IP.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
    {
        // Behind nginx (Docker Compose), every request arrives at Kestrel
        // from nginx's own container IP, not the real client's - without
        // this, all traffic through the proxy shares one rate-limit bucket
        // regardless of which browser it came from. nginx.conf sets
        // X-Real-IP to the real client address; fall back to the raw
        // connection IP for direct/non-proxied access (e.g. local dev).
        var partitionKey = context.Request.Headers["X-Real-IP"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        });
    });
});

var app = builder.Build();

// Make sure the "Admin" role exists before anyone tries to register into
// it (AuthService assigns it to the first user who ever signs up).
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
    }
}

app.UseMiddleware<ExceptionMiddleware>();


app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Deliberately no app.UseHttpsRedirection() here: nothing in this app's
// actual deployment paths terminates TLS at the backend - Docker Compose
// runs the backend on plain HTTP behind nginx, and local dev proxies to it
// over plain HTTP too (see vite.config.ts's proxy target and the comment
// on the refresh cookie's Secure flag in AuthController). Redirecting to
// an HTTPS port here was actively harmful for local dev: the frontend's
// proxy target is the plain-HTTP launch profile, so this middleware would
// 307 every API call to https://localhost:7008 instead - a different
// origin, which makes the browser strip the Authorization header on the
// follow-up request and silently break auth. If a real deployment adds a
// TLS-terminating reverse proxy in front of this app, redirect enforcement
// belongs there, not here.

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
