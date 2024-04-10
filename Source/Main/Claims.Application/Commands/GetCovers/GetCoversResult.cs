using Claims.Domain;

namespace Claims.Application.Commands.GetCovers;

public record GetCoversResult(IEnumerable<Cover> Covers);
