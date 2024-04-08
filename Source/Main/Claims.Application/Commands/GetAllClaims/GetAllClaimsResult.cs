using Claims.Domain;

namespace Claims.Application.Commands.GetAllClaims;

public record GetAllClaimsResult(IEnumerable<Claim> Claims);
