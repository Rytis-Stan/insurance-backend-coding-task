using Claims.Api.Contracts.Dto;

namespace Claims.Api.Contracts.Messages;

public record GetClaimsResponse(IEnumerable<ClaimDto> Claims);