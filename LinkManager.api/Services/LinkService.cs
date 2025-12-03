using LinkManager.Api.Data;
using LinkManager.Api.DTOs;
using LinkManager.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkManager.Api.Services;

public class LinkService
{
    private readonly AppDbContext _context;

    public LinkService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LinkResponse> CreateAsync(int userId, CreateLinkRequest request)
    {
        var link = new Link
        {
            Title = request.Title,
            Url = request.Url,
            UserId = userId
        };

        _context.Links.Add(link);
        await _context.SaveChangesAsync();

        return new LinkResponse(link.Id, link.Title, link.Url);
    }

    public async Task<List<LinkResponse>> GetAllAsync(int userId)
    {
        var links = await _context.Links
            .Where(l => l.UserId == userId)
            .Select(l => new LinkResponse(l.Id, l.Title, l.Url))
            .ToListAsync();

        return links;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var link = await _context.Links.FindAsync(id);

        if (link is null || link.UserId != userId)
        {
            return false;
        }

        _context.Links.Remove(link);
        await _context.SaveChangesAsync();
        return true;
    }
}