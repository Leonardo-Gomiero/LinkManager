namespace LinkManager.Api.DTOs;

public record LoginResponse(string Token, int UserId, string Email);