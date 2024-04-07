using Claims.Domain;

namespace Claims.Application.Commands.GetAllClaims;

public record GetAllClaimsResponse(IEnumerable<Claim> Claims);
