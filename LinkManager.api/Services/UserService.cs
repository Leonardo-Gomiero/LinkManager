using LinkManager.Api.Data;
using LinkManager.Api.DTOs;
using LinkManager.Api.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace LinkManager.Api.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponse?> RegisterAsync(RegisterUserRequest request)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == request.Email || u.PageSlug == request.PageSlug);

        if (existingUser)
        {
            return null;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            PageSlug = request.PageSlug,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return new UserResponse(newUser.Id, newUser.Email, newUser.PageSlug);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user is null) return null;

        return new UserResponse(user.Id, user.Email, user.PageSlug);
    }
}