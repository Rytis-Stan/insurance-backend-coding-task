using Claims.Domain;

namespace Claims.Application.Commands;

public record GetClaimByIdResponse(Claim? Claim);
