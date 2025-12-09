using System.Security.Claims;
using LinkManager.Api.DTOs;
using LinkManager.Api.Services;
using LinkManager.Api.Extensions;

namespace LinkManager.Api.Endpoints;

public static class LinkEndpoints
{
    public static WebApplication MapLinkEndpoints(this WebApplication app)
    {
        var linksGroup = app.MapGroup("/links").RequireAuthorization();

        linksGroup.MapPost("/", CreateLink);
        linksGroup.MapGet("/", GetAllLinks);
        linksGroup.MapDelete("/{id}", DeleteLink);

        return app;
    }

    private static async Task<IResult> CreateLink(LinkService linkService, CreateLinkRequest request, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        if (userId is null) return Results.Unauthorized();

        var newLink = await linkService.CreateAsync(userId.Value, request);
        return Results.Created($"/links/{newLink.Id}", newLink);
    }

    private static async Task<IResult> GetAllLinks(LinkService linkService, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        if (userId is null) return Results.Unauthorized();

        var links = await linkService.GetAllAsync(userId.Value);
        return Results.Ok(links);
    }

    private static async Task<IResult> DeleteLink(LinkService linkService, int id, ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        if (userId is null) return Results.Unauthorized();

        var deleted = await linkService.DeleteAsync(id, userId.Value);
        if (!deleted) return Results.NotFound();

        return Results.NoContent();
    }
}
