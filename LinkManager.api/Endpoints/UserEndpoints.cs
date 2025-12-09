using LinkManager.Api.DTOs;
using LinkManager.Api.Services;

namespace LinkManager.Api.Endpoints;

public static class UserEndpoints
{
    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/users", RegisterUser);
        app.MapGet("/users/{id}", GetUserById);
        app.MapPut("/users/{id}", UpdateUser);
        app.MapPost("/login", Login);

        return app;
    }

    private static async Task<IResult> RegisterUser(UserService userService, RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.BadRequest("Invalid data.");

        var createdUser = await userService.RegisterAsync(request);

        if (createdUser is null)
            return Results.Conflict("Email or Slug already exists.");

        return Results.Created($"/users/{createdUser.Id}", createdUser);
    }

    private static async Task<IResult> GetUserById(UserService userService, int id)
    {
        var user = await userService.GetByIdAsync(id);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> UpdateUser(UserService userService, int id, RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Results.BadRequest("Invalid data.");

        var updatedUser = await userService.UpdateAsync(id, request);

        if (updatedUser is null)
            return Results.Conflict("Email or Slug already exists, or user not found.");

        return Results.Ok(updatedUser);
    }

    private static async Task<IResult> Login(UserService userService, LoginRequest request)
    {
        var response = await userService.LoginAsync(request);

        if (response is null)
            return Results.Unauthorized();

        return Results.Ok(response);
    }
}
