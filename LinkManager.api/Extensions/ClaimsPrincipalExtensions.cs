using System.Security.Claims;

namespace LinkManager.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
    {
        // Tenta pegar pelo padrão do .NET (NameIdentifier)
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Se falhar, tenta pegar pelo padrão JWT ("sub")
        if (string.IsNullOrEmpty(idClaim))
        {
            idClaim = user.FindFirst("sub")?.Value;
        }

        // Tenta converter para Inteiro
        if (int.TryParse(idClaim, out int userId))
        {
            return userId;
        }

        return null; // Não achou ou não é número
    }
}
