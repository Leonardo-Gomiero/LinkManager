using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using LinkManager.Api.Data;
using LinkManager.Api.DTOs;
using LinkManager.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LinkService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("\n\n===================================");
            Console.WriteLine("🛑 ERRO DE AUTENTICAÇÃO DETECTADO:");
            Console.WriteLine(context.Exception.Message);
            Console.WriteLine("===================================\n\n");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("\n\n✅ TOKEN ACEITO! O usuário é válido.\n\n");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "LinkManager API is running securely!");

app.MapPost("/users", async (UserService userService, RegisterUserRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest("Invalid data.");

    var createdUser = await userService.RegisterAsync(request);

    if (createdUser is null)
        return Results.Conflict("Email or Slug already exists.");

    return Results.Created($"/users/{createdUser.Id}", createdUser);
});

app.MapGet("/users/{id}", async (UserService userService, int id) =>
{
    var user = await userService.GetByIdAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/login", async (UserService userService, LoginRequest request) =>
{
    var response = await userService.LoginAsync(request);

    if (response is null)
        return Results.Unauthorized();

    return Results.Ok(response);
});

var linksGroup = app.MapGroup("/links").RequireAuthorization();

linksGroup.MapPost("/", async (LinkService linkService, CreateLinkRequest request, ClaimsPrincipal user) =>
{
    var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");

    if (!int.TryParse(userIdString, out int userId))
        return Results.Unauthorized();

    var newLink = await linkService.CreateAsync(userId, request);
    return Results.Created($"/links/{newLink.Id}", newLink);
});

linksGroup.MapGet("/", async (LinkService linkService, ClaimsPrincipal user) =>
{
    var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userIdString))
    {
        userIdString = user.FindFirstValue("sub");
    }
    if (!int.TryParse(userIdString, out int userId)) return Results.Unauthorized();

    var links = await linkService.GetAllAsync(userId);
    return Results.Ok(links);
});

linksGroup.MapDelete("/{id}", async (LinkService linkService, int id, ClaimsPrincipal user) =>
{
    var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(userIdString))
    {
        userIdString = user.FindFirstValue("sub");
    }
    if (!int.TryParse(userIdString, out int userId)) return Results.Unauthorized();

    var deleted = await linkService.DeleteAsync(id, userId);

    if (!deleted) return Results.NotFound();

    return Results.NoContent();
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.Run();