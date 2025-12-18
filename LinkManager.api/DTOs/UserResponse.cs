namespace LinkManager.Api.DTOs;

public record UserResponse (
    int Id,
    string Email,
    string PageSlug
);