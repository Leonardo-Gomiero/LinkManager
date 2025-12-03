using LinkManager.Api.Data;
using LinkManager.Api.DTOs;
using LinkManager.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.MapGet("/", () => "LinkManager API (Refactored) is running!");

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

    if (user is null) 
        return Results.NotFound();

    return Results.Ok(user);
});

app.Run();