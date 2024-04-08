using Claims.Domain;

namespace Claims.Application.Commands.GetAllCovers;

public record GetAllCoversResult(IEnumerable<Cover> Covers);
