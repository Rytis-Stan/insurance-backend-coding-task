using Claims.Domain;

namespace Claims.Application.Commands.GetClaims;

public record GetClaimsResult(IEnumerable<Claim> Claims);
