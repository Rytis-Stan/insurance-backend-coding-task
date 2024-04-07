using Claims.Domain;

namespace Claims.Application.Commands;

public record GetAllCoversResponse(IEnumerable<Cover> Covers);
