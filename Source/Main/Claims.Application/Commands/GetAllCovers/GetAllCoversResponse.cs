using Claims.Domain;

namespace Claims.Application.Commands.GetAllCovers;

public record GetAllCoversResponse(IEnumerable<Cover> Covers);
