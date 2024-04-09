using Claims.Api.Contracts.Dto;

namespace Claims.Api.Contracts.Messages;

public record GetCoversResponse(IEnumerable<CoverDto> Covers);
