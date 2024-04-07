using Claims.Domain;

namespace Claims.Application.Commands;

public record GetAllClaimsResponse(IEnumerable<Claim> Claims);
