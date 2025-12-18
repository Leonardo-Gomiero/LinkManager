namespace LinkManager.Api.DTOs;

public record RegisterUserRequest(
    string Email,
    string Password,
    string PageSlug
);